using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    private bool isOpponent = false;
    private a_Champion champion;
    public float AttackDistance;
    private NavMeshAgent m_Agent;
    private float m_Distance;
    private Transform m_CurrentTarget;

    private a_Champion m_CurrentOpponent;

    
    private float m_NextTargetUpdateTime;
    public float TargetUpdateInterval = 0.2f;
    private GameObject[] enemies;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        champion = GetComponent<a_Champion>();
        AttackDistance = champion.Attack.Distance;
        Debug.Log($"Attack range {m_Distance}");
        FindNearestTarget();
        Debug.Log("Start finding nearest target");
    }

    void FindNearestTarget()
    {
        if (!isOpponent)
        {
            enemies = GameObject.FindGameObjectsWithTag("Ennemy");
        } else
        {
            enemies = GameObject.FindGameObjectsWithTag("Player");
        }

        float nearestDistance = float.MaxValue;
        Transform nearestTarget = null;
        a_Champion nearestOpponent = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = enemy.transform;
                nearestOpponent = enemy.GetComponent<a_Champion>();
            }
        }

        m_CurrentTarget = nearestTarget;
        m_CurrentOpponent = nearestOpponent;
        if (m_CurrentTarget != null)
        {
            Debug.Log("Found nearest target: " + m_CurrentTarget.name);
        }
        else
        {
            Debug.Log("No targets found");
        }
    }

    public void setIsOpponent(bool boolean)
    {
        isOpponent = boolean;
    }

    void Update()
    {
        // Periodically update the nearest target
        if (Time.time >= m_NextTargetUpdateTime)
        {
            FindNearestTarget();
            m_NextTargetUpdateTime = Time.time + TargetUpdateInterval;
        }

        // If no target, return
        if (m_CurrentTarget == null)
        {
            if (m_Agent.isOnNavMesh)
                {
                    m_Agent.isStopped = true;
                }            
            return;
        }

        // Continuously update the agent's destination
        m_Distance = Vector3.Distance(transform.position, m_CurrentTarget.position);
        if (m_Distance < AttackDistance/2)
        {
            if (m_Agent.isOnNavMesh)
                {
                    Debug.Log("Arrived at target. Stopping agent");
                    Debug.Log($"Arrived at target. My Health: {champion.Health.CurrentHealth} Opponent Health: {m_CurrentOpponent.Health.CurrentHealth}");
                    m_Agent.isStopped = true;
                }            
        }
        else
        {
            if (m_Agent.isOnNavMesh)
                {
                    m_Agent.isStopped = false;
                    m_Agent.SetDestination(m_CurrentTarget.position);
                } 
            

            // Rotate towards movement direction
            Vector3 direction = m_Agent.velocity.normalized;
            if (direction.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}
