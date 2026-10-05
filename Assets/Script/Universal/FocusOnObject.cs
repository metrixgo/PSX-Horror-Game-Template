using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FocusOnObject : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GameObject focusCamera;
    [SerializeField] private GameObject playerCamera;

    [Header("Settings")]
    [SerializeField] private float transitionLength = 1f;
    [SerializeField] private bool enableMouse = true;

    [Header("Rendering")]
    [SerializeField] private RenderTexture gameRenderTexture;

    [Header("IgnoreLayer")]
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Focus Events")]
    [SerializeField] private UnityEvent focusOnEvent;
    [SerializeField] private UnityEvent focusOffEvent;

    private CinemachineBlendDefinition blendStyle;

    private GameInput input;
    private InputAction returnAction;
    private InputAction interactAction;

    private bool focused = false;
    private bool transitioning = false;

    private void Awake()
    {
        input = new GameInput();
        returnAction = input.Game.Return;
        interactAction = input.Player.Interact;

        blendStyle = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut,
            transitionLength
        );
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
        if (returnAction.WasPressedThisFrame() && !transitioning && focused && focusCamera.activeSelf)
            FocusOff();
    }

    public void FocusOn()
    {
        if (!transitioning && !focused && playerCamera.activeSelf)
            StartCoroutine(Focus());
    }

    public void FocusOff()
    {
        if (!transitioning && focused && focusCamera.activeSelf)
            StartCoroutine(Focus());
    }

    public GameObject InteractedObject()
    {
        if(interactAction.WasPressedThisFrame())
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float x = mousePos.x / Screen.width * gameRenderTexture.width;
            float y = mousePos.y / Screen.height * gameRenderTexture.height;
            Vector3 texPos = new Vector3(x, y, 0);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(texPos), out RaycastHit hit, Mathf.Infinity, ~ignoreLayer))
                return hit.collider.gameObject;
        }

        return null;
    }

    private IEnumerator Focus()
    {
        focused = !focused;

        transitioning = true;
        brain.DefaultBlend = blendStyle;
        focusCamera.SetActive(focused);
        playerCamera.SetActive(!focused);

        if (focused)
        {
            MainManager.Instance.BlockPlayer(true);
        }
        else
        {
            focusOffEvent.Invoke();
            yield return new WaitForEndOfFrame();
            MainManager.Instance.BlockPause(false);

            if (enableMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        yield return new WaitForSeconds(transitionLength);

        if (focused)
        {
            focusOnEvent.Invoke();
            MainManager.Instance.BlockPause(true);

            if (enableMouse)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            MainManager.Instance.BlockPlayer(false);
        }

        transitioning = false;
    }
}
