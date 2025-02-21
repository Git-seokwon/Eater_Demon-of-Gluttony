using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFadeEffect : TutorialBase
{
    [SerializeField]
    private FadeEffect fadeEffect;
    [SerializeField]
    private bool isFadeIn = false;
    [SerializeField]
    [Range(0.01f, 10f)]
    private float fadeTime;                 // 페이드 되는 시간

    private bool isCompleted = false;

    public override void Enter()
    {
        if (isFadeIn == true)
        {
            fadeEffect.FadeIn(OnAfterFadeEffect, fadeTime);
        }
        else
        {
            fadeEffect.FadeOut(OnAfterFadeEffect, fadeTime);
        }
    }

    private void OnAfterFadeEffect()
    {
        isCompleted = true;
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
