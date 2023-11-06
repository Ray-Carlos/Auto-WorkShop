using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;


public class AStar : MonoBehaviour
{
    public void FindAllRoad(int[,,] map, int[] num)
    {
        List<Vector2Int> scanList = new List<Vector2Int>
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1)
        };
        Vector2Int start = new Vector2Int(1, 1);
        Vector2Int end = new Vector2Int(1, 1);
        FindRoad(start, end, scanList, map);
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
