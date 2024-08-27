using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ Charge 특성 
// 1. 계속 실행되고 있는 SelectTarget에 의해 Charge가 최소 충전량을 달성했고 유효한 기준점을 찾았을 때 
// Ex) 제라스 Q 스킬 
// 2. Al일 경우에는 최대 충전을 하면 자동으로 사용 
// 3. 충전 지속 시간이 끝났을 때, ChargeFinishActionOption이 Use라면 자동 사용
public class ChargingState : SkillState
{
    // Charge 상태가 종료되었는가?
    // → IsChargeSuccessed가 false인데 IsChargeEnded가 true라면 사용 실패, Cooldown 상태로 전이 
    public bool IsChargeEnded {  get; private set; }

    // Charge가 최소 충전량을 채웠고, Skill이 기준점 검색에 성공했는가?
    // → IsChargeSuccessed가 true이면 전이 
    public bool IsChargeSuccessed { get; private set; }

    public override void Enter()
    {
        // Skill 활성화 
        Entity.Activate();

        if (Entity.Owner.IsPlayer)
        {
            // ※ false : Indicator를 보여주지 않음
            // → Skill이 최소 충전량을 채우고 올바른 기준점을 검색할 때까지 반복해서 기준점 검색을 실행 
            Entity.SelectTarget(OnTargetSearchCompleted, false);
        }

        // SelectTarget에서 true로 설정하면 Indicator는 처음에 보여지고 검색이 완료되면 Indicator가 사라지게 된다.
        // 여기서, OnTargetSearchCompleted 함수에 의해 Charge가 부족하거나 올바른 Target을 검색하지 못하면 검색이 재실행된다. 
        // 이때, true로 설정되었으면 다시 Indicator가 보여지고 사리지게 된다. 이게 반복되면서 Indicator가 깜빡거리는 형태로 보여지게 된다. 
        // → 그래서 ShowIndicator를 따로 실행시킴
        Entity.ShowIndicator();
        Entity.StartCustomActions(SkillCustomActoinType.Charge);

        // Owner로 ToChargingSkillState Command를 보냄 
        TrySendCommandToOwner(Entity, EntityStateCommand.ToChargingSkillState, Entity.ChargeAnimationParameter);
    }

    public override void Update()
    {
        // CurrentChargeDuration이 증가하는 동시에 Charge Power도 동시에 Update 된다. 
        Entity.CurrentChargeDuration += Time.deltaTime;

        // AI는 충전이 다 되었을 때, Skill 사용을 시도한다. 
        if (!Entity.Owner.IsPlayer && Entity.IsMaxChargeCompleted)
        {
            IsChargeEnded = true;
            Entity.SelectTarget(false);
            TryUse();
        }
        else if (Entity.IsChargeDurationEnded)
        {
            IsChargeEnded = true;

            // ChargeFinishActionOption이 Use라면
            if (Entity.ChargeFinishActionOption == SkillChargeFinishActionOption.Use)
            {
                Entity.SelectTargetImmediate(HelperUtilities.GetMouseWorldPosition());

                // Skill 사용 실행 
                TryUse();
            }
        }

        Entity.RunCustomActions(SkillCustomActoinType.Charge);
    }

    public override void Exit()
    {
        IsChargeEnded = false;
        IsChargeSuccessed = false;

        if (Entity.IsSearchingTarget)
            Entity.CancelSelectTarget();
        else
            Entity.HideIndicator();

        Entity.ReleaseCustomActions(SkillCustomActoinType.Charge);
    }

    private bool TryUse()
    {
        // Skill이 최소충전량을 채웠고 올바른 기준점을 검색했다면
        // → 상태 전이 
        if (Entity.IsMinChargeCompleted && Entity.IsTargetSelectSuccessful)
            IsChargeSuccessed = true;

        return IsChargeSuccessed;
    }

    private void OnTargetSearchCompleted(Skill skill, TargetSearcher searcher, TargetSelectionResult result)
    {
        // TryUse에서 false가 return 되면, 재검색을 실행 
        if (!TryUse())
            Entity.SelectTarget(OnTargetSearchCompleted, false);
    }
}
