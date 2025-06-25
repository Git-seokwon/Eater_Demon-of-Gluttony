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
            // Ʃ�丮�� �̹��� Ȱ��ȭ 
            tutorialFadeImage.SetActive(true);
            gameObject.SetActive(true);

            // Ʃ�� �̹��� �Ҵ�
            tutorialImage.sprite = GameResources.Instance.skillInventoryTutorialImage[0];

            // ��ư Ȱ��ȭ & ��Ȱ��ȭ
            rightButton.gameObject.SetActive(true);
            leftButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            // �ؽ�Ʈ Ȱ��ȭ & ��Ȱ��ȭ
            activeText.gameObject.SetActive(true);
            passiveText.gameObject.SetActive(false);

            // ��ư �̺�Ʈ ���
            closeButton.onClick.AddListener(Close);
            rightButton.onClick.AddListener(RightButton);
            leftButton.onClick.AddListener(LeftButton);
        }
        else
        {
            // Ʃ�丮�� �̹��� ��Ȱ��ȭ 
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

        // Ʃ�丮�� �̹��� ��Ȱ��ȭ 
        tutorialFadeImage.SetActive(false);
        gameObject.SetActive(false);
    }
}
