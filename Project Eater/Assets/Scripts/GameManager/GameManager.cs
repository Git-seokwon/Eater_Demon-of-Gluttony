using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Event
    public delegate void ValueChangedHandler(int currentValue, int prevValue);

    public event ValueChangedHandler onBaalFleshValueChanged;
    public event ValueChangedHandler onBaalGreatShardValueChanged;
    #endregion

    [field: SerializeField]
    public PlayerEntity player { get; private set; }
    [field: SerializeField]
    public Baal baal { get; private set; }
    [field: SerializeField]
    public Sigma sigma { get; private set; }
    [field: SerializeField]
    public Charles charles { get; private set; }

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    #region Monster DNA
    [HideInInspector]
    public HashSet<int> hasMonsterDNA = new HashSet<int>();
    [HideInInspector]
    public HashSet<int> hasLatentSkill = new HashSet<int>();

    public void RecordDNADropped(int DNA) => hasMonsterDNA.Add(DNA);  
    public bool isHasDNA(int DNA) => hasMonsterDNA.Contains(DNA);
    public void RecordLatentSkillDropped(int index) => hasLatentSkill.Add(index);
    public bool isHasLatentSkill(int index) => hasLatentSkill.Contains(index);
    #endregion

    #region FadeIn
    [Space(10)]
    [SerializeField]
    private TextMeshProUGUI messageTextTMP;
    [SerializeField]
    private CanvasGroup canvasGroup;
    #endregion

    #region ����ġ
    // �÷��̾� ����
    public int playerLevel { get; private set; }
    // �÷��̾� ����ġ 
    [SerializeField]
    private Stat expStat;
    private int nextExp;
    #endregion

    #region ��ȭ
    private int baalFlesh;
    public int BaalFlesh 
    { 
        get => baalFlesh;
        set
        {
            if (value < 0) // ���� ó��: �Ҹ�
            {
                if (baalFlesh + value < 0) // ���� �Ҹ� ��, ���� ��ȭ���� ũ�� return
                    return;
            }

            int prevBaalFlesh = baalFlesh;
            baalFlesh += value;

            onBaalFleshValueChanged?.Invoke(baalFlesh, prevBaalFlesh);
        }
    }

    private int baal_GreatShard;
    public int Baal_GreatShard
    {
        get => baal_GreatShard;
        set
        {
            if (value < 0) // ���� ó��: �Ҹ�
            {
                if (baal_GreatShard + value < 0) // ���� �Ҹ� ��, ���� ��ȭ���� ũ�� return
                    return;
            }

            int prevBaal_GreatShard = baal_GreatShard;
            baal_GreatShard += value;

            onBaalGreatShardValueChanged?.Invoke(baal_GreatShard, prevBaal_GreatShard);
        }
    }

    // UI ������Ʈ
    public void OnValueChanged() => onBaalFleshValueChanged?.Invoke(baalFlesh, baalFlesh);
    #endregion

    #region ��ų ����
    [SerializeField]
    private int skillChoices = 4;
    [SerializeField]
    private SkillChoices skillChoiceUI;
    #endregion

    #region ��ų �κ��丮 
    [Space(10)]
    [SerializeField]
    private List<EquipSlot> equipActiveSlots;
    [SerializeField]
    private List<EquipSlot> equipPassiveSlots;
    public List<EquipSlot> EquipActiveSlots => equipActiveSlots;
    public List<EquipSlot> EquipPassiveSlots => equipPassiveSlots;
    #endregion

    #region Ÿ�� ��ų 
    [SerializeField]
    private CinemachineTarget cinemachineTarget;
    public CinemachineTarget CinemachineTarget => cinemachineTarget;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    public Vector2 GetPlayerPosition()
    {
        return player.transform.position;
    }

    // �÷��̾� ���� �� ����ġ �ʱ�ȭ : ���� ����
    public void InitializePlayer()
    {
        playerLevel = 1;
        nextExp = Mathf.FloorToInt(player.Stats.ExpStat.MaxValue);
        player.Stats.ExpStat.onValueMax += LevelUp;
    }

    // �÷��̾� ���� �� ����ġ �ʱ�ȭ : ���� ���� 
    public void FinalizePlayer()
    {
        playerLevel = 1;
        player.Stats.ExpStat.MaxValue = 20;
        player.Stats.SetDefaultValue(player.Stats.ExpStat, 0);
        player.Stats.ExpStat.onValueMax -= LevelUp;
    }

    public void GetExp(bool isElite)
    {
        if (isElite)
            player.Stats.IncreaseDefaultValue(expStat, Settings.eliteEXP);
        else
            player.Stats.IncreaseDefaultValue(expStat, 6);
    }

    private void LevelUp(Stat stat, float currentValue, float prevValue)
    {
        playerLevel++;
        int prevNextExp = nextExp;

        // ���Ŀ� ���� nextExp�� ���� �����Ѵ�. 
        nextExp = CalculateEXP(playerLevel);
        // ExpStat�� ������ Copy�̱� ������ player.Stats���� �����ؾ� �Ѵ�. 
        player.Stats.ExpStat.MaxValue = nextExp;

        player.Stats.IncreaseDefaultValue(expStat, -prevNextExp);

        // �÷��̾� ���� �� ���� �ð� ���� 
        player.PlayerMovement.Stop();
        CinemachineTarget.enabled = false;
        PlayerController.Instance.IsInterActive = true;
        PlayerController.Instance.enabled = false;
        Time.timeScale = 0f;

        // ��ų Setting
        var skillChoices = SetSkillChoices();

        // ��ų UI Setting �� UI Ȱ��ȭ 
        skillChoiceUI.SetUpSkillChoices(skillChoices);
        skillChoiceUI.SetUpReRollButton(player);
        skillChoiceUI.gameObject.SetActive(true);
    }

    private int CalculateEXP(int playerLevel)
    {
        if (playerLevel < 30)
            return 18 + 2 * playerLevel;
        else
            return Mathf.CeilToInt(0.03f * (playerLevel * playerLevel)) + 50;
    }

    public List<SkillCombinationSlotNode> SetSkillChoices()
    {
        int remainSkillChoices = skillChoices;
        List<SkillCombinationSlotNode> skills = new();

        int skillCombinationCount = player.SkillSystem.CombinableSkills.Count;
        int skillUpgradeCount = player.SkillSystem.UpgradableSkills.Count;
        int skillAcquisitionCount = player.SkillSystem.AcquirableSkills.Count;

        int skillCombinationChoices = 0;
        int skillUpgradeChoices = 0;
        int skillAcquisitionChoices = 0;

        // ��ų ������ ��ü�� ������ ���, Ex) ȹ�� 1, ��ȭ 1, ���� 1�� ������ ��, 
        // ������ 1���� �������� ��ȭ(goods)�� �ִ� �������� ó���Ѵ�. 
        // �� �ش� ��쿡�� while ���� �� �ʿ䰡 ���� ������ ������ �������� �� ������ List�� Count�� �ʱ�ȭ �����ش�. 
        if (skillCombinationCount + skillUpgradeCount + skillAcquisitionCount <= skillChoices)
        {
            skillCombinationChoices = skillCombinationCount;
            skillUpgradeChoices = skillUpgradeCount;
            skillAcquisitionChoices = skillAcquisitionCount;
        }
        else
        {
            // �÷��̾��� ������ 90�̻��� ���, �پ��� ������ �ִ� ������ 1���� ������ ���Խ�Ų��. 
            if (playerLevel >= 90)
                remainSkillChoices--;

            while (remainSkillChoices > 0)
            {
                // ������ �迭 �� �ϳ��� �����ϰ� ����
                int randomSelection;
                int weightedRandom = Random.Range(0, 7);

                // 4/7�� Ȯ��
                if (weightedRandom < 4)
                    randomSelection = 0;
                // 2/7�� Ȯ��
                else if (weightedRandom < 6)
                    randomSelection = 1;
                // 1/7�� Ȯ��
                else
                    randomSelection = 2;

                // ���� ������ ��ų ���� 
                // �� skillCombinationCount�� 0���� Ŭ���� �̹� ���õ� ���� skillCombinationCount�� �Ѿ�� ���̻� ���� �� ����.
                // Ex) skillCombinationCount : 2, skillCombinationChoices : 2 �� ��� 
                int skillCount = 0;
                switch (randomSelection)
                {
                    case 0:
                        skillCount = skillCombinationCount - skillCombinationChoices;
                        break;
                    case 1:
                        skillCount = skillUpgradeCount - skillUpgradeChoices;
                        break;
                    case 2:
                        skillCount = skillAcquisitionCount - skillAcquisitionChoices;
                        break;
                }

                // ���� ������ ��ų�� �����ϴ� ��� 
                if (skillCount > 0)
                {
                    int choices;
                    remainSkillChoices = CalculateChoices(remainSkillChoices, skillCount, out choices);

                    // ���� �������� �ش� �������� �߰�
                    switch (randomSelection)
                    {
                        case 0:
                            skillCombinationChoices += choices;
                            break;
                        case 1:
                            skillUpgradeChoices += choices;
                            break;
                        case 2:
                            skillAcquisitionChoices += choices;
                            break;
                    }
                }

                // ��� �������� �����ϸ� �ݺ��� ����
                if (remainSkillChoices <= 0) break;
            }
        }

        // ���� ��ų�� List�� �߰��Ѵ�. 
        PopulateSkillsList(skillCombinationChoices, player.SkillSystem.CombinableSkills, skills);
        PopulateSkillsList(skillUpgradeChoices, player.SkillSystem.UpgradableSkills, skills);
        PopulateSkillsList(skillAcquisitionChoices, player.SkillSystem.AcquirableSkills, skills);

        // �����ϰ� ���õ� ��ų + ���� ��ȭ ������(null) ���� ��ȯ
        return skills;
    }

    private void PopulateSkillsList(int skillChoices, IReadOnlyList<SkillCombinationSlotNode> skills, 
        List<SkillCombinationSlotNode> options)
    {
        if (skillChoices == 0) return;

        // HashSet���� �����ϰ� �ߺ� ������ ���� �Ѵ�. 
        HashSet<int> selectedIndices = new HashSet<int>();
        int currentIndex;

        while (selectedIndices.Count < skillChoices)
        {
            currentIndex = Random.Range(0, skills.Count);
            if (!selectedIndices.Contains(currentIndex))
            {
                selectedIndices.Add(currentIndex);
                options.Add(skills[currentIndex]);
            }
        }
    }

    // ������ ��(choices)�� ���� ���� ������ ��(remainChoices)�� ��ȯ�ϴ� �Լ� 
    private int CalculateChoices(int remainChoices, int skillCount, out int choices)
    {
        // �ּ� 1�� ~ �ִ� skillCount(��ü ��ų �׸� �� - �̹� ���õ� ��ų �׸� ��) or ���� �ִ� ������ ��
        // ���� �������� ������ ���� ���� ��ȯ
        // �� ���� ������ �ִ� ��ų �� ���� ���� �������� �����ϴ� ���� �����ϱ� �����̴�. 
        // �� ���� ������ �ִ� ��ų ���� ���� ������ ������ ���� ��, ������ �߻����� �ʱ� �����̴�.
        choices = Random.Range(1, Mathf.Min(remainChoices, skillCount) + 1);
        remainChoices -= choices;
        return remainChoices;
    }

    public void StartDisplayStageNameText() => StartCoroutine(DisplayStageNameText());

    public void StartDisplayStageExitText() => StartCoroutine(DisplayStageExitText());

    // Fade Canvas Group
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        Image image= canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0f;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
    }

    private IEnumerator DisplayStageNameText()
    {
        // Set Screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        string messageText = "STAGE " + StageManager.Instance.CurrentStage.CodeName + "\n\n" +
            StageManager.Instance.CurrentStage.DisplayName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        PlayerController.Instance.enabled = true;

        // Fade In
        yield return StartCoroutine(Fade(1f, 0f, 1f, Color.black));
    }

    private IEnumerator DisplayStageExitText()
    {
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        string messageText = "�����ҷ� ���ƿ��� ��...";

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        // Fade In
        yield return StartCoroutine(Fade(1f, 0f, 1f, Color.black));
    }

    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        // display the message for the given time
        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                yield return null;  
            }
        }
        // else display the message until the return button is pressed
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        messageTextTMP.SetText("");
    }
}
