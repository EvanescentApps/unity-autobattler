using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Unity.AI.Navigation;
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

        public (int enemyType, Vector2Int position)[] GetEnemyCoordinates()
        {
            if (string.IsNullOrEmpty(enemyCoord))
            {
                return null;
            }

            string[] pairs = enemyCoord.Split(';');
            var coordinates = new (int enemyType, Vector2Int position)[pairs.Length];
            for (int i = 0; i < pairs.Length; i++)
            {
                string[] values = pairs[i].Split(',');
                if (values.Length != 3)
                {
                    Debug.LogError("Invalid enemy coordinate format: " + pairs[i]);
                    continue;
                }

                if (int.TryParse(values[0], out int enemyType) && int.TryParse(values[1], out int x) && int.TryParse(values[2], out int y))
                {
                    coordinates[i] = (enemyType, new Vector2Int(x, y));
                }
                else
                {
                    Debug.LogError("Invalid enemy coordinate values: " + pairs[i]);
                }
            }
            return coordinates;
        }
    }

    [Serializable]
    public class ArenaData
    {
        public Arena[] arenas;
    }


    private GameManager gameManager;

    List<ChampionData> allChampions;
    public NavMeshSurface navMesh;
    public GameObject playground;
    public GameObject obstacleTilePrefab;
    public GameObject holeTilePrefab;
    public GameObject sandTilePrefab;
    public GameObject classicTilePrefab;
    public string jsonFileName = "arenas.json";

    private Dictionary<int, GameObject> prefabMapping;

    void Start()
    {


        gameManager = GameManager.Instance;

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

            GenerateArena(arenaData.arenas[0]); // Génère la première arène par défaut
            
        }
        else
        {
            Debug.LogError("Les données de l'arène sont nulles ou vides.");
        }
    }

    void GenerateEnnemy(int enemyType, Vector3 position)
    {
        Debug.Log("Generating enemy of type " + enemyType + " at position " + position);
        // Load the prefab from the Resources folder
        string prefabPath = string.Empty;
        string enemyName = string.Empty;
        string entityId= string.Empty;
        switch (enemyType)
        {
            case 5:
                prefabPath = "Prefabs/Barbarian";
                enemyName = "BarbarianEnemy";
                entityId = "Barbare";
                break;
            case 6:
                prefabPath = "Prefabs/Knight";
                enemyName = "KnightEnemy";
                entityId = "Chevalier";
                break;
            case 7:
                prefabPath = "Prefabs/Mage";
                enemyName = "MageEnemy";
                entityId = "Magicien";
                break;
            case 8:
                prefabPath = "Prefabs/EnemyType8";
                break;
            case 9:
                prefabPath = "Prefabs/EnemyType9";
                break;
            default:
                Debug.LogError("Invalid enemy type: " + enemyType);
                return;
        }


        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            gameManager.championsDatabase.SpawnChampion(enemyName, position, entityId , true);

            
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
            foreach (var (enemyType, position) in enemyCoordinates)
            {
                if (position != null)
                {
                    
                    GenerateEnnemy(enemyType, new Vector3(position.x * 2 - 9f, 0, position.y * 2 - 9f));
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
                        navMesh.BuildNavMesh();
                    }
                    else if (type == 4)
                    {
                        navMesh.BuildNavMesh();
                    }
                    if (type == 2)
                    {
                        tile.tag = "Obstacle"; // Assign the "Obstacle" tag
                    }
                    if(type == 1)
                    {
                        tile.tag = "Sand"; // Assign the "Sand" tag
                    }
                    if (type == 0)
                    {
                        tile.tag = "ClassicTile"; // Assign the "Classic" tag
                    }
                }
                else
                {
                    Debug.LogWarning("Type de tuile inconnu: " + type);
                }
            }
        }
    }
}