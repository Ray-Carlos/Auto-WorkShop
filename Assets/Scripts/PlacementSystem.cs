using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    public GridData machineData;

    [SerializeField]
    private PreviewSystem preview;

    // private Vector3Int lastDetectedPosition = new Vector3Int(5, 0, 5);
    private Vector3Int lastDetectedPosition = Vector3Int.zero;


    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;
    
    private void Start()
    {
        StopPlacement();
        machineData = new();
        machineData.InitMap();
        machineData.InitPlacementData();

        // Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        // Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        // preview.MoveCursor(grid.CellToWorld(gridPosition));
        preview.MoveCursor(grid.CellToWorld(new Vector3Int(5, 0, 5)));
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, grid, preview, database, machineData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, machineData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        // if(inputManager.IsPointerOverUI())
        // {
        //     return;
        // }
        // Debug.Log($"{inputManager.IsPointOverPlane()}");
        if(!inputManager.IsPointOverPlane())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // Debug.Log(gridPosition);

        buildingState.OnAction(gridPosition);
    }

    // private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    // {
    //     GridData selectedData = machineData;

    //     return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    // }

    private void StopPlacement()
    {
        if(buildingState == null)
        {
            return;
        }
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if(buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}
