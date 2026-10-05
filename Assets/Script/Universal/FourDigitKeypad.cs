using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FourDigitKeypad : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip correctEffect;
    [SerializeField] private AudioClip wrongEffect;
    [SerializeField] private AudioClip typeEffect;

    [Header("Events")]
    [SerializeField] private UnityEvent correctEvent;
    [SerializeField] private UnityEvent wrongEvent;

    [Header("Displays")]
    [SerializeField] private TextMeshPro[] displays;
    [SerializeField] private int[] correctCode = { 0, 0, 0, 0 };
    private int displayIndex = -1;
    private int[] displayNumbers = { -1, -1, -1, -1 };
    

    [Header("Keys")]
    [SerializeField] private GameObject[] digitKeys;
    [SerializeField] private GameObject submitKey;
    [SerializeField] private GameObject deleteKey;

    private FocusOnObject focusOn;

    private GameInput input;
    private InputAction submitAction;
    private InputAction deleteAction;
    private InputAction[] digitActions;

    private AudioSource effectsPlayer;

    private bool playSound;

    private void Awake()
    {
        focusOn = GetComponent<FocusOnObject>();

        input = new GameInput();
        submitAction = input.Game.Submit;
        deleteAction = input.Game.Delete;

        digitActions = new InputAction[]
        {
            input.Game.Digit0,
            input.Game.Digit1,
            input.Game.Digit2,
            input.Game.Digit3,
            input.Game.Digit4,
            input.Game.Digit5,
            input.Game.Digit6,
            input.Game.Digit7,
            input.Game.Digit8,
            input.Game.Digit9,
        };

        effectsPlayer = GetComponent<AudioSource>();
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
        if (MainManager.Instance.IsPaused || MainManager.Instance.IsExecutingTriggers) return;

        playSound = false;

        for (int i = 0; i <= 9; i++) if (digitActions[i].WasPressedThisFrame()) PressDigit(i);
        if (deleteAction.WasPressedThisFrame()) PressDelete();
        if (submitAction.WasPressedThisFrame()) PressSubmit();

        GameObject interactedKey = focusOn.InteractedObject();
        if (interactedKey != null)
        {
            for (int i = 0; i <= 9; i++) if (interactedKey == digitKeys[i]) PressDigit(i);
            if (interactedKey == deleteKey) PressDelete();
            if (interactedKey == submitKey) PressSubmit();
        }

        if (playSound) effectsPlayer.PlayOneShot(typeEffect);
    }

    private void PressDigit(int num)
    {
        playSound = true;

        if (displayIndex <= 2)
        {
            displayIndex++;
            displayNumbers[displayIndex] = num;
            displays[displayIndex].text = num.ToString();
        }
    }

    private void PressDelete()
    {
        playSound = true;

        if (displayIndex >= 0)
        {
            displayNumbers[displayIndex] = -1;
            displays[displayIndex].text = "";
            displayIndex--;
        }
    }

    private void PressSubmit()
    {
        playSound = true;

        if (displayNumbers.SequenceEqual(correctCode))
        {
            correctEvent.Invoke();
            effectsPlayer.PlayOneShot(correctEffect);
        }
        else
        {
            wrongEvent.Invoke();
            effectsPlayer.PlayOneShot(wrongEffect);
        }
    }
}