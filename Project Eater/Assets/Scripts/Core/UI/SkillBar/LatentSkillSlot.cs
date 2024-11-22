using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.CullingGroup;

public class LatentSkillSlot : SkillSlot
{
    public Skill Skill
    {
        get => skill;
        set
        {
            skill = value;

            if (skill != null)
            {
                iconImage.color = Color.white;
                iconImage.sprite = skill.Icon;
            }
            else
                iconImage.color = Color.black;
        }
    }
}
