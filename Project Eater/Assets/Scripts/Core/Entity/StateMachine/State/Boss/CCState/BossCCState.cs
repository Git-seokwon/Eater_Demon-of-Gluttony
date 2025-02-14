using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossCCState : State<BossEntity>
{
    // 현재 상태의 설명 or 이름 
    public abstract string Description { get; }

    // 현재 상태에서 실행할 Animation의 Parameter
    protected abstract int AnimationHash { get; }
}
