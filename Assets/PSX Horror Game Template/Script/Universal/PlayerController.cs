using System.Collections;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Sprint,
    CrouchIdle,
    CrouchWalk,
}

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform playerHold;
    [SerializeField] private Transform playerBody;

    [Header("Layers")]
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private LayerMask interactableLayer;

    private AudioSource playerAd;
    private CharacterController controller;

    private Vector3 move = Vector3.zero;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    private float walkSpeed = 3f;
    private float sprintSpeed = 6f;
    private float crouchSpeed = 1.5f;

    private float camHeight = 1.75f;
    private float standHeight = 2f;
    private float crouchHeight = 1f;
    private float standCamHeight = 1.75f;
    private float crouchCamHeight = 0.9f;
    private float crouchTransitionLength = 0.3f;
    private float crouchProgress = 0f;

    private float jumpStrength = 6f;
    private float gravity = -12f;
    private float groundGravity = -2f;

    private float reachRange = 1.5f;
    private float sensitivity = 5f;

    public bool canLook { get; private set; } = true;
    public bool canMove { get; private set; } = true;
    public bool canRun { get; private set; } = true;
    public bool canJump { get; private set; } = true;
    public bool canCrouch { get; private set; } = true;
    public bool canInteract { get; private set; } = true;
    public bool isCrouched { get; private set; } = false;

    private float rotationX = 0f;
    private float velocityY = -1f;

    private PlayerState state = PlayerState.Idle;

    private float camBobbingT = 0f;
    private Vector3 playerHoldPosition = new Vector3(0.15f, -0.1f, 0.2f);

    private Interactable curItem;
    private Interactable newItem;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerAd = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!MainManager.instance.IsPlayerActive || MainManager.instance.IsPaused) return;

        UpdateState();
        CameraBobbing();

        if (canLook) HandleLook();
        if (canCrouch) HandleCrouch();
        if (canJump) HandleJump();
        if (canMove) HandleMove();
        if (canInteract) HandleInteractions();

        MovePlayer();

    }

    private void UpdateState()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");

        if (moveInput.magnitude > 0.01f && canMove)
        {
            if (isCrouched) state = PlayerState.CrouchWalk;
            else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && canRun) state = PlayerState.Sprint;
            else state = PlayerState.Walk;
        }
        else
        {
            if (isCrouched) state = PlayerState.CrouchIdle;
            else state = PlayerState.Idle;
        }

        move = Vector3.zero;

        if (controller.isGrounded) velocityY = groundGravity;
        else velocityY += gravity * Time.deltaTime;

        sensitivity = MainManager.instance.data.sensitivity;
    }

    private void CameraBobbing()
    {
        camBobbingT += Time.deltaTime;

        playerCam.transform.localPosition = Vector3.up * camHeight;

        int curState = controller.isGrounded ? (int)state : (int)PlayerState.Idle;

        float[] bobSteps = { 0.002f, 0.004f, 0.01f, 0.002f, 0.004f };
        float[] bobSpeeds = { 0.6f, 4f, 8f, 0.6f, 2f };

        float bobStep = bobSteps[curState];
        float bobSpeed = bobSpeeds[curState];
        float swayStep = 0.03f;
        float maxSwayStep = 0.3f;
        float swaySpeed = 10f;

        Vector3 offset = Vector3.zero;

        offset.x = (Mathf.Cos(camBobbingT * bobSpeed) * bobStep);
        offset.y = (Mathf.Sin(camBobbingT * 2f * bobSpeed) * bobStep);

        offset.x += Mathf.Clamp(-swayStep * lookInput.x, -maxSwayStep, maxSwayStep);
        offset.y += Mathf.Clamp(-swayStep * lookInput.y, -maxSwayStep, maxSwayStep);

        playerHold.localPosition = Vector3.Lerp(playerHold.localPosition, playerHoldPosition + offset, Time.deltaTime * swaySpeed);
    }

    private void HandleLook()
    {
        rotationX -= lookInput.y * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);
        playerCam.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(0, lookInput.x * sensitivity, 0);
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.C) && controller.isGrounded)
            isCrouched = true;
        else if (!Input.GetKey(KeyCode.C))
            isCrouched = false;

        bool hasCeiling = Physics.CheckCapsule(
                    transform.position + Vector3.up * controller.radius,
                    transform.position + Vector3.up * (standHeight - controller.radius),
                    controller.radius,
                    environmentLayer
                );

        if (hasCeiling && !isCrouched) isCrouched = true;

        float goalProgress = isCrouched ? 1f : 0f;
        crouchProgress = Mathf.MoveTowards(crouchProgress, goalProgress, Time.deltaTime / crouchTransitionLength);

        controller.height = Mathf.Lerp(standHeight, crouchHeight, crouchProgress);
        controller.center = Vector3.up * controller.height * 0.5f;

        float margin = 1.1f;

        playerBody.localScale = new Vector3(controller.radius * 2f, controller.height * 0.5f, controller.radius * 2f) * margin;
        playerBody.localPosition = controller.center;

        camHeight = Mathf.Lerp(standCamHeight, crouchCamHeight, crouchProgress);
    }

    private void HandleJump()
    {
        if (controller.isGrounded && Input.GetKey(KeyCode.Space) && !isCrouched) velocityY = jumpStrength;
    }

    private void HandleMove()
    {
        float speed = state == PlayerState.Sprint ? sprintSpeed : walkSpeed;
        speed = Mathf.Lerp(speed, crouchSpeed, crouchProgress);

        move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized * speed;

        if (move.magnitude > 0.01f &&
            Physics.SphereCast(
                transform.position + Vector3.up * (controller.height - controller.radius),
                controller.radius,
                move.normalized,
                out RaycastHit hit,
                controller.skinWidth + 0.1f,
                environmentLayer
                ) &&
            hit.normal.y < -0.01f && 
            hit.normal.y > -0.99f)
        {
            Vector3 slideDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;
            move = slideDirection * Vector3.Dot(move, slideDirection);
        }
    }

    private void HandleInteractions()
    {
        Ray ray = new Ray(playerCam.position, playerCam.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, reachRange, interactableLayer, QueryTriggerInteraction.Collide) &&
            hit.collider.CompareTag("Interactable"))
        {
            newItem = hit.collider.GetComponent<Interactable>();

            if (newItem == null)
                Debug.LogError("Interactable Object Not Having Interactable Script: " + hit.collider.name);

            if (curItem != null && curItem != newItem)
                curItem.SetFocused(false);

            curItem = newItem;

            if (curItem != null)
                curItem.SetFocused(true);
        }
        else
        {
            newItem = null;

            if (curItem != null) curItem.SetFocused(false);

            curItem = null;
        }

        if (Input.GetMouseButtonDown(0) && curItem != null)
            curItem.Interact();
    }

    private void MovePlayer()
    {
        CollisionFlags flags = controller.Move((move + Vector3.up * velocityY) * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && velocityY > 0f) velocityY = groundGravity;
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
        Vector3 dir = (position - playerCam.position).normalized;
        float y = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float x = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        StartCoroutine(TurnTo(x, y, l));
    }

    private IEnumerator TurnTo(float x, float y, float l)
    {
        float t = 0;
        float startX = rotationX;
        float startY = transform.eulerAngles.y;
        while (t < l)
        {
            rotationX = Mathf.LerpAngle(startX, x, t / l);
            playerCam.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(startY, y, t / l), 0);
            t += Time.deltaTime;
            yield return null;
        }
        rotationX = x;
        playerCam.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation = Quaternion.Euler(0, y, 0);
    }

    public void CanLook(bool can)
    {
        canLook = can;
    }

    public void CanMove(bool can)
    {
        canMove = can;
    }

    public void CanRun(bool can)
    {
        canRun = can;
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