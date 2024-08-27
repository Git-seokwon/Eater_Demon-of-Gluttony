using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SelectTarget : TargetSelectionAction
{
    [Header("Data")]
    [Min(0f)]
    [SerializeField]
    private float range;
    [Range(0f, 360f)]
    [SerializeField]
    private float angle;

    private PlayerController playerController;
    private TargetSearcher targetSearcher;
    private Entity requestEntity;
    private GameObject requestObject;
    private SelectCompletedHandler onSelectCompleted;

    public override float Range => range;
    public override float ScaledRange => range * Scale;
    public override float Angle => angle;

    #region 생성자
    public SelectTarget() { }
    public SelectTarget(SelectTarget copy)
        : base(copy) 
    { 
        range = copy.range;
        angle = copy.angle;
    }
    #endregion

    // ※ SelectImmediateByPlayer
    // → SelectTarget Class에서 구현하는 것이 아니라 SelectTarget의 자식 Class인
    // SelectSelfByOneClick, SelectPosition, SelectEntity Class에서 구현한다. 

    protected void ResetPlayerController()
    {
        playerController.ChangeCursor(CursorType.Default);
        playerController.onLeftClicked -= OnMouseLeftClick;
        playerController.onRightClicked -= OnMouseRightClick;
        playerController = null;
    }

    public override void Select(TargetSearcher targetsearcher, Entity requestEntity, GameObject requestObject, 
        SelectCompletedHandler onSelectCompleted)
    {
        if (requestEntity.IsPlayer)
        {
            this.targetSearcher = targetsearcher;
            this.requestEntity = requestEntity;
            this.requestObject = requestObject;
            this.onSelectCompleted = onSelectCompleted;

            playerController = PlayerController.Instance;
            playerController.ChangeCursor(CursorType.BlueArrow);
            playerController.onLeftClicked += OnMouseLeftClick;
            playerController.onRightClicked += OnMouseRightClick;
        }
        // ※ Target position : Player position
        else
            onSelectCompleted.Invoke(SelectImmediateByEnemy(targetsearcher, requestEntity, requestObject, 
                requestEntity.Target.transform.position));
    }

    public override void CancelSelect(TargetSearcher targetSearcher)
    {
        if (playerController)
            ResetPlayerController();
    }

    public override bool IsInRange(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, Vector2 targetPosition)
    {
        var requestTransform = requestObject.transform;

        // Vector3.SqrMagnitude를 사용하기 때문에 range를 제곱한 값을 사용한다. 
        float sqrRange = ProperRange * ProperRange;
            // range * range * (IsUseScale ? Scale * Scale : 1f);

        // 상대 좌표 구하기 
        Vector2 relativePosition = targetPosition - (Vector2)requestTransform.position;

        // 상대 좌표와 requestTransform의 정방향(2D 기준 X 축)으로 각도를 구한다. 
        // → 나중에 버그가 발생하면 requestEntity와 requestObject가 같은 객체 일때만 이라는 조건을 달아서 true면 requestEntity.EnitytSight를
        //    아니면 -1을 곱한다. 
        float angle = Vector2.Angle(relativePosition, (requestTransform.right * requestEntity.EnitytSight));

        // 구한 각도가 범위 내에 있는 지 확인 
        bool IsInAngle = angle <= (Angle / 2f);

        // 검색 범위가 무한 이거나 
        // target이 Range와 Angle 안에 있다면 true
        return Mathf.Approximately(0f, range) || 
            (Vector2.SqrMagnitude(relativePosition) <= sqrRange && IsInAngle);
    }

    protected override IReadOnlyDictionary<string, string> GetStringsByKetword()
    {
        var dictionary = new Dictionary<string, string>()
        {
            { "range", range.ToString("0.##") }
        };

        return dictionary;
    }

    private void OnMouseLeftClick(Vector2 mousePosition)
    {
        ResetPlayerController();

        // 마우스 좌 클릭을 해야 기준점 검색이 끝난다. 
        // → TargetSelectionResult return 
        onSelectCompleted?.Invoke(SelectImmediateByPlayer(targetSearcher, requestEntity, requestObject, mousePosition));
    }

    private void OnMouseRightClick(Vector2 mousePosition)
    {
        ResetPlayerController();

        // 결과 값으로 검색 실패를 전달 
        onSelectCompleted?.Invoke(new TargetSelectionResult(Vector2.zero, SearchResultMessage.Fail));
    }
}
