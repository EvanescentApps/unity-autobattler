using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Champions Database", menuName = "CustomSO/ChampionsDatabase")]
public class ChampionsDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class EntityData
    {
        public string name;
        public int price;
    }

    [System.Serializable]
    public class HealthData
    {
        public float maxHealth;
        public float armor;
    }

    [System.Serializable]
    public class AttackData
    {
        public float cooldown;
        public float range;
        public float damage;
    }

    [System.Serializable]
    public class MovementData
    {
        public float speed;
    }

    [System.Serializable]
    public class ChampionData
    {
        [Header("Basic Info")]
        [SerializeField] public GameObject prefab;
        public string description;
        public Sprite icon;

        [Header("Stats")]
        public EntityData entityStats;
        public HealthData healthStats;
        public AttackData attackStats;
        public MovementData movementStats;

        // Helper method to get the Champion component
        public a_Champion GetChampion()
        {
            return prefab.GetComponent<a_Champion>();
        }

        // Helper method to initialize a spawned champion with these stats
        public void InitializeChampion(a_Champion champion)
        {
            var entity = champion.gameObject.AddComponent<a_Entity>();
            entity.Initialize(entityStats.name, entityStats.price);

            var health = champion.gameObject.AddComponent<a_Health>();
            health.Initialize(healthStats.maxHealth, healthStats.armor);

            var attack = champion.gameObject.AddComponent<a_Attack>();
            attack.Initialize(attackStats.cooldown, attackStats.range, attackStats.damage);

            var movement = champion.gameObject.AddComponent<a_Movement>();
            movement.Initialize(movementStats.speed);

            // Initialize Champion after all components are ready
            champion.Initialize(entity, health, attack, movement);
        }
    }

    public List<ChampionData> allChampions = new List<ChampionData>();

    // Helper method to spawn a champion
    public a_Champion SpawnChampion(string championName, Vector3 position)
    {
        ChampionData data = allChampions.Find(c => c.entityStats.name == championName);
        if (data != null)
        {
            GameObject instance = Instantiate(data.prefab, position, Quaternion.identity);
            a_Champion champion = instance.GetComponent<a_Champion>();
            data.InitializeChampion(champion);
            return champion;
        }
        Debug.LogError($"Champion {championName} not found in database!");
        return null;
    }
}
