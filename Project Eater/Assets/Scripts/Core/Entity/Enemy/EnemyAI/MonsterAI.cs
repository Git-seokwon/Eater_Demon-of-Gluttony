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
        // ü�� Stat �������� 
        enemyHP = entity.Stats.FullnessStat;
        // �ʱ� ü�� �������� 
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

    // �������� �Ŵ������� ���͸� ������ ��, �ش� �Լ� ȣ��
    public virtual void SetEnemy()
    {
        // ü�� ���� 
        GetComponent<Entity>().Stats.SetDefaultValue(enemyHP, enemyHPValue);

        // ��ų ���� 
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }
    }
}
