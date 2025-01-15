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

    #region Astar 변수 
    // Path Node
    private Stack<Vector3> movementSteps = new Stack<Vector3>();

    // Player Position : Target Position
    private Vector2 playerPosition;

    // Movement Coroutine
    private Coroutine moveEnemyRoutine;
    private WaitForFixedUpdate waitForFixedUpdate;

    private bool isSubscribed = false;

    // Astar Path 최적화 변수 
    [Tooltip("해당 변수를 통해 플레이어와 몬스터 간의 거리 격차를 설정, 몸의 크기에 따라 해당 변수 값을 조절한다.")]
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
            // 이벤트 구독
            onIdle += EnemyIdle;
            onMove += EnemyMove;
            isSubscribed = true;  // 구독 상태 업데이트
        }
    }

    private void OnDisable()
    {
        if (isSubscribed)
        {
            // 이벤트 해제
            onIdle -= EnemyIdle;
            onMove -= EnemyMove;

            isSubscribed = false;  // 구독 상태 초기화
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
        // - 현재 Player 위치가 이전에 설정했던 playerPosition보다 playerDistanceToRebuildPath만큼 차이가 
        //    난다면 경로를 갱신 
        if ((GameManager.Instance.GetPlayerPosition() - playerPosition).sqrMagnitude > playerDistanceToRebuildPath * playerDistanceToRebuildPath)
        {
            // Reset playerPosition 
            playerPosition = GameManager.Instance.GetPlayerPosition();

            // Move the enemy using AStar pathfinding - Trigger rebuild of path to player
            CreatePath();

            // if a path has been found move the enemy
            if (movementSteps != null)
            {
                // 이미 Enemy가 Move를 하고 있으면 새로운 path로 플레이어를 추적
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
        // 몬스터는 Room 중에서 StageRoom 
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
        // → StartGrid는 몬스터가 이미 있는 공간이기 때문에, 해당 gridPosition을 Pop하고 Path를 보낸다. 
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
        // 일단 속력 이동으로 해보다가 별로면 rigidbody 이동으로 수정하기 
        rigidbody.velocity = moveDirection * moveSpeed;
    }
}
