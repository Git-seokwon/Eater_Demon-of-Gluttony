using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossCCState : State<BossEntity>
{
    // ���� ������ ���� or �̸� 
    public abstract string Description { get; }

    // ���� ���¿��� ������ Animation�� Parameter
    protected abstract int AnimationHash { get; }
}
