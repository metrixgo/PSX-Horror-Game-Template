using Glitch;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Walk,
    Sprint,
    CrouchIdle,
    CrouchWalk,
}

public enum SurfaceType
{
    DirtyGround,
    Grass,
    Gravel,
    Leaves,
    Metal,
    Mud,
    Rock,
    Sand,
    Snow,
    Tile,
    Water,
    Wood,
}

[System.Serializable]
public class SurfaceSound
{
    public SurfaceType SurfaceTag;
    public AudioClip[] WalkSounds;
    public AudioClip[] SprintSounds;
}

public class PlayerCanDo
{
    public bool Look = true;
    public bool Move = true;
    public bool Sprint = true;
    public bool Jump = true;
    public bool Crouch = true;
    public bool Interact = true;
}

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform playerHold;
    [SerializeField] private Transform playerBody;

    [Header("Layers")]
    [SerializeField] private LayerMask physicalLayer;
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Footstep Sounds")]
    [SerializeField] private SurfaceSound[] surfaceSounds;

    private AudioSource playerAd;

    private CharacterController controller;

    private CinemachinePanTilt camPanTilt;
    private CinemachineBasicMultiChannelPerlin camBob;
    private CinemachineInputAxisController camController;
    private InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller camX;
    private InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller camY;

    private AnalogGlitchController analogGlitchController;
    private DigitalGlitchController digitalGlitchController;

    private Interactable curItem;
    private Interactable newItem;

    private GameInput input;

    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction interactAction;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private bool sprintInput = false;
    private bool jumpInput = false;
    private bool crouchInput = false;
    private bool interactInput = false;

    private Vector3 move = Vector3.zero;

    private float walkSpeed = 3f;
    private float sprintSpeed = 6f;
    private float crouchSpeed = 1.5f;

    private float curSpeed = 0f;

    private float camHeight = 1.75f;
    private float standHeight = 2f;
    private float crouchHeight = 1f;
    private float standCamHeight = 1.75f;
    private float crouchCamHeight = 0.9f;
    private float crouchProgress = 0f;

    private float jumpStrength = 6f;
    private float gravity = -12f;
    private float groundGravity = -2f;

    private float reachRange = 1.5f;
    private float sensitivity = 5f;

    public PlayerCanDo CanDo = new PlayerCanDo();

    public bool isCrouched { get; private set; } = false;

    private float velocityY = -1f;

    private PlayerState state = PlayerState.Idle;

    private float stepT = 0f;
    private float camBobbingT = 0f;
    private Vector3 playerHoldPosition = new Vector3(0.15f, -0.1f, 0.2f);

    float[] bobAmplitudes = { 0.08f, 0.15f, 0.3f, 0.05f, 0.12f };
    float[] bobFrequencies = { 0.08f, 0.15f, 0.3f, 0.05f, 0.12f };
    float curBobAmplitude = 0.08f;
    float curBobFrequency = 0.08f;
    float[] bobSteps = { 0.002f, 0.004f, 0.01f, 0.002f, 0.003f };
    float[] bobSpeeds = { 0.6f, 4f, 8f, 0.6f, 3f };
    float swayStep = 0.02f;
    float maxSwayStep = 0.15f;

    float transitionSpeed = 7f;

    private void Awake()
    {
        playerAd = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        camPanTilt = playerCam.GetComponent<CinemachinePanTilt>();
        camBob = playerCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        camController = playerCam.GetComponent<CinemachineInputAxisController>();

        analogGlitchController = Camera.main.GetComponent<AnalogGlitchController>();
        digitalGlitchController = Camera.main.GetComponent<DigitalGlitchController>();

        foreach (InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller controller in camController.Controllers)
        {
            if (controller.Name == "Look X (Pan)") camX = controller;
            else if (controller.Name == "Look Y (Tilt)") camY = controller;
            else Debug.LogError("Unknown Cinemachine Controller Name: " + controller.Name);
        }

        input = new GameInput();
        lookAction = input.Player.Look;
        moveAction = input.Player.Move;
        sprintAction = input.Player.Sprint;
        jumpAction = input.Player.Jump;
        crouchAction = input.Player.Crouch;
        interactAction = input.Player.Interact;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        if (MainManager.Instance.IsPaused) return;

        GetInput();
        UpdateState();
        UpdateSensitivity();
        CameraBobbing();

        if (MainManager.Instance.PlayerBlockLayers > 0)
        {
            if (curItem != null) curItem.SetFocused(false);
            newItem = null;
            curItem = null;
            return;
        }

        HandleMove();
        HandleJump();
        HandleCrouch();
        HandleInteractions();

        MovePlayer();
        FootstepSounds();
    }

    private void GetInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
        sprintInput = sprintAction.IsPressed();
        jumpInput = jumpAction.IsPressed();
        crouchInput = crouchAction.IsPressed();
        interactInput = interactAction.WasPressedThisFrame();
    }

    private void UpdateState()
    {
        if (moveInput.magnitude > 0.01f && CanDo.Move && MainManager.Instance.PlayerBlockLayers == 0)
        {
            if (isCrouched) state = PlayerState.CrouchWalk;
            else if (sprintInput && CanDo.Sprint) state = PlayerState.Sprint;
            else state = PlayerState.Walk;
        }
        else
        {
            if (isCrouched) state = PlayerState.CrouchIdle;
            else state = PlayerState.Idle;
        }
    }

    private void UpdateSensitivity()
    {
        sensitivity = (CanDo.Look && MainManager.Instance.PlayerBlockLayers == 0) ? MainManager.Instance.Data.sensitivity : 0;
        camX.Input.Gain = sensitivity;
        camY.Input.Gain = -sensitivity;
    }

    private void CameraBobbing()
    {
        camBobbingT += Time.deltaTime;

        int curState = (controller.isGrounded && MainManager.Instance.PlayerBlockLayers == 0) ? (int)state : (int)PlayerState.Idle;

        float bobAmplitude = bobAmplitudes[curState];
        float bobFrequency = bobFrequencies[curState];

        curBobAmplitude = Mathf.Lerp(curBobAmplitude, bobAmplitude, Time.deltaTime * transitionSpeed);
        curBobFrequency = Mathf.Lerp(curBobFrequency, bobFrequency, Time.deltaTime * transitionSpeed);

        camBob.AmplitudeGain = curBobAmplitude;
        camBob.FrequencyGain = curBobFrequency;

        float bobStep = bobSteps[curState];
        float bobSpeed = bobSpeeds[curState];

        Vector3 offset = Vector3.zero;

        offset.x = (Mathf.Cos(camBobbingT * bobSpeed) * bobStep);
        offset.y = (Mathf.Sin(camBobbingT * 2f * bobSpeed) * bobStep);

        offset.x += Mathf.Clamp(-swayStep * lookInput.x * sensitivity / 1000f, -maxSwayStep, maxSwayStep);
        offset.y += Mathf.Clamp(-swayStep * lookInput.y * sensitivity / 1000f, -maxSwayStep, maxSwayStep);

        playerHold.localPosition = Vector3.Lerp(playerHold.localPosition, playerHoldPosition + offset, Time.deltaTime * transitionSpeed);
    }

    private void HandleMove()
    {
        if (CanDo.Move)
        {
            curSpeed = Mathf.Lerp((state == PlayerState.Sprint ? sprintSpeed : walkSpeed), crouchSpeed, crouchProgress);

            Quaternion yRot = Quaternion.Euler(0f, playerCam.eulerAngles.y, 0f);
            Vector3 camForward = yRot * Vector3.forward;
            Vector3 camRight = yRot * Vector3.right;

            move = (camRight * moveInput.x + camForward * moveInput.y).normalized * curSpeed;

            if (move.magnitude > 0.01f &&
                Physics.SphereCast(
                    transform.position + Vector3.up * (controller.height - controller.radius),
                    controller.radius,
                    move.normalized,
                    out RaycastHit hit,
                    controller.skinWidth + controller.radius * 0.1f,
                    physicalLayer
                ) &&
                hit.normal.y < -0.01f &&
                hit.normal.y > -0.99f)
            {
                Vector3 slideDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;
                move = slideDirection * Vector3.Dot(move, slideDirection);
            }
        }
        else
        {
            move = Vector2.zero;
        }
    }

    private void HandleJump()
    {
        if (controller.isGrounded) velocityY = groundGravity;
        else velocityY += gravity * Time.deltaTime;

        if (controller.isGrounded && jumpInput && crouchProgress < 0.01f && CanDo.Jump)
            velocityY = jumpStrength;
    }

    private void HandleCrouch()
    {
        bool hasCeiling = Physics.CheckCapsule(
                    transform.position + Vector3.up * controller.radius,
                    transform.position + Vector3.up * (standHeight - controller.radius),
                    controller.radius,
                    physicalLayer
                );

        if (hasCeiling && isCrouched && !crouchInput)
            isCrouched = true;
        else if (crouchInput && controller.isGrounded && CanDo.Crouch)
            isCrouched = true;
        else if (!crouchInput || !CanDo.Crouch)
            isCrouched = false;

        float goalProgress = isCrouched ? 1f : 0f;
        crouchProgress = Mathf.Lerp(crouchProgress, goalProgress, Time.deltaTime * transitionSpeed);

        float visualHeight = Mathf.Lerp(standHeight, crouchHeight, crouchProgress);
        controller.height = isCrouched ? crouchHeight : standHeight;
        controller.center = Vector3.up * visualHeight * 0.5f;

        playerBody.localScale = new Vector3(controller.radius * 2f, visualHeight * 0.5f, controller.radius * 2f) * 1.1f;
        playerBody.localPosition = controller.center;

        camHeight = Mathf.Lerp(standCamHeight, crouchCamHeight, crouchProgress);
        playerCam.transform.localPosition = Vector3.up * camHeight;
    }

    private void HandleInteractions()
    {
        if (CanDo.Interact &&
            !MainManager.Instance.IsExecutingTriggers &&
            Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit hit, reachRange, ~ignoreLayer))
        {
            newItem = hit.collider.GetComponentInParent<Interactable>();

            if (newItem == null)
            {
                if (curItem != null) curItem.SetFocused(false);

                curItem = null;
            }
            else
            {
                if (curItem != null && curItem != newItem)
                    curItem.SetFocused(false);

                curItem = newItem;

                if (curItem != null)
                    curItem.SetFocused(true);
            }
        }
        else
        {
            if (curItem != null) curItem.SetFocused(false);
            newItem = null;
            curItem = null;
        }

        if (interactInput && curItem != null)
            curItem.Interact();
    }

    private void MovePlayer()
    {
        CollisionFlags flags = controller.Move((move + Vector3.up * velocityY) * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && velocityY > 0f) velocityY = groundGravity;
    }

    private void FootstepSounds()
    {
        stepT = Mathf.Clamp(stepT - Time.deltaTime, 0f, Mathf.PI / bobSpeeds[(int)state]);

        if (!controller.isGrounded || state == PlayerState.Idle || state == PlayerState.CrouchIdle || stepT > 0) return;

        stepT = Mathf.PI / bobSpeeds[(int)state];

        if (Physics.SphereCast(transform.position + transform.up * controller.radius, 
            controller.radius, -transform.up, out RaycastHit hit, controller.radius * 0.5f, ~ignoreLayer))
        {
            string surfaceTag = hit.collider.tag;
            foreach (SurfaceSound surfaceSound in surfaceSounds)
            {
                if (surfaceSound.SurfaceTag.ToString() == surfaceTag)
                {
                    if (state == PlayerState.Sprint)
                        playerAd.PlayOneShot(surfaceSound.SprintSounds[Random.Range(0, surfaceSound.SprintSounds.Length)]);
                    else
                        playerAd.PlayOneShot(surfaceSound.WalkSounds[Random.Range(0, surfaceSound.WalkSounds.Length)]);

                    break;
                }
            }
        }
    }

    public void Move(Vector3 dir)
    {
        controller.enabled = false;
        transform.position += dir;
        controller.enabled = true;
    }

    public void SetPosition(Vector3 pos)
    {
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }

    public void LookAt(Vector3 position, float l)
    {
        Quaternion rotation = Quaternion.LookRotation(position - playerCam.position);

        StartCoroutine(TurnTo(rotation, l));
    }

    private IEnumerator TurnTo(Quaternion goal, float l)
    {
        float startX = camPanTilt.TiltAxis.Value;
        float startY = camPanTilt.PanAxis.Value;
        float endX = goal.eulerAngles.x;
        float endY = goal.eulerAngles.y;

        float t = 0;
        while (t < l)
        {
            camPanTilt.TiltAxis.Value = Mathf.LerpAngle(startX, endX, t / l);
            camPanTilt.PanAxis.Value = Mathf.LerpAngle(startY, endY, t / l);
            t += Time.deltaTime;
            yield return null;
        }

        camPanTilt.TiltAxis.Value = Mathf.LerpAngle(startX, endX, t / l);
        camPanTilt.PanAxis.Value = Mathf.LerpAngle(startY, endY, t / l);
    }

    public void CanLook(bool can) => CanDo.Look = can;
    public void CanMove(bool can) => CanDo.Move = can;
    public void CanSprint(bool can) => CanDo.Sprint = can;
    public void CanJump(bool can) => CanDo.Jump = can;
    public void CanCrouch(bool can) => CanDo.Crouch = can;
    public void CanInteract(bool can) => CanDo.Interact = can;

    public void SetDigitalGlitch(float intensity) => digitalGlitchController.Intensity = intensity;
    public void SetScanLineJitter(float intensity) => analogGlitchController.ScanLineJitter = intensity;
    public void SetVerticalJump(float intensity) => analogGlitchController.VerticalJump = intensity;
    public void SetHorizontalShake(float intensity) => analogGlitchController.HorizontalShake = intensity;
    public void SetColorDrift(float intensity) => analogGlitchController.ColorDrift = intensity;
    public void SetHorizontalRipple(float intensity) => analogGlitchController.HorizontalRipple = intensity;
}