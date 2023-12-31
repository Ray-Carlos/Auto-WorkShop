using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public int[,,] map = new int[12, 12, 4];

    public int[] num = new int[3];

    public void InitMap()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (i==0 || j==0 || i==map.GetLength(0) - 1 || j==map.GetLength(1) - 1)
                {
                    map[i,j,0] = 99999;
                    map[i,j,1] = 99999;
                    map[i,j,2] = 99999;
                }
                else
                {
                    map[i, j, 0] = -1; // begin ID
                    map[i, j, 1] = -1; // end ID
                    map[i, j, 2] = -1; // occupied 
                    map[i, j, 3] = -1;
                }
            }
        }
    }

    public void InitPlacementData()
    {
        List<Vector3Int> positionToOccupy = new();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (i==0 || j==0 || i==map.GetLength(0) - 1 || j==map.GetLength(1) - 1)
                {
                    positionToOccupy.Add(new Vector3Int(i, 0, j));
                }
            }
        }
        PlacementData data = new PlacementData(positionToOccupy, 99999, 99999, Vector3Int.zero, Vector3Int.zero);

        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell positiojn {pos}");

            placedObjects[pos] = data;
        }
    }

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex, positionToOccupy[0], positionToOccupy[positionToOccupy.Count-1]);

        num[ID]++;
        int i = 0;
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell positiojn {pos}");
            
            placedObjects[pos] = data;

            i++;
            if(i == 1)
            {
                map[pos.x, pos.z, 0] = ID;
                map[pos.x, pos.z, 2] = placedObjectIndex*1000+ID*10+0;
                map[pos.x, pos.z, 3] = placedObjectIndex;
            }
            else if(i == positionToOccupy.Count)
            {
                map[pos.x, pos.z, 1] = ID;
                map[pos.x, pos.z, 2] = placedObjectIndex*1000+ID*10+1;
            }
            else
            {
                map[pos.x, pos.z, 2] = placedObjectIndex*1000+ID*10+2;
            }
        }
    }

    public bool CanPlaceObejctAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        int i = 0;
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            i++;
            if(i == 1)
            {
                num[map[pos.x, pos.z, 2]/10%10]--;
            }
            placedObjects.Remove(pos);
            map[pos.x, pos.z, 0] = -1;
            map[pos.x, pos.z, 1] = -1;
            map[pos.x, pos.z, 2] = -1;
            map[pos.x, pos.z, 3] = -1;
        }
    }

    internal int GetPlacementDataID(Vector3Int gridPosition)
    {
        PlacementData data = placedObjects[gridPosition];
        return data.ID;
    }

    internal int GetPlacementDataIndex(Vector3Int gridPosition)
    {
        PlacementData data = placedObjects[gridPosition];
        return data.PlacedObjectIndex;
    }

    internal Vector3Int GetPlacementDataStartPos(Vector3Int gridPosition)
    {
        PlacementData data = placedObjects[gridPosition];
        return data.StartPos;
    }

    internal Vector3Int GetPlacementDataEndPos(Vector3Int gridPosition)
    {
        PlacementData data = placedObjects[gridPosition];
        return data.EndPos;
    }    
}


public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public Vector3Int StartPos { get; private set;}
    public Vector3Int EndPos { get; private set;}

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex, Vector3Int startPos, Vector3Int endPos)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        StartPos = startPos;
        EndPos = endPos;
    }
}
