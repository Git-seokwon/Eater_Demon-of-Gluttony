using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CC기를 맞으면 모든 행동과 조작이 멈췄다가 CC가 끝나면 다시 조작이 가능해진다. 
public abstract class PlayerCCState : State<PlayerEntity>
{
    // 현재 상태의 설명 or 이름 
    public abstract string Description { get; }
}
