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
    private SkillSpecificDescription specificSkillDescription;
    [SerializeField]
    private SkillTree skillTree;
    [SerializeField]
    private Button reRollButton;
    [SerializeField]
    private Button descriptionButton;
    [SerializeField]
    private Button specificDescriptionButton;
    [SerializeField]
    private TextMeshProUGUI reRollText;
    [SerializeField]
    Button chooseButton;

    [Space(10)]
    [SerializeField]
    private GameObject skillInventory;

    private int rerollCount = 0;

    // ũ�Ⱑ ������ �ִ� ���� �迭�� ���, �װ� �ƴ϶�� List�� ���
    private SkillCombinationSlotNode[] choiceSkills = new SkillCombinationSlotNode[4];
    private SkillCombinationSlotNode currentChoiceSkill;
    public SkillCombinationSlotNode CurrentChoiceSkill
    {
        get => currentChoiceSkill;
        set
        {
            currentChoiceSkill = value;
        }
    }

    // ������ Ŭ�� ����
    // �� �������� Ŭ�� ���� ���� ���, Choose ��ư�� ���� �������� �Ѿ �� ���� �Ѵ�. 
    private bool isClick;
    public bool IsClick
    {
        get => isClick;
        set => isClick = value;
    }

    private void OnEnable()
    {
        // �̺�Ʈ ���
        chooseButton.onClick.AddListener(ChooseSkill);
        descriptionButton.onClick.AddListener(ShowSkillDescription);
        specificDescriptionButton.onClick.AddListener(ShowSpecificDescription);

        // ��ų ���� ��, ���̶���Ʈ �Ǵ� �̺�Ʈ ���
        for (int i = 0; i < choiceSkillSlots.Length; i++)
            choiceSkillSlots[i].onClicked += HighlightSlot;

        isClick = false;
    }

    private void OnDisable()
    {
        currentChoiceSkill = null;

        // �̺�Ʈ ���� 
        chooseButton.onClick.RemoveAllListeners();
        reRollButton.onClick.RemoveAllListeners();
        descriptionButton.onClick.RemoveAllListeners();
        specificDescriptionButton.onClick.RemoveAllListeners();

        // ��ų ���� ��, ���̶���Ʈ �Ǵ� �̺�Ʈ ���� 
        for (int i = 0; i < choiceSkillSlots.Length; i++)
            choiceSkillSlots[i].onClicked -= HighlightSlot;
    }

    public void SetUpSkillChoices(List<SkillCombinationSlotNode> skills)
    {
        SetChoiceSkills(skills);

        // choiceSkills�� ����� �������� choiceSkillSlots�� �Ҵ�
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
        // [0,1,2,3] : choiceSkills�� skill�� ������ ��ġ�� ����
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < 4; i++)
            availableIndices.Add(i);

        // ���� ���� ����
        System.Random random = new System.Random();
        foreach (var skill in skills)
        {
            // ���̻� ������ ������ ������ �ݺ����� �����.
            if (availableIndices.Count == 0) break;

            // random.Next(int maxValue)�� 0 �̻� maxValue �̸��� ����(��, �迭 �ε���)�� �������� ��ȯ
            int randomIndex = random.Next(availableIndices.Count);
            int chosenIndex = availableIndices[randomIndex];
            // choiceSkills[chosenIndex] = skill�� �迭���� ����ϰ� ����Ʈ������ �ǵ��� ������� ����
            // �� ArgumentOutOfRangeException �߻�
            choiceSkills[chosenIndex] = skill;
            // �ߺ� ������ ���� �� �� ���õ� Index�� �����Ѵ�. 
            availableIndices.RemoveAt(randomIndex);
        }
        
        // ������ ���������� ��ȭ ���� ������ �Ҵ� 
        foreach (var index in availableIndices)
            choiceSkills[index] = null;
    }

    private void ChooseSkill()
    {
        if (!isClick) return; 

        // Eat ȿ���� ���
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.eatSound);

        // currentChoiceSkill�� null�̸� ��ȭ�� �����Ѵ�. 
        if (currentChoiceSkill == null)
        {
            GameManager.Instance.BaalFlesh = 50;

            // �÷��̾� ���� ���� & ���� �ð� ����
            GameManager.Instance.CinemachineTarget.enabled = true;
            PlayerController.Instance.IsInterActive = false;
            PlayerController.Instance.enabled = true; 
            Time.timeScale = 1f;

            CursorManager.Instance.ChangeCursor(0);
            gameObject.SetActive(false);
        }
        else
        {
            var player = GameManager.Instance.player;

            // 1. ��ȭ 
            if (player.SkillSystem.ContainsInownskills(currentChoiceSkill.Skill))
            {
                var skill = player.SkillSystem.FindOwnSkill(currentChoiceSkill.Skill);
                player.SkillSystem.SkillLevelUp(skill);
            }
            // 2. ȹ�� or ���� 
            else
                currentChoiceSkill.AcquireSkill(player);

            // ��ų �κ��丮 UI ���� 
            skillInventory.SetActive(true);

            gameObject.SetActive(false);
        }
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

        if (rerollCount < 0)
            return;

        var skillChoices = GameManager.Instance.SetSkillChoices();
        SetUpSkillChoices(skillChoices);

        for (int i = 0; i < 4; i++)
            choiceSkillSlots[i].HighLightImage.gameObject.SetActive(false);

        reRollText.text = rerollCount.ToString();

        skillDescription.EmptySkillDescription();
        specificSkillDescription.EmptySkillDescription();
        skillTree.gameObject.SetActive(false);
        CursorManager.Instance.ChangeCursor(0);
    }

    private void ShowSkillDescription()
    {
        if (!isClick) return;

        // ��ų �� ���� ���߱� 
        specificSkillDescription.EmptySkillDescription();
        // ��ų ���� ���� 
        skillDescription.SetupDescription(CurrentChoiceSkill);
    }

    private void ShowSpecificDescription()
    {
        if (CurrentChoiceSkill == null) return;

        // ���� ��ų ���� ���߱� 
        skillDescription.EmptySkillDescription();
        // ��ų �� ���� ���� ���� 
        specificSkillDescription.SetupDescription(CurrentChoiceSkill);
    }
}