using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Unity.AI.Navigation;
using TMPro;

using static ChampionsDatabaseSO;

public class ArenaGenerator : MonoBehaviour
{    
    [Serializable]
    public class Arena
    {
        public string name;
        public string layout;

        public string enemyCoord;

        public int[][] GetLayoutArray()
        {
            string[] rows = layout.Split(',');
            int[][] layoutArray = new int[rows.Length][];
            for (int i = 0; i < rows.Length; i++)
            {
                layoutArray[i] = Array.ConvertAll(rows[i].ToCharArray(), c => (int)char.GetNumericValue(c));
            }
            return layoutArray;
        }

        // public (int enemyType, Vector2Int position, bool isKing, bool isDefending)[] GetEnemyCoordinates()
        // {
        //     if (string.IsNullOrEmpty(enemyCoord))
        //     {
        //         return null;
        //     }

        //     string[] pairs = enemyCoord.Split(';');
        //     var coordinates = new (int enemyType, Vector2Int position, bool isKing, bool isDefending)[pairs.Length];
        //     for (int i = 0; i < pairs.Length; i++)
        //     {
        //         Debug.Log("Pair: " + pairs[i]);
        //         string[] values = pairs[i].Split(',');
        //         if (values.Length != 5)
        //         {
        //             Debug.LogError("Invalid enemy coordinate format: " + pairs[i]);
        //             continue;
        //         }

        //         if (int.TryParse(values[0], out int enemyType) && int.TryParse(values[1], out int x) && int.TryParse(values[2], out int y)&& bool.TryParse(values[3], out bool isKing) && bool.TryParse(values[4], out bool isDefending))
        //         {
        //             coordinates[i] = (enemyType, new Vector2Int(x, y), isKing, isDefending);
        //         }
        //         else
        //         {
        //             Debug.LogError("Invalid enemy coordinate values: " + pairs[i]);
        //         }
        //     }
        //     return coordinates;
        // }

        public (int enemyType, Vector2Int position, bool isKing, bool isDefending)[] GetEnemyCoordinates()
        {
            if (string.IsNullOrWhiteSpace(enemyCoord))
            {
                return Array.Empty<(int, Vector2Int, bool, bool)>();
            }

            var coordinates = new List<(int, Vector2Int, bool, bool)>();
            string[] pairs = enemyCoord.Split(';', StringSplitOptions.RemoveEmptyEntries);
            List<string> errors = new();

            for (int i = 0; i < pairs.Length; i++)
            {
                string pair = pairs[i];
                string[] values = pair.Split(',');

                if (values.Length != 5)
                {
                    errors.Add($"Pair {i}: Expected 5 values, got {values.Length} - '{pair}'");
                    continue;
                }

                if (!int.TryParse(values[0], out int enemyType))
                {
                    errors.Add($"Pair {i}: Invalid enemy type '{values[0]}'");
                    continue;
                }

                if (!int.TryParse(values[1], out int x))
                {
                    errors.Add($"Pair {i}: Invalid X coordinate '{values[1]}'");
                    continue;
                }

                if (!int.TryParse(values[2], out int y))
                {
                    errors.Add($"Pair {i}: Invalid Y coordinate '{values[2]}'");
                    continue;
                }

                if (!bool.TryParse(values[3], out bool isKing))
                {
                    errors.Add($"Pair {i}: Invalid king flag '{values[3]}'");
                    continue;
                }

                if (!bool.TryParse(values[4], out bool isDefending))
                {
                    errors.Add($"Pair {i}: Invalid defending flag '{values[4]}'");
                    continue;
                }

                coordinates.Add((enemyType, new Vector2Int(x, y), isKing, isDefending));
            }

            if (errors.Count > 0)
            {
                Debug.LogError($"Enemy coordinate parsing errors:\n{string.Join("\n", errors)}");
            }

            return coordinates.ToArray();
        }
    }

    [Serializable]
    public class ArenaData
    {
        public Arena[] arenas;
    }

    [SerializeField] public int arenaIndex;

    private GameManager gameManager;

    List<ChampionData> allChampions;
    public NavMeshSurface navMesh;
    public GameObject playground;
    public GameObject obstacleTilePrefab;
    public GameObject holeTilePrefab;
    public GameObject sandTilePrefab;
    public GameObject classicTilePrefab;

    [SerializeField] public TextMeshProUGUI levelText;

    public string jsonFileName = "arenas.json";

    private Dictionary<int, GameObject> prefabMapping;

    void Start()
    {
        gameManager = GameManager.Instance;
        arenaIndex = SceneData.LevelInt;
        Debug.Log("Current level: " + arenaIndex);

        levelText.text = "Level " + (arenaIndex + 1);

        // Préparation des correspondances
        prefabMapping = new Dictionary<int, GameObject>
        {
            { 0, classicTilePrefab },
            { 1, sandTilePrefab },
            { 2, obstacleTilePrefab },
            { 3, holeTilePrefab }
        };

        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string json = File.ReadAllText(filePath);
        ArenaData arenaData = JsonUtility.FromJson<ArenaData>(json);

        if (arenaData != null && arenaData.arenas != null && arenaData.arenas.Length > 0)
        {
            foreach (var arena in arenaData.arenas)
            {
                if (arena != null)
                {
                    // Debug.Log($"Arena: {JsonUtility.ToJson(arena)}");
                    int[][] layoutArray = arena.GetLayoutArray();
                    if (layoutArray == null)
                    {
                        Debug.LogError($"La disposition de l'arène '{arena.name}' est nulle.");
                    }
                    var enemyCoordinates = arena.GetEnemyCoordinates();
                    if (enemyCoordinates == null)
                    {
                        Debug.LogError($"Les coordonnées des Ennemys de l'arène '{arena.name}' sont nulles.");
                    }
                }
                else
                {
                    Debug.LogError("Une arène est nulle.");
                }
            }

            GenerateArena(arenaData.arenas[arenaIndex]); // Génère la première arène par défaut
            
        }
        else
        {
            Debug.LogError("Les données de l'arène sont nulles ou vides.");
        }
    }

