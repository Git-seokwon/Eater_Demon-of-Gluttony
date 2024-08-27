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

    #region ������
    public SelectTarget() { }
    public SelectTarget(SelectTarget copy)
        : base(copy) 
    { 
        range = copy.range;
        angle = copy.angle;
    }
    #endregion

    // �� SelectImmediateByPlayer
    // �� SelectTarget Class���� �����ϴ� ���� �ƴ϶� SelectTarget�� �ڽ� Class��
    // SelectSelfByOneClick, SelectPosition, SelectEntity Class���� �����Ѵ�. 

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
        // �� Target position : Player position
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

        // Vector3.SqrMagnitude�� ����ϱ� ������ range�� ������ ���� ����Ѵ�. 
        float sqrRange = ProperRange * ProperRange;
            // range * range * (IsUseScale ? Scale * Scale : 1f);

        // ��� ��ǥ ���ϱ� 
        Vector2 relativePosition = targetPosition - (Vector2)requestTransform.position;

        // ��� ��ǥ�� requestTransform�� ������(2D ���� X ��)���� ������ ���Ѵ�. 
        // �� ���߿� ���װ� �߻��ϸ� requestEntity�� requestObject�� ���� ��ü �϶��� �̶�� ������ �޾Ƽ� true�� requestEntity.EnitytSight��
        //    �ƴϸ� -1�� ���Ѵ�. 
        float angle = Vector2.Angle(relativePosition, (requestTransform.right * requestEntity.EnitytSight));

        // ���� ������ ���� ���� �ִ� �� Ȯ�� 
        bool IsInAngle = angle <= (Angle / 2f);

        // �˻� ������ ���� �̰ų� 
        // target�� Range�� Angle �ȿ� �ִٸ� true
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

        // ���콺 �� Ŭ���� �ؾ� ������ �˻��� ������. 
        // �� TargetSelectionResult return 
        onSelectCompleted?.Invoke(SelectImmediateByPlayer(targetSearcher, requestEntity, requestObject, mousePosition));
    }

    private void OnMouseRightClick(Vector2 mousePosition)
    {
        ResetPlayerController();

        // ��� ������ �˻� ���и� ���� 
        onSelectCompleted?.Invoke(new TargetSelectionResult(Vector2.zero, SearchResultMessage.Fail));
    }
}
