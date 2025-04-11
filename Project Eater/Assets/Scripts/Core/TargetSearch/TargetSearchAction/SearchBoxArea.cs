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

    // 검색을 요청한 Entity도 검색 대상에 포함할 것인가? 
    [SerializeField]
    private bool isIncludeSelf;

    // Target이 검색을 요청한 Entity와 같은 Category를 가지고 있어야 하는가? 
    [SerializeField]
    private bool isSearchSameCategory;

    public float Width => width;
    public float Height => height;
    public float ScaledWidth => width * Scale;
    public float ScaledHeight => height * Scale;
    public float ProperWidth => isIncludeSelf ? ScaledWidth : Width;
    public float ProperHeight => isIncludeSelf ? ScaledHeight : Height;

    #region 생성자
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

            // Entity가 null이거나, 이미 죽은 상태거나, 검색을 명령한 Entity인데 isIncludeSelf가 true가 아닐 경우 넘어감
            if (entity == null || entity.IsDead || (entity == requestEntity && !isIncludeSelf))
                continue;

            if (entity != requestEntity)
            {
                // Requester와 Entity가 공유하는 Category가 있는지 확인
                var hasCategory = requestEntity.Categories.Any(x => entity.HasCategory(x));

                // 공유하는 Category가 있지만 isSearchSameCategory가 false거나,
                // 공유하는 Category가 없지만 isSearchSameCategory가 true라면 넘어감
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
