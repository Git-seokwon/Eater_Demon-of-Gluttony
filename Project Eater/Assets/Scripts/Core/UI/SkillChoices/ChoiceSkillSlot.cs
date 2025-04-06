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

    // Slot이 소유한 스킬 
    private SkillCombinationSlotNode skillSlot;

    // 스킬 선택지 (= 부모 오브젝트)
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
            // skill이 null이면 재화 선택
            else
            {
                iconImage.sprite = GameResources.Instance.additionalGoodsChoiceImage;
                // 재화 선택지는 테두리를 고유 스킬로 하기 
                borderImage.sprite = GameResources.Instance.borderImages[1];
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClicked?.Invoke(slotNumber);
        // 하이라이트 된 스킬을 CurrentChoiceSkill으로 설정
        skillChoices.CurrentChoiceSkill = SkillSlot;

        // 스킬 설명 내용 띄우기 
        skillDescription.SetupDescription(SkillSlot);

        // 스킬 트리 내용 띄우기 
        skillTree.SetupSkillTree(skillSlot);
    }
}
