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
                // ��ų ������ �Ҵ� 
                iconImage.sprite = slotSkill.Icon;
                // level Text �Ҵ�
                level.text = slotSkill.Level.ToString();

                // ���� �̹��� �Ҵ�
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
