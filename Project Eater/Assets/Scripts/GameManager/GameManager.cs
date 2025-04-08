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

    #region 경험치
    // 플레이어 레벨
    public int playerLevel { get; private set; }
    // 플레이어 경험치 
    [SerializeField]
    private Stat expStat;
    private int nextExp;
    #endregion

    #region 재화
    private int baalFlesh;
    public int BaalFlesh 
    { 
        get => baalFlesh;
        set
        {
            if (value < 0) // 음수 처리: 소모
            {
                if (baalFlesh + value < 0) // 음수 소모 시, 현재 재화보다 크면 return
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
            if (value < 0) // 음수 처리: 소모
            {
                if (baal_GreatShard + value < 0) // 음수 소모 시, 현재 재화보다 크면 return
                    return;
            }

            int prevBaal_GreatShard = baal_GreatShard;
            baal_GreatShard += value;

            onBaalGreatShardValueChanged?.Invoke(baal_GreatShard, prevBaal_GreatShard);
        }
    }

    // UI 업데이트
    public void OnValueChanged() => onBaalFleshValueChanged?.Invoke(baalFlesh, baalFlesh);
    #endregion

    #region 스킬 선택
    [SerializeField]
    private int skillChoices = 4;
    [SerializeField]
    private SkillChoices skillChoiceUI;
    #endregion

    #region 스킬 인벤토리 
    [Space(10)]
    [SerializeField]
    private List<EquipSlot> equipActiveSlots;
    [SerializeField]
    private List<EquipSlot> equipPassiveSlots;
    public List<EquipSlot> EquipActiveSlots => equipActiveSlots;
    public List<EquipSlot> EquipPassiveSlots => equipPassiveSlots;
    #endregion

    #region 타겟 스킬 
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

    // 플레이어 레벨 및 경험치 초기화 : 전투 진입
    public void InitializePlayer()
    {
        playerLevel = 1;
        nextExp = Mathf.FloorToInt(player.Stats.ExpStat.MaxValue);
        player.Stats.ExpStat.onValueMax += LevelUp;
    }

    // 플레이어 레벨 및 경험치 초기화 : 전투 종료 
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

        // 공식에 의해 nextExp의 값을 갱신한다. 
        nextExp = CalculateEXP(playerLevel);
        // ExpStat은 원본의 Copy이기 때문에 player.Stats으로 접근해야 한다. 
        player.Stats.ExpStat.MaxValue = nextExp;

        player.Stats.IncreaseDefaultValue(expStat, -prevNextExp);

        // 플레이어 정지 및 게임 시간 정지 
        player.PlayerMovement.Stop();
        CinemachineTarget.enabled = false;
        PlayerController.Instance.IsInterActive = true;
        PlayerController.Instance.enabled = false;
        Time.timeScale = 0f;

        // 스킬 Setting
        var skillChoices = SetSkillChoices();

        // 스킬 UI Setting 및 UI 활성화 
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

        // 스킬 선택지 자체가 부족한 경우, Ex) 획득 1, 강화 1, 조합 1만 가능할 때, 
        // 나머지 1개의 선택지를 재화(goods)를 주는 선택지로 처리한다. 
        // → 해당 경우에는 while 문을 돌 필요가 없기 때문에 선택지 종류들을 각 선택지 List의 Count로 초기화 시켜준다. 
        if (skillCombinationCount + skillUpgradeCount + skillAcquisitionCount <= skillChoices)
        {
            skillCombinationChoices = skillCombinationCount;
            skillUpgradeChoices = skillUpgradeCount;
            skillAcquisitionChoices = skillAcquisitionCount;
        }
        else
        {
            // 플레이어의 레벨이 90이상인 경우, 바알의 살점을 주는 선택지 1개를 무조건 포함시킨다. 
            if (playerLevel >= 90)
                remainSkillChoices--;

            while (remainSkillChoices > 0)
            {
                // 선택지 배열 중 하나를 랜덤하게 선택
                int randomSelection;
                int weightedRandom = Random.Range(0, 7);

                // 4/7의 확률
                if (weightedRandom < 4)
                    randomSelection = 0;
                // 2/7의 확률
                else if (weightedRandom < 6)
                    randomSelection = 1;
                // 1/7의 확률
                else
                    randomSelection = 2;

                // 선택 가능한 스킬 개수 
                // → skillCombinationCount이 0보다 클지라도 이미 선택된 수가 skillCombinationCount를 넘어가면 더이상 뽑을 수 없다.
                // Ex) skillCombinationCount : 2, skillCombinationChoices : 2 인 경우 
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

                // 선택 가능한 스킬이 존재하는 경우 
                if (skillCount > 0)
                {
                    int choices;
                    remainSkillChoices = CalculateChoices(remainSkillChoices, skillCount, out choices);

                    // 남은 선택지를 해당 선택지에 추가
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

                // 모든 선택지를 소진하면 반복문 종료
                if (remainSkillChoices <= 0) break;
            }
        }

        // 최종 스킬을 List에 추가한다. 
        PopulateSkillsList(skillCombinationChoices, player.SkillSystem.CombinableSkills, skills);
        PopulateSkillsList(skillUpgradeChoices, player.SkillSystem.UpgradableSkills, skills);
        PopulateSkillsList(skillAcquisitionChoices, player.SkillSystem.AcquirableSkills, skills);

        // 랜덤하게 선택된 스킬 + 무료 재화 선택지(null) 수를 반환
        return skills;
    }

    private void PopulateSkillsList(int skillChoices, IReadOnlyList<SkillCombinationSlotNode> skills, 
        List<SkillCombinationSlotNode> options)
    {
        if (skillChoices == 0) return;

        // HashSet으로 간단하게 중복 선택을 방지 한다. 
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

    // 선택지 수(choices)를 고르고 남은 선택지 수(remainChoices)를 반환하는 함수 
    private int CalculateChoices(int remainChoices, int skillCount, out int choices)
    {
        // 최소 1개 ~ 최대 skillCount(전체 스킬 항목 수 - 이미 선택된 스킬 항목 수) or 남아 있는 선택지 수
        // 에서 랜덤으로 선택지 수를 고르고 반환
        // → 현재 가지고 있는 스킬 수 보다 많은 선택지를 선택하는 것을 방지하기 위함이다. 
        // → 현재 가지고 있는 스킬 수가 남은 선택지 수보다 작을 때, 오류가 발생하지 않기 위함이다.
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

        string messageText = "연구소로 돌아오는 중...";

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
