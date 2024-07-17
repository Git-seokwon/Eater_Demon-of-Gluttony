using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DistanceCollider
{
    public readonly float Distance;
    public readonly Collider2D collider;

    public DistanceCollider(float distance,  Collider2D collider)
        => (Distance, this.collider) = (distance, collider);
}
