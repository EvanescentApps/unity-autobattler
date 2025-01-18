using UnityEngine;

// Abstract class for Health
public class a_Health : Health
{
    public a_Health(float initialHealth) : base(initialHealth)
    {
    }

    public void Initialize(float maxHp, float armor)
    {
        maxHealth = maxHp;
        Armor = armor;
        CurrentHealth = maxHealth;
    }
}
