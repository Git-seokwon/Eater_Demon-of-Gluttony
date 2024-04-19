using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// ※ CursorType : 마우스 커서의 Texture을 바꾸는데 사용
// ex) Skill을 쓸 때, 마우스 커서가 조준점 모양으로 변경 
public enum CursorType { Default, BlueArrow }

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : SingletonMonobehaviour<PlayerController>
{
    #region Event
    public delegate void MovementKeyDownHandler(Vector2 direction, float moveSpeed);
    public delegate void DashKeyDownHandler(Vector3 direction);
    public delegate void IdleHandler();
    // 마우스 클릭을 하면 호출할 Event
    public delegate void ClickedHandler(Vector2 mousePosition);
    #endregion

    // Type에 따라서 정해진 Texture로 Cursor Texture을 변경
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
        // 키보드 입력에 따라 적절한 Event 호출
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

        // 마우스 클릭에 따라 적절한 Event 호출
        if (Input.GetMouseButtonDown(0))
            onLeftClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
        else if (Input.GetMouseButtonDown(1))
            onRightClicked?.Invoke(HelperUtilities.GetMouseWorldPosition());
    }

    private void MovementInput()
    {
        // 수평 입력
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        // 수직 입력
        verticalMovement = Input.GetAxisRaw("Vertical");
        // 스페이스바 입력
        spaceDown = Input.GetButton("Dash");

        // 수평, 수직 입력에 따른 Vector2 값 가져오기 
        MoveDirection = new Vector2(horizontalMovement, verticalMovement);
        // 대각선 이동의 경우 Normalize되어 있지 않기 때문에 0.7을 곱해준다. 
        if (!Mathf.Approximately(horizontalMovement, 0f) && !Mathf.Approximately(verticalMovement, 0f))
            MoveDirection *= 0.7f;
    }

    public void ChangeCursor(CursorType newType)
    {
        if (newType == CursorType.Default)
            // ※ null : 기본 Mouse Texture
            // ※ Vector2.zero : Pivot(0, 0)
            // ※ CursorMode.Auto : CursorMode는 Platform에 따라 자동 선택
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        else
        {
            // ※ First : 데이터 집합에서 조건을 만족하는 첫 번째 요소를 반환
            var cursorTexture = cursorDatas.First(x => x.type == newType).texture;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
