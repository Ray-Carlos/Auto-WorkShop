using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DebugSystem : MonoBehaviour
{
    [SerializeField]
    private PlacementSystem placementSystem;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private bool isRunGridMapData;

    [SerializeField]
    private int level;

    [SerializeField]
    private AStar aStar;

    [SerializeField]
    private bool isRunAStar;

    [SerializeField]
    private bool isRunPlacedObjects;

    [SerializeField]
    private GameObject aStarParent;

    [SerializeField]
    private ScrollRect scrollRect;

    private void Start()
    {
        InvokeRepeating("Repeating1", 0f, 1f);
        InvokeRepeating("Repeating2", 0f, 1f);
    }

    private void Repeating1()
    {
        if(isRunGridMapData)
        {
            GridMapDataDebug();
        }

        if(isRunPlacedObjects)
        {
            PlaceObjectsDebug();
        }
    }

    private void Repeating2()
    {
        if(isRunAStar)
        {
            AStarDebug();
        }
    }

    private void PlaceObjectsDebug()
    {
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int[,,] map = placementSystem.machineData.map;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if(map[i,j,level] >= 0)
                {
                    GameObject textGameObject = new GameObject("CellText");
                    textGameObject.transform.parent = parent.transform;
                    
                    textGameObject.transform.position = grid.WorldToCell(new Vector3Int(i, 0, j)) + new Vector3(10.1f, 4, -1.55f);

                    TextMeshPro textMesh = textGameObject.AddComponent<TextMeshPro>();

                    Vector3Int pos = placementSystem.machineData.GetPlacementDataStartPos(new Vector3Int(i, 0, j));
                    textMesh.text = $"{pos.x}, {pos.z}";
                    textMesh.fontSize = 3;

                    textGameObject.transform.Rotate(90, 0, 0);
                }
            }
        }
    }

    public void AStarDebug()
    {
        foreach (Transform child in aStarParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<TimeTable> timeTables = aStar.FindAllRoad(placementSystem.machineData);

        int i = 0;
        foreach(var nodes in timeTables)
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // 创建新的游戏对象和LineRenderer组件
            GameObject lineObject = new GameObject("Line" + i);
            lineObject.transform.parent = aStarParent.transform;
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

            // 设置材质和颜色
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = randomColor;
            lineRenderer.endColor = randomColor;

            // 设置线条宽度
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            Stack<Node> path = nodes.NodeList;

            lineRenderer.positionCount = nodes.Count+1;

            int j = 0;

            Vector3 startPosition = grid.CellToWorld(new Vector3Int(nodes.StartPos.x, 0, nodes.StartPos.y)) + new Vector3(0.5f, 3, 0.5f);

            lineRenderer.SetPosition(j, startPosition);

            foreach(var node in path)
            {
                j++;

                Vector3 position = grid.CellToWorld(new Vector3Int(node.pos.x, 0,node.pos.y)) + new Vector3(0.5f, 3, 0.5f);

                lineRenderer.SetPosition(j, position);               
            }

            
        }
    }



    private void GridMapDataDebug()
    {
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int[,,] map = placementSystem.machineData.map;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if(map[i,j,level] >= 0)
                {
                    GameObject textGameObject = new GameObject("CellText");
                    textGameObject.transform.parent = parent.transform;
                    
                    textGameObject.transform.position = grid.WorldToCell(new Vector3Int(i, 0, j)) + new Vector3(10.1f, 4, -1.55f);

                    TextMeshPro textMesh = textGameObject.AddComponent<TextMeshPro>();
                    textMesh.text = $"{map[i,j,level]:D5}";
                    textMesh.fontSize = 3;
                    // textMesh.alignment = TextAlignmentOptions.Justified;
                    // textMesh.alignment = TextAlignmentOptions.Center;


                    // textGameObject.transform.LookAt(Camera.main.transform.position);
                    textGameObject.transform.Rotate(90, 0, 0);
                }
            }
        }
    }
}
