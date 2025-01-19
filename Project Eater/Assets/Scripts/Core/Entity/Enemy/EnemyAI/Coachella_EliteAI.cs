using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Coachella_EliteAI : MonsterAI
{
    [SerializeField]
    protected Skill extraSkill; // 자폭 스킬 
    [SerializeField]
    private float hpUnderLine;

    protected Skill extraEqippedSkill;
    private bool isSuicideSkillUsed = false;

    protected override void Awake()
    {
        base.Awake();

        // Target 설정 
        entity.Target = GameManager.Instance.player;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (extraEqippedSkill != null)
        {
            entity.SkillSystem.Disarm(extraEqippedSkill);
            entity.SkillSystem.Unregister(extraEqippedSkill);
            extraEqippedSkill = null;
        }
    }

    public override void SetEnemy()
    {
        base.SetEnemy();

        // 추가 스킬 장착 
        if (extraSkill != null)
        {
            var clone = entity.SkillSystem.Register(extraSkill);
            extraEqippedSkill = entity.SkillSystem.Equip(clone);
        }

        // 스킬 AI 시작 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // 몬스터 사망시 코루틴 종료 
        entity.onDead += OnDead;

        isSuicideSkillUsed = false;
    }

    private void Update()
    {
        // extraEqippedSkill는 자폭 스킬이라 스킬 사용 이후
        // Enemy의 피가 0으로 되어 스킬 사용 이후 SetActive(false) 처리가 이루어진다. 
        if (enemyHP.Value <= hpUnderLine && !isSuicideSkillUsed)
        {
            extraEqippedSkill.Use();
            isSuicideSkillUsed = true;
        }
    }

    // 일정 시간 간격으로 타겟과의 거리 체크
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if ((GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                eqippedSkill.Use();
            }

            // 지정된 시간 만큼 대기
            yield return waitForSeconds;
        }
    }

    private void OnDead(Entity entity)
    {
        if (playerDistanceCheckCoroutine != null)
            StopCoroutine(CheckPlayerDistance());

        playerDistanceCheckCoroutine = null;
    }
}
