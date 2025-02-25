using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpriteFader : TutorialBase
{
    [SerializeField] private SpriteRenderer targetSprite;
    [SerializeField] private float fadeDuration = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float targetAlpha = 0.5f; // 목표 알파 값
    [SerializeField]
    private bool isActive = true;

    private bool isCompleted = false;

    public override void Enter()
    {
        if (targetSprite != null)
        {
            StartCoroutine(FadeSpriteAlpha(targetSprite, targetAlpha, fadeDuration));
        }
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        targetSprite.gameObject.SetActive(isActive);
    }

    private IEnumerator FadeSpriteAlpha(SpriteRenderer sprite, float targetAlpha, float duration)
    {
        float startAlpha = sprite.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
            yield return null;
        }

        // 최종 값 보정
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, targetAlpha);

        isCompleted = true;
    }
}
