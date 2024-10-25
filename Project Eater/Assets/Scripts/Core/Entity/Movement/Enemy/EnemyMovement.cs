using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyMovement : EntityMovement
{
    #region Event
    public delegate void IdleHander();
    public delegate void moveHander(Vector3 movePosition, float speed);
    #endregion

    public event IdleHander onIdle;
    public event moveHander onMove;

    // Path Node
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    // Player Position : Target Position
    private Vector2 playerPosition;
    // Movement Coroutine
    private Coroutine moveEnemyRoutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isSubscribed = false;

    // Astar Path ����ȭ ���� 
    // �� Enemy Spawner���� ���� set �ȴ�. 
    [HideInInspector] public int updateFrameNumber = 1;

    [Tooltip("�ش� ������ ���� �÷��̾�� ���� ���� �Ÿ� ������ ����, ���� ũ�⿡ ���� �ش� ���� ���� �����Ѵ�.")]
    [SerializeField]
    private float playerDistanceToRebuildPath;

    private void Awake()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    public override void Setup(Entity owner)
    {
        base.Setup(owner);
    }

    private void Start()
    {
        playerPosition = GameManager.Instance.GetPlayerPosition();
    }

    private void OnEnable()
    {
        if (!isSubscribed)
        {
            // �̺�Ʈ ����
            onIdle += EnemyIdle;
            onMove += EnemyMove;
            isSubscribed = true;  // ���� ���� ������Ʈ
        }
    }

    private void OnDisable()
    {
        if (isSubscribed)
        {
            // �̺�Ʈ ����
            onIdle -= EnemyIdle;
            onMove -= EnemyMove;
            isSubscribed = false;  // ���� ���� �ʱ�ȭ
        }
    }

    private void EnemyIdle()
    {
        rigidbody.velocity = Vector2.zero;
    }

    private void EnemyMove(Vector3 movePosition, float moveSpeed)
    {
        // �ϴ� �ӷ� �̵����� �غ��ٰ� ���θ� rigidbody �̵����� �����ϱ� 
        // rigidbody.velocity = unitVector * moveSpeed;
        Vector2 unitVector = Vector3.Normalize(movePosition - transform.position);
        rigidbody.MovePosition(rigidbody.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }

    private void Update()
    {
        MoveEnemy();
    }

    // Use Astar pathfinding to build a path to the player - and then move the enemy to each grid location on the path 
    private void MoveEnemy()
    {
        // �� Time.frameCount : https://docs.unity3d.com/ScriptReference/Time-frameCount.html
        // Only process A star path rebuild on certain frames to spread the load between enemies
        // Ex) updateFrameNumber�� 1�� ���,  Time.frameCount�� 1, 61, 121ó�� Ư�� �������� �ɶ��� if���� �������� �ʾ�
        //     �Ʒ� �ڵ��(CreatePath)�� �����Ѵ�.
        // �� 60 �����Ӹ��� Path�� �缳�� 
        if (Time.frameCount % Settings.targetFrameRateForPathFind != updateFrameNumber) return;

        // if the movement cooldown timer reached or player has moved more than required distance
        // then rebuild the enemy path and move the enemy 
        // - ���� Player ��ġ�� ������ �����ߴ� playerPosition���� playerDistanceToRebuildPath��ŭ ���̰� 
        //    ���ٸ� ��θ� ���� 
        if ((GameManager.Instance.GetPlayerPosition() - playerPosition).sqrMagnitude > playerDistanceToRebuildPath * playerDistanceToRebuildPath)
        {
            // Reset playerPosition 
            playerPosition = GameManager.Instance.GetPlayerPosition();

            // Move the enemy using AStar pathfinding - Trigger rebuild of path to player
            CreatePath();

            // if a path has been found move the enemy
            if (movementSteps != null)
            {
                // �̹� Enemy�� Move�� �ϰ� ������ ���ο� path�� �÷��̾ ����
                if (moveEnemyRoutine != null)
                {
                    onIdle?.Invoke();
                    StopCoroutine(moveEnemyRoutine);
                }

                // Move enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    // Use the AStar static class to create a path for the enemy
    private void CreatePath()
    {
        // ���ʹ� Room �߿��� StageRoom 
        Room currentRoom = GameManager.Instance.GetCurrentRoom() as StageRoom;

        if (currentRoom == null)
            return;

        Grid grid = currentRoom.grid;

        // Get players position on the grid
        Vector3Int playerGridPosition = GetNearsetPlayerPosition(currentRoom);

        // Get enemy position on the grid
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        // Build a path for the enemy to move on 
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        // Take off first step on path - this is the grid square the enemy is already on 
        // �� StartGrid�� ���Ͱ� �̹� �ִ� �����̱� ������, �ش� gridPosition�� Pop�ϰ� Path�� ������. 
        if (movementSteps != null)
            movementSteps.Pop();
        else
        {
            onIdle?.Invoke();
        }
    }

    // Coroutine to move the enemy to the next location on the path 
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            // while not very close(0.2) continue to move - when close move onto the next step
            while ((nextPosition - transform.position).sqrMagnitude > 0.2f * 0.2f)
            {
                // movement event
                onMove?.Invoke(nextPosition, MoveSpeed);

                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        // End of path steps
        onIdle?.Invoke();   
    }

    // Get the nearest position to the player 
    private Vector3Int GetNearsetPlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.grid.WorldToCell(playerPosition);

        return playerCellPosition;
    }

    // Set the frame number that the enemy path will be recalculated on - to avoid performance spike
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }
}
