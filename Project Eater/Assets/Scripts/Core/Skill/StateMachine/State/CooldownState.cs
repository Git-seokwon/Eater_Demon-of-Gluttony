using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cooldown�� ���̵Ǵ� State
public class CooldownState : State<Skill>
{
    public override void Enter()
    {
        // �� CoolDown�� ��, ��ų ��Ȱ��ȭ 
        // CooldownState�� ���� Layer�� 0���̰�, Activate ���¶�� Deactivate �Լ��� ���� 
        // �� 0�� Layer�� Mian Layer�� 0�� Layer�� CurrentState�� CooldownState�� �Ǹ�, Skill�� Deactiverk ����ȴٴ� �� 
        // �� SkillState�� ���(Instant, Toggle ...), Skill ���� ���õ� State�� ��� 0�� Layer���� ���� 
        //    (ToggleSkillState�� ��쿡�� 1�� Layer�� ����Ѵ�)
        if (Layer == 0 && Entity.IsActivated)
            Entity.Deactivate();

        // �� CoolDown ���� 
        // ������ Skill�� Cooldown�� 0�ʶ��, ���� Cooldown�� SKill�� �⺻ Cooldown ������ ���� 
        if (Entity.IsCooldownCompleted)
            Entity.CurrentCooldown = Entity.Cooldown;
    }

    // �� Cooldown
    // �� ���� ���ǿ� ���ؼ� CurrentCooldown�� 0�ʰ� �Ǹ�, ReadyStaet�� ���� �ȴ�. 
    public override void Update() => Entity.CurrentCooldown -= Time.deltaTime;
}
