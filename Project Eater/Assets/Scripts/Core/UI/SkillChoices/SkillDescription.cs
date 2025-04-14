using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI typeText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    private void OnDisable() => EmptySkillDescription();

    public void SetupDescription(SkillCombinationSlotNode skillSlot)
    {
        // skillSlot¿Ã null¿Ã∏È ¿Á»≠ ¡ˆ±ﬁ º±≈√
        if (skillSlot == null)
        {
            nameText.text = string.Empty;
            levelText.text = string.Empty;
            typeText.text = string.Empty;
            descriptionText.text = "πŸæÀ¿« ªÏ¡°¿ª 50∞≥ »πµÊ«’¥œ¥Ÿ.";

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
        string skillType = (skill.Type == SkillType.Active) ? "Active" : "Passive";

        nameText.text = skill.DisplayName;
        levelText.text = "[Level " + skill.Level + "]";
        typeText.text = "[" + skillType + "]";

        descriptionText.text = skill.Description;
    }

    public void EmptySkillDescription()
    {
        nameText.text = string.Empty;
        levelText.text = string.Empty;
        typeText.text = string.Empty;
        descriptionText.text = string.Empty;
    }
}
