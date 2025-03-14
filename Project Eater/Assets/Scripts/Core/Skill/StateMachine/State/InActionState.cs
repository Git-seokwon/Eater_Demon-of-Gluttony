using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Apply Type�� Instant�� Skill�� Apply ��Ű�� State
// �� Apply Type�� Animation�̸�, InActionState���� Apply�� �Ͼ�� ���� �ƴ϶� Animation�� ������ Timing�� ����
//    Apply�� ���� 
public class InActionState : SkillState
{
    // Skill�� ExcutionType�� Auto������ ��Ÿ���� ���� 
    // �� �ش� ������ false�̸� ExcutionType�� Input�̶�� ��
    private bool isAutoExecutionType;

    // Skill�� �ߵ��� �ﰢ���� InstantType���� ��Ÿ���� ���� 
    // �� �ش� ������ false��� AnimationType�̰�, �̴� Animation���� �ߵ� Timing�� ���Ѵٴ� �ǹ�
    private bool isInstantApplyType;

    protected override void Setup() => UpdateProperty();

    public override void Enter()
    {
        UpdateProperty();

        // InPrecedingAction�� ���������� Casting�� ChargingState�� �� ��ġ�� ���� ���� �ֱ� ������ 
        // Activate ���°� �ƴ϶�� Activate ���ֱ� 
        if (!Entity.IsActivated)
            Entity.Activate();

        Entity.StartActoin();

        Apply();
    }

    // 1. Auto + Instant Type�� Skill�� �����Ű�� �Լ� - Update
    // �� Logic
    // 1. InActionState�� Update �Լ����� CurrentApplyCycle�� ������Ŵ
    // 2. CurrentApplyCycle�� ApplyCycle�� �����ϸ� Apply �Լ��� ���� 
    // 3. Apply �Լ��� ����Ǹ� CurrentApplyCycle�� �ʱ�ȭ �ȴ�. 
    // 4. 1 ~ 3�� ���� �ݺ� 
    public override void Update()
    {
        Entity.CurrentDuration += Time.deltaTime;
        Entity.CurrentApplyCycle += Time.deltaTime;

        if (isAutoExecutionType && Entity.IsApplicable)
        {
            if (!Entity.IsSearchingTarget && Entity.IsTargetSelectionTiming(TargetSelectionTimingOption.UseInAction))
                Entity.SelectTarget();

            Apply();
        }
    }

    public override void Exit()
    {
        Entity.CancelSelectTarget();
        Entity.ReleaseActoin();
    }

    // 2. Input + Instant Type�� Skill�� �����Ű�� �Լ� - OnReceiveMessage
    // �� Execute Type�� Input�� ���, Skill�� Use �Լ��� ���� Use Message�� �Ѿ���� Apply �Լ��� ȣ����
    //    ��, Skill�� �ߵ��� Update �Լ����� �ڵ�(Auto)���� �Ǵ� ���� �ƴ϶�, ������� �Է�(Input)�� ���� �� �Լ����� ��
    public override bool OnReceiveMessage(int message, object data)
    {
        // Input Type�� ��, Player�� Skill Button�� ������ Skill�� Use �Լ��� ����ǰ� Use Message�� �Ѿ���� ó�� 
        // �� Skill�� InActionState ������ �� Skill::Use �Լ��� �����ϸ� StateMachine.SendMessage(SkillStateMessage.Use)�� ����ǰ�
        //    
        var stateMessage = (SkillStateMessage)message;
        if (stateMessage != SkillStateMessage.Use || isAutoExecutionType)
            return false;

        if (Entity.IsApplicable)
        {
            if (Entity.IsTargetSelectionTiming(TargetSelectionTimingOption.UseInAction))
            {
                // Skill�� Searching���� �ƴ϶�� SelectTarget �Լ��� ������ �˻��� ����,
                // ������ �˻��� �����ϸ� OnTargetSelectionCompleted Callback �Լ��� ȣ��Ǿ� Apply �Լ��� ȣ����
                if (!Entity.IsSearchingTarget)
                    Entity.SelectTarget(OnTargetSelectionCompleted);
            }
            // SelectionTiming�� UseInAction, Both�� �ƴ϶��(Use ���) �ٷ� Apply �Լ��� ���� 
            else
                Apply();

            return true;
        }
        else
            return false;
    }

    private void Apply()
    {
        // Owner���� ToInSkillActionState�� Command ������ 
        TrySendCommandToOwner(Entity, EntityStateCommand.ToInSkillActionState, Entity.ActionAnimationParameter);

        // Skill�� Instant Type�̶�� Skill�� Apply �Լ��� �����Ͽ� Skill�� �ߵ��Ѵ�.
        // �� Skill�� ApplyType�� Animation�̶�� Animation���� Apply ��ȣ�� ���� ���̱� ������ ���� Apply �Լ��� �������� �ʴ´�. 
        if (isInstantApplyType)
            Entity.Apply();
        // Skill�� Input Type�̶�� CurrentApplyCount�� 1ȸ ������Ŵ �� ��� ������ Ƚ���� �� �� ���� 
        // �� �ش� else if���� ����Ƿ��� ApplyType�� Animation�̰�, ExecuteType�� Input�̾�� �Ѵ�. 
        // �� User�� Skill Button�� ������, Animation�� ���� �ߵ��Ǵ� Skill�� ��쿡 �ش� else if���� ����ȴٴ� ��
        // �� �� Animation���� Skill�� �ߵ��� �� ��� Ƚ���� �������� �ʰ�, Input�� ������ ��, �ٷ� ��� Ƚ���� �����ϴ°�?
        // �� ��ų�� �ߵ� ���� ���ο� ������� Input�� ������ �ٷ� ��� Ƚ���� ��´�. 
        // Ex) ���� Q ��ų�� 3Ÿ ����� ����� ��, Stun�� �ɸ��� ��ų�� ���� �ŷ� ó���� 
        // �� �׷��� ���߿� Animation���� Apply �Լ��� ������ ���� Skill�� Input Type�� ���, Apply�� ������ isConsumeAppylCount�� false�� �༭
        //    ApplyCount�� �� �� ���̴� ���� ������ �Ѵ�. 
        // �� CurrentApplyCount�� ���� SkillApplyAction ���� ����
        // 1. Apply �Լ� ���� 
        // 2. TrySendCommandToOwner �Լ��� ���� CurrentApplyCount�� �ش��ϴ� ��ų ���� �ѱ�� 
        // 3. Skill �ߵ� ����/���� ������ ������� CurrentApplyCount 1 ���� 
        // 4. SkillApplyAction ���� 
        else if (!isAutoExecutionType)
            Entity.CurrentApplyCount++;

        UpdateProperty();
    }

    private void OnTargetSelectionCompleted(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        // Skill�� �ʿ�� �ϴ� ������ Type�� TargetSearcher�� �˻��� �������� Type�� ��ġ�ϸ� 
        if (skill.HasValidTargetSelectionResult)
            Apply();
    }

    private void UpdateProperty()
    {
        isAutoExecutionType = Entity.ExecutionType == SkillExecutionType.Auto;
        isInstantApplyType = Entity.ApplyType == SkillApplyType.Instant;
    }
}
