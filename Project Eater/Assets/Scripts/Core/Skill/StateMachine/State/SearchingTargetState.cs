using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skill을 사용할 때 먼저 기준점 검색이 필요한 경우에 전이되는 State
// Ex) Skill Button을 누르면 어디에 Skill을 쓸지 기준점을 선택하고 Skill을 사용
public class SearchingTargetState : State<Skill>
{
    public override void Enter()
    {
        Entity.SelectTarget();
    }
    public override void Exit()
    {
        Entity.CancelSelectTarget();
    }
}
