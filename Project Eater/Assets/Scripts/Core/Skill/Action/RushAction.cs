using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RushAction : SkillAction
{
    [SerializeField]
    private float rushSpeed;

    private Vector2 direction;

    public override void Start(Skill skill)
    {
        var boss = (skill.Owner as BossEntity);
        // �÷��̾� ���� ���� �� ����
        boss.BossMovement.enabled = false;
        // ���� ��ų ���� SetUp
        boss.SetUpRushAssault(skill);
        // ���� ���� Flag Ȱ��ȭ 
        boss.IsRushAssault = true;

        // ���� ���� 
        direction = (GameManager.Instance.player.transform.position - skill.Owner.transform.position).normalized;
    }

    public override void Apply(Skill skill)
    {
        // ����
        skill.Owner.rigidbody.velocity = direction * rushSpeed;
    }

    public override void Release(Skill skill)
    {
        var boss = (skill.Owner as BossEntity);

        // ����
        boss.rigidbody.velocity = Vector2.zero;
        // ���� ���� Flag ��Ȱ��ȭ 
        boss.IsRushAssault = false;
        // ���� ��ų ���� SetOff
        boss.SetOffRushAssault();
        // �÷��̾� ���� ���� 
        boss.BossMovement.enabled = true;
    }

    public override object Clone()
    {
        return new RushAction()
        {
            rushSpeed = rushSpeed
        };
    }
}
