// using UnityEngine;

// // Abstract class for Health
// public abstract class HealthAbstract
// {
//     public float CurrentHealth { get; private set; }

//     protected HealthAbstract(float initialHealth)
//     {
//         CurrentHealth = initialHealth;
//     }

//     public abstract void Upgrade();

//     public void TakeDamage(float amount)
//     {
//         CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
//     }

//     public float GetHealth()
//     {
//         return CurrentHealth;
//     }

//     public void SetHealth(float health)
//     {
//         CurrentHealth = health;
//     }
// }