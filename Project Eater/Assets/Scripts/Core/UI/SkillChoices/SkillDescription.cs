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
        // skillSlot¿Ã null¿Ã∏È ¿Á»≠ ¡ˆ±ﬁ º±≈√
        if (skillSlot == null)
        {
            nameAndLevelText.text = string.Empty;
            descriptionText.text = "πŸæÀ¿« ªÏ¡°¿ª 500∞≥ »πµÊ«’¥œ¥Ÿ.";
            cooldownText.text = string.Empty;

            return;
        }

        var player = GameManager.Instance.player;
        Skill skill;
        // 1. »πµÊ «— Ω∫≈≥ = ∞≠»≠ 
        if (player.SkillSystem.ContainsInownskills(skillSlot.Skill))
        {
            skill = player.SkillSystem.FindOwnSkill(skillSlot.Skill).Clone() as Skill;
            skill.LevelUp(true);

            ShowSkillDescription(skill);
            Destroy(skill);
        }
        // 2. »πµÊ ¿¸ Ω∫≈≥ = »πµÊ, ¡∂«’
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
