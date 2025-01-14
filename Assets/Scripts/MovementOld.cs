// using System.Collections.Generic;

// // Abstract class for Movement
// public abstract class Movement: Attribute
// {
//     public float Speed { get; private set; }
//     public List<float> SpeedMultipliers { get; private set; } = new();

//     protected Movement(float speed)
//     {
//         Speed = speed;
//     }

//     public abstract void Upgrade();

//     public float GetSpeed()
//     {
//         float multiplier = 1;
//         foreach (var speedMultiplier in SpeedMultipliers)
//         {
//             multiplier *= speedMultiplier;
//         }
//         return Speed * multiplier;
//     }

//     public void SetSpeedMultiplier(float multiplier)
//     {
//         SpeedMultipliers.Add(multiplier);
//     }

//     public List<float> GetSpeedMultipliers()
//     {
//         return SpeedMultipliers;
//     }
// }


