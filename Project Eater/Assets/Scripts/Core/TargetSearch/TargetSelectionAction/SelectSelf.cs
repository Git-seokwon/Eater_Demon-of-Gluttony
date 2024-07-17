using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectSelf : TargetSelectionAction
{
    // 자기 자신을 반환하기 때문에 Range ~ Angle 정보는 필요없다. 
    public override float Range => 0f;
    public override float ScaledRange => 0f;
    public override float Angle => 0f;

    #region 생성자
    public SelectSelf() { }
    public SelectSelf(SelectSelf copy) : base(copy) { }
    #endregion

    // 바로 기준점(requsetObject(이 경우에는 자기 자신))을 return 한다. 
    protected override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requestEntity,
        GameObject requsetObject, Vector2 position)
    => new TargetSelectionResult(requsetObject, SearchResultMessage.FindTarget);

    // Enemy의 경우, SelectImmediateByPlayer을 사용하여 기준점을 return 한다. 
    protected override TargetSelectionResult SelectImmediateByEnemy(TargetSearcher targetSearcher, Entity requestEntity, 
        GameObject requestObject, Vector2 position)
    => SelectImmediateByPlayer(targetSearcher, requestEntity, requestObject, position);

    // SelectSelf의 경우, 기다릴 것 없이 바로 onSelectCompleted에 SelectImmediateByPlayer 결과 값을 전달하여 검색을 끝낸다.
    public override void Select(TargetSearcher targetsearcher, Entity requestEntity, GameObject requestObject,
        SelectCompletedHandler onSelectCompleted)
    => onSelectCompleted?.Invoke(SelectImmediateByPlayer(targetsearcher, requestEntity, requestObject, Vector2.zero));

    // Select 함수가 즉시 완료되므로 취소할 수 있는 상황이 없기 때문에 내용을 비워 둔다. 
    public override void CancleSelect(TargetSearcher targetSearcher) { }

    // 자기 자신을 찾는 것이기 때문에 무조건 true를 반환
    public override bool IsInRange(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, Vector2 targetPosition)
    => true;

    public override object Clone()
    => new SelectSelf(this);
}