    void GenerateEnnemy(int enemyType, Vector3 position, bool isKing, bool isDefending)
    {
        Debug.Log("Generating enemy of type " + enemyType + " at position " + position);
        // Load the prefab from the Resources folder
        string prefabPath = string.Empty;
        string enemyName = string.Empty;
        string entityId= string.Empty;
        switch (enemyType)
        {
            case 5:
                prefabPath = "Prefabs/Barbare";
                enemyName = "BarbareEnnemy";
                entityId = "Barbare";
                break;
            case 6:
                prefabPath = "Prefabs/Chevalier";
                enemyName = "ChevalierEnnemy";
                entityId = "Chevalier";
                break;
            case 7:
                prefabPath = "Prefabs/Magicien";
                enemyName = "MagicienEnnemy";
                entityId = "Magicien";
                break;
            case 8:
                prefabPath = "Prefabs/Archer";
                enemyName = "ArcherEnnemy";
                entityId = "Archer";
                break;
            case 9:
                prefabPath = "Prefabs/Robinhood";
                enemyName = "RobinhoodEnnemy";
                entityId = "Robinhood";
                break;
            default:
                Debug.LogError("Invalid enemy type: " + enemyType);
                return;
        }


        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            a_Champion newEnnemy = gameManager.championsDatabase.SpawnChampion(enemyName, position, entityId , true);
            if (isDefending)
            {
                newEnnemy.currentUnitMode = UnitMode.Defense;
            }
            gameManager.AddEntityToEnnemyEntities(newEnnemy);
            newEnnemy.transform.SetParent(GameManager.Instance.team2Parent.transform);
        }
        else
        {
            Debug.LogError("Prefab not found in Resources folder! "+ prefabPath);
        }
    }

    void GenerateArena(Arena arena)
    {
        if (arena == null)
        {
            Debug.LogError("L'arène est nulle.");
            return;
        }

        if (string.IsNullOrEmpty(arena.layout))
        {
            Debug.LogError("La disposition de l'arène est nulle ou vide.");
            return;
        }


        var enemyCoordinates = arena.GetEnemyCoordinates();
        if (enemyCoordinates != null)
        {
            foreach (var (enemyType, position, isKing, isDefending) in enemyCoordinates)
            {
                if (position != null)
                {
                   
                    GenerateEnnemy(enemyType, new Vector3(position.x * 2 - 9f, 0, position.y * 2 - 9f), isKing, isDefending);
                }
                else
                {
                    Debug.LogError("Position is null for enemyType: " + enemyType);
                }
            }
        }else
        {
            Debug.LogError("enemyCoordinates is null.");
        }

        int[][] layoutArray = arena.GetLayoutArray();
        int height = layoutArray.Length;

        for (int y = 0; y < height; y++)
        {
            if (layoutArray[y] == null)
            {
                Debug.LogError("La ligne de disposition de l'arène est nulle à l'index " + y);
                continue;
            }

            for (int x = 0; x < layoutArray[y].Length; x++)
            {
                int type = layoutArray[y][x];
                if (prefabMapping.ContainsKey(type))
                {
                    // Adjust the y-coordinate to revert symmetrically vertically
                    GameObject tile = Instantiate(prefabMapping[type], new Vector3((height - 1 - x)  * 2 - 9f, 0, y* 2 - 9f), Quaternion.identity);
                    tile.transform.localScale = new Vector3(0.2f, 1, 0.2f); // Adjust the scale as needed
                    tile.transform.SetParent(playground.transform);



                    // Check if the type is 3 and assign the "Hole" tag
                    if (type == 3)
                    {
                        tile.tag = "Hole"; // Assign the "Hole" tag
                        try
                        {
                            navMesh.BuildNavMesh();
                        }
                        catch (Exception)
                        {
                            //Debug.LogWarning("Failed to build NavMesh: " + ex.Message);
                        }
                    }
                    
                    if (type == 2)
                    {
                        tile.tag = "Obstacle"; // Assign the "Obstacle" tag
                        
                        try
                        {
                            navMesh.BuildNavMesh();
                        }
                        catch (Exception)
                        {
                            //Debug.LogWarning("Failed to build NavMesh: " + ex.Message);
                        }
                    }
                    if(type == 1)
                    {
                        tile.tag = "Sand"; // Assign the "Sand" tag
                    }
                    if (type == 0)
                    {
                        tile.tag = "ClassicTile"; // Assign the "Classic" tag
                    }
                    // }else if (type == 4)
                    // {
                    //     navMesh.BuildNavMesh();
                    // }

                }
                else
                {
                    Debug.LogWarning("Type de tuile inconnu: " + type);
                }
            }
        }
    }
}