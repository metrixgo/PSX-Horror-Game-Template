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
    private float sprintSpeed = 6f;
    private float crouchSpeed = 1.5f;
    private float standHeight = 2f;
    private float crouchHeight = 0.6f;
    private float standCamHeight = 1.75f;
    private float crouchCamHeight = 0.4f;
    private float jumpStrength = 6f;
    private float gravity = -12f;
    private float groundGravity = -1f;
    private float transitionLength = 0.3f;
    private float reachRange = 1.5f;
    private float normalFieldOfView = 60f;
    private float sprintFieldOfView = 65f;
    private float sensitivity = 5f;

    private bool isCrouched = false;
    private float camHeight = 0.75f;
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
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f)
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
        offsets[3] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 3f) * 0.005f;
        offsets[4] = Mathf.Sin(camBobbingT * 2 * Mathf.PI / 0.8f) * 0.025f;

        float weightSum = 0f;
        float offsetSum = 0f;
        int curState = (int)state;
        if (!controller.isGrounded) curState = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            if (i != curState) weights[i] = Mathf.MoveTowards(weights[i], 0f, Time.deltaTime / transitionLength);
            else weights[i] = Mathf.MoveTowards(weights[i], 1f, Time.deltaTime / transitionLength);
            weightSum += weights[i];
            offsetSum += weights[i] * offsets[i];
        }

        float goalCamHeight = isCrouched ? crouchCamHeight : standCamHeight;
        camHeight = Mathf.MoveTowards(camHeight, goalCamHeight, Mathf.Abs(standCamHeight - crouchCamHeight) * Time.deltaTime / transitionLength);
        if (weightSum > 0f) playerCam.transform.localPosition = new Vector3(0f, camHeight + offsetSum / weightSum, 0f);

        float goalView = state == PlayerState.Sprint ? sprintFieldOfView : normalFieldOfView;
        playerCam.fieldOfView = Mathf.MoveTowards(playerCam.fieldOfView, goalView, Mathf.Abs(sprintFieldOfView - normalFieldOfView) * Time.deltaTime / transitionLength);
    }

    private void MovePlayer()
    {
        float speed = walkSpeed;
        if (state == PlayerState.Sprint) speed = sprintSpeed;
        if (isCrouched) speed = crouchSpeed;

        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.C)) isCrouched = !isCrouched;
            if (Input.GetKeyDown(KeyCode.Space) && !isCrouched) velocityY = jumpStrength;
            else velocityY = groundGravity;
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
        }

        float goalHeight = isCrouched ? crouchHeight : standHeight;
        controller.height = Mathf.MoveTowards(controller.height, goalHeight, Mathf.Abs(standHeight - crouchHeight) * Time.deltaTime / transitionLength);
        controller.center = Vector3.up * controller.height * 0.5f;

        Vector3 move = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized * speed;
        
        CollisionFlags flags = controller.Move((move + Vector3.up * velocityY) * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && velocityY > 0f) velocityY = groundGravity;
    }
}