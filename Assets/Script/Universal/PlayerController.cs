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

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform playerHold;
    [SerializeField] private InputActionAsset playerInput;

    private float walkSpeed = 3f;
    private float sprintSpeed = 5f;
    private float crouchSpeed = 1.5f;
    private float standHeight = 0.75f;
    private float crouchHeight = -0.6f;
    private float jumpStrength = 3f;
    private float gravity = -12f;
    private float transitionLength = 0.2f;
    private float reachRange = 1.5f;
    private float normalFieldOfView = 60f;
    private float sprintFieldOfView = 65f;
    private float sensitivity = 1f;

    private bool isCrouched = false;
    private float rotationX = 0f;
    private float velocityY = -1f;

    private PlayerState state = PlayerState.Idle;
    private float camBobbingT = 0f;
    private float[] offsets = new float[5];
    private float[] weights = new float[5];

    private AudioSource playerAd;
    private CharacterController controller;

    private InputActionMap playerMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    private void Awake()
    {
        playerAd = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();

        playerMap = playerInput.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        sprintAction = playerMap.FindAction("Sprint");
        crouchAction = playerMap.FindAction("Crouch");

        weights[(int)state] = 1f;
    }

    private void OnEnable()
    {
        playerMap.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
    }

    private void Update()
    {
        UpdateState();
        UpdateCamera();

        if (MainManager.instance.gameState != GameState.Normal) return;

        MovePlayer();

    }

    private void UpdateState()
    {
        if (moveAction.IsPressed())
        {
            if (isCrouched) state = PlayerState.CrouchWalk;
            else if (sprintAction.IsPressed()) state = PlayerState.Sprint;
            else state = PlayerState.Walk;
        }
        else
        {
            if (isCrouched) state = PlayerState.CrouchIdle;
            else state = PlayerState.Idle;
        }
    }

    private void UpdateCamera()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        /*transform.Rotate(Vector3.up * lookInput.x * sensitivity);
        rotationX = Mathf.Clamp(rotationX - lookInput.y * sensitivity, -90.0f, 90.0f);
        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);*/

        rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
        if (rotationX > 180.0f) rotationX -= 360.0f;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);
        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);

        camBobbingT += Time.deltaTime;
        camBobbingT = Mathf.Repeat(camBobbingT, 3f * 0.8f * 0.5f * 10f);

        offsets[0] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 3f) * 0.01f;
        offsets[1] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.8f) * 0.05f;
        offsets[2] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.5f) * 0.08f;
        offsets[3] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 2f) * 0.007f;
        offsets[4] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.7f) * 0.03f;

        float weightSum = 0f;
        float offsetSum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            if (i != (int)state) weights[i] = Mathf.MoveTowards(weights[i], 0f, Time.deltaTime / transitionLength);
            else weights[i] = Mathf.MoveTowards(weights[i], 1f, Time.deltaTime / transitionLength);
            weightSum += weights[i];
            offsetSum += weights[i] * offsets[i];
        }
        if (weightSum > 0f) playerCam.transform.localPosition = new Vector3(0f, standHeight + offsetSum / weightSum, 0f);

        if (state == PlayerState.Sprint) playerCam.fieldOfView = Mathf.MoveTowards(playerCam.fieldOfView, sprintFieldOfView, Mathf.Abs(sprintFieldOfView - normalFieldOfView) * Time.deltaTime / transitionLength);
        else playerCam.fieldOfView = Mathf.MoveTowards(playerCam.fieldOfView, normalFieldOfView, Mathf.Abs(sprintFieldOfView - normalFieldOfView) * Time.deltaTime / transitionLength);
    }

    private void MovePlayer()
    {
        float speed = walkSpeed;
        if (sprintAction.IsPressed()) speed = sprintSpeed;
        if (isCrouched) speed = crouchSpeed;

        if (controller.isGrounded) velocityY = -1f;
        else velocityY += gravity * Time.deltaTime;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized * speed + transform.up * velocityY;

        controller.Move(move * Time.deltaTime);
    }
}