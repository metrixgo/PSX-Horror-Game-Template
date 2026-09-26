using UnityEngine;
using UnityEngine.Events;

public class PickUpItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioClip pickUpEffect;
    [SerializeField] private GameObject playerHold;
    [SerializeField] private GameObject putBackItem;

    [Header("Offsets")]
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private Vector3 scale = Vector3.one;

    [Header("Additional Effect")]
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private UnityEvent additionalEffect;

    private bool pickedUpBefore = false;

    private Collider[] colliders;

    private void Start()
    {
        colliders = GetComponents<Collider>();
    }

    public void PickUp()
    {
        if (MainManager.instance.IsPaused) return;

        if (playerHold != null)
        {
            if (playerHold.transform.childCount > 0)
            {
                Trigger trig = new Trigger();
                trig.triggerType = TriggerType.DisplayDialogue;
                trig.displayDialogueSpeaker = "You";
                trig.displayDialogueContent = "My hands are full...";
                MainManager.instance.AddTrigger(trig);
            }
            else
            {
                tag = "Untagged";

                foreach(Collider collider in colliders)
                    collider.enabled = false;

                transform.SetParent(playerHold.transform);
                transform.localPosition = position;
                transform.localRotation = rotation;
                transform.localScale = scale;

                if(!pickedUpBefore && oneTimeUse)
                {
                    pickedUpBefore = true;
                    additionalEffect.Invoke();
                }

                if(putBackItem != null)
                    putBackItem.SetActive(true);
            }
        }
        else
        {
            additionalEffect.Invoke();
            Destroy(gameObject);
        }

        MainManager.instance.AddItem(name);
        MainManager.instance.PlayEffect(pickUpEffect);
    }
}