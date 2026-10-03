using System.Collections.Generic;
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
    private Transform itemParent;
    private Dictionary<Transform, int> transformLayers = new Dictionary<Transform, int>();
    private Dictionary<Collider, bool> colliderEnables = new Dictionary<Collider, bool>();

    private void Update()
    {
        if (pickUpItem == null) Destroy(gameObject);
    }

    public void Configure(string s)
    {
        itemName = s;
        itemParent = pickUpItem.transform.parent;

        foreach (Transform t in pickUpItem.GetComponentsInChildren<Transform>(true))
            transformLayers[t] = t.gameObject.layer;

        foreach (Collider c in pickUpItem.GetComponentsInChildren<Collider>(true))
            colliderEnables[c] = c.enabled;
    }

    public void Putdown()
    {
        pickUpItem.transform.SetParent(itemParent);
        pickUpItem.transform.position = transform.position;
        pickUpItem.transform.rotation = transform.rotation;
        pickUpItem.transform.localScale = transform.localScale;

        foreach (Transform t in transformLayers.Keys)
            t.gameObject.layer = transformLayers[t];

        foreach (Collider c in colliderEnables.Keys)
            c.enabled = colliderEnables[c];

        gameObject.SetActive(false);

        if (!putDownBefore || !oneTimeUse)
        {
            putDownBefore = true;
            additionalEffect.Invoke();
        }

        MainManager.Instance.RemoveItem(itemName);
        MainManager.Instance.PlayEffect(putDownEffect);
    }
}