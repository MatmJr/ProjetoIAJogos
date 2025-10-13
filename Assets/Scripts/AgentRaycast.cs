using UnityEngine;
using UnityEngine.AI;

public class AgentFSMRaycast : MonoBehaviour
{
    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    [Header("Patrulha")]
    [SerializeField] Transform[] patrolPoints;
    private int currentPatrolPointIndex;
    private float currentWaitingTime;
    private float maxWaitingTime;

    [Header("Perseguição")]
    [SerializeField] private float visionRange = 5f;        // alcance da visão
    [SerializeField] private float visionAngle = 30f;       // ângulo do cone de visão
    [SerializeField] private Vector2 lookDirection = Vector2.right; // direção fixa que o inimigo "olha"
    
    private GameObject target;
    private bool hasLineOfSight = false;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentPatrolPointIndex = -1;
        currentWaitingTime = 0;
        maxWaitingTime = 0;

        target = GameObject.FindGameObjectWithTag("Player");

        GoToNextPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    void FixedUpdate()
    {
        CheckVision();

        // troca de estado dependendo da visão
        if (hasLineOfSight)
            currentState = State.Chase;
        else
            currentState = State.Patrol;
    }

    // ------------------------
    // PATRULHA
    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            if (maxWaitingTime == 0)
                maxWaitingTime = Random.Range(2, 4);

            if (currentWaitingTime >= maxWaitingTime)
            {
                maxWaitingTime = 0;
                currentWaitingTime = 0;
                GoToNextPoint();
            }
            else
            {
                currentWaitingTime += Time.deltaTime;
            }
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length != 0)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }

    // ------------------------
    // PERSEGUIÇÃO
    void Chase()
    {
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    // ------------------------
    // VERIFICAÇÃO DE VISÃO
    void CheckVision()
    {
        // Atualiza a direção de olhar baseada na velocidade do agente
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            lookDirection = agent.velocity.normalized;
        }

        Vector2 toPlayer = (target.transform.position - transform.position).normalized;
        float angle = Vector2.Angle(lookDirection, toPlayer);

        if (angle <= visionAngle)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, toPlayer, visionRange);

            if (ray.collider != null && ray.collider.CompareTag("Player"))
            {
                hasLineOfSight = true;
                Debug.DrawRay(transform.position, toPlayer * visionRange, Color.green);
                return;
            }
        }

        hasLineOfSight = false;
        Debug.DrawRay(transform.position, lookDirection * visionRange, Color.red);
    }

}

