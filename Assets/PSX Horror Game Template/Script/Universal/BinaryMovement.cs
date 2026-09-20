using System.Collections;
using UnityEngine;

public class BinaryMovement : MonoBehaviour
{
    [SerializeField] private bool opened = false;
    [SerializeField] private float openLength = 0.6f;
    [SerializeField] private float closeLength = 0.6f;
    [SerializeField] private float openDelay = 0f;
    [SerializeField] private float closeDelay = 0f;
    [SerializeField] private AudioClip openEffect;
    [SerializeField] private AudioClip closeEffect;
    [SerializeField] private Vector3 deltaPosition;
    [SerializeField] private Quaternion deltaRotation;
    [SerializeField] private Vector3 deltaScale = Vector3.one;

    private bool isMoving = false;
    private AudioSource ad;

    private void Awake()
    {
        ad = GetComponent<AudioSource>();

        if (Mathf.Abs(deltaScale.x * deltaScale.y * deltaScale.z) < 0.01f)
            Debug.LogError("Delta Scale of Binary Movement Cannot Be Zero: " + gameObject.name);
    }

    public void Interact()
    {
        if (!isMoving)
        {
            StartCoroutine(StartMoving());
            StartCoroutine(PlaySounds());
        }
    }

    private IEnumerator StartMoving()
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 startScale = transform.localScale;

        Vector3 endPosition = startPosition + deltaPosition * (!opened ? 1 : -1);
        Quaternion endRotation = startRotation * (!opened ? deltaRotation : Quaternion.Inverse(deltaRotation));
        Vector3 endScale = Vector3.Scale(startScale, !opened ? deltaScale :
            new Vector3(1f / deltaScale.x, 1f / deltaScale.y, 1f / deltaScale.z));

        float t = 0;
        float length = !opened ? openLength : closeLength;
        while (t < length)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t / length);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / length);
            transform.localScale = Vector3.Lerp(startScale, endScale, t / length);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        transform.rotation = endRotation;
        transform.localScale = endScale;

        opened = !opened;
        isMoving = false;
    }

    private IEnumerator PlaySounds()
    {
        bool opened = this.opened;

        yield return new WaitForSeconds(!opened ? openDelay : closeDelay);

        if (ad != null)
            ad.PlayOneShot(!opened ? openEffect : closeEffect);
    }

}