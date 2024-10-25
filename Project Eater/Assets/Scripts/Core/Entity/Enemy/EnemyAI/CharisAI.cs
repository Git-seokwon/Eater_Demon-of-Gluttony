using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharisAI : MonoBehaviour
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
    private bool hasUsedSkill = false;

    private void Start()
    {
        // Target(Player) 설정 
        var entity = GetComponent<Entity>();
        entity.Target = target;
        // 체력 Stat 가져오기 
        enemyHP = entity.Stats.FullnessStat;
        // 초기 체력 가져오기 
        enemyHPValue = entity.Stats.FullnessStat.MaxValue;

        // 스킬 장착 
        var clone = entity.SkillSystem.Register(skill);
        eqippedSkill = entity.SkillSystem.Equip(clone);

        waitForSeconds = new WaitForSeconds(checkInterval);

        // 몬스터 사망시 코루틴 종료 
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
            // 체력 복구 
            GetComponent<Entity>().Stats.SetDefaultValue(enemyHP, enemyHPValue);
            // 스킬 AI 시작 
            playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());
            hasUsedSkill = false;
        }
    }

    // 일정 시간 간격으로 타겟과의 거리 체크
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if (!hasUsedSkill && eqippedSkill.IsUseable
            && (target.transform.position - transform.position).sqrMagnitude < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                eqippedSkill.Use();
                hasUsedSkill = true;
            }

            // 지정된 시간 만큼 대기
            yield return waitForSeconds;
        }
    }
}
