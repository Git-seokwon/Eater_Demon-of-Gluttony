using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SelectEntity : SelectTarget
{
    // �˻��� ��û�� Entity�� �˻� ��� ������ ���ΰ�?
    [SerializeField]
    private bool isIncludeSelf;

    // Target�� �˻��� ��û�� Entity�� ���� Category�� ������ �־�� �ϴ°�? 
    // �� �Ʊ� ����, ���� ����
    [SerializeField]
    private bool isSelectSameCategory;

    #region ������
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
            // Enitity�� null�̰ų�, �̹� ���� ���°ų�, �˻��� ����� Entity�ε� isIncludeSelf�� true�� �ƴ� ��� �˻� ����
            if (entity == null || entity.IsDead || (entity == requestEntity && !isIncludeSelf))
                return new TargetSelectionResult(collier2D.transform.position, SearchResultMessage.Fail);

            if (entity != requestEntity)
            {
                // requestEntity�� Entity�� �����ϴ� Category�� �ִ��� Ȯ�� 
                var hasCategory = requestEntity.Categories.Any(x => entity.HasCategory(x));
                // �����ϴ� Category�� ������(�Ʊ�) isSelectSameCategory�� false(���� ���)�ų�,
                // �����ϴ� Category�� ������(����) isSelectSameCategory�� true(�Ʊ� ���)��� �˻� ����
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
        // Enemy�� ���, ������ �˻� ����� Entity�� Target�� �ȴ�. 
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
