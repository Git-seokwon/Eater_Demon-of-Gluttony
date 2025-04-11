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

    // ���� ����Ʈ ���� 
    [Min(0f)]
    [SerializeField]
    private int count;

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

            // �� ���Ͱ� ������
            // �� �ߺ��̹Ƿ� �ѱ�� 
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
