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
        // �� ���� ��Ī ��� 
        // �� Ư�� �������� ���¸� �˻��ϰ�, ���ϴ� ������ ������ ��� ���� �����ϴ� ���
        // �� if, switch, is ���� ������ �� �����ϰ� ���������� �ۼ��� �� �ִ�.
        // �� is ���� ��Ī
        // �� C# 9 �̻󿡼��� Ư�� �Ӽ��� �˻��ϴ� ������ ����� �� �ִ�. 
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
