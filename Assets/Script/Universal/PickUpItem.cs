using UnityEngine;
using UnityEngine.Events;

public class PickUpItem : MonoBehaviour
{
    [Header("Item Name")]
    [SerializeField] private string itemName;

    [Header("References")]
    [SerializeField] private AudioClip pickUpEffect;
    [SerializeField] private GameObject playerHold;
    [SerializeField] private PutDownItem putDownItem;

    [Header("Offsets")]
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private float scale = 1f;

    [Header("Additional Effect")]
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private UnityEvent additionalEffect;

    private bool pickedUpBefore = false;

    private Transform[] allTransforms;
    private Collider[] allColliders;

    private void Awake()
    {
        allTransforms = gameObject.GetComponentsInChildren<Transform>(true);
        allColliders = gameObject.GetComponentsInChildren<Collider>(true);

        if (putDownItem != null)
            putDownItem.Configure(itemName);
    }

    public void PickUp()
    {
        if (playerHold != null)
        {
            if (playerHold.transform.childCount > 0)
            {
                Trigger trig = new Trigger();
                trig.triggerType = TriggerType.DisplayDialogue;
                trig.displayDialogueSpeaker = "You";
                trig.displayDialogueContent = "My hands are full...";
                MainManager.Instance.AddTrigger(trig);
            }
            else
            {
                MainManager.Instance.AddItem(itemName);
                MainManager.Instance.PlayEffect(pickUpEffect);

                transform.SetParent(playerHold.transform);
                transform.localPosition = position;
                transform.localRotation = rotation;
                transform.localScale *= scale;

                foreach (Transform t in allTransforms)
                    t.gameObject.layer = playerHold.layer;

                foreach (Collider c in allColliders)
                    c.enabled = false;

                if (!pickedUpBefore || !oneTimeUse)
                {
                    pickedUpBefore = true;
                    additionalEffect.Invoke();
                }

                if (putDownItem != null)
                    putDownItem.gameObject.SetActive(true);

            }
        }
        else
        {
            MainManager.Instance.AddItem(itemName);
            MainManager.Instance.PlayEffect(pickUpEffect);

            additionalEffect.Invoke();
            Destroy(gameObject);
        }
    }
}