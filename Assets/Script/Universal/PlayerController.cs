using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform playerHold;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;

    private float walkSpeed = 3f;
    private float runSpeed = 5f;
    private float crouchSpeed = 1.5f;
    private float standHeight = 0.75f;
    private float crouchHeight = -0.6f;
    private float jumpStrength = 3f;
    private float gravity = -12f;
    private float transitionLength = 0.2f;
    private float reachRange = 1.5f;
    private float normalFieldOfView = 60f;
    private float runFieldOfView = 65f;

    private bool isCrouched = false;
    private float rotationX = 0f;
    private float velocityY = -1f;

    private PlayerState state = PlayerState.Idle;
    private float camBobbingT = 0f;
    private float[] offsets = new float[3];
    private float[] weights = new float[3];

    private AudioSource playerAd;
    private CharacterController controller;

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
    }

    private void Start()
    {
        playerAd = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();

        weights[(int)state] = 1f;
    }

    private void Update()
    {
        UpdateCamera();
        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame) state = 0;
            if (Keyboard.current.wKey.wasPressedThisFrame) state = (PlayerState)1;
            if (Keyboard.current.eKey.wasPressedThisFrame) state = (PlayerState)2;
        }

        if (MainManager.instance.gameState != GameState.Normal) return;

        MovePlayer();

    }

    private void UpdateCamera()
    {
        camBobbingT += Time.deltaTime;
        camBobbingT = Mathf.Repeat(camBobbingT, 3f * 0.8f * 0.5f * 10f);

        offsets[0] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 3f) * 0.01f;
        offsets[1] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.8f) * 0.05f;
        offsets[2] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.5f) * 0.08f;

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

        if (state == PlayerState.Run) playerCam.fieldOfView = Mathf.MoveTowards(playerCam.fieldOfView, runFieldOfView, Mathf.Abs(runFieldOfView - normalFieldOfView) * Time.deltaTime / transitionLength);
        else playerCam.fieldOfView = Mathf.MoveTowards(playerCam.fieldOfView, normalFieldOfView, Mathf.Abs(runFieldOfView - normalFieldOfView) * Time.deltaTime / transitionLength);
    }

    private void MovePlayer()
    {
        float speed = walkSpeed;
        if (sprintAction.action.IsPressed()) speed = runSpeed;
        if (isCrouched) speed = crouchSpeed;

        if (controller.isGrounded) velocityY = -1f;
        else velocityY += gravity * Time.deltaTime;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized * speed + transform.up * velocityY;

        controller.Move(move * Time.deltaTime);
    }
}