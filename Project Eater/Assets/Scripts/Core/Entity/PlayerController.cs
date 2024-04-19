using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// �� CursorType : ���콺 Ŀ���� Texture�� �ٲٴµ� ���
// ex) Skill�� �� ��, ���콺 Ŀ���� ������ ������� ���� 
public enum CursorType { Default, BlueArrow }

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

    // Type�� ���� ������ Texture�� Cursor Texture�� ����
    [System.Serializable]
    private struct CursorData
    {
        public CursorType type;
        public Texture2D texture;
    }

    [SerializeField]
    private CursorData[] cursorDatas;

    #region Event variable
    public event MovementKeyDownHandler onMovementKeyDown;
    public event DashKeyDownHandler onDashKeyDown;
    public event IdleHandler onIdle;
    public event ClickedHandler onLeftClicked;
    public event ClickedHandler onRightClicked;
    #endregion

    private PlayerMovement player;

    #region Player Input
    float horizontalMovement, verticalMovement;
    private bool spaceDown;
    public Vector2 MoveDirection { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        MovementInput();
    }

    private void FixedUpdate()
    {
        // Ű���� �Է¿� ���� ������ Event ȣ��
        if (MoveDirection != Vector2.zero)
        {
            if (!spaceDown)
            {
                onMovementKeyDown?.Invoke(MoveDirection, player.MoveSpeed);
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

        // ���콺 Ŭ���� ���� ������ Event ȣ��
        if (Input.GetMouseButtonDown(0))
            onLeftClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
        else if (Input.GetMouseButtonDown(1))
            onRightClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
    }

    private void MovementInput()
    {
        // ���� �Է�
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        // ���� �Է�
        verticalMovement = Input.GetAxisRaw("Vertical");
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
}
