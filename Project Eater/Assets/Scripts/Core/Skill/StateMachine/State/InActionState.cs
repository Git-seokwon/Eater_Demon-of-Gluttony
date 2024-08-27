using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Apply Type이 Instant인 Skill을 Apply 시키는 State
// → Apply Type이 Animation이면, InActionState에서 Apply가 일어나는 것이 아니라 Animation이 정해준 Timing에 맞춰
//    Apply가 실행 
public class InActionState : SkillState
{
    // Skill의 ExcutionType이 Auto인지를 나타내는 변수 
    // → 해당 변수가 false이면 ExcutionType이 Input이라는 뜻
    private bool isAutoExecutionType;

    // Skill의 발동이 즉각적인 InstantType인지 나타내는 변수 
    // → 해당 변수가 false라면 AnimationType이고, 이는 Animation에서 발동 Timing을 정한다는 의미
    private bool isInstantApplyType;

    protected override void Setup()
    {
        isAutoExecutionType = Entity.ExecutionType == SkillExecutionType.Auto;
        isInstantApplyType = Entity.ApplyType == SkillApplyType.Instant;
    }

    public override void Enter()
    {
        // InPrecedingAction과 마찬가지로 Casting과 ChargingState를 안 거치고 왔을 수도 있기 때문에 
        // Activate 상태가 아니라면 Activate 해주기 
        if (!Entity.IsActivated)
            Entity.Activate();

        Entity.StartActoin();

        Apply();
    }

    // 1. Auto + Instant Type이 Skill을 실행시키는 함수 - Update
    // ★ Logic
    // 1. InActionState의 Update 함수에서 CurrentApplyCycle을 증가시킴
    // 2. CurrentApplyCycle이 ApplyCycle에 도달하면 Apply 함수를 실행 
    // 3. Apply 함수가 실행되면 CurrentApplyCycle이 초기화 된다. 
    // 4. 1 ~ 3번 구조 반복 
    public override void Update()
    {
        Entity.CurrentDuration += Time.deltaTime;
        Entity.CurrentApplyCycle += Time.deltaTime;

        if (isAutoExecutionType && Entity.IsApplicable)
            Apply();
    }

    public override void Exit()
    {
        Entity.CancelSelectTarget();
        Entity.ReleaseActoin();
    }

    // 2. Input + Instant Type이 Skill을 실행시키는 함수 - OnReceiveMessage
    // → Execute Type이 Input일 경우, Skill의 Use 함수를 통해 Use Message가 넘어오면 Apply 함수를 호출함
    //    즉, Skill의 발동이 Update 함수에서 자동(Auto)으로 되는 것이 아니라, 사용자의 입력(Input)을 통해 이 함수에서 됨
    public override bool OnReceiveMessage(int message, object data)
    {
        // Input Type일 때, Player가 Skill Button을 누르면 Skill의 Use 함수가 실행되고 Use Message가 넘어오면 처리 
        // → Skill이 InActionState 전 상태(Ready)일 때 Skill::Use 함수를 실행하면 StateMachine.ExecuteCommand(SkillExecuteCommand.Use)가 실행되고 
        //    InActionState 상태이면 StateMachine.SendMessage(SkillStateMessage.Use)가 실행된다. 
        var stateMessage = (SkillStateMessage)message;
        if (stateMessage != SkillStateMessage.Use || isAutoExecutionType)
            return false;

        if (Entity.IsApplicable)
        {
            if (Entity.IsTargetSelectionTiming(TargetSelectionTimingOption.UseInAction))
            {
                // Skill이 Searching중이 아니라면 SelectTarget 함수로 기준점 검색을 실행,
                // 기준점 검색이 성공하면 OnTargetSelectionCompleted Callback 함수가 호출되어 Apply 함수를 호출함
                if (!Entity.IsSearchingTarget)
                    Entity.SelectTarget(OnTargetSelectionCompleted);
            }
            // SelectionTiming이 UseInAction, Both가 아니라면(Use 라면) 바로 Apply 함수를 실행 
            else
                Apply();

            return true;
        }
        else
            return false;
    }

    private void Apply()
    {
        // Owner에게 ToInSkillActionState의 Command 보내기 
        TrySendCommandToOwner(Entity, EntityStateCommand.ToInSkillActionState, Entity.ActionAnimationParameter);

        // Skill이 Instant Type이라면 Skill의 Apply 함수를 실행하여 Skill을 발동한다.
        // → Skill의 ApplyType이 Animation이라면 Animation에서 Aplly 신호를 보낼 것이기 때문에 따로 Apply 함수를 실행하지 않는다. 
        if (isInstantApplyType)
            Entity.Apply();
        // Skill이 Input Type이라면 CurrentApplyCount를 1회 증가시킴 → 사용 가능한 횟수를 한 번 차감 
        // → 해당 else if문이 실행되려면 ApplyType이 Animation이고, ExecuteType이 Input이어야 한다. 
        // → User가 Skill Button을 누르고, Animation에 의해 발동되는 Skill일 경우에 해당 else if문이 실행된다는 것
        // ※ 왜 Animation에서 Skill이 발동할 때 사용 횟수를 차감하지 않고, Input이 들어왔을 때, 바로 사용 횟수를 차감하는가?
        // → 스킬의 발동 성공 여부와 상관없이 Input이 들어오면 바로 사용 횟수를 깎는다. 
        // Ex) 리븐 Q 스킬도 3타 모션이 진행될 때, Stun에 걸리면 스킬이 사용된 거로 처리됨 
        // → 그래서 나중에 Animation에서 Apply 함수를 실행할 때는 Skill이 Input Type인 경우, Apply의 인자인 isConsumeAppylCount를 false로 줘서
        //    ApplyCount가 두 번 깎이는 일이 없도록 한다. 
        else if (!isAutoExecutionType)
            Entity.CurrentApplyCount++;
    }

    private void OnTargetSelectionCompleted(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        // Skill이 필요로 하는 기준점 Type과 TargetSearcher가 검색한 기준점의 Type이 일치하면 
        if (skill.HasValidTargetSelectionResult)
            Apply();
    }
}
