using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SelectEntity : SelectTarget
{
    // 검색을 요청한 Entity도 검색 대상에 포함할 것인가?
    [SerializeField]
    private bool isIncludeSelf;

    // Target이 검색을 요청한 Entity와 같은 Category를 가지고 있어야 하는가? 
    // → 아군 선택, 적군 선택
    [SerializeField]
    private bool isSelectSameCategory;

    #region 생성자
    public SelectEntity() { }
    public SelectEntity(SelectEntity copy)
        : base(copy) 
    { 
        isIncludeSelf = copy.isIncludeSelf;
        isSelectSameCategory = copy.isSelectSameCategory;
    }
    #endregion

    protected override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requestEntity, 
        GameObject requsetObject, Vector2 position)
    {
        var collier2D = Physics2D.OverlapPoint(position);

        if (collier2D != null)
        {
            var entity = collier2D.GetComponent<Entity>();
            // Enitity가 null이거나, 이미 죽은 상태거나, 검색을 명령한 Entity인데 isIncludeSelf가 true가 아닐 경우 검색 실패
            if (entity == null || entity.IsDead || (entity == requestEntity && !isIncludeSelf))
                return new TargetSelectionResult(collier2D.transform.position, SearchResultMessage.Fail);

            if (entity != requestEntity)
            {
                // requestEntity와 Entity가 공유하는 Category가 있는지 확인 
                var hasCategory = requestEntity.Categories.Any(x => entity.HasCategory(x));
                // 공유하는 Category가 있지만(아군) isSelectSameCategory가 false(적군 대상)거나,
                // 공유하는 Category가 없지만(적군) isSelectSameCategory가 true(아군 대상)라면 검색 실패
                if ((hasCategory && !isSelectSameCategory) || (!hasCategory && isSelectSameCategory))
                    return new TargetSelectionResult(collier2D.transform.position, SearchResultMessage.Fail);
            }

            if (IsInRange(targetSearcher, requestEntity, requsetObject, position))
                return new TargetSelectionResult(entity.gameObject, SearchResultMessage.FindTarget);
            else
                return new TargetSelectionResult(entity.gameObject, SearchResultMessage.OutOfRange);
        }
        else
            return new TargetSelectionResult(requsetObject.transform.position, SearchResultMessage.Fail);
    }

    protected override TargetSelectionResult SelectImmediateByEnemy(TargetSearcher targetSearcher, Entity requestEntity, 
        GameObject requestObject, Vector2 position)
    {
        // Enemy의 경우, 무조건 검색 대상이 Entity의 Target이 된다. 
        var target = requestEntity.Target;

        if (!target)
            return new TargetSelectionResult(position, SearchResultMessage.Fail);
        else if (targetSearcher.IsInRange(requestEntity, requestObject, target.transform.position))
            return new TargetSelectionResult(target.gameObject, SearchResultMessage.FindTarget);
        else
            return new TargetSelectionResult(target.gameObject, SearchResultMessage.OutOfRange);
    }

    public override object Clone() => new SelectEntity(this);
}
