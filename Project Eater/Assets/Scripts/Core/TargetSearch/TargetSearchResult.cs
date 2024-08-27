using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct TargetSearchResult
{
    public readonly IReadOnlyList<GameObject> targets;
    public readonly IReadOnlyList<Vector2> positions;

    #region ������
    // 1) Target�� ã�� ��� 
    // �� positions ������ Target���� ��ġ ���� ����.
    public TargetSearchResult(GameObject[] targets)
        => (this.targets, positions) = (targets, targets.Select(x => (Vector2)x.transform.position).ToArray());

    // 2) Position�� ã�� ��� 
    // �� target�� Empty�� �ȴ�. 
    public TargetSearchResult(Vector2[] positions)
        => (targets, this.positions) = (Array.Empty<GameObject>(), positions);
    #endregion
}
