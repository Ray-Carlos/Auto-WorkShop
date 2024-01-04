using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFSPSolver : MonoBehaviour
{
    [SerializeField]
    private Text component1;
    [SerializeField]
    private Text component2;
    [SerializeField]
    private Text component3;
    [SerializeField]
    private Text component4;
    [SerializeField]
    private Text component5;
    [SerializeField]
    private Text ETR;

    [SerializeField]
    private PlacementSystem placementSystem;

    [SerializeField]
    private Gantt gantt;

    [SerializeField]
    private HFSPClassical hPSPClassical;

    private void Start()
    {
        
    }

    public void Solver()
    {
        if(placementSystem.timeTables == null)
        {
            Debug.Log("there is no tables");
            return;
        }
        List<TimeTable> timeTables = placementSystem.timeTables;
        if((placementSystem.timeTableCount[0] == 0) || (placementSystem.timeTableCount[1] == 0))
        {
            Debug.Log("there is no effetive paths");
            return;
        }

        int.TryParse(component1.text, out int com1);
        int.TryParse(component2.text, out int com2);
        int.TryParse(component3.text, out int com3);
        int.TryParse(component4.text, out int com4);
        int.TryParse(component5.text, out int com5);
        int.TryParse(ETR.text, out int etr_t);

        Debug.Log($"零件1:{com1},零件2:{com2},零件3:{com3},零件4:{com4},零件5:{com5},ETR:{etr_t}%");

        double avePath1=0;
        double avePath2=0;
        double[] avePath = new double[2];
        int n =0;

        foreach(var nodes in timeTables)
        {
            if(nodes.Start == 0)
            {
                // Debug.Log($"{nodes.Start}->{nodes.End},{nodes.StartIndex}->{nodes.EndIndex},lenth:{nodes.Count}");
                avePath1+=nodes.Count;
                n++;
            }
        }
        avePath[0] = avePath1/n;
        n=0;
        foreach(var nodes in timeTables)
        {
            if(nodes.Start == 1)
            {
                // Debug.Log($"{nodes.Start}->{nodes.End},{nodes.StartIndex}->{nodes.EndIndex},lenth:{nodes.Count}");
                avePath2+=nodes.Count;
                n++;
            }
        }
        avePath[1] = avePath2/n;

        Debug.Log($"{avePath1},{avePath2}");

        int[,,] map = placementSystem.machineData.map;

        List<int> p1ObjectIDs = new List<int>();
        List<int> p2ObjectIDs = new List<int>();
        List<int> p3ObjectIDs = new List<int>();
        int row = map.GetLength(0);
        int col = map.GetLength(1);
        for (int i = 1; i < row - 1; i++)
        {
            for (int j = 1; j < col - 1; j++)
            {
                if(map[i, j, 3] > -1)
                {
                    switch(map[i, j, 0])
                    {
                        case 0:
                            p1ObjectIDs.Add(map[i, j, 3]);
                            break;
                        case 1:
                            p2ObjectIDs.Add(map[i, j, 3]);
                            break;
                        case 2:
                            p3ObjectIDs.Add(map[i, j, 3]);
                            break;
                    }
                }
            }
        }
        
        int allCount = com1+com2+com3+com4+com5;
        if (allCount < 1)
        {
            Debug.Log("there is no components");
            return;
        }
        int[] countlist = new int[allCount];
        n = 0;
        for (int i = 0; i < com1; i++)
        {
            countlist[n] = 0;
            n++;
        }
        for (int i = 0; i < com2; i++)
        {
            countlist[n] = 1;
            n++;
        }
        for (int i = 0; i < com3; i++)
        {
            countlist[n] = 2;
            n++;
        }
        for (int i = 0; i < com4; i++)
        {
            countlist[n] = 3;
            n++;
        }
        for (int i = 0; i < com5; i++)
        {
            countlist[n] = 4;
            n++;
        }
        int[,] workTime = new int[,] { { 10, 9, 12 }, { 6, 13, 18 }, { 4, 13, 19 }, { 10, 15, 18 }, { 13, 13, 7 }};
        int[,] transTime = new int[,] { { 1, 2 }, { 5, 2 }, { 10, 10 }, { 2, 8 }, { 4, 5 }};
        int[] power = new int[] {240, 2800, 279, 1125, 264, 1478};
        double[] h_j = new double[] { 0.1502, 0.2113, 0.1338, 0.0553, 0.1799};
        int[,] workTimeArray = new int[allCount, 3];
        int[,] transTimeArray = new int[allCount, 2];
        int[] powerArray = new int[(p1ObjectIDs.Count+p2ObjectIDs.Count+p3ObjectIDs.Count)*2];
        n = 0;
        for (int i = 0; i < p1ObjectIDs.Count; i++)
        {
            powerArray[2*n] = power[0];
            powerArray[2*n+1] = power[1];
            n++;
        }
        for (int i = 0; i < p2ObjectIDs.Count; i++)
        {
            powerArray[2*n] = power[2];
            powerArray[2*n+1] = power[3];
            n++;
        }
        for (int i = 0; i < p3ObjectIDs.Count; i++)
        {
            powerArray[2*n] = power[4];
            powerArray[2*n+1] = power[5];
            n++;
        }
        double[] h_jArray = new double[allCount];
        for (int i = 0; i < allCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (j < 3)
                    workTimeArray[i, j] = workTime[countlist[i], j];
                if (j < 2)
                {
                    transTimeArray[i, j] = transTime[countlist[i], j]*(int)Math.Ceiling(avePath[j]);
                }
                if (j < 1)
                    h_jArray[i] = h_j[countlist[i]];
            }
        }
        int[] equSize = new int[] {p1ObjectIDs.Count, p2ObjectIDs.Count, p3ObjectIDs.Count};
        int pieceSize = allCount;
        int procSize = 3;
        double b_l = etr_t*1f/100;

        double[,] bestptr;
        int[,] bestper;
        double blance;
        (bestptr, bestper, blance) = hPSPClassical.Solver(workTimeArray, equSize, pieceSize, procSize, transTimeArray, powerArray, h_jArray, b_l);

        gantt.DrawGantt(bestptr, bestper, blance, equSize);
    }
}
