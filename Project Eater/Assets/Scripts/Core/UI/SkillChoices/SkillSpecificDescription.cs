using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillSpecificDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    private void OnDisable() => EmptySkillDescription();

    public void SetupDescription(SkillCombinationSlotNode skillSlot)
    {
        // skillSlot�� null�̸� ��ȭ ���� ����
        if (skillSlot == null) return;

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
        nameText.text = skill.DisplayName;
        descriptionText.text = skill.SpecificDescription;
    }

    public void EmptySkillDescription()
    {
        nameText.text = string.Empty;
        descriptionText.text = string.Empty;
    }
}
