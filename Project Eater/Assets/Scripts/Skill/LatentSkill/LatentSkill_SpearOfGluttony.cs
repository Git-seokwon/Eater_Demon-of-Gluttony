using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatentSkill_SpearOfGluttony : LatentSkill
{
    protected override void Start()
    {
        base.Start();
    }

    // 기본 스킬
    public override void StartSkillFunc()
    {
        base.StartSkillFunc();

        // 1. pool에서 투사체 오브젝트 꺼내오기 및 초기 셋팅 
        GameObject startSkill = PoolManager.Instance.ReuseGameObject(latentSkill.projectilePrefab, player.startSkillShootPosition, Quaternion.identity);

        // 2. 투사체 스크립트를 가져와서 발사 메서드 실행하기 
        
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
