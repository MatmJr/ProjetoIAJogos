using UnityEngine;
using UnityEngine.AI;

public class AgentFSM : MonoBehaviour
{
    private enum State { Patrol, Engage }
    private State currentState = State.Patrol;

    [Header("Referências")]
    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    [Header("Configurações de Movimento")]
    [SerializeField] private float chaseRadius = 6f;     // Distância mínima para começar a perseguir
    [SerializeField] private float patrolRadius = 8f;    // Raio para escolher destinos aleatórios
    [SerializeField] private float patrolInterval = 3f;  // Tempo entre mudanças de destino no Patrol

    private float patrolTimer;
    
    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Engage:
                Engage();
                break;
        }
    }

    private void Patrol()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Se o jogador está perto, muda para o estado ENGAGE
        if (distanceToTarget <= chaseRadius)
        {
            currentState = State.Engage;
            return;
        }

        // Continua andando aleatoriamente
        patrolTimer -= Time.deltaTime;
        if (!agent.pathPending && agent.remainingDistance <= 0.2f || patrolTimer <= 0f)
        {
            SetRandomPatrolDestination();
            patrolTimer = patrolInterval;
        }
    }

    
    private void Engage()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Persegue o jogador
        agent.SetDestination(target.position);

        // Se o jogador sair do alcance, volta para o PATROL
        if (distanceToTarget > chaseRadius)
        {
            currentState = State.Patrol;
            SetRandomPatrolDestination();
            patrolTimer = patrolInterval;
        }
    }

    
    private void SetRandomPatrolDestination()
    {
        Vector3 center = transform.position;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float r = Random.Range(0f, patrolRadius);
        Vector3 candidate = new Vector3(center.x + Mathf.Cos(angle) * r, center.y + Mathf.Sin(angle) * r, 0f);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Configuração para NavMesh 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        patrolTimer = patrolInterval;
        SetRandomPatrolDestination();
    }


    

}
