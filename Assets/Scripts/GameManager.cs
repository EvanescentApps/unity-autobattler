using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ChampionsDatabaseSO;
public class GameManager : Manager<GameManager>
{
    public ChampionsDatabaseSO entitiesDatabase;

    public Transform team1Parent;
    public Transform team2Parent;

    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action<a_Champion> OnUnitDied;

    List<a_Champion> team1Entities = new List<a_Champion>();
    List<a_Champion> team2Entities = new List<a_Champion>();

    int unitsPerTeam = 6;

    public void OnEntityBought(ChampionsDatabaseSO.ChampionData championData)
    {
        a_Champion newEntity = Instantiate(championData.prefab, team1Parent).GetComponent<a_Champion>();
        newEntity.gameObject.name = championData.entityStats.name;
        team1Entities.Add(newEntity);

        // newEntity.Setup(Team.Team1, GridManager.Instance.GetFreeNode(Team.Team1));
    }

    private void Start()
    {
        Vector3 spawnPosition = new Vector3(15.6499996f, 0.140000105f, 12.7299995f);
        entitiesDatabase.SpawnChampion("Barbare", spawnPosition);
    }

    public List<a_Champion> GetEntitiesAgainst(Team against)
    {
        if (against == Team.Team1)
            return team2Entities;
        else
            return team1Entities;
    }

    public void UnitDead(a_Champion entity)
    {
        team1Entities.Remove(entity);
        team2Entities.Remove(entity);

        OnUnitDied?.Invoke(entity);

        Destroy(entity.gameObject);
    }
}

public enum Team
{
    Team1,
    Team2
}
