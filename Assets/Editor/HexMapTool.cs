using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System;

public class HexMapTool : EditorWindow
{
    public GameObject tilePrefab;
    [SerializeField] private int mapSize;
    [SerializeField] private int tileWidth;
    [SerializeField] private int tileHeight;
    [SerializeField] private int hexTileSize;


    [MenuItem("Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<HexMapTool>("Map Generator");
    }
    private void OnGUI()
    {
        GUILayout.Label("Map Settings", EditorStyles.boldLabel);

        tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
        tileWidth = EditorGUILayout.IntField("Map Width", tileWidth);
        tileHeight = EditorGUILayout.IntField("Map Height", tileHeight);

        if (GUILayout.Button("Generate Map"))
        {
            GenerateHexMap();
        }
    }

    private void GenerateHexMap()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("No tile set");
            return;
        }
        for (int q = 0; q < tileWidth; q++)
        {
            for (int r = 0; r < tileHeight; r++)
            {
                Vector3 hexPosition = CalculateHexPosition(q, r);
                GameObject hexTile = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                hexTile.transform.position = hexPosition;
                hexTile.transform.SetParent(null);
                Undo.RegisterCreatedObjectUndo(hexTile, "Generate Hix Tile");
            }
        }

    }
    private Vector3 CalculateHexPosition(int q, int r)
    {
        float x = hexTileSize * (3.0f / 2.0f * q);
        float y = hexTileSize * (Mathf.Sqrt(3.0f) * (r + q / 2.0f));
        return new Vector3(x, 0, y);
    }
}

   


