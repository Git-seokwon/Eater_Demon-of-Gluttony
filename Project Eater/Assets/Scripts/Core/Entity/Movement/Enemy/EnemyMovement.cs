using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

public class EnemyMovement : EntityMovement
{
    #region Event
    public delegate void IdleHander();
    public delegate void moveHander(Vector3 movePosition, float speed);
    public event IdleHander onIdle;
    public event moveHander onMove;
    #endregion

    [SerializeField]
    private float separationRadius;
    private float neighborUpdateTime = 0f; // ������ ���� �ð�
    private HashSet<Transform> neighborEnemies = new HashSet<Transform>(); // �̿� ����
    private bool isSubscribed = false;

    public override void Setup(Entity owner)
    {
        base.Setup(owner);
    }

    private void OnEnable()
    {
        if (!isSubscribed)
        {
            neighborUpdateTime = 0f;

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

    private void EnemyMove(Vector3 moveDirection, float moveSpeed)
    {
        // �ϴ� �ӷ� �̵����� �غ��ٰ� ���θ� rigidbody �̵����� �����ϱ� 
        rigidbody.velocity = moveDirection * moveSpeed;
    }

    private void FixedUpdate()
    {
        if (GridController.Instance.currentFlowField == null) return;
        neighborUpdateTime += Time.fixedDeltaTime;

        if (neighborUpdateTime > Settings.neighborUpdateInterval)
        {
            UpdateNeighborEnemies();
            neighborUpdateTime = 0f;
        }

        Cell cellBelow = GridController.Instance.currentFlowField.GetCellFromWorldPos(transform.position);
        if (cellBelow.bestDirection == GridDirection.None)
            onIdle?.Invoke();
        else
        {
            Vector3 moveDriection = new Vector3(cellBelow.bestDirection.Vector.x, cellBelow.bestDirection.Vector.y, 0f);
            Vector3 separationDirection = CalculateSeparation();

            Vector3 finalDirection = (moveDriection + separationDirection).normalized;
            onMove?.Invoke(finalDirection, MoveSpeed);
        }
    }

    private void UpdateNeighborEnemies()
    {
        neighborEnemies.Clear(); // ���� ����Ʈ �ʱ�ȭ
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius, LayerMask.GetMask("Enemy"));

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject) // ������ ����
            {
                neighborEnemies.Add(neighbor.transform);
            }
        }
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;

        foreach (var neighbor in neighborEnemies)
        {
            if (neighbor != null) // ������ ����
            {
                Vector3 directionAway = transform.position - neighbor.transform.position;
                float distance = directionAway.magnitude;
                if (distance > 0f)
                    separationForce += directionAway.normalized / distance; // �Ÿ��� �������� �� ���ϰ� ȸ��
            }
        }

        return separationForce.normalized; // ���� Separation ����
    }
}
