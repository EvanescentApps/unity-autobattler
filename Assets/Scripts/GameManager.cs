using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ChampionsDatabaseSO;

using TMPro;
using UnityEngine.UI;

public class GameManager : Manager<GameManager>
{
    [SerializeField] public ChampionsDatabaseSO championsDatabase;
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] public Button startButton;
    [SerializeField] public Button resetButton;

    [SerializeField] public GameObject storePanel;
    [SerializeField] public GameObject UnitInfos;

    [SerializeField] public TextMeshProUGUI TimerText;
    [SerializeField] public TextMeshProUGUI PlayersAliveText;
    [SerializeField] public TextMeshProUGUI EnnemiesAliveText;

    [SerializeField] public GameObject Popup;

    [SerializeField] public TextMeshProUGUI PopupTitleText;
    [SerializeField] public TextMeshProUGUI PopupRecapText;
    [SerializeField] public TextMeshProUGUI PopupActionButtonText;

    [SerializeField] public Button PopupHomeButton;
    [SerializeField] public Button PopupActionButton;


    public Transform team1Parent;
    public Transform team2Parent;



    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action<a_Champion> OnUnitDied;

    public bool IsGameStarted { get; private set; } = false;

    List<a_Champion> playerEntities = new List<a_Champion>();
    List<a_Champion> ennemyEntities = new List<a_Champion>();
    public int Money { get; private set; }

    private int initialMoney = 300;

    public void SpendMoney(int amount)
    {
        Debug.Log("Spending money: " + amount);
        Money -= amount;
        moneyText.text = Money.ToString();
    }

    public void setMoney(int amount)
    {
        Money = amount;
        moneyText.text = Money.ToString();
    }

    private float elapsedTime;
    private bool isCounting;


  

    public void killUnit(a_Champion unit)
    {
        playerEntities.Remove(unit);
        ennemyEntities.Remove(unit);
        Vector3 deathPosition = unit.transform.position;
        Destroy(unit.gameObject);
        AnimateDeath(deathPosition);
    }

    public bool CanAfford(int amount)
    {
        return amount <= Money;
    }

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

    public void AddEntityToEnnemyEntities(a_Champion newEntity)
    {
        ennemyEntities.Add(newEntity);
        Debug.Log("Ennemies entities count: " + playerEntities.Count);
    }

    public void StartBattle()
    {
        // TODO : CHECK IF UNITS ARE PLACED, ELSE WARN
        if (playerEntities.Count == 0)
        {
            Debug.Log("Cannot start the battle without any units placed!");
            return;
        }
        IsGameStarted = true;
        Debug.Log("Hiding buttons...");

        startButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        storePanel.SetActive(false);
        UnitInfos.SetActive(false);
        TimerText.gameObject.SetActive(true);
        PlayersAliveText.gameObject.SetActive(true);
        EnnemiesAliveText.gameObject.SetActive(true);

        isCounting = true;
        elapsedTime = 0f;
        // ShowVictoryPopup(); 
       // OnRoundStart?.Invoke(); ??? TODO
    }

    private void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;
            TimerText.text = $"Time: {elapsedTime:F2}";
            PlayersAliveText.text = $"Players alive: {playerEntities.Count}";
            EnnemiesAliveText.text = $"Ennemies alive: {ennemyEntities.Count}";
            if (playerEntities.Count == 0)
            {
                ShowDefeatPopup();
                isCounting = false;
            }
            else if (ennemyEntities.Count == 0)
            {
                ShowVictoryPopup();
                isCounting = false;
            }
        }
    }

    public void ResetBattle()
    {
        if (IsGameStarted)
        {
            Debug.Log("Cannot reset the units during the battle!");
        } else {
            Debug.Log("Resetting the battle...");
            // TODO : RESET UNITS POSITIONS
            // TODO : RESET MONEY
            // TODO : reset list of Champions and destroy them

             // 1) Reset unit positions (example: move them to origin)
            foreach (var champ in playerEntities)
            {
                champ.transform.position = Vector3.zero;
            }
            foreach (var champ in ennemyEntities)
            {
                champ.transform.position = Vector3.zero;
            }

            // 2) Reset money
            setMoney(initialMoney); 
            moneyText.text = Money.ToString();

            // 3) Destroy champions and clear lists
            foreach (var champ in playerEntities) Destroy(champ.gameObject);
            playerEntities.Clear();
            foreach (var champ in ennemyEntities) Destroy(champ.gameObject);
            ennemyEntities.Clear();
        }
        // OnRoundEnd?.Invoke(); ??? TODO
    }

    public void ShowDefeatPopup()
    {
        PopupTitleText.text = "Défaite!";
        string time = elapsedTime.ToString("F2");
        PopupRecapText.text = "Toutes vos unités ont été vaincues en " + time + " secondes!";
        PopupActionButtonText.text = "Retenter";
        Popup.SetActive(true);
        Debug.Log("Victory popup shown!");
        Debug.Log("Defeat popup shown!");
    }

    public void ShowVictoryPopup()
    {
        PopupTitleText.text = "Victoire!";
        string time = elapsedTime.ToString("F2");
        PopupRecapText.text = "Tous les unités ennemies ont été vaincues en " + time + " secondes!";
        PopupActionButtonText.text = "Niveau suivant";
        Popup.SetActive(true);
        Debug.Log("Victory popup shown!");
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
            { "Chevalier", 2.5f },
            { "Archer", 0.0f },
            { "Robinhood", -2.5f },
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

        Debug.Log("GameManager loaded!");
        foreach (var champion in championPositions)
        {
            Vector3 spawnPosition = new Vector3(champion.Value, 0f, 13f);
            Debug.Log("Spawning champion: " + champion.Key + " at position: " + spawnPosition);
            championsDatabase.SpawnChampion(champion.Key, spawnPosition, champion.Key);
        }

        setMoney(initialMoney);
        moneyText.text = Money.ToString();
        
    }

    public void AnimateDeath(Vector3 deathPosition) {
        GameObject desintegrateParticlePrefab = Resources.Load<GameObject>("Prefabs/CubeDesintegrate");
        for (int i = 0; i < 100; i++)
        {
            GameObject instance = Instantiate(desintegrateParticlePrefab, deathPosition, Quaternion.identity);
            instance.name = "DesintegrationParticle";
        }
    }

    public void RespawnEntityInStore(string entityName)
{
    if (championPositions.TryGetValue(entityName, out float x))
    {
        Vector3 spawnPosition = new Vector3(x, 0f, 13f);
        championsDatabase.SpawnChampion(entityName, spawnPosition, entityName);
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
