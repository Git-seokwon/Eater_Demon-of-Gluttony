using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : EntityMovement
{
    #region Dash Variable
    [SerializeField]
    private float dashDistance = 8f; // 대쉬 거리 
    [SerializeField]
    private float dashSpeed = 10f; // 대쉬 속도
    [SerializeField]
    private float playerDashCoolTime = 2.0f; // 대쉬 쿨타임
    private float playerDashTimer; // 쿨타임 체크
    #endregion

    // 현재 대쉬 중인지에 대한 여부
    public bool IsDashing { get; private set; }

    // Dash Coroutine 관련 변수들
    private Coroutine playerDashCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;

    // Player Look & Aim 관련 변수들
    [HideInInspector] public Vector3 aimDirection;
    [HideInInspector] public float aimAngleDegrees, playerAngleDegrees;
    [HideInInspector] public AimDirection playerLookDirection;
    // basicAttackPosition은 Animation에서 생성한 basicAttackPosition 빈 게임 오브젝트를 할당할 예정
    public Transform attackPosition;

    // PlayerMovement Setup 함수 - 부모 클래스의 Awake 이벤트 메서드에서 실행됨
    public override void Setup(Entity owner)
    {
        base.Setup(owner);

        // 이벤트 구독 
        PlayerController.Instance.onIdle += PlayerIdle;
        PlayerController.Instance.onMovementKeyDown += PlayerMove;
        PlayerController.Instance.onDashKeyDown += PlayerDash;

        // waitForFixedUpdate 초기화
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void OnDisable()
    {
        // 이벤트 구독 취소 
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

    // 플레이어 Aim 관련 변수들 Set
    private void SetAimArgument(out Vector3 aimDirection, out float aimAngleDegrees, out float playerAngleDegrees, out AimDirection playerLookDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        // attackPosition의 Vector3 좌표 가져오기 (transform의 position 값은 Vector3임)
        Vector3 basicAttackPosition = this.attackPosition.position;

        // basicAttackPosition → mouseWorldPosition의 방향 구하기 
        aimDirection = (mouseWorldPosition - basicAttackPosition);

        // transform.position → mouseWorldPosition의 방향 구하기 
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        aimAngleDegrees = HelperUtilities.GetAngleFromVector(aimDirection);

        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        playerLookDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
    }

    // 플레이어가 바라보는 방향 설정
    private void LookAt(AimDirection playerLookDirection)
    {
        // 대쉬 중일 때는 방향 전환 X
        if (IsDashing)
            return;

        // 보내준 캐릭터가 왼쪽을 보고 있기 때문에 해당 스프라이트의 flipX가 true가 되야 오른쪽을 바라본다. 
        switch (playerLookDirection)
        {
            // 오른쪽 보기 
            case AimDirection.Right:
                sprite.flipX = true;
                break;

            // 왼쪽 보기 
            case AimDirection.Left:
                sprite.flipX = false;
                break;

            default:
                break;
        }
    }

    #region 충돌 처리 : 충돌 시 대쉬를 멈춘다. 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopPlayerDashRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerDashRoutine();
    }
    #endregion

    // 플레이어 정지
    private void PlayerIdle()
    {
        rigidbody.velocity = Vector2.zero;
    }

    // 플레이어 이동
    private void PlayerMove(Vector2 direction, float moveSpeed)
    {
        rigidbody.velocity = direction * moveSpeed; 
    }

    // 플레이어 대쉬
    private void PlayerDash(Vector3 direction)
    {
        // 대쉬 쿨타임이 남아 있다면 재사용하지 못한다. 
        if (playerDashTimer > 0f)
            return;

        // 대쉬 flag On
        IsDashing = true;

        // 실제로 대쉬를 수행하는 코루틴 함수 
        // → playerDashCoroutine로 리턴 값을 받아놓는 이유는 위에 충돌 처리 때문에 받아 놓는 것임
        playerDashCoroutine = StartCoroutine(PlayerDashRoutine(direction));

        // 대쉬 flag Off
        IsDashing = false;
    }

    private IEnumerator PlayerDashRoutine(Vector3 direction)
    {
        // 최소 거리 오차
        float minDistance = 0.2f;

        // 대쉬 타겟 Position
        Vector3 targetPosition = transform.position + (Vector3)direction * dashDistance;

        // while 문으로 대쉬 타겟 포지션에 도달했는지 체크 
        while (Vector3.SqrMagnitude(targetPosition - transform.position) > Mathf.Pow(minDistance, 2))
        {
            // 유닛 백터 구하기 
            Vector2 unitVec = Vector3.Normalize(targetPosition - transform.position);

            // Rigidbody2D의 MovePosition 메서드를 이용하여 이동 
            rigidbody.MovePosition(rigidbody.position + (unitVec * dashSpeed * Time.fixedDeltaTime));

            // FixedUpdate 주기마다 실행
            // → FixedUpdate에서 딱 한번만 실행하고 대기한다. 
            yield return waitForFixedUpdate;
        }

        // 쿨타임 재설정
        playerDashTimer = playerDashCoolTime;
    }

    // 대쉬 코루틴 정지 함수 
    private void StopPlayerDashRoutine()
    {
        if (playerDashCoroutine != null)
        {
            StopCoroutine(playerDashCoroutine);

            IsDashing = false;
            playerDashTimer = playerDashCoolTime;
        }
    }

    // 대쉬 쿨타임 함수
    private void PlayerDashCoolDownTimer()
    {
        if (playerDashTimer >= 0f)
            playerDashTimer -= Time.deltaTime;
    }
}
