using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachellaAI : MonoBehaviour
{
    [SerializeField]
    private Entity target;
    [SerializeField]
    private Skill skill;
    [SerializeField]
    private float PlayerDistanceToUseSkill;
    [SerializeField]
    private float checkInterval = 0.1f;

    private Skill eqippedSkill;
    private WaitForSeconds waitForSeconds;
    private bool isFirstSpawn = true;
    private Coroutine playerDistanceCheckCoroutine;
    private float enemyHPValue;
    private Stat enemyHP;

    private void Start()
    {
        // Target(Player) ���� 
        var entity = GetComponent<Entity>();
        entity.Target = target;
        // ü�� Stat �������� 
        enemyHP = entity.Stats.FullnessStat;
        // �ʱ� ü�� �������� 
        enemyHPValue = entity.Stats.FullnessStat.MaxValue;

        // ��ų ���� 
        var clone = entity.SkillSystem.Register(skill);
        eqippedSkill = entity.SkillSystem.Equip(clone);

        waitForSeconds = new WaitForSeconds(checkInterval);

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += (Entity x) => StopCoroutine(CheckPlayerDistance());

        if (isFirstSpawn)
        {
            playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());
            isFirstSpawn = false;
        }
    }

    private void OnEnable()
    {
        if (!isFirstSpawn && playerDistanceCheckCoroutine == null)
        {
            // ü�� ���� 
            GetComponent<Entity>().Stats.SetDefaultValue(enemyHP, enemyHPValue);
            // ��ų AI ���� 
            playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());
        }
    }

    // ���� �ð� �������� Ÿ�ٰ��� �Ÿ� üũ
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if (eqippedSkill.IsUseable
            && (target.transform.position - transform.position).sqrMagnitude < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
                eqippedSkill.Use();

            // ������ �ð� ��ŭ ���
            yield return waitForSeconds;
        }
    }
}
