using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� Charge Ư�� 
// 1. ��� ����ǰ� �ִ� SelectTarget�� ���� Charge�� �ּ� �������� �޼��߰� ��ȿ�� �������� ã���� �� 
// Ex) ���� Q ��ų 
// 2. Al�� ��쿡�� �ִ� ������ �ϸ� �ڵ����� ��� 
// 3. ���� ���� �ð��� ������ ��, ChargeFinishActionOption�� Use��� �ڵ� ���
public class ChargingState : SkillState
{
    // Charge ���°� ����Ǿ��°�?
    // �� IsChargeSuccessed�� false�ε� IsChargeEnded�� true��� ��� ����, Cooldown ���·� ���� 
    public bool IsChargeEnded {  get; private set; }

    // Charge�� �ּ� �������� ä����, Skill�� ������ �˻��� �����ߴ°�?
    // �� IsChargeSuccessed�� true�̸� ���� 
    public bool IsChargeSuccessed { get; private set; }

    public override void Enter()
    {
        // Skill Ȱ��ȭ 
        Entity.Activate();

        if (Entity.Owner.IsPlayer)
        {
            // �� false : Indicator�� �������� ����
            // �� Skill�� �ּ� �������� ä��� �ùٸ� �������� �˻��� ������ �ݺ��ؼ� ������ �˻��� ���� 
            Entity.SelectTarget(OnTargetSearchCompleted, false);
        }

        // SelectTarget���� true�� �����ϸ� Indicator�� ó���� �������� �˻��� �Ϸ�Ǹ� Indicator�� ������� �ȴ�.
        // ���⼭, OnTargetSearchCompleted �Լ��� ���� Charge�� �����ϰų� �ùٸ� Target�� �˻����� ���ϸ� �˻��� �����ȴ�. 
        // �̶�, true�� �����Ǿ����� �ٽ� Indicator�� �������� �縮���� �ȴ�. �̰� �ݺ��Ǹ鼭 Indicator�� �����Ÿ��� ���·� �������� �ȴ�. 
        // �� �׷��� ShowIndicator�� ���� �����Ŵ
        Entity.ShowIndicator();
        Entity.StartCustomActions(SkillCustomActoinType.Charge);

        // Owner�� ToChargingSkillState Command�� ���� 
        TrySendCommandToOwner(Entity, EntityStateCommand.ToChargingSkillState, Entity.ChargeAnimationParameter);
    }

    public override void Update()
    {
        // CurrentChargeDuration�� �����ϴ� ���ÿ� Charge Power�� ���ÿ� Update �ȴ�. 
        Entity.CurrentChargeDuration += Time.deltaTime;

        // AI�� ������ �� �Ǿ��� ��, Skill ����� �õ��Ѵ�. 
        if (!Entity.Owner.IsPlayer && Entity.IsMaxChargeCompleted)
        {
            IsChargeEnded = true;
            Entity.SelectTarget(false);
            TryUse();
        }
        else if (Entity.IsChargeDurationEnded)
        {
            IsChargeEnded = true;

            // ChargeFinishActionOption�� Use���
            if (Entity.ChargeFinishActionOption == SkillChargeFinishActionOption.Use)
            {
                Entity.SelectTargetImmediate(HelperUtilities.GetMouseWorldPosition());

                // Skill ��� ���� 
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
        // Skill�� �ּ��������� ä���� �ùٸ� �������� �˻��ߴٸ�
        // �� ���� ���� 
        if (Entity.IsMinChargeCompleted && Entity.IsTargetSelectSuccessful)
            IsChargeSuccessed = true;

        return IsChargeSuccessed;
    }

    private void OnTargetSearchCompleted(Skill skill, TargetSearcher searcher, TargetSelectionResult result)
    {
        // TryUse���� false�� return �Ǹ�, ��˻��� ���� 
        if (!TryUse())
            Entity.SelectTarget(OnTargetSearchCompleted, false);
    }
}
