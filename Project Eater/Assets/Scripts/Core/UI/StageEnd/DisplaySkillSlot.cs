using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySkillSlot : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    protected Image borderImage;
    [SerializeField]
    private TextMeshProUGUI level;

    private Skill slotSkill;

    public Skill SlotSkill
    {
        get => slotSkill;
        set
        {
            slotSkill = value;

            if (slotSkill != null)
            {
                // 스킬 아이콘 할당 
                iconImage.sprite = slotSkill.Icon;
                // level Text 할당
                level.text = slotSkill.Level.ToString();

                // 슬롯 이미지 할당
                borderImage.sprite = GameResources.Instance.GetBorderImageByGrade(slotSkill.Grade);

                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
            }
            else
                gameObject.SetActive(false);
        }
    }

    public void Setup(Skill skill) => SlotSkill = skill;
}
