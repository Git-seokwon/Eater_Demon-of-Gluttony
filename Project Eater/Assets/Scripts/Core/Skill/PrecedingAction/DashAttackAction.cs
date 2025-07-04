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
    private float maxDuration = 0.35f; // 최대 x초 후 강제 종료
    private float elapsedTime = 0f;

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
        elapsedTime = 0f;
        isReachPlayer = isReachEnemy = false;

        // 대쉬 목적지를 정함 
        dashPosition = skill.Owner.transform.position + new Vector3(dashDistance * skill.Owner.EntitytSight, 0, 0);
        // 적 최종 위치 설정 
        pullPosition = skill.Owner.transform.position + new Vector3(pullDistance * skill.Owner.EntitytSight, 0f, 0f);

        // 플레이어 & 몬스터들 정지 
        StopEntity(skill);
    }

    // 0.35초 이후 상태 전이 
    public override bool Run(Skill skill)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= maxDuration || (isReachPlayer && isReachEnemy))
            return true;
        return false;
    }

    public override void FixedRun(Skill skill)
    {
        isReachPlayer = MovePlayer(skill.Owner);
        isReachEnemy = MoveEnemy(skill.Targets);
    }

    public override void Release(Skill skill)
    {
        // 이동 직후 속도를 0으로 설정하여 이후 운동에 영향 주지 않기 
        MoveEntity(skill);
        // 콜라이더를 켜서 슈퍼 아머 상태 해제 
        skill.Owner.Collider.enabled = true;
    }

    private bool MovePlayer(Entity owner)
    {
        return MoveEntity(owner, dashPosition, dashSpeed);
    }

    private bool MoveEnemy(IReadOnlyList<Entity> targets)
    {
        if (targets == null || targets.Count == 0)
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

    private void StopEntity(Skill skill)
    {
        skill.Owner.rigidbody.velocity = Vector2.zero;
        foreach (var target in skill.Targets)
        {
            target.rigidbody.velocity = Vector2.zero;

            // EnemyMovement가 존재하면 비활성화
            if (target.TryGetComponent<EntityMovement>(out var movement))
            {
                movement.enabled = false;
            }
        }
    }

    private void MoveEntity(Skill skill)
    {
        skill.Owner.rigidbody.velocity = Vector2.zero;
        foreach (var target in skill.Targets)
        {
            target.rigidbody.velocity = Vector2.zero;

            // EnemyMovement가 존재하면 활성화
            if (target.TryGetComponent<EntityMovement>(out var movement))
            {
                movement.enabled = true;
            }
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
