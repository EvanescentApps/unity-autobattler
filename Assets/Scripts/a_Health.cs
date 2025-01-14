using UnityEngine;

// Abstract class for Health
public class a_Health : Attribute
{
    public float maxHealth;
    public float CurrentHealth { get; private set; }
    public float Armor { get; private set; }

    public a_Health(float initialHealth)
    {
        CurrentHealth = initialHealth;
    }

    public void Initialize(float maxHp, float armor)
    {
        maxHealth = maxHp;
        Armor = armor;
        CurrentHealth = maxHealth;
    }

    public override void Upgrade(int upgrade)
    {
        maxHealth += upgrade;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        float reducedDamage = amount;

        // Check if Armor is set and calculate reduced damage
        if (Armor > 0)
        {
            reducedDamage = amount * (1f - Armor / 100f);
            reducedDamage = Mathf.Clamp(reducedDamage, 0f, amount);
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - reducedDamage);
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