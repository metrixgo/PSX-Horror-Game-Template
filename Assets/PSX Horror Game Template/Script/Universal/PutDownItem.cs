using UnityEngine;

public class PutDownItem : MonoBehaviour
{
    [SerializeField] private AudioClip soundEffect;
    [SerializeField] private GameObject item;

    private Collider[] colls;

    private void Start()
    {
        colls = item.GetComponents<Collider>();
    }

    public void Putdown()
    {
        if (MainManager.instance.IsPaused) return;

        MainManager.instance.RemoveItem(item.name);
        MainManager.instance.PlayEffect(soundEffect);

        item.transform.SetParent(null);
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        item.tag = "Interactable";
        foreach (Collider c in colls) c.enabled = true;
        gameObject.SetActive(false);
    }
}