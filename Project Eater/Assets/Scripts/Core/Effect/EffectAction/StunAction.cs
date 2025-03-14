using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StunAction : EffectAction
{
    [SerializeField]
    private Category removeTargetCategory;

    private bool isSuperArmor = false;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        // ※ 패턴 매칭 사용 
        // → 특정 데이터의 형태를 검사하고, 원하는 조건이 충족될 경우 값을 추출하는 기능
        // → if, switch, is 등의 문법을 더 간결하고 직관적으로 작성할 수 있다.
        // ※ is 패턴 매칭
        // → C# 9 이상에서는 특정 속성을 검사하는 패턴을 사용할 수 있다. 
        if (target is PlayerEntity player && player.StateMachine.IsInState<PlayerSuperArmorState>())
        {
            isSuperArmor = true;
            return true;
        }

        target.SkillSystem.RemoveEffectAll(removeTargetCategory);
        if (target.IsPlayer)
            (target as PlayerEntity).StateMachine.ExecuteCommand(EntityStateCommand.ToStunningState);
        else if (target is EnemyEntity)
            (target as EnemyEntity).StateMachine.ExecuteCommand(EntityStateCommand.ToStunningState);
        else 
            (target as BossEntity).StateMachine.ExecuteCommand(EntityStateCommand.ToStunningState);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        if (isSuperArmor)
        {
            isSuperArmor = false;
            return;
        }

        if (target is PlayerEntity player)
        {
            player.StateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
        }
        else if (target is EnemyEntity)
            (target as EnemyEntity).StateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
        else
            (target as BossEntity).StateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
    }

    public override object Clone() => new StunAction() { removeTargetCategory = removeTargetCategory };
}
