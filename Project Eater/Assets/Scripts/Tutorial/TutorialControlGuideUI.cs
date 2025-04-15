using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControlGuideUI : TutorialBase
{
    [SerializeField]
    private VisibleObject[] objects;
    [SerializeField]
    private Button exitButton;

    private bool isClick;

    public override void Enter()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;
        PlayerController.Instance.IsInterActive = true;
        Time.timeScale = 0f;

        for (int i = 0; i < objects.Length; ++i)
        {
            objects[i].visibleObject.SetActive(objects[i].visible);
        }

        exitButton.onClick.AddListener(ExitButton);
    }

    public override void Execute(TutorialController controller)
    {
        if (isClick)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        for (int i = 0; i < objects.Length; ++i)
        {
            objects[i].visibleObject.SetActive(false);
        }

        exitButton.onClick.RemoveAllListeners();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        Time.timeScale = 1f;
    }

    private void ExitButton() => isClick = true;
}
