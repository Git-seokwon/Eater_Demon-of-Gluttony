using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public class DashAttackAction : SkillPrecedingAction
{
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashDistance;

    [SerializeField]
    private float pullSpeed;
    [SerializeField]
    private float pullDistance;

    private Vector3 dashPosition;
    // 미는 위치도 캐릭터 기준 전방으로 잡는다. 
    // → 캐릭터 대쉬 위치보다 전방으로 하여 콜라이더가 켜졌을 때, 바로 데미지를 입지 않도록 한다. 
    private Vector3 pullPosition;

    private bool isReachPlayer;
    private bool isReachEnemy;

    public override void Start(Skill skill)
    {
        // 콜라이더를 꺼서 슈퍼 아머 상태로 만듬 
        skill.Owner.Collider.enabled = false;
        // 대쉬 목적지를 정함 
        dashPosition = skill.Owner.transform.position + new Vector3(dashDistance * skill.Owner.EntitytSight, 0, 0);
        // 적 최종 위치 설정 
        pullPosition = skill.Owner.transform.position + new Vector3(pullDistance * skill.Owner.EntitytSight, 0f, 0f);
    }

    public override bool Run(Skill skill)
    {
        isReachPlayer = MovePlayer(skill.Owner);
        isReachEnemy = MoveEnemy(skill.Targets);

        if (isReachPlayer && isReachEnemy)
            return true;
        else
            return false;
    }

    public override void Release(Skill skill)
    {
        // 콜라이더를 켜서 슈퍼 아머 상태 해제 
        skill.Owner.Collider.enabled = true;
        // 대쉬 목적지로 캐릭터 이동 
        skill.Owner.rigidbody.MovePosition(dashPosition);
        // Pull 위치로 적들 이동 
        if (skill.Targets != null)
        {
            foreach (var target in skill.Targets)
                target.rigidbody.MovePosition(pullPosition);
        }
    }

    private bool MovePlayer(Entity owner)
    {
        return MoveEntity(owner, dashPosition, dashSpeed);
    }

    private bool MoveEnemy(IReadOnlyList<Entity> targets)
    {
        if (targets == null)
            return true;

        bool isReachPosition = true;
        foreach (Entity target in targets)
        {
            if (!MoveEntity(target, pullPosition, pullSpeed))
                isReachPosition = false;
        }
        return isReachPosition;
    }

    private bool MoveEntity(Entity entity, Vector3 targetPosition, float speed)
    {
        if ((entity.rigidbody.position - (Vector2)targetPosition).sqrMagnitude <= 0.04f)
            return true;
        else
        {
            Vector3 unitVector = Vector3.Normalize(targetPosition - entity.transform.position);
            entity.rigidbody.MovePosition(entity.transform.position + (unitVector * speed * Time.fixedDeltaTime));
            return false;
        }
    }

    public override object Clone()
    {
        return new DashAttackAction()
        {
            dashSpeed = dashSpeed,
            pullSpeed = pullSpeed,
            dashDistance = dashDistance,
            pullDistance = pullDistance
        };
    }
}
