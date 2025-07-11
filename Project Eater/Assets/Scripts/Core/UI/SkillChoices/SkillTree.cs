using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    [SerializeField]
    private SkillTreeSlot leftPrecedingSkill;
    [SerializeField]
    private SkillTreeSlot rightPrecedingSkill;
    [SerializeField]
    private SkillTreeSlot topTierSkill;
    [SerializeField]
    private Button rightButton;
    [SerializeField]
    private Button leftButton;

    private SkillCombinationSlotNode[] skills;
    private SkillCombinationSlotNode currentSkill;
    private int currentSkillTreeIndex = 0;

    public SkillCombinationSlotNode CurrentSkill => currentSkill;

    private void OnDisable()
    {
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
        currentSkillTreeIndex = 0;

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void SetupSkillTree(SkillCombinationSlotNode skill) // skill : 현재 내가 선택한 스킬 
    {
        if (skill == null)
        {
            gameObject.SetActive(false);
            return;
        }
        currentSkill = skill;

        // 상위 스킬이고, 처음 획득하는 스킬이라면 하위 스킬 항목을 보여주고 어떤 스킬이 사라지는 지 확인
        if (skill.Tier > 0 && GameManager.Instance.player.SkillSystem.CombinableSkills.Contains(skill))
        {
            skills = new SkillCombinationSlotNode[1] { skill };
            ShowSkillTree();
        }
        // 상위 스킬 트리를 보여줌
        else
        {
            // 상위 스킬 가져오기 
            skills = skill.GetTopSkillSlotNodes();
            if (skills.Length == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            if (skills.Length == 1)
                ShowSkillTree();
            else if (skills.Length > 1)
            {
                // 버튼 활성화
                rightButton.gameObject.SetActive(true);
                leftButton.gameObject.SetActive(true);

                // 버튼 이벤트 등록 
                rightButton.onClick.AddListener(OnRightButton);
                leftButton.onClick.AddListener(OnLeftButton);

                ShowSkillTree();
            }
        }
    }

    private void OnRightButton()
    {
        currentSkillTreeIndex = (currentSkillTreeIndex + 1) % skills.Length;

        ShowSkillTree();
    }

    private void OnLeftButton()
    {
        currentSkillTreeIndex = (currentSkillTreeIndex - 1) < 0 ? skills.Length - 1 : currentSkillTreeIndex - 1;

        ShowSkillTree();
    }

    private void ShowSkillTree()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        var preceding = skills[currentSkillTreeIndex].GetPrecedingSlotNodes();

        topTierSkill.SkillSlot = skills[currentSkillTreeIndex];
        leftPrecedingSkill.SkillSlot = preceding[0];
        rightPrecedingSkill.SkillSlot = preceding[1];
    }
}
