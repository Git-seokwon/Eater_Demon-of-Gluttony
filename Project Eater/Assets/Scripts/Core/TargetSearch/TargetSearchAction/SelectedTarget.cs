using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectedTarget : TargetSearchAction
{
    // Search ����� Target�̱� ������ Range �� Angle�� �ʿ� ����. 
    public override float Range => 0f;
    public override float ScaledRange => 0f;
    public override float Angle => 0f;

    #region ������
    public SelectedTarget() { }
    public SelectedTarget(SelectedTarget cop) : base(cop) { }
    #endregion

    public override object Clone() => new SelectedTarget(this);

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, 
        TargetSelectionResult selectResult)
    {
        // �������� GameObject�̸� TargetSearchResult�� selectedTarget�� �־ ��ȯ�ϰ� 
        // position�̶�� TargetSearchResult�� selectedPosition�� �־ ��ȯ
        return selectResult.selectedTarget ? new TargetSearchResult(new GameObject[] { selectResult.selectedTarget })
            : new TargetSearchResult(new Vector2[] { selectResult.selectedPosition });
    }
}
