using System;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Sprint,
    CrouchIdle,
    CrouchWalk,
}

public class CanDo
{
    public bool Look = true;
    public bool Move = true;
    public bool Run = true;
    public bool Jump = true;
    public bool Crouch = true;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform playerHold;

    private float walkSpeed = 3f;
    private float sprintSpeed = 6f;
    private float crouchSpeed = 1.5f;

    private Vector3 move = Vector3.zero;

    private float camHeight = 1.75f;
    private float standHeight = 2f;
    private float crouchHeight = 0.6f;
    private float standCamHeight = 1.75f;
    private float crouchCamHeight = 0.4f;
    private float crouchTransitionLength = 0.4f;
    private float crouchProgress = 0f;

    private float jumpStrength = 6f;
    private float gravity = -12f;
    private float groundGravity = -1f;

    private float reachRange = 1.5f;
    private float sensitivity = 5f;

    private CanDo canDo = new CanDo();
    private bool isCrouched = false;

    private float rotationX = 0f;
    private float velocityY = -1f;

    private PlayerState state = PlayerState.Idle;
    private float camBobbingT = 0f;
    private float bobbingTransitionLength = 0.2f;
    private float[] offsets = new float[5];
    private float[] weights = new float[5];

    private AudioSource playerAd;
    private CharacterController controller;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerAd = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();

        weights[(int)state] = 1f;
    }

    private void Update()
    {
        CameraBobbing();

        if (MainManager.instance.gameState != GameState.Normal) return;

        UpdateState();
        UpdateVelocity();

        if (canDo.Look) CameraLook();
        if (canDo.Crouch) HandleCrouch();
        if (canDo.Jump) HandleJump();
        if (canDo.Move) HandleMove();

        MovePlayer();

    }

    private void CameraBobbing()
    {
        camBobbingT += Time.deltaTime;
        camBobbingT = Mathf.Repeat(camBobbingT, 3f * 0.8f * 0.5f * 10f);

        offsets[0] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 3f) * 0.01f;
        offsets[1] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.8f) * 0.05f;
        offsets[2] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.5f) * 0.08f;
        offsets[3] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 3f) * 0.005f;
        offsets[4] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.8f) * 0.025f;

        float weightSum = 0f;
        float offsetSum = 0f;
        int curState = (int)state;
        if (!controller.isGrounded) curState = 0;
        if (MainManager.instance.gameState != GameState.Normal) curState = isCrouched ? 3 : 0;

        for (int i = 0; i < weights.Length; i++)
        {
            if (i != curState) weights[i] = Mathf.MoveTowards(weights[i], 0f, Time.deltaTime / bobbingTransitionLength);
            else weights[i] = Mathf.MoveTowards(weights[i], 1f, Time.deltaTime / bobbingTransitionLength);
            weightSum += weights[i];
            offsetSum += weights[i] * offsets[i];
        }
        
        if (weightSum > 0f) playerCam.transform.localPosition = new Vector3(0f, camHeight + offsetSum / weightSum, 0f);
    }

    private void UpdateState()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f)
        {
            if (isCrouched) state = PlayerState.CrouchWalk;
            else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && canDo.Run) state = PlayerState.Sprint;
            else state = PlayerState.Walk;
        }
        else
        {
            if (isCrouched) state = PlayerState.CrouchIdle;
            else state = PlayerState.Idle;
        }
    }

    private void UpdateVelocity()
    {
        move = Vector3.zero;

        if (controller.isGrounded) velocityY = groundGravity;
        else velocityY += gravity * Time.deltaTime;
    }

    private void CameraLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);
        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C) && controller.isGrounded)
        {
            if (!isCrouched) isCrouched = true;
            else
            {
                bool hasCeiling = Physics.CapsuleCast(
                    transform.position + controller.center - Vector3.up * (controller.height / 2 - controller.radius),
                    transform.position + controller.center + Vector3.up * (controller.height / 2 - controller.radius),
                    controller.radius,
                    Vector3.up,
                    standHeight - crouchHeight
                );

                if (!hasCeiling) isCrouched = false;
            }
        }

        float goalProgress = isCrouched ? 1f : 0f;
        crouchProgress = Mathf.MoveTowards(crouchProgress, goalProgress, Time.deltaTime / crouchTransitionLength);

        controller.height = Mathf.Lerp(standHeight, crouchHeight, crouchProgress);
        controller.center = Vector3.up * controller.height * 0.5f;

        camHeight = Mathf.Lerp(standCamHeight, crouchCamHeight, crouchProgress);
    }

    private void HandleJump()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space) && !isCrouched) velocityY = jumpStrength;
    }

    private void HandleMove()
    {
        float speed = state == PlayerState.Sprint ? sprintSpeed : walkSpeed;
        speed = Mathf.Lerp(speed, crouchSpeed, crouchProgress);

        move = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized * speed;
    }

    private void MovePlayer()
    {
        CollisionFlags flags = controller.Move((move + Vector3.up * velocityY) * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && velocityY > 0f) velocityY = groundGravity;
    }
}