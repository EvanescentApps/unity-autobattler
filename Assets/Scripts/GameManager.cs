using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ChampionsDatabaseSO;
using UnityEngine.SceneManagement;

using TMPro;
using UnityEngine.UI;
using System.Linq;

public enum UnitMode
{
    Attack,
    Defense
}

public class GameManager : Manager<GameManager>
{
    [SerializeField] public ChampionsDatabaseSO championsDatabase;
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] public Button startButton;
    [SerializeField] public Button resetButton;

    [SerializeField] public GameObject storePanel;
    [SerializeField] public GameObject UnitInfos;
    [SerializeField] public GameObject ItemsDescription;
    [SerializeField] public GameObject ItemsObjects;

    [SerializeField] public TextMeshProUGUI TimerText;
    [SerializeField] public TextMeshProUGUI PlayersAliveText;
    [SerializeField] public TextMeshProUGUI EnnemiesAliveText;

    [SerializeField] public GameObject Popup;

    [SerializeField] public TextMeshProUGUI PopupTitleText;
    [SerializeField] public TextMeshProUGUI PopupRecapText;

    [SerializeField] public TextMeshProUGUI PopupDamageRecapText;
    [SerializeField] public TextMeshProUGUI PopupActionButtonText;

    [SerializeField] public Button PopupHomeButton;
    [SerializeField] public Button PopupActionButton;


    [SerializeField] public Transform team1Parent;
    [SerializeField] public Transform team2Parent;

    [SerializeField] public Transform unitStoreParent;

    public bool victory = false;

    public UnitMode currentUnitMode;

    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action<a_Champion> OnUnitDied;

    public bool gameStarted { get; private set; } = false;

    [SerializeField] List<a_Champion> playerEntities = new List<a_Champion>();
    [SerializeField] List<a_Champion> ennemyEntities = new List<a_Champion>();
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

    public void SetCurrentModeAttack() {
        currentUnitMode = UnitMode.Attack;
        Debug.Log("Current unit mode set to Attack");
    }

    public void SetCurrentModeDefense() {
        currentUnitMode = UnitMode.Defense;
        Debug.Log("Current unit mode set to Defense");
    }

    public void StartBattle()
    {
        // TODO : CHECK IF UNITS ARE PLACED, ELSE WARN
        if (playerEntities.Count == 0 || !playerEntities.Any(champion => champion.IsKing))
        {
            Debug.Log("Cannot start the battle without any units placed!");
            FindObjectOfType<ToastManager>().ShowToast("No units placed !", 2.0f);

            return;
        }
        gameStarted = true;
        Debug.Log("Hiding buttons...");

        startButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        storePanel.SetActive(false);
        UnitInfos.SetActive(false);
        ItemsDescription.SetActive(false);
        ItemsObjects.SetActive(false);
        TimerText.gameObject.SetActive(true);
        PlayersAliveText.gameObject.SetActive(true);
        EnnemiesAliveText.gameObject.SetActive(true);
        unitStoreParent.gameObject.SetActive(false);

        isCounting = true;
        elapsedTime = 0f;
       
    }

    public void ActionButtonPressed(){
        Debug.Log("Action button pressed!");

        if (victory) {
            SceneData.LevelInt+=1;
            Debug.Log("Loading next level: " + SceneData.LevelInt);
        } 
        SceneManager.LoadScene("GameScene");

    }

    private void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;
            TimerText.text = $"Time: {elapsedTime:F2}";
            PlayersAliveText.text = $"Players alive: {playerEntities.Count}";
            EnnemiesAliveText.text = $"Ennemies alive: {ennemyEntities.Count}";

            // Check if there are no player entities left
            if (playerEntities.Count == 0)
            {
                StartCoroutine(WaitAndShowDefeatPopup());
                isCounting = false;
            }
            // Check if there are no champions with IsKing == true
            else if (!playerEntities.Any(champion => champion.IsKing))
            {
                StartCoroutine(WaitAndShowDefeatPopup());
                isCounting = false;
            }

            // TODO !!!
            // else if (!ennemyEntities.Any(champion => champion.IsKing))
            // {
            //     StartCoroutine(WaitAndShowVictoryPopup());
            //     isCounting = false;
            // }
            // Check if there are no enemy entities left
            else if (ennemyEntities.Count == 0)
            {
                StartCoroutine(WaitAndShowVictoryPopup());
                isCounting = false;
            }
        }
    }


    private IEnumerator WaitAndShowVictoryPopup()
    {
        yield return new WaitForSeconds(1f);
        victory = true;
        ShowVictoryPopup();
    }

    private IEnumerator WaitAndShowDefeatPopup()
    {
        yield return new WaitForSeconds(1f);
        ShowDefeatPopup();
    }


    public void ResetBattle()
    {
        if (gameStarted)
        {
            Debug.Log("Cannot reset the units during the battle!");
        } else {
            Debug.Log("Resetting the battle...");
    

            foreach (var champ in playerEntities)
            {
                champ.transform.position = Vector3.zero;
            }
           

            setMoney(initialMoney); 
            moneyText.text = Money.ToString();

            foreach (var champ in playerEntities) Destroy(champ.gameObject);
            playerEntities.Clear();
           
        }
    }

    public void ShowDefeatPopup()
    {
        PopupTitleText.text = "Défaite!";
        string time = elapsedTime.ToString("F2");
        PopupRecapText.text = "Vous avez perdu en " + time + " secondes :(";
        string healthInfo = "";
        for (int i = 0; i < ennemyEntities.Count; i++)
        {
            var champ = ennemyEntities[i];
            healthInfo += $"{champ.name}: {champ.Health.CurrentHealth}/{champ.Health.maxHealth}";
            if (i < ennemyEntities.Count - 1) healthInfo += ", ";
        }

        PopupDamageRecapText.text = "Santé des ennemis restants: " + healthInfo;
        PopupActionButtonText.text = "Retenter";
        Popup.SetActive(true);
        Debug.Log("Defeat popup shown!");
    }

    public void ShowVictoryPopup()
    {
        PopupTitleText.text = "Victoire!";
        string time = elapsedTime.ToString("F2");
        PopupRecapText.text = "Tous les unités ennemies ont été vaincues en " + time + " secondes!" + " Il vous reste " + playerEntities.Count + " unités.";

        string healthInfo = "";
        for (int i = 0; i < playerEntities.Count; i++)
        {
            var champ = playerEntities[i];
            healthInfo += $"{champ.name}: {champ.Health.CurrentHealth}/{champ.Health.maxHealth}";
            if (i < playerEntities.Count - 1) healthInfo += ", ";
        }

        PopupDamageRecapText.text = "Santé des unités restantes: " + healthInfo;
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

    Dictionary<string, float> firstRowChampionPositions = new Dictionary<string, float>
        {
            { "Barbare", 7.5f },
            { "Magicien", 5.0f },
            { "Chevalier", 2.5f },
            { "Archer", 0.0f },
            { "Robinhood", -2.5f },
        };

    Dictionary<string, float> secondRowChampionPositions = new Dictionary<string, float>
        {
            { "Skeleton_mage", 7.5f },
            { "Skeleton_rogue", 5.0f },
            { "Skeleton_warrior", 2.5f }
        };

    private void Start()
    {
        unitStoreParent.gameObject.SetActive(true);

        Popup.SetActive(false);

        championsDatabase = Resources.Load<ChampionsDatabaseSO>("Champions Database");
        if (championsDatabase == null)
        {
            Debug.LogError("ChampionsDatabaseSO not found in Resources folder!");
            return;
        }

        Debug.Log("GameManager loaded!");
        foreach (var champion in firstRowChampionPositions)
        {
            Vector3 spawnPosition = new Vector3(champion.Value, 0f, 13f);
            Debug.Log("Spawning champion: " + champion.Key + " at position: " + spawnPosition);
            a_Champion c = championsDatabase.SpawnChampion(champion.Key, spawnPosition, champion.Key);
            c.gameObject.transform.SetParent(unitStoreParent.transform);
        }

        foreach (var champion in secondRowChampionPositions)
        {
            Vector3 spawnPosition = new Vector3(champion.Value, 0f, 15f);
            Debug.Log("Spawning champion: " + champion.Key + " at position: " + spawnPosition);
            a_Champion c = championsDatabase.SpawnChampion(champion.Key, spawnPosition, champion.Key);
            c.gameObject.transform.SetParent(unitStoreParent.transform);
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
    if (firstRowChampionPositions.TryGetValue(entityName, out float x))
    {
        Vector3 spawnPosition = new Vector3(x, 0f, 13f);
        a_Champion c = championsDatabase.SpawnChampion(entityName, spawnPosition, entityName);
        c.gameObject.transform.SetParent(unitStoreParent.transform);
    } else if (secondRowChampionPositions.TryGetValue(entityName, out float x2)){
        Vector3 spawnPosition = new Vector3(x2, 0f, 15f);
        a_Champion c = championsDatabase.SpawnChampion(entityName, spawnPosition, entityName);
        c.gameObject.transform.SetParent(unitStoreParent.transform);
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
