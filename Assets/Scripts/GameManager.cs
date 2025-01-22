using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ChampionsDatabaseSO;
public class GameManager : Manager<GameManager>
{
    [HideInInspector] [SerializeField] private ChampionsDatabaseSO championsDatabase;

    public Transform team1Parent;
    public Transform team2Parent;

    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action<a_Champion> OnUnitDied;

    List<a_Champion> playerEntities = new List<a_Champion>();
    List<a_Champion> ennemyEntities = new List<a_Champion>();

    // public void OnEntityBought(ChampionsDatabaseSO.ChampionData championData)
    // {
    //     a_Champion newEntity = entitiesDatabase.SpawnChampionInStore(championData.entityStats.name,new Vector3(0f, 0f, 13f)); //Instantiate(championData.prefab, team1Parent).GetComponent<a_Champion>();
    //     newEntity.gameObject.name = championData.entityStats.name;
    //     // newEntity.gameObject.tag = "Player";
    //     playerEntities.Add(newEntity);

    // }

    // TODO : SET TAGS

    // Method to add entity to player entities
    public void AddEntityToPlayerEntities(a_Champion newEntity)
    {
        playerEntities.Add(newEntity);
        Debug.Log("Player entities count: " + playerEntities.Count);
    }

    public void OnEnnemySpawn(ChampionsDatabaseSO.ChampionData championData)
    {
        a_Champion newEntity = Instantiate(championData.prefab, team2Parent).GetComponent<a_Champion>();
        newEntity.gameObject.name = championData.entityStats.name;
        newEntity.gameObject.tag = "Ennemy";
        ennemyEntities.Add(newEntity);
    }

    Dictionary<string, float> championPositions = new Dictionary<string, float>
        {
            { "Barbare", 7.5f },
            { "Magicien", 5.0f },
            { "Chevalier", 2.5f }
        };

    private void Start()
    {
        // Load the ChampionsDatabaseSO asset from the Resources folder
        championsDatabase = Resources.Load<ChampionsDatabaseSO>("Champions Database");
        if (championsDatabase == null)
        {
            Debug.LogError("ChampionsDatabaseSO not found in Resources folder!");
            return;
        }
        foreach (var champion in championPositions)
        {
            Vector3 spawnPosition = new Vector3(champion.Value, 0f, 13f);
            championsDatabase.SpawnChampionInStore(champion.Key, spawnPosition);
        }
    }

    public void AnimateDeath(Vector3 deathPosition) {
        GameObject desintegrateParticlePrefab = Resources.Load<GameObject>("Prefabs/CubeDesintegrate");
        if (desintegrateParticlePrefab != null)
        {
            GameObject instance = Instantiate(desintegrateParticlePrefab, deathPosition, Quaternion.identity);
            instance.name = "DesintegrationParticle";
        }
        else
        {
            Debug.LogError("Prefab not found in Resources folder!");
        }
    }

    public void RespawnEntityInStore(string entityName)
{
    if (championPositions.TryGetValue(entityName, out float x))
    {
        Vector3 spawnPosition = new Vector3(x, 0f, 13f);
        championsDatabase.SpawnChampionInStore(entityName, spawnPosition);
    }
    else
    {
        Debug.LogError("Entity name not found in champion positions dictionary: " + entityName);
    }
}

    public List<a_Champion> GetEntitiesAgainst(Team against)
    {
        if (against == Team.Team1)
            return ennemyEntities;
        else
            return playerEntities;
    }

    public void UnitDead(a_Champion entity)
    {
        playerEntities.Remove(entity);
        ennemyEntities.Remove(entity);

        OnUnitDied?.Invoke(entity);

        Destroy(entity.gameObject);
        AnimateDeath(entity.transform.position);

    }
}

public enum Team
{
    Team1,
    Team2
}
