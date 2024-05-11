using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : EntityMovement
{
    #region Dash Variable
    [SerializeField]
    private float dashDistance = 8f; // �뽬 �Ÿ� 
    [SerializeField]
    private float dashSpeed = 10f; // �뽬 �ӵ�
    [SerializeField]
    private float playerDashCoolTime = 2.0f; // �뽬 ��Ÿ��
    private float playerDashTimer; // ��Ÿ�� üũ
    #endregion

    // ���� �뽬 �������� ���� ����
    public bool IsDashing { get; private set; }

    // Dash Coroutine ���� ������
    private Coroutine playerDashCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;

    // Player Look & Aim ���� ������
    [HideInInspector] public Vector3 aimDirection;
    [HideInInspector] public float aimAngleDegrees, playerAngleDegrees;
    [HideInInspector] public AimDirection playerLookDirection;
    // basicAttackPosition�� Animation���� ������ basicAttackPosition �� ���� ������Ʈ�� �Ҵ��� ����
    public Transform attackPosition;

    // PlayerMovement Setup �Լ� - �θ� Ŭ������ Awake �̺�Ʈ �޼��忡�� �����
    public override void Setup(Entity owner)
    {
        base.Setup(owner);

        // �̺�Ʈ ���� 
        PlayerController.Instance.onIdle += PlayerIdle;
        PlayerController.Instance.onMovementKeyDown += PlayerMove;
        PlayerController.Instance.onDashKeyDown += PlayerDash;

        // waitForFixedUpdate �ʱ�ȭ
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void OnDisable()
    {
        // �̺�Ʈ ���� ��� 
        PlayerController.Instance.onIdle -= PlayerIdle;
        PlayerController.Instance.onMovementKeyDown -= PlayerMove;
        PlayerController.Instance.onDashKeyDown -= PlayerDash;
    }

    private void Update()
    {
        SetAimArgument(out aimDirection, out aimAngleDegrees, out playerAngleDegrees, out playerLookDirection);

        LookAt(playerLookDirection);

        PlayerDashCoolDownTimer();
    }

    // �÷��̾� Aim ���� ������ Set
    private void SetAimArgument(out Vector3 aimDirection, out float aimAngleDegrees, out float playerAngleDegrees, out AimDirection playerLookDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        // attackPosition�� Vector3 ��ǥ �������� (transform�� position ���� Vector3��)
        Vector3 basicAttackPosition = this.attackPosition.position;

        // basicAttackPosition �� mouseWorldPosition�� ���� ���ϱ� 
        aimDirection = (mouseWorldPosition - basicAttackPosition);

        // transform.position �� mouseWorldPosition�� ���� ���ϱ� 
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        aimAngleDegrees = HelperUtilities.GetAngleFromVector(aimDirection);

        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        playerLookDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
    }

    // �÷��̾ �ٶ󺸴� ���� ����
    private void LookAt(AimDirection playerLookDirection)
    {
        // �뽬 ���� ���� ���� ��ȯ X
        if (IsDashing)
            return;

        // ������ ĳ���Ͱ� ������ ���� �ֱ� ������ �ش� ��������Ʈ�� flipX�� true�� �Ǿ� �������� �ٶ󺻴�. 
        switch (playerLookDirection)
        {
            // ������ ���� 
            case AimDirection.Right:
                sprite.flipX = true;
                break;

            // ���� ���� 
            case AimDirection.Left:
                sprite.flipX = false;
                break;

            default:
                break;
        }
    }

    #region �浹 ó�� : �浹 �� �뽬�� �����. 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopPlayerDashRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerDashRoutine();
    }
    #endregion

    // �÷��̾� ����
    private void PlayerIdle()
    {
        rigidbody.velocity = Vector2.zero;
    }

    // �÷��̾� �̵�
    private void PlayerMove(Vector2 direction, float moveSpeed)
    {
        rigidbody.velocity = direction * moveSpeed; 
    }

    // �÷��̾� �뽬
    private void PlayerDash(Vector3 direction)
    {
        // �뽬 ��Ÿ���� ���� �ִٸ� �������� ���Ѵ�. 
        if (playerDashTimer > 0f)
            return;

        // �뽬 flag On
        IsDashing = true;

        // ������ �뽬�� �����ϴ� �ڷ�ƾ �Լ� 
        // �� playerDashCoroutine�� ���� ���� �޾Ƴ��� ������ ���� �浹 ó�� ������ �޾� ���� ����
        playerDashCoroutine = StartCoroutine(PlayerDashRoutine(direction));

        // �뽬 flag Off
        IsDashing = false;
    }

    private IEnumerator PlayerDashRoutine(Vector3 direction)
    {
        // �ּ� �Ÿ� ����
        float minDistance = 0.2f;

        // �뽬 Ÿ�� Position
        Vector3 targetPosition = transform.position + (Vector3)direction * dashDistance;

        // while ������ �뽬 Ÿ�� �����ǿ� �����ߴ��� üũ 
        while (Vector3.SqrMagnitude(targetPosition - transform.position) > Mathf.Pow(minDistance, 2))
        {
            // ���� ���� ���ϱ� 
            Vector2 unitVec = Vector3.Normalize(targetPosition - transform.position);

            // Rigidbody2D�� MovePosition �޼��带 �̿��Ͽ� �̵� 
            rigidbody.MovePosition(rigidbody.position + (unitVec * dashSpeed * Time.fixedDeltaTime));

            // FixedUpdate �ֱ⸶�� ����
            // �� FixedUpdate���� �� �ѹ��� �����ϰ� ����Ѵ�. 
            yield return waitForFixedUpdate;
        }

        // ��Ÿ�� �缳��
        playerDashTimer = playerDashCoolTime;
    }

    // �뽬 �ڷ�ƾ ���� �Լ� 
    private void StopPlayerDashRoutine()
    {
        if (playerDashCoroutine != null)
        {
            StopCoroutine(playerDashCoroutine);

            IsDashing = false;
            playerDashTimer = playerDashCoolTime;
        }
    }

    // �뽬 ��Ÿ�� �Լ�
    private void PlayerDashCoolDownTimer()
    {
        if (playerDashTimer >= 0f)
            playerDashTimer -= Time.deltaTime;
    }
}
