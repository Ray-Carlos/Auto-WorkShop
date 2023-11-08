using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;


public class AStar : MonoBehaviour
{
    public List<TimeTable> FindAllRoad(GridData gridData)
    {
        int[,,] map = gridData.map;

        List<Vector2Int> list0End = new List<Vector2Int>();
        List<Vector2Int> list1End = new List<Vector2Int>();
        List<Vector2Int> list2End = new List<Vector2Int>();
        List<TimeTable> timeTables = new List<TimeTable>();

        int row = map.GetLength(0);
        int col = map.GetLength(1);
        for (int i = 1; i < row - 1; i++)
        {
            for (int j = 1; j < col - 1; j++)
            {
                if(map[i, j, 1] == 0)
                {
                    list0End.Add(new Vector2Int(i, j));
                }
                else if(map[i, j, 1] == 1)
                {
                    list1End.Add(new Vector2Int(i, j));
                }
                else if(map[i, j, 1] == 2)
                {
                    list2End.Add(new Vector2Int(i, j));
                }
            }
        }

        List<Vector2Int> scanList = new List<Vector2Int>
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1)
        };

        foreach(var pos0 in list0End)
        {
            foreach(var pos1 in list1End)
            {
                int startID = gridData.GetPlacementDataID(new Vector3Int(pos0.x, 0, pos0.y));
                int endID = gridData.GetPlacementDataID(new Vector3Int(pos1.x, 0, pos1.y));
                int startIndex = gridData.GetPlacementDataIndex(new Vector3Int(pos0.x, 0, pos0.y));
                int endIndex = gridData.GetPlacementDataIndex(new Vector3Int(pos1.x, 0, pos1.y));
                Vector3Int startPos1 = gridData.GetPlacementDataStartPos(new Vector3Int(pos1.x, 0, pos1.y));
                Stack<Node> nodes = FindRoad(pos0, new Vector2Int(startPos1.x, startPos1.z), scanList, map);
                timeTables.Add(new TimeTable(startID, endID, startIndex, endIndex, pos0, nodes.Count, nodes));
            }
        }

        foreach(var pos1 in list1End)
        {
            foreach(var pos2 in list2End)
            {
                int startID = gridData.GetPlacementDataID(new Vector3Int(pos1.x, 0, pos1.y));
                int endID = gridData.GetPlacementDataID(new Vector3Int(pos2.x, 0, pos2.y));
                int startIndex = gridData.GetPlacementDataIndex(new Vector3Int(pos1.x, 0, pos1.y));
                int endIndex = gridData.GetPlacementDataIndex(new Vector3Int(pos2.x, 0, pos2.y));
                Vector3Int startPos2 = gridData.GetPlacementDataStartPos(new Vector3Int(pos2.x, 0, pos2.y));
                Stack<Node> nodes = FindRoad(pos1, new Vector2Int(startPos2.x, startPos2.z), scanList, map);
                timeTables.Add(new TimeTable(startID, endID, startIndex, endIndex, pos1, nodes.Count, nodes));
            }
        }

        return timeTables;
    }

    public Stack<Node> FindRoad(Vector2Int start, Vector2Int end, List<Vector2Int> scanList, int[,,] map)
    {   
        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        Node present;
        int row = map.GetLength(0);
        int col = map.GetLength(1);
        Node[,] nodeMap = new Node[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                nodeMap[i, j] = new Node
                {
                    pos = new Vector2Int(i, j)
                };
                if(new Vector2Int(i, j) == end)
                {
                    nodeMap[i, j].mapSign = MapSign.FREE;
                }
                else if(map[i, j, 2] >= 0)
                {
                    nodeMap[i, j].mapSign = MapSign.BAN;
                }
                else
                {
                    nodeMap[i, j].mapSign = MapSign.FREE;
                }
                
            }
        }

        nodeMap[start.x, start.y].startcost = 0;
        nodeMap[start.x, start.y].mapSign = MapSign.OPEN;
        openList.Add(nodeMap[start.x, start.y]);

        while(openList.Count > 0)
        {
            present = openList[0];
            foreach(Node node in openList)
            {
                if(node.cost < present.cost)
                {
                    present = node;
                }
            }
            openList.Remove(present);
            present.mapSign = MapSign.CLOSE;
            closeList.Add(present);

            if (present.pos == end)
            {
                Stack<Node> road = new();
                while(present.fatherNode != null)
                {
                    road.Push(present);
                    present = present.fatherNode;
                }
                return road;
            }

            int x, y;
            foreach(Vector2Int scan in scanList)
            {
                x = present.pos.x + scan.x;
                y = present.pos.y + scan.y;
                if(x >=0 && y >= 0 && x < row && y < col)
                {
                    Node n = nodeMap[x, y];
                    if(n.mapSign == MapSign.FREE)
                    {
                        n.startcost = present.startcost + 1;
                        n.cost = n.startcost + CalculateCost(n.pos, end);
                        n.fatherNode = present;
                        n.mapSign = MapSign.OPEN;
                        openList.Add(n);
                    }
                    else if(n.mapSign == MapSign.OPEN)
                    {
                        float new_startcost = present.startcost + 1;
                        if(new_startcost < n.startcost)
                        {
                            n.cost = n.cost - n.startcost + new_startcost;
                            n.startcost = new_startcost;
                            n.fatherNode =present;
                        }
                    }
                }
            }
        }
        return new Stack<Node>();
    }

    private int CalculateCost(Vector2Int a, Vector2Int b)
    {
        int dx = Math.Abs(a.x - b.x);
        int dy = Math.Abs(a.y - b.y);
        return dx + dy;
    }
    
}

public class Node
{
    public Vector2Int pos;
    public float startcost;
    public float cost;
    public MapSign mapSign;
    public Node fatherNode;
}

public enum MapSign
{
    FREE,
    OPEN,
    CLOSE,
    BAN,
}

public class TimeTable
{
    public int Start { get; private set; }
    public int End { get; private set; }
    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }
    public Vector2Int StartPos { get; private set; }
    public int Count { get; private set; }
    public Stack<Node> NodeList { get; private set; }

    public TimeTable(int start, int end, int startIndex, int endIndex, Vector2Int startPos, int count, Stack<Node> nodeList)
    {
        Start = start;
        End = end;
        StartIndex = startIndex;
        EndIndex = endIndex;
        StartPos = startPos;
        Count = count;
        NodeList = nodeList;
    }
}
