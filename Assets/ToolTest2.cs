using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public class ToolTest2 : EditorWindow
{
    public GameObject centreTilePrefab;  // Prefab for the center tile
    private List<GameObject> hexes = new List<GameObject>();  // List of additional tile prefabs
    private int numberOfRings = 3;  // Number of rings around the center tile
    private float radius = 0.55f;  // Radius of each hexagon
    private GameObject parent;  // Parent object to organize the tiles
    private List<GameObject> generatedHexagons = new List<GameObject>();  // Track generated hexagons

    [MenuItem("Tool/Hex Grid Generator")]
    public static void ShowWindow()
    {
        GetWindow<ToolTest2>("Hex Grid Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Hex Grid Settings", EditorStyles.boldLabel);

        // Input field for the center tile prefab
        centreTilePrefab = (GameObject)EditorGUILayout.ObjectField(
            "Centre Tile", centreTilePrefab, typeof(GameObject), false);

        GUILayout.Label("Hexagon Tile Prefabs", EditorStyles.boldLabel);

        // List of tile prefabs
        for (int i = 0; i < hexes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            hexes[i] = (GameObject)EditorGUILayout.ObjectField(
                $"Tile Prefab {i + 1}", hexes[i], typeof(GameObject), false);

            if (GUILayout.Button("-", GUILayout.Width(40)))
            {
                hexes.RemoveAt(i);  // Remove selected tile prefab
            }

            EditorGUILayout.EndHorizontal();
        }

        // Add new tile slot button
        if (GUILayout.Button("Add Tile Slot"))
        {
            hexes.Add(null);
        }

        // Input field for number of rings
        numberOfRings = EditorGUILayout.IntField("Number of Rings", numberOfRings);

        // Generate grid button
        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid();
        }

        // Clear grid button
        if (GUILayout.Button("Clear Grid"))
        {
            ClearGrid();
        }
    }

    private void GenerateGrid()
    {
        // Clear previous parent object if it exists
        if (parent != null)
        {
            Undo.DestroyObjectImmediate(parent);
        }

        // Create a new parent object to hold the tiles
        parent = new GameObject("Map Parent");

        // Ensure the center tile prefab is assigned
        if (centreTilePrefab == null)
        {
            Debug.LogError("Centre Tile Prefab is required!");
            return;
        }

        // Generate the center tile at the origin
        GameObject centreTile = PrefabUtility.InstantiatePrefab(centreTilePrefab) as GameObject;
        centreTile.transform.position = Vector3.zero;
        centreTile.transform.SetParent(parent.transform);
        centreTile.name = "Centre Tile";
        Undo.RegisterCreatedObjectUndo(centreTile, "Generate Centre Tile");

        // Loop through the grid coordinates and create hex tiles
        for (int q = -numberOfRings; q <= numberOfRings; q++)
        {
            int r1 = Mathf.Max(-numberOfRings, -q - numberOfRings);
            int r2 = Mathf.Min(numberOfRings, -q + numberOfRings);

            for (int r = r1; r <= r2; r++)
            {
                if (q == 0 && r == 0) continue;  // Skip the center tile

                Vector3 position = HexToWorld(q, r);
                CreateHex(position);
            }
        }
    }

    private Vector3 HexToWorld(int q, int r)
    {
        float x = radius * (1.5f * q);
        float z = radius * (Mathf.Sqrt(3) * (r + q / 2f));
        return new Vector3(x, z, 0f);
    }

    private void CreateHex(Vector3 position)
    {
        // Ensure there are tile prefabs in the list to choose from
        if (hexes.Count == 0)
        {
            Debug.LogError("No tile prefabs available in the hex list!");
            return;
        }

        // Randomly select a tile prefab from the list
        GameObject selectedTile = hexes[Random.Range(0, hexes.Count)];
        if (selectedTile != null)
        {
            GameObject hexTile = PrefabUtility.InstantiatePrefab(selectedTile) as GameObject;
            hexTile.transform.position = position;
            hexTile.transform.SetParent(parent.transform);
            hexTile.name = "Hex Tile";
            Undo.RegisterCreatedObjectUndo(hexTile, "Generate Hex Tile");
        }
    }

    private void ClearGrid()
    {
        // Destroy the parent object and all its children
        if (parent != null)
        {
            Undo.DestroyObjectImmediate(parent);
        }
    }

}
