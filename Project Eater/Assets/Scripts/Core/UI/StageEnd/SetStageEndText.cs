using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetStageEndText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stageName_UI;
    [SerializeField]
    private TextMeshProUGUI stageResult_UI;
    [SerializeField]
    private TextMeshProUGUI stageBaalFlesh_UI;
    [SerializeField]
    private TextMeshProUGUI stageClearCount_UI;
    [SerializeField]
    private TextMeshProUGUI stageKillCount_UI;
    [SerializeField]
    private TextMeshProUGUI stageEvolutionCount_UI;

    private string stageName;
    private string stageResult;
    private int getBaalFlesh;
    private int clearCount;
    private int killCount;
    private int evolutionCount;

    private void OnEnable()
    {
        SetText();
        DisPlayText();
    }

    private void SetText()
    {
        var stageManager = StageManager.Instance;

        stageName = stageManager.CurrentStage.DisplayName;
        stageResult = stageManager.IsClear ? "Clear" : "Fail";
        getBaalFlesh = stageManager.GetBaalFlesh;

        clearCount = stageManager.CurrentStage.ClearCount;

        killCount = stageManager.KillCount;
        // 플레이어 level은 1부터 시작하기 때문에 -1을 하여 진화 횟수를 구한다.
        evolutionCount = GameManager.Instance.playerLevel - 1;
    }

    private void DisPlayText()
    {
        stageName_UI.text = stageName;
        stageResult_UI.text = stageResult;
        stageBaalFlesh_UI.text = "획득한 바알의 살점\n" + getBaalFlesh.ToString();
        stageClearCount_UI.text = "총 클리어 횟수\n" + clearCount.ToString();
        stageKillCount_UI.text = "먹어치운 실험체 수\n" + killCount.ToString();
        stageEvolutionCount_UI.text = "진화 횟수\n" + evolutionCount.ToString();
    }
}
