using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow
{
    public List<Tile> rowTiles = new List<Tile>();
}

public class GridBuildingManager : MonoBehaviour
{

    //stats
    public float wallThickness = .25f;
    public int buildingSize = 3;
    private int wallHeight = 1;
    public List<TileRow> buildingMap = new List<TileRow>();

    //Prefabs
    private GameObject darkTile;
    private GameObject lightTile;
    private GameObject wall;

    //Holders
    private GameObject wallsHolder;
    private GameObject tileHolder;

    GameObject generateWall(Vector3 pos, Vector3 size)
    {
        Vector3 offset = new Vector3(buildingSize/2, 0, buildingSize/2);
        GameObject newWall = Instantiate((GameObject)Resources.Load("Wall"), pos+offset, Quaternion.identity);
        newWall.transform.localScale = size;
        newWall.transform.parent = wallsHolder.transform;
        return newWall;
    }

    Tile MakeTile(int x, int y)
    {
        Tile tile = new Tile();

        int tileIndex = x + y;
        GameObject tilePrefab = (tileIndex % 2 == 0) ? darkTile : lightTile;
        GameObject tilePiece = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity, tileHolder.transform);
        tile.tilePiece = tilePiece;
        
        return tile;
    }
    
    void UpdateTiles()
    {
        for (int x = 0; x < buildingSize; x++)
        {
            
            if (buildingMap.Count < x+1)
            {
                TileRow newRow = new TileRow();
                buildingMap.Add(newRow);
                for (int y = 0; y < buildingSize; y++)
                {
                    newRow.rowTiles.Add(MakeTile(x, y));
                }
            }
            else
            {
                for (int y = 0; y < buildingSize; y++)
                {
                    if (buildingMap[x].rowTiles.Count < y+1)
                    {
                        buildingMap[x].rowTiles.Add(MakeTile(x, y));
                    }
                }
            }
            
            
        }

        
        foreach (Transform child in wallsHolder.transform) {
            Destroy(child.gameObject);
        }
        
        //Create walls
        GameObject RightWall = generateWall(new Vector3((buildingSize/2f)+(wallThickness/2), wallHeight/2f, 0),
            new Vector3(wallThickness, wallHeight, buildingSize + (wallThickness*2)));

        GameObject BackWall = generateWall(new Vector3(0, wallHeight/2f, (buildingSize / 2f)+(wallThickness/2)),
            new Vector3(buildingSize + (wallThickness*2), wallHeight, wallThickness));
        
        GameObject LeftWall = generateWall(new Vector3(-(buildingSize/2f)-(wallThickness/2), 0, 0),
            new Vector3(wallThickness, .1f, buildingSize + (wallThickness*2)));

        GameObject FrontWall = generateWall(new Vector3(0, 0, -(buildingSize / 2f)-(wallThickness/2)),
            new Vector3(buildingSize + (wallThickness*2), .1f, wallThickness));

    }
    
    // Start is called before the first frame update
    void Start()
    {
        wallsHolder = GameObject.Find("Walls");
        tileHolder = GameObject.Find("TileArea");
        darkTile = (GameObject)Resources.Load("DarkTile");
        lightTile = (GameObject)Resources.Load("LightTile");
        UpdateTiles();

    }

    private void Update()
    {
        UpdateTiles();
    }
}