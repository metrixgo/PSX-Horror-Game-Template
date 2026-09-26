using UnityEngine;
using UnityEngine.Events;

public class PutDownItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioClip putDownEffect;
    [SerializeField] private GameObject pickUpItem;

    [Header("Additional Effect")]
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private UnityEvent additionalEffect;

    private bool putDownBefore = false;

    private string itemName;
    private int itemLayer;
    private Transform itemParent;
    private Transform[] allTransforms;
    private Collider[] allColliders;

    private void Update()
    {
        if (pickUpItem == null) Destroy(gameObject);
    }

    public void Configure(string s)
    {
        itemName = s;
        itemLayer = pickUpItem.layer;
        itemParent = pickUpItem.transform.parent;
        allTransforms = pickUpItem.GetComponentsInChildren<Transform>(true);
        allColliders = pickUpItem.GetComponentsInChildren<Collider>(true);
    }

    public void Putdown()
    {
        pickUpItem.transform.SetParent(itemParent);
        pickUpItem.transform.position = transform.position;
        pickUpItem.transform.rotation = transform.rotation;
        pickUpItem.transform.localScale = transform.localScale;

        foreach (Transform t in allTransforms)
            t.gameObject.layer = itemLayer;

        foreach (Collider c in allColliders)
            c.enabled = true;

        gameObject.SetActive(false);

        if (!putDownBefore || !oneTimeUse)
        {
            putDownBefore = true;
            additionalEffect.Invoke();
        }

        MainManager.instance.RemoveItem(itemName);
        MainManager.instance.PlayEffect(putDownEffect);
    }
}