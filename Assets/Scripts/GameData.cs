// using UnityEngine;
// using System.Collections.Generic;

// // Concrete implementation of Entity
// public class ConcreteEntity : Entity
// {
//     public ConcreteEntity(string name, int price) : base(name, price) { }
// }


// // Concrete implementation of Champion
// public class ConcreteChampion : Champion
// {
//     public ConcreteChampion(Entity entity, Health health, Attack attack, Movement movement) 
//         : base(entity, health, attack, movement) { }
// }

// // Magicien class
// public class Magicien : ConcreteChampion
// {
//     public Magicien() : base(
//         new ConcreteEntity("Magicien", 100),
//         new Health(50f),
//         new Attack(2f, 10f, 2f),
//         new Movement(3f))
//     {
//     }
// }


// // Archer class
// public class Archer : ConcreteChampion
// {
//     public Archer() : base(
//         new ConcreteEntity("Archer", 100),
//         new Health( 20f),
//         new Attack(3f, 10f, 30f),
//         new Movement(5f))
//     {
//     }
// }

// // Chevalier class
// public class Chevalier : ConcreteChampion
// {
//     public Chevalier() : base(
//         new ConcreteEntity("Chevalier", 100),
//         new Health( 80f),
//         new Attack(3f, 2f, 0f),
//         new Movement(2f))
//     {
//     }
// }

// // Barbare class
// public class Barbare : ConcreteChampion
// {
//     public Barbare() : base(
//         new ConcreteEntity("Barbare", 100),
//         new Health( 50f),
//         new Attack(3f, 1f, 4f),
//         new Movement(6f))
//     {
//     }
// }

// // Robinhood class
// public class Robinhood : ConcreteChampion
// {
//     public Robinhood() : base(
//         new ConcreteEntity("Robinhood", 100),
//         new Health( 25f),
//         new Attack(2f, 10f, 30f),
//         new Movement( 7f))
//     {
//     }
// }
// // Example usage
// public class Game : MonoBehaviour
// {
//     void Start()
//     {
//            // Create instances of concrete classes
//         Magicien magicien = new();
//         Archer archer = new();
//         Chevalier chevalier = new();
//         Barbare barbare = new();
//         Robinhood robinhood = new();

//         // Use the instances
//         Debug.Log(magicien.Entity.GetName());
//         Debug.Log(archer.Health.GetHealth());
//         Debug.Log(chevalier.Attack.GetDamage());
//         Debug.Log(barbare.Movement.GetSpeed());
//         Debug.Log(robinhood.Entity.GetName());
//     }

// }

