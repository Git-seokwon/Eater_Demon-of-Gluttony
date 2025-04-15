using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeSlot : MonoBehaviour
{
    [SerializeField]
    protected Image iconImage;
    [SerializeField]
    protected Image borderImage;
    [SerializeField]
    protected Image highlightImage;

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

            }
        }
    }
}
