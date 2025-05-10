using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField]
    protected TextMeshProUGUI optionTypeText;

    [Space(10)]
    [SerializeField]
    private SkillDescription skillDescription;
    [SerializeField]
    private SkillSpecificDescription specificSkillDescription;
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

                var player = GameManager.Instance.player;
                if (player.SkillSystem.AcquirableSkills.Contains(skillSlot))
                    optionTypeText.text = "<color=yellow>ȹ��!</color>";
                else if (player.SkillSystem.CombinableSkills.Contains(skillSlot))
                    optionTypeText.text = "<color=red>����!</color>";
                else
                    optionTypeText.text = "";
            }
            // skill�� null�̸� ��ȭ ����
            else
            {
                iconImage.sprite = GameResources.Instance.additionalGoodsChoiceImage;
                // ��ȭ �������� �׵θ��� ���� ��ų�� �ϱ� 
                borderImage.sprite = GameResources.Instance.borderImages[1];
                optionTypeText.text = "";
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        skillChoices.IsClick = true;

        // �� ��ų ���� ���̶���Ʈ
        onClicked?.Invoke(slotNumber);
        // ���̶���Ʈ �� ��ų�� CurrentChoiceSkill���� ����
        skillChoices.CurrentChoiceSkill = SkillSlot;

        // �� ������ ����� ������ ���ֱ� 
        specificSkillDescription.EmptySkillDescription();
        // ��ų ���� ���� ���� 
        skillDescription.SetupDescription(SkillSlot);

        // ��ų Ʈ�� ���� ���� 
        skillTree.SetupSkillTree(skillSlot);
    }
}
