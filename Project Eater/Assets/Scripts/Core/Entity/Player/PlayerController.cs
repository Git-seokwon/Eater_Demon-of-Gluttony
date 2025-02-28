using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// �� CursorType : ���콺 Ŀ���� Texture�� �ٲٴµ� ���
// ex) Skill�� �� ��, ���콺 Ŀ���� ������ ������� ���� 
public enum CursorType { Default, BlueArrow }

// �� PlayerMode : �÷��̾� ���� 
// �� PlayerMode�� ���� Animator Cotroller�� �����ϰų� Ư�� ����� Open �ϰų� Close �Ѵ�. 
public enum PlayerMode { Default, Devil }

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : SingletonMonobehaviour<PlayerController>
{
    #region Event
    public delegate void MovementKeyDownHandler(Vector2 direction, float moveSpeed);
    public delegate void DashKeyDownHandler(Vector3 direction);
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

    // Type�� ���� ������ Texture�� Cursor Texture�� ����
    [System.Serializable]
    private struct CursorData
    {
        public CursorType type;
        public Texture2D texture;
    }

    [SerializeField]
    private CursorData[] cursorDatas;

    [Space(10)]
    [SerializeField]
    private RuntimeAnimatorController defaultAnimatorController;
    [SerializeField]
    private RuntimeAnimatorController devilAnimatorController;

    private PlayerMovement playerMovement;

    #region Player Input
    float horizontalMovement, verticalMovement;
    [HideInInspector]
    public bool spaceDown;
    public Vector2 MoveDirection { get; private set; }
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
        playerMode = PlayerMode.Devil;
    }

    private void OnDisable()
    {
        MoveDirection = Vector2.zero;
    }

    private void Update()
    {
        MovementInput();

        if (playerMode != PlayerMode.Devil)
            return;

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
            if (!spaceDown)
            {
                onMovementKeyDown?.Invoke(MoveDirection, playerMovement.MoveSpeed);
            }
            else
            {
                onDashKeyDown?.Invoke((Vector3)MoveDirection);
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

        if (playerMode == PlayerMode.Devil)
            // �����̽��� �Է�
            spaceDown = Input.GetButton("Dash");

        // ����, ���� �Է¿� ���� Vector2 �� �������� 
        MoveDirection = new Vector2(horizontalMovement, verticalMovement);
        // �밢�� �̵��� ��� Normalize�Ǿ� ���� �ʱ� ������ 0.7�� �����ش�. 
        if (!Mathf.Approximately(horizontalMovement, 0f) && !Mathf.Approximately(verticalMovement, 0f))
            MoveDirection *= 0.7f;
    }

    public void ChangeCursor(CursorType newType)
    {
        if (newType == CursorType.Default)
            // �� null : �⺻ Mouse Texture
            // �� Vector2.zero : Pivot(0, 0)
            // �� CursorMode.Auto : CursorMode�� Platform�� ���� �ڵ� ����
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        else
        {
            // �� First : ������ ���տ��� ������ �����ϴ� ù ��° ��Ҹ� ��ȯ
            var cursorTexture = cursorDatas.First(x => x.type == newType).texture;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
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
}
