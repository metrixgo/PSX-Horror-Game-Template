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

    [Header("Rendering")]
    [SerializeField] private RectTransform gameView;

    [Header("Settings")]
    [SerializeField] private float transitionLength = 1f;
    [SerializeField] private bool enableMouse = true;

    [Header("Events")]
    [SerializeField] private UnityEvent focusOnEvent;
    [SerializeField] private UnityEvent focusOffEvent;

    private CinemachineBlendDefinition blendStyle;

    private InputSystem input;
    private InputAction returnAction;
    private InputAction interactAction;

    private bool transitioning = false;

    private void Awake()
    {
        input = new InputSystem();
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
        if (returnAction.WasPressedThisFrame() && !transitioning)
            FocusOff();
    }

    public void FocusOn()
    {
        if (!transitioning)
            StartCoroutine(Focus(true));
    }

    public void FocusOff()
    {
        if (!transitioning)
            StartCoroutine(Focus(false));
    }

    public GameObject InteractedObject()
    {
        if(interactAction.WasPressedThisFrame())
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float x = mousePos.x / Screen.width * gameView.rect.width;
            float y = mousePos.y / Screen.height * gameView.rect.height;
            Vector3 texPos = new Vector3(x, y, 0);// NEED FIX AND CHECK!!! PROBABLY NEED RENDER TEXTURE
            if (Physics.Raycast(Camera.main.ScreenPointToRay(texPos), out RaycastHit hit))
                return hit.collider.gameObject;
        }

        return null;
    }

    private IEnumerator Focus(bool focus)
    {
        transitioning = true;
        brain.DefaultBlend = blendStyle;
        focusCamera.SetActive(focus);
        playerCamera.SetActive(!focus);

        if (focus)
        {
            MainManager.instance.SetPlayerActive(false);
        }
        else
        {
            focusOffEvent.Invoke();
            yield return new WaitForEndOfFrame();
            MainManager.instance.CanPauseGame(true);

            if (enableMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        yield return new WaitForSeconds(transitionLength);

        if (focus)
        {
            focusOnEvent.Invoke();
            MainManager.instance.CanPauseGame(false);

            if (enableMouse)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            MainManager.instance.SetPlayerActive(true);
        }

        transitioning = false;
    }
}
