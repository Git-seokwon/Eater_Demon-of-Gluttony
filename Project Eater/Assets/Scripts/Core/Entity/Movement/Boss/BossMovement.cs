using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : EntityMovement
{
    #region Event
    public delegate void IdleHander();
    public delegate void moveHander(Vector3 movePosition, float speed);
    public event IdleHander onIdle;
    public event moveHander onMove;
    #endregion

    #region Astar ���� 
    // Path Node
    private Stack<Vector3> movementSteps = new Stack<Vector3>();

    // Player Position : Target Position
    private Vector2 playerPosition;

    // Movement Coroutine
    private Coroutine moveEnemyRoutine;
    private WaitForFixedUpdate waitForFixedUpdate;

    private bool isSubscribed = false;

    // Astar Path ����ȭ ���� 
    [Tooltip("�ش� ������ ���� �÷��̾�� ���� ���� �Ÿ� ������ ����, ���� ũ�⿡ ���� �ش� ���� ���� �����Ѵ�.")]
    private float playerDistanceToRebuildPath = 0.4f;
    #endregion

    private void Awake()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    public override void Setup(Entity owner)
    {
        base.Setup(owner);
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

    private void Start()
    {
        playerPosition = GameManager.Instance.player.transform.position;
    }

    #region Astar
    // Use Astar pathfinding to build a path to the player - and then move the enemy to each grid location on the path 
    private void MoveEnemy()
    {
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
        Room currentRoom = StageManager.Instance.CurrentRoom as StageRoom;

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
        Vector3 playerPosition = this.playerPosition;

        Vector3Int playerCellPosition = currentRoom.grid.WorldToCell(playerPosition);

        return playerCellPosition;
    }
    #endregion

    private void EnemyIdle()
    {
        rigidbody.velocity = Vector2.zero;
    }

    private void EnemyMove(Vector3 moveDirection, float moveSpeed)
    {
        // �ϴ� �ӷ� �̵����� �غ��ٰ� ���θ� rigidbody �̵����� �����ϱ� 
        rigidbody.velocity = moveDirection * moveSpeed;
    }
}
