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
    protected float enemyHPValue;
    protected Stat enemyHP;
    protected Entity entity;

    protected virtual void Awake()
    { 
        entity = GetComponent<Entity>();
        // 체력 Stat 가져오기 
        enemyHP = entity.Stats.FullnessStat;
        // 초기 체력 가져오기 
        enemyHPValue = entity.Stats.FullnessStat.MaxValue;

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

    // 스테이지 매니저에서 몬스터를 스폰할 때, 해당 함수 호출
    public virtual void SetEnemy()
    {
        // 체력 복구 
        GetComponent<Entity>().Stats.SetDefaultValue(enemyHP, enemyHPValue);

        // 스킬 장착 
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }
    }
}
