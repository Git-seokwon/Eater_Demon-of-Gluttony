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
        // Target(Player) 설정 
        var entity = GetComponent<Entity>();
        entity.Target = target;
        // 체력 Stat 가져오기 
        enemyHP = entity.Stats.FullnessStat;
        // 초기 체력 가져오기 
        enemyHPValue = entity.Stats.FullnessStat.MaxValue;

        // 스킬 장착 
        var clone_01 = entity.SkillSystem.Register(skill[0]);
        eqippedSkill[0] = entity.SkillSystem.Equip(clone_01);
        var clone_02 = entity.SkillSystem.Register(skill[1]);
        eqippedSkill[1] = entity.SkillSystem.Equip(clone_02);

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
            isSuicideSkillUsed = false;
        }
    }

    private void Update()
    {
        // eqippedSkill[1]은 자폭 스킬이라 스킬 사용 이후 Enemy의 피가 0으로 되어 스킬 사용 이후 SetActive(false) 처리가 이루어진다. 
        if (enemyHP.Value <= hpUnderLine && eqippedSkill[1].IsUseable && !isSuicideSkillUsed)
        {
            eqippedSkill[1].Use();
            isSuicideSkillUsed = true;
        }
    }

    // 일정 시간 간격으로 타겟과의 거리 체크
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if (eqippedSkill[0].IsUseable
            && (target.transform.position - transform.position).sqrMagnitude < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
                eqippedSkill[0].Use();

            // 지정된 시간 만큼 대기
            yield return waitForSeconds;
        }
    }
}
