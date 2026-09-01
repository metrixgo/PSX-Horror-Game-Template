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
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform playerHold;

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
    private float sensitivity = 5f;

    private bool isCrouched = false;
    private float rotationX = 0f;
    private float velocityY = -1f;

    private PlayerState state = PlayerState.Idle;
    private float camBobbingT = 0f;
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
        UpdateState();
        UpdateCamera();

        if (MainManager.instance.gameState != GameState.Normal) return;

        MovePlayer();

    }

    private void UpdateState()
    {
        if (Input.GetAxisRaw("Horizontal") > 0.01f || Input.GetAxisRaw("Vertical") > 0.01f)
        {
            if (isCrouched) state = PlayerState.CrouchWalk;
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) state = PlayerState.Sprint;
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
        rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
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
        if (state == PlayerState.Sprint) speed = sprintSpeed;
        if (isCrouched) speed = crouchSpeed;

        if (controller.isGrounded) velocityY = -1f;
        else velocityY += gravity * Time.deltaTime;

        Vector3 move = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized * speed;
        controller.Move((move + Vector3.up * velocityY) * Time.deltaTime);
    }
}