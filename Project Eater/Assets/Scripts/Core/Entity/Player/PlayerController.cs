using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// �� PlayerMode : �÷��̾� ���� 
// �� PlayerMode�� ���� Animator Cotroller�� �����ϰų� Ư�� ����� Open �ϰų� Close �Ѵ�. 
public enum PlayerMode { Default, Devil }

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : SingletonMonobehaviour<PlayerController>
{
    #region Event
    public delegate void MovementKeyDownHandler(Vector2 direction, float moveSpeed);
    public delegate void DashKeyDownHandler(Vector2 direction, float moveSpeed);
    public delegate void IdleHandler();
    // ���콺 Ŭ���� �ϸ� ȣ���� Event
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

    #region UI ��ȣ�ۿ�
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
        // ���콺 Ŭ���� ���� ������ Event ȣ��
        if (Input.GetMouseButtonDown(0))
            onLeftClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
        else if (Input.GetMouseButtonDown(1))
            onRightClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
    }

    private void FixedUpdate()
    {
        // Ű���� �Է¿� ���� ������ Event ȣ��
        if (MoveDirection != Vector2.zero)
        {
            if (dashQueued)
            {
                // �뽬 ����
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
        // ���� �Է�
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        // ���� �Է�
        verticalMovement = Input.GetAxisRaw("Vertical");

        // ����, ���� �Է¿� ���� Vector2 �� �������� 
        MoveDirection = new Vector2(horizontalMovement, verticalMovement);
        // �밢�� �̵��� ��� Normalize�Ǿ� ���� �ʱ� ������ 0.7�� �����ش�. 
        if (!Mathf.Approximately(horizontalMovement, 0f) && !Mathf.Approximately(verticalMovement, 0f))
            MoveDirection *= 0.7f;
    }

    // �÷��̾� �̵� ������ �ٶ󺸴� �Լ� 
    private void LookMoveDirection(Vector2 direction, float moveSpeed)
    {
        // ���� �̵��� ���, ���콺 Ŀ�� ������ �ٶ󺸵��� �Ѵ�. 
        if (Mathf.Approximately(direction.x, 0f) || playerMovement.IsDashing) return;

        // �̵� ����(�¿�)�� direction.x ������ �����ȴ�. 
        if (direction.x > 0)
            transform.localScale = new Vector2(-1, 1);
        if (direction.x < 0)
            transform.localScale = new Vector2(1, 1);
    }

    // �÷��̾� ��� ���� �Լ� 
    // �� �������� ���� ��, PlayerMode.Devil
    // �� �κ� ���� ��, PlayerMode.Default
    public void SetPlayerMode(PlayerMode newMode)
    {
        playerMode = newMode;

        // �� �÷��� ��忡 ���� �߰� ���� ex) �ִϸ����� ����
        if (newMode == PlayerMode.Devil)
        {
            GameManager.Instance.player.Animator.runtimeAnimatorController = devilAnimatorController;
        }
        else
        {
            GameManager.Instance.player.Animator.runtimeAnimatorController = defaultAnimatorController;
        }
    }

    // ��ų �ε������Ͱ� Ȱ��ȭ ���� �÷��̾� ���� �� ��, ��ų ����� ����ϱ� ���� ���� Public �Լ�
    // �� TargetSelect���� ���콺 ��Ŭ�� ��, Select�� ����Ѵ�. 
    public void OnRightClickedEventHandle() => onRightClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
}
