using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// ※ PlayerMode : 플레이어 변신 
// → PlayerMode에 따라 Animator Cotroller를 변경하거나 특정 기능을 Open 하거나 Close 한다. 
public enum PlayerMode { Default, Devil }

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : SingletonMonobehaviour<PlayerController>
{
    #region Event
    public delegate void MovementKeyDownHandler(Vector2 direction, float moveSpeed);
    public delegate void DashKeyDownHandler(Vector2 direction, float moveSpeed);
    public delegate void IdleHandler();
    // 마우스 클릭을 하면 호출할 Event
    public delegate void ClickedHandler(Vector2 mousePosition);
    #endregion

    #region Event variable
    public event MovementKeyDownHandler onMovementKeyDown;
    public event DashKeyDownHandler onDashKeyDown;
    public event IdleHandler onIdle;
    public event ClickedHandler onLeftClicked;
    public event ClickedHandler onRightClicked;
    #endregion

    private PlayerMode playerMode;
    public PlayerMode PlayerMode => playerMode;

    [Space(10)]
    [SerializeField]
    private RuntimeAnimatorController defaultAnimatorController;
    [SerializeField]
    private RuntimeAnimatorController devilAnimatorController;

    private PlayerMovement playerMovement;

    #region Player Input
    float horizontalMovement, verticalMovement;
    public Vector2 MoveDirection { get; private set; }

    private bool dashQueued = false;
    #endregion

    #region UI 상호작용
    private bool isInterActive;
    public bool IsInterActive
    {
        get => isInterActive;
        set => isInterActive = value;
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        dashQueued = false;
        MoveDirection = Vector2.zero;
        horizontalMovement = verticalMovement = 0f;
        onMovementKeyDown += LookMoveDirection;
    }

    private void OnDisable()
    {
        dashQueued = false;
        MoveDirection = Vector2.zero;
        horizontalMovement = verticalMovement = 0f;
        onMovementKeyDown -= LookMoveDirection;
    }

    private void Update()
    {
        MovementInput();

        if (playerMode != PlayerMode.Devil)
            return;

        if (Input.GetButtonDown("Dash"))
            dashQueued = true;
        // 마우스 클릭에 따라 적절한 Event 호출
        if (Input.GetMouseButtonDown(0))
            onLeftClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
        else if (Input.GetMouseButtonDown(1))
            onRightClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
    }

    private void FixedUpdate()
    {
        // 키보드 입력에 따라 적절한 Event 호출
        if (MoveDirection != Vector2.zero)
        {
            if (dashQueued)
            {
                // 대쉬 실행
                onDashKeyDown?.Invoke((Vector3)MoveDirection, playerMovement.MoveSpeed);
                dashQueued = false;
            }
            else
            {
                onMovementKeyDown?.Invoke(MoveDirection, playerMovement.MoveSpeed);
            }
        }
        else
        {
            onIdle?.Invoke();
        }
    }

    private void MovementInput()
    {
        // 수평 입력
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        // 수직 입력
        verticalMovement = Input.GetAxisRaw("Vertical");

        // 수평, 수직 입력에 따른 Vector2 값 가져오기 
        MoveDirection = new Vector2(horizontalMovement, verticalMovement);
        // 대각선 이동의 경우 Normalize되어 있지 않기 때문에 0.7을 곱해준다. 
        if (!Mathf.Approximately(horizontalMovement, 0f) && !Mathf.Approximately(verticalMovement, 0f))
            MoveDirection *= 0.7f;
    }

    // 플레이어 이동 방향을 바라보는 함수 
    private void LookMoveDirection(Vector2 direction, float moveSpeed)
    {
        // 수직 이동의 경우, 마우스 커서 방향을 바라보도록 한다. 
        if (Mathf.Approximately(direction.x, 0f) || playerMovement.IsDashing) return;

        // 이동 방향(좌우)은 direction.x 값으로 결정된다. 
        if (direction.x > 0)
            transform.localScale = new Vector2(-1, 1);
        if (direction.x < 0)
            transform.localScale = new Vector2(1, 1);
    }

    // 플레이어 모드 변경 함수 
    // → 스테이지 입장 시, PlayerMode.Devil
    // → 로비 입장 시, PlayerMode.Default
    public void SetPlayerMode(PlayerMode newMode)
    {
        playerMode = newMode;

        // 각 플레이 모드에 따른 추가 설정 ex) 애니메이터 변경
        if (newMode == PlayerMode.Devil)
        {
            GameManager.Instance.player.Animator.runtimeAnimatorController = devilAnimatorController;
        }
        else
        {
            GameManager.Instance.player.Animator.runtimeAnimatorController = defaultAnimatorController;
        }
    }

    // 스킬 인디케이터가 활성화 이후 플레이어 레벨 업 시, 스킬 사용을 취소하기 위해 만든 Public 함수
    // → TargetSelect에서 마우스 우클릭 시, Select를 취소한다. 
    public void OnRightClickedEventHandle() => onRightClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
}
