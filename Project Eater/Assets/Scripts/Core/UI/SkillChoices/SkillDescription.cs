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
        // skillSlot이 null이면 재화 지급 선택
        if (skillSlot == null)
        {
            nameAndLevelText.text = string.Empty;
            descriptionText.text = "바알의 살점을 500개 획득합니다.";
            cooldownText.text = string.Empty;

            return;
        }

        var player = GameManager.Instance.player;
        Skill skill;
        // 1. 획득 한 스킬 = 강화 
        if (player.SkillSystem.ContainsInownskills(skillSlot.Skill))
        {
            skill = player.SkillSystem.FindOwnSkill(skillSlot.Skill).Clone() as Skill;
            skill.LevelUp(true);

            ShowSkillDescription(skill);
            Destroy(skill);
        }
        // 2. 획득 전 스킬 = 획득, 조합
        else
        {
            skill = player.SkillSystem.Register(skillSlot.Skill);

            ShowSkillDescription(skill);
            player.SkillSystem.Unregister(skill);
        }
    }

    private void ShowSkillDescription(Skill skill)
    {
        string skillType = (skill.Type == SkillType.Active) ? "액티브" : "패시브";

        nameAndLevelText.text = skill.DisplayName + " - " + (skill.Level) + " Level - " + skillType;
        descriptionText.text = skill.Description;
        if (skill.Type == SkillType.Active)
            cooldownText.text = "Cooldown : " + skill.Cooldown.ToString();
        else
            cooldownText.text = "";
    }

    public void EmptySkillDescription()
    {
        nameAndLevelText.text = string.Empty;
        descriptionText.text = string.Empty;
        cooldownText.text = string.Empty;
    }
}
