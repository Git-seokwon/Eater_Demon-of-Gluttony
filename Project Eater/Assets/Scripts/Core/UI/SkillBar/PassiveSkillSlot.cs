using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class PassiveSkillSlot : SkillSlot
{
    public Skill Skill
    {
        get => skill;
        set
        {
            skill = value;

            if (skill != null)
            {
                iconImage.gameObject.SetActive(true);
                iconImage.sprite = skill.Icon;
            }
            else
                SetSkillUIAction(false);
        }
    }

    private void OnEnable()
    {
        SetSkillUIAction(false);
    }

    private void SetSkillUIAction(bool isOn) => iconImage.gameObject.SetActive(isOn);
}
