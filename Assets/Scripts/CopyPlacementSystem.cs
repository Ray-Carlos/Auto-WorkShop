using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CopyPlacementSystem : MonoBehaviour
{
    [SerializeField]
    private PlacementSystem placementSystem;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    ObjectPlacer objectPlacer;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject objectParent;

    [SerializeField]
    private AStar aStar;

    [SerializeField]
    private GameObject aStarParent;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject toggleParent;

    [SerializeField]
    private GameObject toggleParent2;

    [SerializeField]
    private GameObject togglePrefab;

    [SerializeField]
    private ScrollRect scrollRect;

    public void CopyPlacement()
    {
        foreach (Transform child in objectParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GridData gridData = placementSystem.machineData;

        for(int i = 1; i < 11; i++)
        {
            for(int j = 1; j < 11; j++)
            {
                Vector3Int pos = new Vector3Int(i, 0, j);
                if (!gridData.CanPlaceObejctAt(pos, new Vector2Int(1, 1)))
                {
                    // Debug.Log($"{pos.x}, {pos.z}");
                    if(gridData.GetPlacementDataStartPos(pos) == pos)
                    {
                        // Debug.Log($"{pos.x}, {pos.z}");
                        objectPlacer.PlaceObject(database.objectsData[gridData.GetPlacementDataID(pos)].Prefab, grid.CellToWorld(pos));
                    }
                }
            }
        }
        AStarDisplay();
        StartCoroutine(ResetScrollRect());
    }

    private IEnumerator ResetScrollRect()
    {
        Vector2 targetPosition = new Vector2(0, 1f);

        float elapsedTime = 0;
        float smoothTime = 0.5f;
        Vector2 startingPosition = scrollRect.normalizedPosition;

        while (elapsedTime < smoothTime)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(startingPosition, targetPosition, (elapsedTime / smoothTime));
            elapsedTime += Time.deltaTime;
            yield return null; // 等待一帧
        }

        scrollRect.normalizedPosition = targetPosition; // 确保最终位置准确
    }

    public void AStarDisplay()
    {
        foreach (Transform child in aStarParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in toggleParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in toggleParent2.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        // Debug.Log("---------------------------------------");

        List<TimeTable> timeTables = aStar.FindAllRoad(placementSystem.machineData);

        int i = 0;
        int length = timeTables.Count;
        // Debug.Log($"{length}");
        foreach(var nodes in timeTables)
        {
            i++;
            Color randomColor = new Color(Random.value, Random.value, Random.value)
            {
                a = 0.8f
            };

            GameObject lineObject = new GameObject("Line" + i);
            lineObject.transform.parent = aStarParent.transform;
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = randomColor;
            lineRenderer.endColor = randomColor;

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            Stack<Node> path = nodes.NodeList;

            lineRenderer.positionCount = nodes.Count+1;

            int j = 0;
            Vector3 startPosition = grid.CellToWorld(new Vector3Int(nodes.StartPos.x, 0, nodes.StartPos.y)) + new Vector3(0.5f, 0.8f, 0.5f);
            lineRenderer.SetPosition(j, startPosition);

            foreach(var node in path)
            {
                j++;
                Vector3 position = grid.CellToWorld(new Vector3Int(node.pos.x, 0,node.pos.y)) + new Vector3(0.5f, 0.8f, 0.5f);
                lineRenderer.SetPosition(j, position);
            }

            // GameObject toggleObj = Instantiate(togglePrefab, new Vector3((i-1) / 8 *200, -((i-1) % 8) *50, 0), Quaternion.identity);
            GameObject toggleObj = Instantiate(togglePrefab, new Vector3((i-1) % 3 *200, -((i-1) / 3) *50, 0), Quaternion.identity);
            // Debug.Log($"{i}: {(i-1) % 3 *200}, {-((i-1) / 8) *50}");
            if(length > 21)
            {
                toggleObj.transform.SetParent(toggleParent.transform, false);
            }
            else
            {
                toggleObj.transform.SetParent(toggleParent2.transform, false);
            }
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.name = "Toggle" + i;
            Text label = toggle.GetComponentInChildren<Text>();
            if(label != null)
            {
                label.text = nodes.StartIndex.ToString() + " → " + nodes.EndIndex.ToString();
            }

            toggle.onValueChanged.AddListener(delegate { ToggleLine(toggle, lineObject); });
        }
    }

    void ToggleLine(Toggle toggle, GameObject line)
    {
        line.SetActive(toggle.isOn);
    }
}
