using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    protected Image iconImage;
    [SerializeField]
    protected Image borderImage;
    [SerializeField]
    protected Image highlightImage;
    [SerializeField]
    private SkillSelectorTooltip skillTooltip;
    [SerializeField]
    private TextMeshProUGUI levelText;

    private SkillCombinationSlotNode skillSlot;

    private SkillTree skillTree;

    private void OnEnable()
    {
        skillTree = GetComponentInParent<SkillTree>(true);
    }

    private void OnDisable()
    {
        skillTree = null;

        if (highlightImage.gameObject.activeSelf)
            highlightImage.gameObject.SetActive(false);
    }

    public SkillCombinationSlotNode SkillSlot
    {
        get => skillSlot;
        set
        {
            skillSlot = value;

            if (skillSlot != null)
            {
                iconImage.sprite = skillSlot.Skill.Icon;

                if (skillSlot.IsInherent)
                    borderImage.sprite = GameResources.Instance.borderImages[1];
                else
                    borderImage.sprite = GameResources.Instance.borderImages[0];

                if (skillSlot == skillTree.CurrentSkill)
                    highlightImage.gameObject.SetActive(true);
                else
                    highlightImage.gameObject.SetActive(false);

                if (levelText != null)
                {
                    Skill skill = GameManager.Instance.player.SkillSystem.FindOwnSkill(skillSlot.Skill);
                    string skillLevelText = (skill == null) ? "0" : skill.Level.ToString();

                    // 스킬 레벨 정보 표기 
                    levelText.text = (skill != null && skill.Level >= 5) ? "<color=yellow>" + skillLevelText + "/5</color>" : 
                                                                           "<color=red>" + skillLevelText + "/5</color>";
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Skill Tooltip 보여주기 
        if (SkillSlot != null && skillTooltip != null)
            skillTooltip.Show(SkillSlot.Skill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Skill Tooltip 끄기
        if (SkillSlot != null && skillTooltip != null)
            skillTooltip.Hide();
    }
}
