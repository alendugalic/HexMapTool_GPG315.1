using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System;

public class HexMapTool : EditorWindow
{
    public List<GameObject> tilePrefab = new List<GameObject>();
    public GameObject centreTilePrefab;
    private GameObject mapMade;
    [SerializeField] private int numberOfRings;
    private const float hexTileSpacing = 0.9f;

    [MenuItem("Tool/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<HexMapTool>("Map Generator");
    }
    private void OnGUI()
    {
        GUILayout.Label("Map Settings", EditorStyles.boldLabel);

        centreTilePrefab = (GameObject)EditorGUILayout.ObjectField("Centre Tile", centreTilePrefab, typeof(GameObject), false);

        EditorGUILayout.Space();

        for (int i = 0; i < tilePrefab.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            tilePrefab[i] = (GameObject)EditorGUILayout.ObjectField($"Tile Prefab {i + 1}", tilePrefab[i], typeof(GameObject), false);
           
            if (GUILayout.Button("-", GUILayout.Width(40)))
            {
                tilePrefab.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Tile Slots"))
        {
            tilePrefab.Add(null);
        }

        EditorGUILayout.Space();

        numberOfRings = EditorGUILayout.IntField("Number of Rings", numberOfRings);


        if (GUILayout.Button("Generate Map"))
        {
            GenerateHexMap();
        }
    }

    private void GenerateHexMap()
    {
        if (mapMade != null)
        {
            Undo.DestroyObjectImmediate(mapMade);
        }
        mapMade = new GameObject("Hex Map");

        if (centreTilePrefab != null)
        {
            GameObject centreTile = PrefabUtility.InstantiatePrefab(centreTilePrefab) as GameObject;
            centreTile.transform.position = Vector3.zero;
            centreTile.transform.SetParent(mapMade.transform);
            centreTile.name = "Centre Tile";
            Undo.RegisterCreatedObjectUndo(centreTile, "Generate Centre Tile");
        }


        if (tilePrefab.Count == 0 || tilePrefab[0] == null)
        {
            Debug.LogError("No tile set"); //Need to change so that the error comes up for the person using the tool 
            return;
        }

        for (int ring = 1; ring <= numberOfRings; ring++)
        {
            GenerateRing(ring);
        }
    }

    private void GenerateRing(int ring) 
    {
        int hexCount = ring * 6;
        for (int i = 0; i < hexCount; i++)
        {
            float angle = 2 * Mathf.PI / hexCount * i;

            Vector3 hexPosition = new Vector3((hexTileSpacing * 1) * ring * MathF.Sin(angle), (hexTileSpacing * 1) * ring * MathF.Cos(angle), 0);
            GameObject selectedTile = tilePrefab[UnityEngine.Random.Range(0, tilePrefab.Count)];
            if (selectedTile != null)
            {
                GameObject hexTile = PrefabUtility.InstantiatePrefab(selectedTile) as GameObject;
                hexTile.transform.position = hexPosition;
                hexTile.transform.SetParent(mapMade.transform);
                hexTile.name = $"Hex_{ring}_{i}";
                Undo.RegisterCreatedObjectUndo(hexTile, "Generate Hex Tile");
            }
            //The tiles past ring 1 arent spaceing correctly,
            //tried to fix but couldnt but weirdly enough once you add about 10+ rings it almost fixes itself or tries to
        }
       
    }
}

   



   


