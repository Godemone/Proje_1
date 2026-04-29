using UnityEngine;
using UnityEngine.AI;

public class Enemy_Chase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float chaseRange = 20f;
    [SerializeField] private float stopRange = 2f;
    [SerializeField] private float updateRate = 0.1f;

    private float updateTimer;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (agent != null)
            agent.stoppingDistance = stopRange;
    }

    private void Update()
    {
        if (target == null || agent == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > chaseRange)
        {
            agent.ResetPath();
            return;
        }

        updateTimer -= Time.deltaTime;

        if (updateTimer <= 0f)
        {
            agent.SetDestination(target.position);
            updateTimer = updateRate;
        }

        if (distance <= stopRange)
        {
            agent.ResetPath();
        }
    }
}