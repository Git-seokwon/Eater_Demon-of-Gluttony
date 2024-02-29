using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : Skill
{
    protected float coolDown; // ���߿� ��Ƽ�� ��ų ��ũ���ͺ� ������Ʈ���� �����´�.
    protected float coolDownTimer;

    // TODO : ��Ƽ�� ��ų���� �ߺ��Ǵ� �������� �����´�. 
    //      : ��ũ���ͺ� ������Ʈ ������ �����´�.

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

    // TODO : ��Ƽ�� ��ų���� ���� ����ϴ� ���(�޼���) ���α�
}
