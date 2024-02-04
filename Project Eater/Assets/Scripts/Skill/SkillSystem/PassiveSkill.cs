using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : Skill
{
    // TODO : 패시브 스킬에서 중복되는 변수들은 빼놓는다. 
    //      : 스크립터블 오브젝트 변수도 빼놓는다.

    protected override void Update()
    {
        
    }

    // 패시브 스킬의 경우, SkillFunction 기능이 Update에서 계속 돌아가거나
    // Player에서 SkillFunction 기능을 한 번 호출하는 식으로 적용
    public override void SkillFunction()
    {
        
    }

    // TODO : 패시브 스킬에서 많이 사용하는 기능(메서드) 빼두기
}
