using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillInventoryTutorial : MonoBehaviour
{
    public int isTutorialClear = 0; // 0 : false, 1 : true

    [SerializeField] private GameObject tutorialFadeImage;

    [SerializeField] private Image tutorialImage;

    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button closeButton;

    [SerializeField] TextMeshProUGUI activeText;
    [SerializeField] TextMeshProUGUI passiveText;

    private void OnEnable()
    {
        if (isTutorialClear == 0)
        {
            // 튜토리얼 이미지 활성화 
            tutorialFadeImage.SetActive(true);
            gameObject.SetActive(true);

            // 튜토 이미지 할당
            tutorialImage.sprite = GameResources.Instance.skillInventoryTutorialImage[0];

            // 버튼 활성화 & 비활성화
            rightButton.gameObject.SetActive(true);
            leftButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            // 텍스트 활성화 & 비활성화
            activeText.gameObject.SetActive(true);
            passiveText.gameObject.SetActive(false);

            // 버튼 이벤트 등록
            closeButton.onClick.AddListener(Close);
            rightButton.onClick.AddListener(RightButton);
            leftButton.onClick.AddListener(LeftButton);
        }
        else
        {
            // 튜토리얼 이미지 비활성화 
            tutorialFadeImage.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void RightButton()
    {
        closeButton.gameObject.SetActive(true);

        tutorialImage.sprite = GameResources.Instance.skillInventoryTutorialImage[1];

        activeText.gameObject.SetActive(false);
        passiveText.gameObject.SetActive(true);

        leftButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(false);
    }

    private void LeftButton()
    {
        closeButton.gameObject.SetActive(false);

        tutorialImage.sprite = GameResources.Instance.skillInventoryTutorialImage[0];

        activeText.gameObject.SetActive(true);
        passiveText.gameObject.SetActive(false);

        rightButton.gameObject.SetActive(true);
        leftButton.gameObject.SetActive(false);
    }

    private void Close()
    {
        isTutorialClear = 1;

        tutorialImage.sprite = null;

        closeButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();

        // 튜토리얼 이미지 비활성화 
        tutorialFadeImage.SetActive(false);
        gameObject.SetActive(false);
    }
}
