using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceSkillSlot : MonoBehaviour, IPointerClickHandler
{
    public delegate void OnClickedHandler(int number);

    public event OnClickedHandler onClicked;

    [SerializeField]
    protected Image iconImage;
    [SerializeField]
    protected Image borderImage;
    [SerializeField]
    private Image highlightImage;
    [SerializeField]
    private int slotNumber;

    [Space(10)]
    [SerializeField]
    private SkillDescription skillDescription;
    [SerializeField]
    private SkillTree skillTree;

    // Slot�� ������ ��ų 
    private SkillCombinationSlotNode skillSlot;

    // ��ų ������ (= �θ� ������Ʈ)
    private SkillChoices skillChoices;

    public Image HighLightImage => highlightImage;

    private void OnEnable()
    {
        skillChoices = GetComponentInParent<SkillChoices>(true);
    }

    private void OnDisable()
    {
        skillChoices = null;

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
            }
            // skill�� null�̸� ��ȭ ����
            else
            {
                iconImage.sprite = GameResources.Instance.additionalGoodsChoiceImage;
                // ��ȭ �������� �׵θ��� ���� ��ų�� �ϱ� 
                borderImage.sprite = GameResources.Instance.borderImages[1];
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClicked?.Invoke(slotNumber);
        // ���̶���Ʈ �� ��ų�� CurrentChoiceSkill���� ����
        skillChoices.CurrentChoiceSkill = SkillSlot;

        // ��ų ���� ���� ���� 
        skillDescription.SetupDescription(SkillSlot);

        // ��ų Ʈ�� ���� ���� 
        skillTree.SetupSkillTree(skillSlot);
    }
}
