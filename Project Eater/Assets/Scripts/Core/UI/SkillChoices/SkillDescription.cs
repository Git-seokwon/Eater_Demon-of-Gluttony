using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameAndLevelText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private TextMeshProUGUI cooldownText;

    private void OnDisable() => EmptySkillDescription();

    public void SetupDescription(SkillCombinationSlotNode skillSlot)
    {
        if (skillSlot == null) return;


        var player = GameManager.Instance.player;
        Skill skill;
        // 1. È¹µæ ÇÑ ½ºÅ³ = °­È­ 
        if (player.SkillSystem.ContainsInownskills(skillSlot.Skill))
        {
            skill = player.SkillSystem.FindOwnSkill(skillSlot.Skill).Clone() as Skill;
            skill.LevelUp(true);

            ShowSkillDescription(skill);
            Destroy(skill);
        }
        // 2. È¹µæ Àü ½ºÅ³ = È¹µæ, Á¶ÇÕ
        else
        {
            skill = player.SkillSystem.Register(skillSlot.Skill);

            ShowSkillDescription(skill);
            player.SkillSystem.Unregister(skill);
        }
    }

    private void ShowSkillDescription(Skill skill)
    {
        nameAndLevelText.text = skill.DisplayName + " - " + (skill.Level) + " Level";
        descriptionText.text = skill.Description;
        if (skill.Type == SkillType.Active)
            cooldownText.text = "Cooldown : " + skill.Cooldown.ToString();
    }

    public void EmptySkillDescription()
    {
        nameAndLevelText.text = string.Empty;
        descriptionText.text = string.Empty;
        cooldownText.text = string.Empty;
    }
}
