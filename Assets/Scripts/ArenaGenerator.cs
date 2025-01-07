using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ArenaGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Arena
    {
        public string name;
        public int[][] layout;
    }

    [System.Serializable]
    public class ArenaData
    {
        public Arena[] arenas;
    }

    public GameObject obstacleTilePrefab;
    public GameObject holeTilePrefab;
    public GameObject sandTilePrefab;
    public GameObject classicTilePrefab;
    public string jsonFileName = "arenas.json";

    private Dictionary<int, GameObject> prefabMapping;


    // Sand=1, Obstacle=2, Hole=3

    void Start()
    {
        // Préparation des correspondances
        prefabMapping = new Dictionary<int, GameObject>
        {
            { 0, classicTilePrefab },
            { 1, sandTilePrefab },
            { 2, obstacleTilePrefab },
            { 3, holeTilePrefab }
        };

        // Chargement et génération
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ArenaData arenaData = JsonUtility.FromJson<ArenaData>(json);

            GenerateArena(arenaData.arenas[0]); // Génère la première arène par défaut
        }
        else
        {
            Debug.LogError("Fichier JSON non trouvé: " + filePath);
        }
    }

    void GenerateArena(Arena arena)
    {
        for (int y = 0; y < arena.layout.Length; y++)
        {
            for (int x = 0; x < arena.layout[y].Length; x++)
            {
                int type = arena.layout[y][x];
                if (prefabMapping.ContainsKey(type))
                {
                    Instantiate(prefabMapping[type], new Vector3(x, 0, y), Quaternion.identity);
                }
            }
        }
    }
}
