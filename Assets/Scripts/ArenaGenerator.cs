using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;


public class ArenaGenerator : MonoBehaviour
{
    [Serializable]
    public class Arena
    {
        public string name;
        public string layout;

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
    }

    [Serializable]
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

        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string json = File.ReadAllText(filePath);
        ArenaData arenaData = JsonUtility.FromJson<ArenaData>(json);

        if (arenaData != null && arenaData.arenas != null && arenaData.arenas.Length > 0)
        {
            foreach (var arena in arenaData.arenas)
            {
                if (arena != null)
                {
                    Debug.Log($"Arena: {JsonUtility.ToJson(arena)}");
                    int[][] layoutArray = arena.GetLayoutArray();
                    if (layoutArray != null)
                    {
                        for (int y = 0; y < layoutArray.Length; y++)
                        {
                            Debug.Log($"Row {y}: {string.Join(",", layoutArray[y])}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"La disposition de l'arène '{arena.name}' est nulle.");
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

                    // Check if the type is 3 and assign the "Hole" tag
                    if (type == 3)
                    {
                        tile.tag = "Hole"; // Assign the "Hole" tag
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