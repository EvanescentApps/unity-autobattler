using UnityEngine;

// Abstract class for Health
public abstract class Health : Attribute
{
    public float Armor { get; protected set; }
    public float CurrentHealth { get; protected set; }
    public float maxHealth { get; protected set; }

    protected Health(float initialHealth)
    {
        maxHealth = initialHealth;
        CurrentHealth = initialHealth;
        Armor = 0; 
    }
    protected Health(float initialHealth, int armor)
    {
        maxHealth = initialHealth;
        CurrentHealth = initialHealth;
        Armor = armor;
    }

    public override void Upgrade(int upgrade)
    {
        maxHealth += upgrade;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public void SetHealth(float health)
    {
        CurrentHealth = health;
    }
}