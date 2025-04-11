using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SearchRandomPoint : TargetSearchAction
{
    [Header("Data")]
    [Min(0f)]
    [SerializeField]
    private float range;

    [Range(0f, 360f)]
    [SerializeField]
    private float angle = 360f;

    // 랜덤 포인트 개수 
    [Min(0f)]
    [SerializeField]
    private int count;

    // 검색을 요청한 Entity도 검색 대상에 포함할 것인가? 
    [SerializeField]
    private bool isIncludeSelf;

    // Target이 검색을 요청한 Entity와 같은 Category를 가지고 있어야 하는가? 
    [SerializeField]
    private bool isSearchSameCategory;

    public override float Range => range;
    public override float ScaledRange => range * Scale;
    public override float Angle => angle;

    #region 생성자
    public SearchRandomPoint() { }

    public SearchRandomPoint(SearchRandomPoint copy)
        : base(copy)
    {
        range = copy.range;
        count = copy.count;
        isIncludeSelf = copy.isIncludeSelf;
        isSearchSameCategory = copy.isSearchSameCategory;
    }
    #endregion

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, 
        TargetSelectionResult selectResult)
    {
        var points = new List<Vector2>();
        float epsilon = 0.0001f;

        var spherePosition = (selectResult.resultMessage == SearchResultMessage.FindTarget)
            ? (Vector2)selectResult.selectedTarget.transform.position + new Vector2(0, 1f)
            : selectResult.selectedPosition;

        var prevPoint = Vector2.zero;
        for (int i = 0; i < count; i++)
        {
            var randomPos = Random.insideUnitCircle * ProperRange + spherePosition;

            // 두 벡터가 동일함
            // → 중복이므로 넘기기 
            if ((randomPos - prevPoint).sqrMagnitude < epsilon * epsilon)
            {
                i++;
                continue;
            }
            else
            {
                points.Add(randomPos);
                prevPoint = randomPos;
            }
        }

        return new TargetSearchResult(points.ToArray());
    }

    // ex) 0.targetSearcher.searchAction.count
    protected override IReadOnlyDictionary<string, string> GetStringsByKeyword()
    {
        var dictionary = new Dictionary<string, string>()
        { 
            { "range", range.ToString("0.##") },
            { "count", count.ToString() },
        };
        return dictionary;
    }

    public override object Clone() => new SearchRandomPoint(this);
}
