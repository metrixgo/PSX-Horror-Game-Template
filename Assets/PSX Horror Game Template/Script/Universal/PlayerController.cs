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
    public SurfaceType surfaceTag;
    public AudioClip[] walkSounds;
    public AudioClip[] sprintSounds;
}

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform playerHold;
    [SerializeField] private Transform playerBody;

    [Header("Layers")]
    [SerializeField] private LayerMask physicalLayer;

    [Header("Footstep Sounds")]
    [SerializeField] private SurfaceSound[] surfaceSounds;

    private AudioSource playerAd;

    private CharacterController controller;

    private CinemachinePanTilt camPanTilt;
    private CinemachineBasicMultiChannelPerlin camBob;
    private CinemachineInputAxisController camController;
    private InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller camX;
    private InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller camY;

    private Interactable curItem;
    private Interactable newItem;

    private InputSystem input;

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

    public bool canLook { get; private set; } = true;
    public bool canMove { get; private set; } = true;
    public bool canSprint { get; private set; } = true;
    public bool canJump { get; private set; } = true;
    public bool canCrouch { get; private set; } = true;
    public bool canInteract { get; private set; } = true;
    public bool isCrouched { get; private set; } = false;

    private float velocityY = -1f;

    private PlayerState state = PlayerState.Idle;

    private float stepT = 0f;
    private float camBobbingT = 0f;
    private Vector3 playerHoldPosition = new Vector3(0.15f, -0.1f, 0.2f);

    float[] bobAmplitudes = { 0.08f, 0.15f, 0.3f, 0.05f, 0.12f};
    float[] bobFrequencies = { 0.08f, 0.15f, 0.3f, 0.05f, 0.12f};
    float curBobAmplitude = 0.08f;
    float curBobFrequency = 0.08f;
    float[] bobSteps = { 0.002f, 0.004f, 0.01f, 0.002f, 0.003f };
    float[] bobSpeeds = { 0.6f, 4f, 8f, 0.6f, 3f };
    float swayStep = 0.02f;
    float maxSwayStep = 0.15f;

    float transitionSpeed = 7f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerAd = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        camPanTilt = playerCam.GetComponent<CinemachinePanTilt>();
        camBob = playerCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        camController = playerCam.GetComponent<CinemachineInputAxisController>();

        foreach (InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller controller in camController.Controllers)
        {
            if (controller.Name == "Look X (Pan)") camX = controller;
            else if (controller.Name == "Look Y (Tilt)") camY = controller;
            else Debug.LogError("Unknown Cinemachine Controller Name: " + controller.Name);
        }

        input = new InputSystem();
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
        if (MainManager.instance.IsPaused) return;

        GetInput();
        UpdateState();
        UpdateSensitivity();
        CameraBobbing();

        if (!MainManager.instance.IsPlayerActive) return;

        UpdateGravity();

        if (canMove) HandleMove();
        if (canJump) HandleJump();
        if (canCrouch) HandleCrouch();
        if (canInteract) HandleInteractions();

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
        if (moveInput.magnitude > 0.01f && canMove && MainManager.instance.IsPlayerActive)
        {
            if (isCrouched) state = PlayerState.CrouchWalk;
            else if (sprintInput && canSprint) state = PlayerState.Sprint;
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
        sensitivity = (canLook && MainManager.instance.IsPlayerActive) ? MainManager.instance.data.sensitivity : 0;
        camX.Input.Gain = sensitivity;
        camY.Input.Gain = -sensitivity;
    }

    private void CameraBobbing()
    {
        camBobbingT += Time.deltaTime;

        int curState = (controller.isGrounded && MainManager.instance.IsPlayerActive) ? (int)state : (int)PlayerState.Idle;

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

    private void UpdateGravity()
    {
        if (controller.isGrounded) velocityY = groundGravity;
        else velocityY += gravity * Time.deltaTime;
    }

    private void HandleMove()
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
                controller.skinWidth + 0.1f,
                physicalLayer
            ) &&
            hit.normal.y < -0.01f &&
            hit.normal.y > -0.99f)
        {
            Vector3 slideDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;
            move = slideDirection * Vector3.Dot(move, slideDirection);
        }
    }

    private void HandleJump()
    {
        if (controller.isGrounded && jumpInput && !isCrouched)
            velocityY = jumpStrength;
    }

    private void HandleCrouch()
    {
        if (crouchInput && controller.isGrounded)
            isCrouched = true;
        else if (!crouchInput)
            isCrouched = false;

        bool hasCeiling = Physics.CheckCapsule(
                    transform.position + Vector3.up * controller.radius,
                    transform.position + Vector3.up * (standHeight - controller.radius),
                    controller.radius,
                    physicalLayer
                );

        if (hasCeiling && !isCrouched) isCrouched = true;

        float goalProgress = isCrouched ? 1f : 0f;
        crouchProgress = Mathf.Lerp(crouchProgress, goalProgress, Time.deltaTime * transitionSpeed);

        controller.height = Mathf.Lerp(standHeight, crouchHeight, crouchProgress);
        controller.center = Vector3.up * controller.height * 0.5f;

        float margin = 1.1f;

        playerBody.localScale = new Vector3(controller.radius * 2f, controller.height * 0.5f, controller.radius * 2f) * margin;
        playerBody.localPosition = controller.center;

        camHeight = Mathf.Lerp(standCamHeight, crouchCamHeight, crouchProgress);

        playerCam.transform.localPosition = Vector3.up * camHeight;
    }

    private void HandleInteractions()
    {
        if (Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit hit, reachRange))
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
            newItem = null;

            if (curItem != null) curItem.SetFocused(false);

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

        if (Physics.SphereCast(transform.position + transform.up * controller.radius, controller.radius, -transform.up, out RaycastHit hit, 0.1f))
        {
            string surfaceTag = hit.collider.tag;
            foreach (SurfaceSound surfaceSound in surfaceSounds)
            {
                if (surfaceSound.surfaceTag.ToString() == surfaceTag)
                {
                    if(state == PlayerState.Sprint)
                        playerAd.PlayOneShot(surfaceSound.sprintSounds[Random.Range(0, surfaceSound.sprintSounds.Length)]);
                    else
                        playerAd.PlayOneShot(surfaceSound.walkSounds[Random.Range(0, surfaceSound.walkSounds.Length)]);

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

    public void CanLook(bool can)
    {
        canLook = can;
    }

    public void CanMove(bool can)
    {
        canMove = can;
    }

    public void CanSprint(bool can)
    {
        canSprint = can;
    }

    public void CanJump(bool can)
    {
        canJump = can;
    }

    public void CanCrouch(bool can)
    {
        canCrouch = can;
    }

    public void CanInteract(bool can)
    {
        canInteract = can;
    }

}