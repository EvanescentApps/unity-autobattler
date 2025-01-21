using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    private bool isEnnemi = false;
    private a_Champion championInfos;
    public float AttackDistance;
    private NavMeshAgent m_Agent;
    private float m_Distance;
    private Transform m_CurrentTarget;
    private float m_NextTargetUpdateTime;
    public float TargetUpdateInterval = 0.2f;
    private GameObject[] enemies;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        championInfos = GetComponent<a_Champion>();
        AttackDistance = championInfos.Attack.Distance;
        Debug.Log($"Attack range {m_Distance}");
        FindNearestTarget();
        Debug.Log("Start finding nearest target");
    }

    void FindNearestTarget()
    {
        if (!isEnnemi)
        {
            enemies = GameObject.FindGameObjectsWithTag("ennemi");
        } else
        {
            enemies = GameObject.FindGameObjectsWithTag("Player");
        }

        float nearestDistance = float.MaxValue;
        Transform nearestTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = enemy.transform;
            }
        }

        m_CurrentTarget = nearestTarget;
        if (m_CurrentTarget != null)
        {
            Debug.Log("Found nearest target: " + m_CurrentTarget.name);
        }
        else
        {
            Debug.Log("No targets found");
        }
    }

    public void setIsEnnemi(bool boolean)
    {
        isEnnemi = boolean;
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
            m_Agent.isStopped = true;
            return;
        }

        // Continuously update the agent's destination
        m_Distance = Vector3.Distance(transform.position, m_CurrentTarget.position);
        if (m_Distance < AttackDistance / 2)
        {
            m_Agent.isStopped = true;
            Debug.Log("Arrived at target. Stopping agent");
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(m_CurrentTarget.position);
            Debug.Log("Moving towards target");

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
