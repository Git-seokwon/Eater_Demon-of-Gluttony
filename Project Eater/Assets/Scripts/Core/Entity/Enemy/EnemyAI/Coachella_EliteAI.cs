using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Coachella_EliteAI : MonsterAI
{
    [SerializeField]
    protected Skill extraSkill; // ���� ��ų 
    [SerializeField]
    private float hpUnderLine;

    protected Skill extraEqippedSkill;
    private bool isSuicideSkillUsed = false;

    protected override void Awake()
    {
        base.Awake();

        // Target ���� 
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

        // �߰� ��ų ���� 
        if (extraSkill != null)
        {
            var clone = entity.SkillSystem.Register(extraSkill);
            extraEqippedSkill = entity.SkillSystem.Equip(clone);
        }

        // ��ų AI ���� 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;

        isSuicideSkillUsed = false;
    }

    private void Update()
    {
        // extraEqippedSkill�� ���� ��ų�̶� ��ų ��� ����
        // Enemy�� �ǰ� 0���� �Ǿ� ��ų ��� ���� SetActive(false) ó���� �̷������. 
        if (enemyHP.Value <= hpUnderLine && !isSuicideSkillUsed)
        {
            extraEqippedSkill.Use();
            isSuicideSkillUsed = true;
        }
    }

    // ���� �ð� �������� Ÿ�ٰ��� �Ÿ� üũ
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
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

    private void OnDead(Entity entity)
    {
        if (playerDistanceCheckCoroutine != null)
            StopCoroutine(CheckPlayerDistance());

        playerDistanceCheckCoroutine = null;
    }
}
