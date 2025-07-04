using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectPosition : SelectTarget
{
    // Layer : Room 으로 하기 
    [Header("Layer")]
    [SerializeField]
    private LayerMask layerMask;

    #region 생성자
    public SelectPosition() { }

    public SelectPosition(SelectPosition copy) 
        : base(copy)
    { 
        layerMask = copy.layerMask;
    }
    #endregion

    protected override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requestEntity, 
        GameObject requsetObject, Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            if (IsInRange(targetSearcher, requestEntity, requsetObject, hit.point))
                return new TargetSelectionResult(hit.point, SearchResultMessage.FindPosition);
            else
                return new TargetSelectionResult(hit.point, SearchResultMessage.OutOfRange);
        }
        else
            return new TargetSelectionResult(requsetObject.transform.position, SearchResultMessage.Fail);
    }

    protected override TargetSelectionResult SelectImmediateByEnemy(TargetSearcher targetSearcher, Entity requestEntity, 
        GameObject requestObject, Vector2 position)
    {
        var target = requestEntity.Target;

        if (target == null)
            return new TargetSelectionResult(position, SearchResultMessage.Fail);
        else if (targetSearcher.IsInRange(requestEntity, requestObject, position))
            return new TargetSelectionResult(position, SearchResultMessage.FindPosition);
        else
            return new TargetSelectionResult(position, SearchResultMessage.OutOfRange);
    }

    public override object Clone() => new SelectPosition(this);
}
