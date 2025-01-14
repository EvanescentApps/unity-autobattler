using System.Collections.Generic;
using UnityEngine;

public class a_Champion : MonoBehaviour
{
    public a_Entity Entity { get; private set; }
    public a_Health Health { get; private set; }
    public a_Attack Attack { get; private set; }
    public a_Movement Movement { get; private set; }

    public HealthBar barPrefab;
    protected HealthBar healthBar;
    protected bool dead = false;

    public List<a_Item> Items { get; private set; } = new();

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
}
