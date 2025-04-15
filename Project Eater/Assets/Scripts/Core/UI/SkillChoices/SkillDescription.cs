using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI gradeText;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI typeText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    #region Color
    Color32 commonColor = new Color32(141, 140, 140, 255); // �Ϲ�
    Color32 rareColor = new Color32(33, 99, 0, 255);     // ���
    Color32 uniqueColor = new Color32(225, 185, 0, 255);   // ����ũ
    #endregion

    private void OnDisable() => EmptySkillDescription();

    public void SetupDescription(SkillCombinationSlotNode skillSlot)
    {
        // skillSlot�� null�̸� ��ȭ ���� ����
        if (skillSlot == null)
        {
            nameText.text = string.Empty;
            gradeText.text = string.Empty;
            levelText.text = string.Empty;
            typeText.text = string.Empty;
            descriptionText.text = "�پ��� ������ 50�� ȹ���մϴ�.";

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
        string skillGrade;
        switch (skill.Grade)
        {
            case SkillGrade.Latent:
                skillGrade = "";
                break;
            case SkillGrade.Common:
                skillGrade = "Common";
                gradeText.color = commonColor;
                break;
            case SkillGrade.Rare:
                skillGrade = "Rare";
                gradeText.color = rareColor;
                break;
            case SkillGrade.Unique:
                skillGrade = "Unique";
                gradeText.color = uniqueColor;
                break;
            default:
                skillGrade = "";
                break;
        }

        string skillType = (skill.Type == SkillType.Active) ? "Active" : "Passive";

        nameText.text = skill.DisplayName;
        gradeText.text = skillGrade;
        levelText.text = "[Level " + skill.Level + "]";
        typeText.text = "[" + skillType + "]";

        descriptionText.text = skill.Description;
    }

    public void EmptySkillDescription()
    {
        nameText.text = string.Empty;
        gradeText.text = string.Empty;
        levelText.text = string.Empty;
        typeText.text = string.Empty;
        descriptionText.text = string.Empty;
    }
}
