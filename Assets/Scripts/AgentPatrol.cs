using UnityEngine;
using UnityEngine.AI;

public class AgentPatrol : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    int currentPatrolPointIndex;

    NavMeshAgent agent;

    float currentWaintingTime;
    float maxWaitingTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        currentPatrolPointIndex = -1;

        currentWaintingTime = 0;

        maxWaitingTime = 0;

        GoToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (maxWaitingTime == 0)
            maxWaitingTime = Random.Range(3, 5);
        if (currentWaintingTime >= maxWaitingTime)
        {
            maxWaitingTime = 0;
            currentWaintingTime = 0;
            GoToNextPoint();
        }
        else
            currentWaintingTime += Time.deltaTime;
        
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length != 0)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }
}
