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
        // 플레이어 추적 중지 및 정지
        boss.BossMovement.enabled = false;
        // 돌진 스킬 변수 SetUp
        boss.SetUpRushAssault(skill);
        // 돌진 공격 Flag 활성화 
        boss.IsRushAssault = true;

        // 방향 설정 
        direction = (GameManager.Instance.player.transform.position - skill.Owner.transform.position).normalized;
    }

    public override void Apply(Skill skill)
    {
        // 돌진
        skill.Owner.rigidbody.velocity = direction * rushSpeed;
    }

    public override void Release(Skill skill)
    {
        var boss = (skill.Owner as BossEntity);

        // 정지
        boss.rigidbody.velocity = Vector2.zero;
        // 돌진 공격 Flag 비활성화 
        boss.IsRushAssault = false;
        // 돌진 스킬 변수 SetOff
        boss.SetOffRushAssault();
        // 플레이어 추적 시작 
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
