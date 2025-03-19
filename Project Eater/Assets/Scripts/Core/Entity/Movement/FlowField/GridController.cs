using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : SingletonMonobehaviour<GridController>
{
    private float playerDistanceToRebuildPath = 0.4f;
    public FlowField currentFlowField;

    private Vector3 playerPosition;
    private Cell destinationCell;
    private bool isEntered;

    private float timeSinceLastRebuild = 0f;
    private float rebuildInterval = 0.5f;

    public void EnterStage()
    {
        Room room = StageManager.Instance.CurrentRoom;
        playerPosition = GameManager.Instance.player.transform.position;

        currentFlowField = new FlowField(room);
        currentFlowField.CreateGrid();

        UpdateFlowField();

        isEntered = true;
    }

    public void ExitStage()
    {
        isEntered = false;

        currentFlowField.ClearGrid();
        currentFlowField = null;
    }

    public void UpdateFlowField()
    {
        destinationCell = currentFlowField.GetCellFromWorldPos(playerPosition);
        if (destinationCell == null)
            Debug.Log("destinationCell이 null 입니다.");

        currentFlowField.CreateIntegrationField(destinationCell);
        currentFlowField.CreateFlowField();
    }

    private void Update()
    {
        timeSinceLastRebuild += Time.deltaTime;

        if (isEntered && timeSinceLastRebuild >= rebuildInterval && 
            (GameManager.Instance.player.transform.position - playerPosition).sqrMagnitude >
             playerDistanceToRebuildPath * playerDistanceToRebuildPath)
        {
            timeSinceLastRebuild = 0f;
            currentFlowField.RestFlowFieldForUpdate();
            playerPosition = GameManager.Instance.player.transform.position;
            UpdateFlowField();
        }
    }
}
