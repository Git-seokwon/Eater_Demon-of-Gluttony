using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatentSkill_SpearOfGluttony : LatentSkill
{
    protected override void Start()
    {
        base.Start();
    }

    // �⺻ ��ų
    public override void StartSkillFunc()
    {
        base.StartSkillFunc();

        // 1. pool���� ����ü ������Ʈ �������� �� �ʱ� ���� 
        GameObject startSkill = PoolManager.Instance.ReuseGameObject(latentSkill.projectilePrefab, player.startSkillShootPosition, Quaternion.identity);

        // 2. ����ü ��ũ��Ʈ�� �����ͼ� �߻� �޼��� �����ϱ� 
        
    }

    public override void ApplyLatentSkillTrait()
    {
        base.ApplyLatentSkillTrait();
    }


    public override void UltimateSkillFunc()
    {
        base.UltimateSkillFunc();
    }
}
