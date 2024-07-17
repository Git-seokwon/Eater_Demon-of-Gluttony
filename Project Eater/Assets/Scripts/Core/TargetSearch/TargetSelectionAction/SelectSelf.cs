using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectSelf : TargetSelectionAction
{
    // �ڱ� �ڽ��� ��ȯ�ϱ� ������ Range ~ Angle ������ �ʿ����. 
    public override float Range => 0f;
    public override float ScaledRange => 0f;
    public override float Angle => 0f;

    #region ������
    public SelectSelf() { }
    public SelectSelf(SelectSelf copy) : base(copy) { }
    #endregion

    // �ٷ� ������(requsetObject(�� ��쿡�� �ڱ� �ڽ�))�� return �Ѵ�. 
    protected override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requestEntity,
        GameObject requsetObject, Vector2 position)
    => new TargetSelectionResult(requsetObject, SearchResultMessage.FindTarget);

    // Enemy�� ���, SelectImmediateByPlayer�� ����Ͽ� �������� return �Ѵ�. 
    protected override TargetSelectionResult SelectImmediateByEnemy(TargetSearcher targetSearcher, Entity requestEntity, 
        GameObject requestObject, Vector2 position)
    => SelectImmediateByPlayer(targetSearcher, requestEntity, requestObject, position);

    // SelectSelf�� ���, ��ٸ� �� ���� �ٷ� onSelectCompleted�� SelectImmediateByPlayer ��� ���� �����Ͽ� �˻��� ������.
    public override void Select(TargetSearcher targetsearcher, Entity requestEntity, GameObject requestObject,
        SelectCompletedHandler onSelectCompleted)
    => onSelectCompleted?.Invoke(SelectImmediateByPlayer(targetsearcher, requestEntity, requestObject, Vector2.zero));

    // Select �Լ��� ��� �Ϸ�ǹǷ� ����� �� �ִ� ��Ȳ�� ���� ������ ������ ��� �д�. 
    public override void CancleSelect(TargetSearcher targetSearcher) { }

    // �ڱ� �ڽ��� ã�� ���̱� ������ ������ true�� ��ȯ
    public override bool IsInRange(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, Vector2 targetPosition)
    => true;

    public override object Clone()
    => new SelectSelf(this);
}
