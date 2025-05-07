using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CounterAttackAction : SkillAction
{
    [SerializeField]
    private GameObject skillObjectPrefab;
    [SerializeField]
    private string spawnPointSocketName;
    [SerializeField]
    private float scaleUpSpeed;

    public override void Start(Skill skill)
    {
        if (skill.Owner is BossEntity boss)
        {
            boss.IsFlipped = false;
            boss.IsCounter = true;
            boss.Animator.speed = 0;
            boss.SetCounterAttackEvent();
            boss.StartCoroutine(boss.CancelCounterAttack(boss, skill));
        }
    }

    public override void Apply(Skill skill)
    {
        if (!(skill.Owner as BossEntity).IsCounterApply)
            return;

        // skillObjectPrefab을 Spawn할 위치를 가져온다. 
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        var skillObject = PoolManager.Instance.ReuseGameObject(skillObjectPrefab, socket.position, Quaternion.identity);

        skillObject.GetComponent<ScaledUpSkillObject>().Setup(skill.Owner, scaleUpSpeed, skill);
    }

    public override void Release(Skill skill)
    {
        if (skill.Owner is BossEntity boss)
        {
            boss.IsFlipped = true;
            boss.IsCounter = false;
            boss.Animator.speed = 1;
            boss.UnSetCounterAttackEvent();
            boss.IsCounterApply = false;
        }
    }

    public override object Clone()
    {
        return new CounterAttackAction()
        {
            skillObjectPrefab = skillObjectPrefab,
            spawnPointSocketName = spawnPointSocketName,
            scaleUpSpeed = scaleUpSpeed
        };
    }
}
