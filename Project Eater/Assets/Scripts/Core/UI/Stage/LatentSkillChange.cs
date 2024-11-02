using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LatentSkillChange : MonoBehaviour
{
    [SerializeField]
    private PlayerEntity player;
    [SerializeField]
    private Image centerLatentSkill;
    [SerializeField]
    private Image rightLatentSkill;
    [SerializeField]
    private Image leftLatentSkill;
    [SerializeField]
    private Button rightButton;
    [SerializeField]
    private Button leftButton;

    private Skill currentLatentSkill;
    private int currentLatentSkillIndex;
    private int maxLatentSkillIndex;

    private void OnEnable()
    {
        maxLatentSkillIndex = player.LatentSkills.Count;
        Debug.Log(maxLatentSkillIndex);
        currentLatentSkill = player.currentLatentSkill[2];

        ShowLatentSkillIcon();

        rightButton.onClick.AddListener(OnRightButton);
        leftButton.onClick.AddListener(OnLeftButton);
    }

    private void OnDisable()
    {
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
    }

    private void ShowLatentSkillIcon()
    {
        centerLatentSkill.sprite = currentLatentSkill.Icon;

        int rightIndex = (currentLatentSkillIndex + 1) % maxLatentSkillIndex;
        int leftIndex = (currentLatentSkillIndex - 1) < 0 ? maxLatentSkillIndex - 1 : currentLatentSkillIndex - 1;

        rightLatentSkill.sprite = player.LatentSkills[rightIndex][2].Icon;
        leftLatentSkill.sprite = player.LatentSkills[leftIndex][2].Icon;
    }

    private void OnRightButton()
    {
        currentLatentSkillIndex = (currentLatentSkillIndex + 1) % maxLatentSkillIndex;
        Debug.Log(currentLatentSkillIndex);
        ShowLatentSkillIcon();
    }

    private void OnLeftButton()
    {
        currentLatentSkillIndex = (currentLatentSkillIndex - 1) < 0 ? maxLatentSkillIndex - 1 : currentLatentSkillIndex - 1;
        Debug.Log(currentLatentSkillIndex);
        ShowLatentSkillIcon();
    }
}
