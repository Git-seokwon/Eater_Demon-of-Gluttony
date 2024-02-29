using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : Skill
{
    protected float coolDown; // 나중에 액티브 스킬 스크립터블 오브젝트에서 가져온다.
    protected float coolDownTimer;

    // TODO : 액티브 스킬에서 중복되는 변수들은 빼놓는다. 
    //      : 스크립터블 오브젝트 변수도 빼놓는다.

    protected override void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }

    public override void SkillFunction()
    {
        // do some skill spesific things
    }

    public virtual bool CanUseSkill()
    {
        if (coolDownTimer <= 0)
        {
            coolDownTimer = coolDown;
            SkillFunction();
            return true;
        }

        return false;
    }

    // TODO : 엑티브 스킬에서 많이 사용하는 기능(메서드) 빼두기
}
