using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoices : MonoBehaviour
{
    [SerializeField]
    ChoiceSkillSlot[] choiceSkillSlots = new ChoiceSkillSlot[4];
    [SerializeField]
    private SkillDescription skillDescription;
    [SerializeField]
    private SkillTree skillTree;
    [SerializeField]
    private Button reRollButton;
    [SerializeField]
    private TextMeshProUGUI reRollText;
    [SerializeField]
    Button chooseButton;

    [Space(10)]
    [SerializeField]
    private GameObject skillInventory;

    private int rerollCount = 0;

    // 크기가 정해져 있는 경우면 배열을 사용, 그게 아니라면 List를 사용
    private SkillCombinationSlotNode[] choiceSkills = new SkillCombinationSlotNode[4];
    private SkillCombinationSlotNode currentChoiceSkill;
    public SkillCombinationSlotNode CurrentChoiceSkill
    {
        get => currentChoiceSkill;
        set
        {
            if (value == null)
                return;

            currentChoiceSkill = value;
        }
    }

    private void OnEnable()
    {
        chooseButton.onClick.AddListener(ChooseSkill);

        for (int i = 0; i < choiceSkillSlots.Length; i++)
            choiceSkillSlots[i].onClicked += HighlightSlot;
    }

    private void OnDisable()
    {
        currentChoiceSkill = null;

        chooseButton.onClick.RemoveAllListeners();
        reRollButton.onClick.RemoveAllListeners();

        for (int i = 0; i < choiceSkillSlots.Length; i++)
            choiceSkillSlots[i].onClicked -= HighlightSlot;
    }

    public void SetUpSkillChoices(List<SkillCombinationSlotNode> skills)
    {
        SetChoiceSkills(skills);

        // choiceSkills에 저장된 정보들을 choiceSkillSlots에 할당
        for (int i = 0; i < choiceSkills.Length; i++)
            choiceSkillSlots[i].SkillSlot = choiceSkills[i];
    }

    public void SetUpReRollButton(Entity entity)
    {
        rerollCount = Mathf.FloorToInt(entity.Stats.ReRollStat.Value);
        reRollText.text = rerollCount.ToString();
        reRollButton.onClick.AddListener(ReRoll);
    }

    private void SetChoiceSkills(List<SkillCombinationSlotNode> skills)
    {
        // [0,1,2,3] : choiceSkills에 skill을 삽입할 위치를 생성
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < 4; i++)
            availableIndices.Add(i);

        // 랜덤 숫자 생성
        System.Random random = new System.Random();
        foreach (var skill in skills)
        {
            // 더이상 삽입할 공간이 없으면 반복문을 멈춘다.
            if (availableIndices.Count == 0) break;

            // random.Next(int maxValue)는 0 이상 maxValue 미만의 정수(즉, 배열 인덱스)를 무작위로 반환
            int randomIndex = random.Next(availableIndices.Count);
            int chosenIndex = availableIndices[randomIndex];
            // choiceSkills[chosenIndex] = skill는 배열에서 사용하고 리스트에서는 되도록 사용하지 말기
            // → ArgumentOutOfRangeException 발생
            choiceSkills[chosenIndex] = skill;
            // 중복 방지를 위해 한 번 선택된 Index는 삭제한다. 
            availableIndices.RemoveAt(randomIndex);
        }

        foreach (var index in availableIndices)
            choiceSkills[index] = null;
    }

    private void ChooseSkill()
    {
        if (currentChoiceSkill == null)
            return;

        var player = GameManager.Instance.player;

        // 1. 강화 
        if (player.SkillSystem.ContainsInownskills(currentChoiceSkill.Skill))
        {
            var skill = player.SkillSystem.FindOwnSkill(currentChoiceSkill.Skill);
            player.SkillSystem.SkillLevelUp(skill);
        }
        // 2. 획득 or 조합 
        else
            currentChoiceSkill.AcquireSkill(player);

        // 스킬 인벤토리 UI 띄우기 
        skillInventory.SetActive(true);

        gameObject.SetActive(false);
    }

    private void HighlightSlot(int slotNumber)
    {
        for (int i = 0; i < choiceSkillSlots.Length; i++)
        {
            if (i != slotNumber)
            {
                choiceSkillSlots[i].HighLightImage.gameObject.SetActive(false);
                continue;
            }

            choiceSkillSlots[i].HighLightImage.gameObject.SetActive(true);
        }
    }

    private void ReRoll()
    {
        rerollCount--;

        if (rerollCount <= 0)
            return;

        var skillChoices = GameManager.Instance.SetSkillChoices();
        SetUpSkillChoices(skillChoices);

        for (int i = 0; i < 4; i++)
            choiceSkillSlots[i].HighLightImage.gameObject.SetActive(false);

        reRollText.text = rerollCount.ToString();

        skillDescription.EmptySkillDescription();
        skillTree.gameObject.SetActive(false);
    }
}