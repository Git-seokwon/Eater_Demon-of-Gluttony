using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct TargetSearchResult
{
    public readonly IReadOnlyList<GameObject> targets;
    public readonly IReadOnlyList<Vector2> positions;

    #region 생성자
    // 1) Target을 찾은 경우 
    // → positions 값으로 Target들의 위치 값이 들어간다.
    public TargetSearchResult(GameObject[] targets)
        => (this.targets, positions) = (targets, targets.Select(x => (Vector2)x.transform.position).ToArray());

    // 2) Position을 찾은 경우 
    // → target은 Empty가 된다. 
    public TargetSearchResult(Vector2[] positions)
        => (targets, this.positions) = (Array.Empty<GameObject>(), positions);
    #endregion
}
