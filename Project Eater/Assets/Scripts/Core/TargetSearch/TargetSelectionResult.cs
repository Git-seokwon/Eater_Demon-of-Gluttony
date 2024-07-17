using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SearchResultMessage
{
    // �������� ã�� ����
    Fail,
    // �������� ���������� �˻� ���� �ۿ� ���� 
    OutOfRange,
    // ���� Target�� ã�� 
    FindTarget,
    // ���� Position�� ã�� 
    FindPosition
}

public readonly struct TargetSelectionResult
{
    // �˻��� ��ǥ ��� (ex. �� ĳ����, �� ĳ����, �Ʊ� ĳ���� ��)
    public readonly GameObject selectedTarget;
    // ��ǥ ����� ��ǥ Ȥ�� ������ ��ǥ 
    public readonly Vector2 selectedPosition;
    public readonly SearchResultMessage resultMessage;

    // �� ������ 1 : ���� ���(selectedTarget)�� ���ڷ� ����
    public TargetSelectionResult(GameObject selectedTarget, SearchResultMessage resultMessage)
        => (this.selectedTarget, selectedPosition, this.resultMessage) = (selectedTarget, selectedTarget.transform.position, resultMessage);

    // �� ������ 2 : ������ ��ǥ(selectedPosition)�� ���ڷ� ���� 
    public TargetSelectionResult(Vector2 selectedPosition, SearchResultMessage resultMessage)
        => (selectedTarget, this.selectedPosition, this.resultMessage) = (null, selectedPosition, resultMessage);
}
