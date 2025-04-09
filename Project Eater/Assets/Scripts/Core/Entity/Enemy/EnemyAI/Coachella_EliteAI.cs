using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Coachella_EliteAI : MonsterAI
{
    [SerializeField]
    protected Skill extraSkill; // ���� ��ų 

    protected Skill extraEqippedSkill;

    protected override void Awake()
    {
        base.Awake();

        // Target ���� : �÷��̾� 
        entity.Target = GameManager.Instance.player;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (extraEqippedSkill != null)
        {
            extraEqippedSkill.onDeactivated -= OnDeactivatedSkill;
            entity.onSelfDestruct -= OnSelfDestruct;
            entity.SkillSystem.Disarm(extraEqippedSkill);
            entity.SkillSystem.Unregister(extraEqippedSkill);
            extraEqippedSkill = null;
        }
    }

    public override void SetEnemy(int wave, int stage)
    {
        base.SetEnemy(wave, stage);

        // ���� ��ų ���� & event ���
        if (extraSkill != null)
        {
            var clone = entity.SkillSystem.Register(extraSkill);
            extraEqippedSkill = entity.SkillSystem.Equip(clone);
            extraEqippedSkill.onDeactivated += OnDeactivatedSkill;
            entity.onSelfDestruct += OnSelfDestruct;
        }

        // ��ų AI ���� 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;

        // ���� ���� ���� �� ���� 
        var enemy = entity as EnemyEntity;
        // ���� ���� ��ġ ��� 
        float hp = enemy.defaultHp + (0.45f * wave + 4.5f * (stage + 1));
        float attack = enemy.defaultAttack + (0.28f * wave + 2.8f * (stage + 1));
        float defence = enemy.defaultDefence + (0.15f * wave + 1.5f * (stage + 1));

        // ���� ����
        ApplyStatsCorrection(hp, attack, defence);
    }

    // ���� �ð� �������� Ÿ�ٰ��� �Ÿ� üũ
    private IEnumerator CheckPlayerDistance()
    {
        while (!entity.IsDead)
        {
            if ((GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                eqippedSkill.Use();
            }

            // ������ �ð� ��ŭ ���
            yield return waitForSeconds;
        }
    }

    private void OnDead(Entity entity, bool isRealDead)
    {
        if (playerDistanceCheckCoroutine != null)
            StopCoroutine(CheckPlayerDistance());

        playerDistanceCheckCoroutine = null;
    }

    private void OnSelfDestruct()
    {
        // �⺻ ���� ���� ���� �ֱ� ������ ��ų ����� ����Ѵ�. 
        entity.SkillSystem.Cancel(eqippedSkill);

        // ��ÿ�� ���� ȿ���� ���
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.coachellaSuicide);


        extraEqippedSkill.Use();
    }

    private void OnDeactivatedSkill(Skill skill)
    {
        (entity as EnemyEntity).OnDead();        
    }
}