using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum KillerState
{
    Idle,
    Patrol,
    Chase,
    Killing,
}

public class KillerAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Cameras")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GameObject jumpscareCamera;

    [Header("Settings")]
    [SerializeField] private float reachRange = 2f;
    [SerializeField] private float jumpscareTime = 0.1f;

    [Header("Sounds")]
    [SerializeField] private AudioClip movingSound;
    [SerializeField] private AudioClip jumpScareSound;

    [Header("Kill Event")]
    [SerializeField] private UnityEvent killEvent;

    private KillerState state = KillerState.Idle;

    private NavMeshAgent agent;
    private AudioSource effectsPlayer;

    private CinemachineBlendDefinition blendStyle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        effectsPlayer = GetComponent<AudioSource>();

        effectsPlayer.clip = movingSound;
        effectsPlayer.Play();

        blendStyle = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut,
            jumpscareTime
        );
    }

    private void Update()
    {
        if (MainManager.Instance.IsPaused || state == KillerState.Killing) return;

        agent.SetDestination(player.position);

        if (agent.remainingDistance < reachRange &&
            agent.pathStatus == NavMeshPathStatus.PathComplete &&
            Vector3.Distance(transform.position, player.position) < reachRange &&
            !agent.pathPending)
            StartCoroutine(Kill());
    }

    private IEnumerator Kill()
    {
        state = KillerState.Killing;

        yield return new WaitUntil(() => 
            !MainManager.Instance.IsExecutingTriggers &&
            !MainManager.Instance.IsPaused);

        effectsPlayer.PlayOneShot(jumpScareSound);

        agent.isStopped = true;

        brain.DefaultBlend = blendStyle;
        jumpscareCamera.SetActive(true);
        ((MonoBehaviour)brain.ActiveVirtualCamera).gameObject.SetActive(false);

        MainManager.Instance.BlockPlayer(true);

        killEvent.Invoke();
    }
}