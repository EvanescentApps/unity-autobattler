using System.Collections;
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
    
    protected bool attackReady;
    private a_Champion m_CurrentOpponent;

    private Animator animator;

    private float m_NextTargetUpdateTime;
    public float TargetUpdateInterval = 0.01f;
    private GameObject[] enemies;

    void Start()
    {
        animator = GetComponent<Animator>();

        m_Agent = GetComponent<NavMeshAgent>();
        champion = GetComponent<a_Champion>();
        AttackDistance = champion.Attack.Distance;
        Debug.Log($"Attack range {m_Distance}");
        FindNearestTarget();
        attackReady = true;
    }

    void FindNearestTarget()
    {
        if (!isOpponent)
        {
            enemies = GameObject.FindGameObjectsWithTag("Ennemy");
        }
        else
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
            //Debug.Log("Found nearest target: " + m_CurrentTarget.name);
        }
        // else
        // {
        //     Debug.Log("No targets found");
        // }
    }

    IEnumerator AttackCooldownRoutine(float cooldown)
    {
        attackReady = false;
        yield return new WaitForSeconds(cooldown);
        attackReady = true;
    }

    public void setIsOpponent(bool boolean)
    {
        isOpponent = boolean;
    }

    void Update()
    {
        

        if (GameManager.Instance.gameStarted)
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
                    animator.SetBool("isMoving", false);
                }
                return;
            }

           

            m_Distance = Vector3.Distance(transform.position, m_CurrentTarget.position);

            if (m_Distance < AttackDistance * 0.75)
            {
                // STOP AND ATTACK

                if (m_Agent.isOnNavMesh)
                {
                    //Debug.Log($"Arrived at target. My Health: {champion.Health.CurrentHealth} Opponent Health: {m_CurrentOpponent.Health.CurrentHealth}");
                    m_Agent.isStopped = true;
                    animator.SetBool("isMoving", false);

                    
                     Vector3 directionTowardsOpponent = m_CurrentOpponent.transform.position - transform.position;
                    directionTowardsOpponent.y = 0f;
                    Quaternion targetRotation = Quaternion.LookRotation(directionTowardsOpponent);
                    float rotationSpeed = 5f;
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    

                    if (m_CurrentOpponent != null)
                    {
                        if (attackReady)
                        {
                            // ATTACKING
                            animator.SetTrigger("Attack");

                            m_CurrentOpponent.TakeDamage(champion.Attack.Damage);
                            Debug.Log($"Dealt {champion.Attack.Damage} damage to the opponent. Opponent's remaining health: {m_CurrentOpponent.Health.CurrentHealth}");

                            // TODO SAID Fill mana when receiving damage or giving damage

                            AudioManager.Instance.PlayAttack();
                            AudioManager.Instance.PlayHurt();

                            StartCoroutine(AttackCooldownRoutine(champion.Attack.Cooldown));
                        }

                        // TODO SAID: If MANA FULL : attack with special attack
                    }
                }
            }
            else
            {
                if (champion.currentUnitMode == UnitMode.Defense)
                {
                    m_Agent.isStopped = true;

                } else {
                     if (m_Agent.isOnNavMesh)
                    {
                        m_Agent.isStopped = false;
                        animator.SetBool("isMoving", true);

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
    }
}
