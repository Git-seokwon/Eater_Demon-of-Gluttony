using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageChange : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stageNumber;
    [SerializeField]
    private Image stageImage;
    [SerializeField]
    private Button rightButton;    
    [SerializeField]
    private Button leftButton;

    private Stage currentStage;
    private int currentStageIndex;
    private int maxStageIndex;

    // 스테이지 UI가 활성화 될 때는 인 게임 상황이라 Load 순서 걱정은 안해도 된다. 
    private void OnEnable()
    {
        maxStageIndex = StageManager.Instance.Stages.Count;
        ShowStageIcon();

        rightButton.onClick.AddListener(OnRightButton);
        leftButton.onClick.AddListener(OnLeftButton);
    }

    private void OnDisable()
    {
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
    }

    private void OnRightButton()
    {
        currentStageIndex = (currentStageIndex + 1) % maxStageIndex;
        ShowStageIcon();
    }

    private void OnLeftButton()
    {
        currentStageIndex = (currentStageIndex - 1) < 0 ? maxStageIndex - 1 : currentStageIndex - 1;
        ShowStageIcon();
    }

    private void ShowStageIcon()
    {
        currentStage = StageManager.Instance.Stages[currentStageIndex];
        StageManager.Instance.CurrentStage = currentStage;

        stageImage.sprite = currentStage.Icon;
        stageNumber.text = "Stage\n" + currentStageIndex + 1;
    }
}
