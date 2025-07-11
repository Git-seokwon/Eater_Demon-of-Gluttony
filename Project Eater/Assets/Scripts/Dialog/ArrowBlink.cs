using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowBlink : MonoBehaviour
{
    [SerializeField]
    private float fadeTime;     // 페이드 되는 시간
    private Image fadeImage;    // 페이드 효과에 사용되는 Image UI

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        // Fade 효과를 In → Out 무한 반복한다. 
        StartCoroutine("FadeInOut");
    }

    private void OnDisable()
    {
        StopCoroutine("FadeInOut");
    }

    private IEnumerator FadeInOut()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(1, 0));    // Fade In

            yield return StartCoroutine(Fade(0, 1));    // Fade Out
        }
    }

    // 이미지의 alpha 값을 fadeTime 시간동안 start에서 end까지 변화시킨다.
    private IEnumerator Fade(int start, int end)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(start, end, percent);
            fadeImage.color = color;

            yield return null;
        }
    }
}
