using System.Collections.Generic;
using UnityEngine;

// Abstract class for Champion
public abstract class ChampionAbstract: MonoBehaviour
{
    public Entity Entity { get; private set; }
    public Health Health { get; private set; }
    public Attack Attack { get; private set; }
    public Movement Movement { get; private set; }

    public List<Item> Items { get; private set; } = new();

    
    public HealthBar barPrefab;
    protected HealthBar healthBar;
    protected bool dead = false;

    protected ChampionAbstract(Entity entity, Health health, Attack attack, Movement movement)
    {
        Entity = entity;
        Health = health;
        Attack = attack;
        Movement = movement;
    }

    public abstract void Initialize(Entity entity, Health health, Attack attack, Movement movement);
    public abstract void TakeDamage(float amount);
}
