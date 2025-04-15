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

    private Coroutine currentSetCoroutine;

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

        entity.Stats.SetDefaultValue(entity.Stats.FullnessStat, hp);
        entity.Stats.SetDefaultValue(entity.Stats.AttackStat, attack);
        entity.Stats.SetDefaultValue(entity.Stats.DefenceStat, defence);
        // 광폭화로 인해 증가된 이동속도 정상화 
        entity.Stats.SetDefaultValue(entity.Stats.MoveSpeedStat, (entity as EnemyEntity).defaultMoveSpeed);
    }

    public void SetEnemy(int wave, int stage)
    {
        if (currentSetCoroutine != null)
        {
            StopCoroutine(currentSetCoroutine);
        }
        currentSetCoroutine = StartCoroutine(SetEnemyCoroutine(wave, stage));
    }

    protected virtual IEnumerator SetEnemyCoroutine(int wave, int stage)
    {
        entity.Collider.enabled = false;
        entity.Animator.speed = 0f;
        var enemyEntity = entity as EnemyEntity;
        if (enemyEntity.IsSelfDestructive)
            enemyEntity.StateMachine.ExecuteCommand(EntityStateCommand.ToStunningState);
        enemyEntity.StopMovement();
        enemyEntity.isSpawning = false;

        // 페이드인 먼저 실행 (3초간)
        yield return StartCoroutine(FadeInSprite(enemyEntity.Sprite, 3f));

        if (enemyEntity.IsSelfDestructive)
            enemyEntity.StateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
        entity.Collider.enabled = true;
        entity.Animator.speed = 1f;
        enemyEntity.EnemyMovement.enabled = true;
        enemyEntity.isSpawning = true;

        // 페이드인이 끝난 후 스킬 장착
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }
    }

    private IEnumerator FadeInSprite(SpriteRenderer spriteRenderer, float duration)
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            spriteRenderer.color = color;
            yield return null;
        }

        // 보정: 최종 알파값 확실히 1로 설정
        color.a = 1f;
        spriteRenderer.color = color;
    }
}
