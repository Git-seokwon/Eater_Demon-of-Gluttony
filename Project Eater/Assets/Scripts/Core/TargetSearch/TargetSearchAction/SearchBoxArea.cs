using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SearchBoxArea : TargetSearchAction
{
    [Header("Data")]
    [Min(0f)]
    [SerializeField]
    private float width;

    [Min(0f)]
    [SerializeField]
    private float height;

    // �˻��� ��û�� Entity�� �˻� ��� ������ ���ΰ�? 
    [SerializeField]
    private bool isIncludeSelf;

    // Target�� �˻��� ��û�� Entity�� ���� Category�� ������ �־�� �ϴ°�? 
    [SerializeField]
    private bool isSearchSameCategory;

    public float Width => width;
    public float Height => height;
    public float ScaledWidth => width * Scale;
    public float ScaledHeight => height * Scale;
    public float ProperWidth => isIncludeSelf ? ScaledWidth : Width;
    public float ProperHeight => isIncludeSelf ? ScaledHeight : Height;

    #region ������
    public SearchBoxArea() { }
    public SearchBoxArea(SearchBoxArea copy)
        : base(copy)
    {
        width = copy.width;
        height = copy.height;
        isIncludeSelf = copy.isIncludeSelf;
        isSearchSameCategory = copy.isSearchSameCategory;
    }
    #endregion

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, 
        TargetSelectionResult selectResult)
    {
        var targets = new List<GameObject>();

        var boxPositionA = (selectResult.resultMessage == SearchResultMessage.FindTarget)
                          ? (Vector2)selectResult.selectedTarget.transform.position
                          : selectResult.selectedPosition;

        var boxPositionB = new Vector2(boxPositionA.x + ProperWidth * requestEntity.EntitytSight, 
                                       boxPositionA.y + ProperHeight);
             
        var colliders = Physics2D.OverlapAreaAll(boxPositionA, boxPositionB);

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
            
            targets.Add(entity.gameObject);
        }
        return new TargetSearchResult(targets.ToArray());
    }

    // ex) 0.targetSearcher.searchAction.width
    protected override IReadOnlyDictionary<string, string> GetStringsByKeyword()
    {
        var dictionary = new Dictionary<string, string>()
        { 
            { "width", width.ToString("0.##") },
            { "height", height.ToString("0.##") }
        };
        return dictionary;
    }

    public override object Clone() => new SearchBoxArea(this);
}
