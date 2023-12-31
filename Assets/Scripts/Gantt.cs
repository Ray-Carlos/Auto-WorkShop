using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Gantt : MonoBehaviour
{
    [SerializeField]
    private PlacementSystem placementSystem;

    [SerializeField]
    public GameObject rectanglePrefab;

    [SerializeField]
    public Transform specifiedPanel;

    public void DrawGantt()
    {
        foreach (Transform child in specifiedPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<int> objectIDs = new List<int>();
        int[,,] map = placementSystem.machineData.map;

        int row = map.GetLength(0);
        int col = map.GetLength(1);
        for (int i = 1; i < row - 1; i++)
        {
            for (int j = 1; j < col - 1; j++)
            {
                if(map[i, j, 3] > -1)
                {
                    objectIDs.Add(map[i, j, 3]);
                }
            }
        }

        int numberOfRectangles = objectIDs.Count;

        float panelWidth = specifiedPanel.GetComponent<RectTransform>().rect.width;
        float panelHeight = specifiedPanel.GetComponent<RectTransform>().rect.height;

        float rectangleWidth = panelWidth;
        float rectangleHeight = panelHeight / numberOfRectangles;

        for (int i = 0; i < numberOfRectangles; i++)
        {
            GameObject rectangle = Instantiate(rectanglePrefab, specifiedPanel);

            float xPos = panelWidth/2;
            float yPos = i*rectangleHeight;
  
            SetRectangle(rectangle, panelWidth, panelHeight, xPos, yPos, rectangleWidth/2, rectangleHeight);

            Color color = new Color(0.1568628f,0.1568628f,0.1568628f);
            rectangle.GetComponent<Image>().color = color;

            GameObject rectangle1 = Instantiate(rectanglePrefab, specifiedPanel);

            float xPos1 = panelWidth/2;
            float yPos1 = i*rectangleHeight;
  
            SetRectangle1(rectangle1, panelWidth, panelHeight, xPos1, yPos1, rectangleWidth/2, rectangleHeight);

            Color color1 = new Color(1f,1f,1f);
            rectangle1.GetComponent<Image>().color = color1;
        }
    }
    
    void SetRectangle(GameObject rectangle, float panelWidth, float panelHeight, float xPos, float yPos, float width, float height)
    {
        rectangle.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        float x = -panelWidth/2 + xPos + width/2;
        float y = -panelHeight/2 + yPos + height/2;
        rectangle.transform.localPosition = new Vector3(x, y, 0f);
    }

        void SetRectangle1(GameObject rectangle, float panelWidth, float panelHeight, float xPos, float yPos, float width, float height)
    {
        rectangle.GetComponent<RectTransform>().sizeDelta = new Vector2(width-3, height-3);

        float x = -panelWidth/2 + xPos + width/2 + 1.5f;
        float y = -panelHeight/2 + yPos + height/2 + 1.5f;
        rectangle.transform.localPosition = new Vector3(x, y, 0f);
    }
}