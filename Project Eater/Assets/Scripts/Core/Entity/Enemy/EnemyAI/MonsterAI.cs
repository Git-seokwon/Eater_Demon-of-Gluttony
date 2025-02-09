using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class MonsterAI : MonoBehaviour
{
    [SerializeField]
    protected Skill skill;
    [SerializeField]
    protected float PlayerDistanceToUseSkill;
    [SerializeField]
    protected float checkInterval = 0.1f;

    protected Skill eqippedSkill;
    protected WaitForSeconds waitForSeconds;
    protected Coroutine playerDistanceCheckCoroutine;
    protected Entity entity;

    protected virtual void Awake()
    { 
        entity = GetComponent<Entity>();

        waitForSeconds = new WaitForSeconds(checkInterval);
    }

    protected virtual void OnDisable()
    {
        if (eqippedSkill != null)
        {
            entity.SkillSystem.Disarm(eqippedSkill);
            entity.SkillSystem.Unregister(eqippedSkill);
            eqippedSkill = null;
        }
    }

    // 몬스터의 스탯을 보정하고 적용하는 함수
    protected void ApplyStatsCorrection(float hp, float attack, float defence)
    {
        entity.Stats.FullnessStat.MaxValue = hp;
        entity.Stats.AttackStat.MaxValue = attack;
        entity.Stats.DefenceStat.MaxValue = defence;

        entity.Stats.SetDefaultValue(entity.Stats.FullnessStat, hp);
        entity.Stats.SetDefaultValue(entity.Stats.AttackStat, attack);
        entity.Stats.SetDefaultValue(entity.Stats.DefenceStat, defence);
    }

    // 스테이지 매니저에서 몬스터를 스폰할 때, 해당 함수 호출
    public virtual void SetEnemy(int wave, int stage)
    {
        // 스킬 장착 
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }
    }
}
