using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBlink : MonoBehaviour
{
    [SerializeField]
    private float fadeTime;     // 페이드 되는 시간
    private TextMeshProUGUI fadeText;    // 페이드 효과에 사용되는 Image UI

    private void Awake()
    {
        fadeText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine("FadeInOut");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FadeInOut()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(1,0)); // Fade In

            yield return StartCoroutine(Fade(0,1)); // Fade Out
        }
    }

    private IEnumerator Fade(int start, int end)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.unscaledDeltaTime;
            percent = current / fadeTime;

            Color color = fadeText.color;
            color.a = Mathf.Lerp(start, end, percent);
            fadeText.color = color;

            yield return null;
        }
    }
}
