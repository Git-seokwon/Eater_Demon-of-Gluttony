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
    [field: SerializeField]
    public PlayerEntity player { get; private set; }

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    #region Monster DNA
    private HashSet<string> hasMonsterDNA;

    public void RecordDNADropped(string DNA) => hasMonsterDNA.Add(DNA);
    public bool isHasDNA(string DNA) => hasMonsterDNA.Contains(DNA);
    #endregion

    #region FadeIn
    [SerializeField]
    private TextMeshProUGUI messageTextTMP;
    [SerializeField]
    private CanvasGroup canvasGroup;
    #endregion

    #region ����ġ
    // �÷��̾� ����
    public int playerLevel { get; private set; }
    // �÷��̾� ����ġ 
    private int exp;
    private float nextExp;
    #endregion

    #region ��ų ����
    [SerializeField]
    private int skillChoices = 4;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        
    }

    // �÷��̾� ���� �� ����ġ �ʱ�ȭ
    public void InitializePlayer()
    {
        playerLevel = 0;
        nextExp = Mathf.FloorToInt(player.Stats.ExpStat.Value);
    }

    public Vector2 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public void GetExp(bool isElite)
    {
        if (isElite)
            exp += Settings.eliteEXP;
        else
            exp++;

        if (exp >= nextExp)
        {
            playerLevel++;
            exp = 0;
            // TODO : ���Ŀ� ���� nextExp�� ���� �����Ѵ�. 
            // nextExp = 

            SetSkillChoices();

            // TODO : ������ UI�� Show �Ѵ�. 
        }
    }

    private void SetSkillChoices()
    {
        int remainChoices = skillChoices;
        int goodsChoices = 0;
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
            goodsChoices = skillChoices - (skillCombinationCount + skillUpgradeCount + skillAcquisitionCount);

            skillCombinationChoices = skillCombinationCount;
            skillUpgradeChoices = skillUpgradeCount;
            skillAcquisitionChoices = skillAcquisitionCount;
        }
        else
        {
            while (remainChoices > 0)
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

                if (skillCount > 0)
                {
                    int choices;
                    remainChoices = CalculateChoices(remainChoices, skillCount, out choices);

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
                if (remainChoices <= 0)
                    break;
            }
        }
    }

    private int CalculateChoices(int remainChoices, int skillCount, out int choices)
    {
        choices = Random.Range(1, Mathf.Min(remainChoices, skillCount) + 1);
        remainChoices -= choices;
        return remainChoices;
    }

    public void StartDisplayStageNameText() => StartCoroutine(DisplayStageNameText());

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
