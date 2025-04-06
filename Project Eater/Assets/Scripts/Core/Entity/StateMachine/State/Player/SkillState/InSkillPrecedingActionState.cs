using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CastingSkillState�� ����
public class InSkillPrecedingActionState : PlayerSkillState
{
    public override bool OnReceiveMessage(int message, object data)
    {
        if (!base.OnReceiveMessage(message, data))
            return false;

        // ��ų SFX ���
        if (RunningSkill.InPrecedingActionSkillSFXs.Count != 0)
            SoundEffectManager.Instance.PlaySoundEffect(RunningSkill.InPrecedingActionSkillSFXs[RunningSkill.PrecedingSFXIndex]);

        return true;
    }
}
