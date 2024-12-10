using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ※ SearchArea : 기준점에서 범위 안에 있는 Entity들 중 조건이 맞는 Entity들을 Target으로 return 
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

            // requesterPosition → entityPosition의 거리와 방향 구하기
            var direction = (Vector2)entity.transform.position - requestPosition;

            // ※ requestObject와 requestEntity가 다른 경우는 원형 공격이고, 같은 경우에만 원뿔 형 공격이 존재하기 때문에 
            //    requestObject.transform.right * requestEntity.EnitytSight 이렇게 적어둔 것 
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
