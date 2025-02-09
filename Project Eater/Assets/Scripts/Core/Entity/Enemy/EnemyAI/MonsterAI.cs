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

    // ������ ������ �����ϰ� �����ϴ� �Լ�
    protected void ApplyStatsCorrection(float hp, float attack, float defence)
    {
        entity.Stats.FullnessStat.MaxValue = hp;
        entity.Stats.AttackStat.MaxValue = attack;
        entity.Stats.DefenceStat.MaxValue = defence;

        entity.Stats.SetDefaultValue(entity.Stats.FullnessStat, hp);
        entity.Stats.SetDefaultValue(entity.Stats.AttackStat, attack);
        entity.Stats.SetDefaultValue(entity.Stats.DefenceStat, defence);
    }

    // �������� �Ŵ������� ���͸� ������ ��, �ش� �Լ� ȣ��
    public virtual void SetEnemy(int wave, int stage)
    {
        // ��ų ���� 
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }
    }
}
