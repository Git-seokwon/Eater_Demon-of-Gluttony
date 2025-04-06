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
        // skillSlot�� null�̸� ��ȭ ���� ����
        if (skillSlot == null)
        {
            nameAndLevelText.text = string.Empty;
            descriptionText.text = "�پ��� ������ 500�� ȹ���մϴ�.";
            cooldownText.text = string.Empty;

            return;
        }

        var player = GameManager.Instance.player;
        Skill skill;
        // 1. ȹ�� �� ��ų = ��ȭ 
        if (player.SkillSystem.ContainsInownskills(skillSlot.Skill))
        {
            skill = player.SkillSystem.FindOwnSkill(skillSlot.Skill).Clone() as Skill;
            skill.LevelUp(true);

            ShowSkillDescription(skill);
            Destroy(skill);
        }
        // 2. ȹ�� �� ��ų = ȹ��, ����
        else
        {
            skill = player.SkillSystem.Register(skillSlot.Skill);

            ShowSkillDescription(skill);
            player.SkillSystem.Unregister(skill);
        }
    }

    private void ShowSkillDescription(Skill skill)
    {
        string skillType = (skill.Type == SkillType.Active) ? "��Ƽ��" : "�нú�";

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
