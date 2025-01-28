using System.Collections.Generic;
using UnityEngine;

public class a_Champion : MonoBehaviour
{
    public a_Entity Entity { get; private set; }
    public a_Health Health { get; private set; }
    public a_Attack Attack { get; private set; }
    public a_Movement Movement { get; private set; }
    public HealthBar barPrefab;
    public GameObject crownPrefab; 
    protected HealthBar healthBar;
    protected GameObject crownInstance;
    protected bool dead = false;
    protected bool isKing = false;
    public bool IsKing => isKing;
    public List<a_Item> Items { get; private set; } = new();

    [SerializeField] private Vector3 crownOffset = new Vector3(0, 0.5f, 0); 

    public void Initialize(a_Entity entity, a_Health health, a_Attack attack, a_Movement movement)
    {
        Entity = entity;
        Health = health;
        Attack = attack;
        Movement = movement;
        SetupHealthBar();
    }

    private void SetupHealthBar()
    {
        healthBar = Instantiate(barPrefab, this.transform);
        healthBar.Setup(this.transform, Health.maxHealth);
    }

    public void SetKingStatus(bool kingStatus)
    {
        if (isKing == kingStatus) return; 

        isKing = kingStatus;

        if (isKing)
        {
            if (crownInstance == null && crownPrefab != null)
            {
                crownInstance = Instantiate(crownPrefab, transform);
                crownInstance.transform.localPosition = crownOffset;
            }
            if (crownInstance != null)
            {
                crownInstance.SetActive(true);
            }
        }
        else
        {
            if (crownInstance != null)
            {
                crownInstance.SetActive(false);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        Health.TakeDamage(amount);
        healthBar.UpdateBar(Health.CurrentHealth);
        if (Health.CurrentHealth <= 0 && !dead)
        {
            dead = true;
            GameManager.Instance.UnitDead(this);
        }
    }

    private void OnDestroy()
    {
        if (crownInstance != null)
        {
            Destroy(crownInstance);
        }
    }
}
