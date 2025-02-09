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
        // �÷��̾� level�� 1���� �����ϱ� ������ -1�� �Ͽ� ��ȭ Ƚ���� ���Ѵ�.
        evolutionCount = GameManager.Instance.playerLevel - 1;
    }

    private void DisPlayText()
    {
        stageName_UI.text = stageName;
        stageResult_UI.text = stageResult;
        stageBaalFlesh_UI.text = "ȹ���� �پ��� ����\n" + getBaalFlesh.ToString();
        stageClearCount_UI.text = "�� Ŭ���� Ƚ��\n" + clearCount.ToString();
        stageKillCount_UI.text = "�Ծ�ġ�� ����ü ��\n" + killCount.ToString();
        stageEvolutionCount_UI.text = "��ȭ Ƚ��\n" + evolutionCount.ToString();
    }
}
