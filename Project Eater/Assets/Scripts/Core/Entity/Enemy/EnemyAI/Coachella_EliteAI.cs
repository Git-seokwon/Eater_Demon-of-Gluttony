using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coachella_EliteAI : MonoBehaviour
{
    [SerializeField]
    private Entity target;
    [SerializeField]
    private Skill[] skill = new Skill[2];
    [SerializeField]
    private float PlayerDistanceToUseSkill;
    [SerializeField]
    private float checkInterval = 0.1f;
    [SerializeField]
    private float hpUnderLine;

    private Skill[] eqippedSkill = new Skill[2];
    private WaitForSeconds waitForSeconds;
    private bool isFirstSpawn = true;
    private Coroutine playerDistanceCheckCoroutine;
    private float enemyHPValue;
    private Stat enemyHP;
    private bool isSuicideSkillUsed = false;

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
        var clone_01 = entity.SkillSystem.Register(skill[0]);
        eqippedSkill[0] = entity.SkillSystem.Equip(clone_01);
        var clone_02 = entity.SkillSystem.Register(skill[1]);
        eqippedSkill[1] = entity.SkillSystem.Equip(clone_02);

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
            isSuicideSkillUsed = false;
        }
    }

    private void Update()
    {
        // eqippedSkill[1]�� ���� ��ų�̶� ��ų ��� ���� Enemy�� �ǰ� 0���� �Ǿ� ��ų ��� ���� SetActive(false) ó���� �̷������. 
        if (enemyHP.Value <= hpUnderLine && eqippedSkill[1].IsUseable && !isSuicideSkillUsed)
        {
            eqippedSkill[1].Use();
            isSuicideSkillUsed = true;
        }
    }

    // ���� �ð� �������� Ÿ�ٰ��� �Ÿ� üũ
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if (eqippedSkill[0].IsUseable
            && (target.transform.position - transform.position).sqrMagnitude < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
                eqippedSkill[0].Use();

            // ������ �ð� ��ŭ ���
            yield return waitForSeconds;
        }
    }
}
