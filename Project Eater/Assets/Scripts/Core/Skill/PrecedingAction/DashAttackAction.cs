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
    // �̴� ��ġ�� ĳ���� ���� �������� ��´�. 
    // �� ĳ���� �뽬 ��ġ���� �������� �Ͽ� �ݶ��̴��� ������ ��, �ٷ� �������� ���� �ʵ��� �Ѵ�. 
    private Vector3 pullPosition;

    private bool isReachPlayer;
    private bool isReachEnemy;

    public override void Start(Skill skill)
    {
        // �ݶ��̴��� ���� ���� �Ƹ� ���·� ���� 
        skill.Owner.Collider.enabled = false;
        // �뽬 �������� ���� 
        dashPosition = skill.Owner.transform.position + new Vector3(dashDistance * skill.Owner.EntitytSight, 0, 0);
        // �� ���� ��ġ ���� 
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
        // �ݶ��̴��� �Ѽ� ���� �Ƹ� ���� ���� 
        skill.Owner.Collider.enabled = true;
        // �뽬 �������� ĳ���� �̵� 
        skill.Owner.rigidbody.MovePosition(dashPosition);
        // Pull ��ġ�� ���� �̵� 
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
