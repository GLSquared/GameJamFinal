using UnityEngine;

public enum OccupationType
{
    Empty,
    Staff,
    Manager
}

public class Tile
{
    public OccupationType occupiedType;
    
    public int staffLevel = 0;
    public GameObject tilePiece;
}
