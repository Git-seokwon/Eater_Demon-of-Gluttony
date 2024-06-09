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
    private Vector3 playerPosition;
    // Movement Coroutine
    private Coroutine moveEnemyRoutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    // 경로 갱신 시간
    private float currentEnemyPathRebuildCooldown;
    // 플레이어를 쫓는 중 인지 여부 
    private bool chasePlayer = false;
    // 추적 거리 
    [SerializeField] private float chaseDistance;

    // Astar Path 최적화 변수 
    // → Default value, Enemy Spawner에서 값이 set 된다. 
    [HideInInspector] public int updateFrameNumber = 1;

    public override void Setup(Entity owner)
    {
        base.Setup(owner);

        // 이벤트 구독 
        onIdle += EnemyIdle;
        onMove += EnemyMove;

        waitForFixedUpdate = new WaitForFixedUpdate();
        playerPosition = GameManager.Instance.GetPlayerPosition();
    }

    private void OnDisable()
    {
        onIdle -= EnemyIdle;
        onMove -= EnemyMove;
    }

    private void EnemyIdle()
    {
        rigidbody.velocity = Vector2.zero;
    }

    private void EnemyMove(Vector3 movePosition, float moveSpeed)
    {
        // 일단 속력 이동으로 해보다가 별로면 rigidbody 이동으로 수정하기 
        Vector2 unitVector = Vector3.Normalize(movePosition - transform.position);
        rigidbody.velocity = unitVector * moveSpeed;
    }

    private void Update()
    {
        MoveEnemy();
    }

    // Use Astar pathfinding to build a path to the player - and then move the enemy to each grid location on the path 
    private void MoveEnemy()
    {
        // Movement cooldown timer
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        // Check distance to player to see if enemy should start chasing 
        if (!chasePlayer &&
            Vector3.SqrMagnitude(playerPosition - transform.position) < Mathf.Pow(chaseDistance, 2))
        {
            chasePlayer = true;
        }

        if (!chasePlayer)
            return;

        // ※ Time.frameCount : https://docs.unity3d.com/ScriptReference/Time-frameCount.html
        // Only process A star path rebuild on certain frames to spread the load between enemies
        // Ex) updateFrameNumber이 1일 경우,  Time.frameCount가 1, 61, 121처럼 60 프레임이 될때만 if문을 만족하지 않아
        //     아래 코드들(CreatePath)을 실행한다.
        // → 60 프레임마다 Path를 재설정 
        if (Time.frameCount % Settings.targetFrameRateForPathFind != updateFrameNumber) return;

        // if the movement cooldown timer reached or player has moved more than required distance
        // then rebuild the enemy path and move the enemy 
        // 1. currentEnemyPathRebuildCooldown 시간이 지나면 경로 갱신
        // 2. 현재 Player 위치가 이전에 설정했던 playerPosition보다 playerDistanceToRebuildPath만큼 차이가 
        //    난다면 경로를 갱신 
        if (currentEnemyPathRebuildCooldown <= 0f ||
            Vector3.SqrMagnitude(GameManager.Instance.GetPlayerPosition() - playerPosition) > Mathf.Pow(Settings.playerDistanceToRebuildPath, 2))
        {
            // Reset path rebuild cooldown timer
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;
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
        // → TargetGrid는 몬스터가 이미 있는 공간이기 때문에, 해당 gridPosition을 Pop하고 Path를 보낸다. 
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
            while (Vector3.SqrMagnitude(nextPosition - transform.position) > Mathf.Pow(0.2f, 2))
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
