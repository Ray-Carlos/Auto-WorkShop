using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Gantt : MonoBehaviour
{
    [SerializeField]
    private PlacementSystem placementSystem;

    [SerializeField]
    public GameObject rectanglePrefab;

    [SerializeField]
    public Transform specifiedPanel;
    [SerializeField]
    private Font font;

    public void DrawGantt(double[,] bestptr, int[,] bestper, double blance, int[] equSize)
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

        int numberOfRectangles = equSize[0]+equSize[1]+equSize[2];

        float panelWidth = specifiedPanel.GetComponent<RectTransform>().rect.width;
        float panelHeight = specifiedPanel.GetComponent<RectTransform>().rect.height;

        float rectangleWidth;
        float rectangleHeight = panelHeight / numberOfRectangles;

        row = bestptr.GetLength(0);
        col = bestptr.GetLength(1);
        double timeSpan = bestptr.Cast<double>().Max();

        for (float i = 10; i<timeSpan; i+=10)
        {
            SetRect(panelWidth, panelHeight, (float)(i / timeSpan)*panelWidth, 0, 2, panelHeight, Color.white);
        }

        for (int i = 0; i < row; i++)
        {
            Color randomColor = new Color(Random.value*0.5f+0.5f, Random.value*0.5f+0.5f, Random.value*0.5f+0.5f);
            for (int j = 0; j < 3; j++)
            {
                int id = 0;
                switch(j)
                {
                    case 0:
                        id = bestper[i,j];
                        break;
                    case 1:
                        id = equSize[0] + bestper[i,j];
                        break;
                    case 2:
                        id = equSize[0] + equSize[1] + bestper[i,j];
                        break;
                }
                // GameObject rectangle = new GameObject();
                rectangleWidth = (float)((bestptr[i,j*3+1]-bestptr[i,j*3])/timeSpan*panelWidth);
                GameObject rectangle = SetRect(panelWidth, panelHeight, (float)(bestptr[i,j*3]/timeSpan*panelWidth), id*rectangleHeight, rectangleWidth, rectangleHeight*0.7f, randomColor);
                SetText(rectangle, rectangleWidth, rectangleHeight*0.7f, i.ToString()+" - "+(j+1).ToString());
                rectangleWidth = (float)((bestptr[i,j*3+2]-bestptr[i,j*3+1])/timeSpan*panelWidth);
                SetRect(panelWidth, panelHeight, (float)(bestptr[i,j*3+1]/timeSpan*panelWidth), id*rectangleHeight, rectangleWidth, rectangleHeight*0.7f, Color.gray);
            }
        }

        GameObject textBoxGO = new GameObject("TextBox");
        textBoxGO.transform.SetParent(specifiedPanel, false);
        Text textBox = textBoxGO.AddComponent<Text>();
        RectTransform rectTransform = textBoxGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 100);
        rectTransform.localPosition = new Vector3(100, 225, 0);
        textBox.alignment = TextAnchor.MiddleLeft;
        textBox.text = "加权最小值:"+blance.ToString("F2");
        textBox.font = font;
        textBox.fontSize = 32;
        textBox.color = new Color(1f, 1f, 1f);

        GameObject textBoxGO1 = new GameObject("TextBox");
        textBoxGO1.transform.SetParent(specifiedPanel, false);
        Text textBox1 = textBoxGO1.AddComponent<Text>();
        RectTransform rectTransform1 = textBoxGO1.GetComponent<RectTransform>();
        rectTransform1.sizeDelta = new Vector2(300, 100);
        rectTransform1.localPosition = new Vector3(410, 225, 0);
        textBox1.alignment = TextAnchor.MiddleLeft;
        textBox1.text = "加工时间:"+timeSpan.ToString("F0")+"h";
        textBox1.font = font;
        textBox1.fontSize = 32;
        textBox1.color = new Color(1f, 1f, 1f);
    }

    void SetText(GameObject gameObject, float rectangleWidth, float height, string str)
    {
        GameObject textBoxGO = new GameObject("TextBox");
        textBoxGO.transform.SetParent(gameObject.transform, false);

        Text textBox = textBoxGO.AddComponent<Text>();
        
        RectTransform rectTransform = textBoxGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, height);
        rectTransform.localPosition = new Vector3((100 - rectangleWidth)/2+5, 0, 0f);

        textBox.alignment = TextAnchor.MiddleLeft;
        
        textBox.text = str;

        textBox.font = font;
        textBox.fontSize = 20;

        textBox.color = new Color(0f, 0f, 0f);

        textBox.resizeTextForBestFit = true;
        textBox.resizeTextMinSize = 5;
        textBox.resizeTextMaxSize = 20;
    }

    GameObject SetRect(float panelWidth, float panelHeight, float xPos, float yPos, float width, float height, Color color)
    {

        GameObject rectangle = Instantiate(rectanglePrefab, specifiedPanel);
        SetRectangle(rectangle, panelWidth, panelHeight, xPos, yPos, width, height);
        rectangle.GetComponent<Image>().color = color;
        return rectangle;
    }
    
    void SetRectangle(GameObject rectangle, float panelWidth, float panelHeight, float xPos, float yPos, float width, float height)
    {
        rectangle.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        float x = -panelWidth/2 + xPos + width/2;
        float y = -panelHeight/2 + yPos + height/2;
        rectangle.transform.localPosition = new Vector3(x, y, 0f);
    }
}