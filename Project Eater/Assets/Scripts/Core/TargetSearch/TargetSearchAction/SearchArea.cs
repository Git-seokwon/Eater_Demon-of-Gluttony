using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// �� SearchArea : ���������� ���� �ȿ� �ִ� Entity�� �� ������ �´� Entity���� Target���� return 
[System.Serializable]
public class SearchArea : TargetSearchAction
{
    [Header("Data")]
    [Min(0f)]
    [SerializeField]
    private float range;

    [Range(0f, 360f)]
    [SerializeField]
    private float angle = 360f;

    [SerializeField]
    private bool isOffsetCenter;

    // �˻��� ��û�� Entity�� �˻� ��� ������ ���ΰ�? 
    [SerializeField]
    private bool isIncludeSelf;

    // Target�� �˻��� ��û�� Entity�� ���� Category�� ������ �־�� �ϴ°�? 
    [SerializeField]
    private bool isSearchSameCategory;

    public override float Range => range;
    public override float ScaledRange => range * Scale;
    public override float Angle => angle;

    #region ������ 
    public SearchArea() { }
    public SearchArea(SearchArea copy)
        : base(copy) 
    { 
        range = copy.range;
        isIncludeSelf = copy.isIncludeSelf;
        isSearchSameCategory = copy.isSearchSameCategory;
    }
    #endregion

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, 
        TargetSelectionResult selectResult)
    {
        var targets = new List<GameObject>();

        var rendererSize = isOffsetCenter ? requestEntity.GetComponent<SpriteRenderer>().bounds.size.y : 0f;

        var spherePosition = (selectResult.resultMessage == SearchResultMessage.FindTarget)
            ? (Vector2)selectResult.selectedTarget.transform.position + Vector2.up * rendererSize * 0.5f
            : selectResult.selectedPosition;

        var colliders = Physics2D.OverlapCircleAll(spherePosition, ProperRange);

        Vector2 requestPosition = requestObject.transform.position;

        foreach (var collider in colliders)
        {
            var entity = collider.GetComponent<Entity>();

            // Entity�� null�̰ų�, �̹� ���� ���°ų�, �˻��� ����� Entity�ε� isIncludeSelf�� true�� �ƴ� ��� �Ѿ
            if (entity == null || entity.IsDead || (entity == requestEntity && !isIncludeSelf))
                continue;

            if (entity != requestEntity)
            {
                // Requester�� Entity�� �����ϴ� Category�� �ִ��� Ȯ��
                var hasCategory = requestEntity.Categories.Any(x => entity.HasCategory(x));

                // �����ϴ� Category�� ������ isSearchSameCategory�� false�ų�,
                // �����ϴ� Category�� ������ isSearchSameCategory�� true��� �Ѿ
                if ((hasCategory && !isSearchSameCategory) || (!hasCategory && isSearchSameCategory))
                    continue;
            }

            // requesterPosition �� entityPosition�� �Ÿ��� ���� ���ϱ�
            var direction = (Vector2)entity.transform.position - requestPosition;

            // �� requestObject�� requestEntity�� �ٸ� ���� ���� �����̰�, ���� ��쿡�� ���� �� ������ �����ϱ� ������ 
            //    requestObject.transform.right * requestEntity.EnitytSight �̷��� ����� �� 
            if (Vector2.Angle(requestObject.transform.right * requestEntity.EntitytSight, direction) < (angle * 0.5f))
                targets.Add(entity.gameObject);
        }
        return new TargetSearchResult(targets.ToArray());
    }

    protected override IReadOnlyDictionary<string, string> GetStringsByKeyword()
    {
        var dictionary = new Dictionary<string, string>() { { "range", range.ToString("0.##") } };
        return dictionary;
    }

    public override object Clone() => new SearchArea(this);
}
