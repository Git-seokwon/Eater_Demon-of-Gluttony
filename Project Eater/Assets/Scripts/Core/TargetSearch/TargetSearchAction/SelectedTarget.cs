using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectedTarget : TargetSearchAction
{
    // Search 대상이 Target이기 때문에 Range 및 Angle이 필요 없다. 
    public override float Range => 0f;
    public override float ScaledRange => 0f;
    public override float Angle => 0f;

    #region 생성자
    public SelectedTarget() { }
    public SelectedTarget(SelectedTarget cop) : base(cop) { }
    #endregion

    public override object Clone() => new SelectedTarget(this);

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, 
        TargetSelectionResult selectResult)
    {
        // 기준점이 GameObject이면 TargetSearchResult에 selectedTarget을 넣어서 반환하고 
        // position이라면 TargetSearchResult에 selectedPosition을 넣어서 반환
        return selectResult.selectedTarget ? new TargetSearchResult(new GameObject[] { selectResult.selectedTarget })
            : new TargetSearchResult(new Vector2[] { selectResult.selectedPosition });
    }
}
