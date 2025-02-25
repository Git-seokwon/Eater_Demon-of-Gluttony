using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialEnemyMovement : EntityMovement
{
    public bool isMoveStart;

    [SerializeField]
    private StageRoom tutorialStage;

    public delegate void IdleHander();
    public delegate void moveHander(Vector3 movePosition, float speed);
    public event IdleHander onIdle;
    public event moveHander onMove;

    // Path Node
    private Stack<Vector3> movementSteps = new Stack<Vector3>();

    // Player Position : Target Position
    private Vector2 playerPosition;

    // Movement Coroutine
    private Coroutine moveEnemyRoutine;

    [Tooltip("�ش� ������ ���� �÷��̾�� ���� ���� �Ÿ� ������ ����, ���� ũ�⿡ ���� �ش� ���� ���� �����Ѵ�.")]
    private float playerDistanceToRebuildPath = 0.4f;

    public override void Setup(Entity owner)
    {
        base.Setup(owner);
    }

    private void OnEnable()
    {
        // �̺�Ʈ ����
        onIdle += EnemyIdle;
        onMove += EnemyMove;
    }

    private void OnDisable()
    {
        // �̺�Ʈ ����
        onIdle -= EnemyIdle;
        onMove -= EnemyMove;
        
        // ����
        EnemyIdle();
    }

    private void Start()
    {
        playerPosition = GameManager.Instance.player.transform.position;
    }

    private void Update()
    {
        if (isMoveStart)
            MoveEnemy();
    }

    public void MoveEnemy()
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
        if (tutorialStage == null)
            return;

        Grid grid = tutorialStage.grid;

        // Get players position on the grid
        Vector3Int playerGridPosition = GetNearsetPlayerPosition(tutorialStage);

        // Get enemy position on the grid
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        // Build a path for the enemy to move on 
        movementSteps = AStar.BuildPath(tutorialStage, enemyGridPosition, playerGridPosition);

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

                yield return null;
            }

            yield return null;
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

    private void EnemyIdle()
    {
        rigidbody.velocity = Vector2.zero;
    }

    private void EnemyMove(Vector3 targetPosition, float moveSpeed)
    {
        Vector2 direction = ((Vector2)targetPosition - rigidbody.position).normalized;
        Vector2 newPosition = rigidbody.position + direction * moveSpeed * Time.deltaTime * 3f;
        rigidbody.MovePosition(newPosition);
    }

    public IEnumerator MoveToPosition(Vector3 movePosition, System.Action<bool> callback)
    {
        while ((movePosition - transform.position).sqrMagnitude > 0.1f * 0.1f)
        {
            Vector3 direction = (movePosition - transform.position).normalized;
            rigidbody.velocity = direction * MoveSpeed;
            yield return null;
        }

        onIdle?.Invoke();
        callback?.Invoke(true);
    }
}
