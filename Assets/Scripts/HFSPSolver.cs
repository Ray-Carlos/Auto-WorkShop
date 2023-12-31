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
        }

        int com1 = 1;
        int com2 = 3;
        int com3 = 5;
        int com4 = 2;
        int com5 = 1;
        int etr_t = 40;

        int.TryParse(component1.text, out com1);
        int.TryParse(component2.text, out com2);
        int.TryParse(component3.text, out com3);
        int.TryParse(component4.text, out com4);
        int.TryParse(component5.text, out com5);
        int.TryParse(ETR.text, out etr_t);

        float etr = etr_t*1f/100;

        Debug.Log($"零件1:{com1},零件2:{com2},零件3:{com3},零件4:{com4},零件5:{com5},ETR:{etr_t}%");

        foreach(var nodes in timeTables)
        {
            if(nodes.Start == 0)
                Debug.Log($"{nodes.Start}->{nodes.End},{nodes.StartIndex}->{nodes.EndIndex},lenth:{nodes.Count}");
        }
        foreach(var nodes in timeTables)
        {
            if(nodes.Start == 1)
                Debug.Log($"{nodes.Start}->{nodes.End},{nodes.StartIndex}->{nodes.EndIndex},lenth:{nodes.Count}");
        }

        gantt.DrawGantt();
    }
}
