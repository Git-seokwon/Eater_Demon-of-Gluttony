using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    // �� UnityEvent : Unity���� �����ϴ� �̺�Ʈ �ý��� �� System.Action �Ǵ� System.Delegate�� ����� ����
    //               : UnityEditor�� �ν����Ϳ��� ���� �̺�Ʈ �����ʸ� ����� �� �ִٴ� ���� Ư¡
    // ���̵� ȿ���� ������ �� ȣ���ϰ� ���� �޼ҵ带 ���, ȣ���ϴ� �̺�Ʈ Ŭ����
    [System.Serializable]
    private class FadeEvent : UnityEvent { }            // 
    private FadeEvent onFadeEvent = new FadeEvent();    // 

    [SerializeField]
    private AnimationCurve fadeCurve;       // ���̵� ȿ���� ����Ǵ� ���� ���� ��� ������ ����
                                            // �� ��� ������� ���� �����ϴ� ����� ����
                                            //    0 ~ 1 ������ �Է� ���� ������� ��� Ư�� ���������� ��� ���� ������ �� �ִ�.
    private Image fadeImage;		        // ���̵� ȿ���� ���Ǵ� ���� ���� �̹���

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public void FadeIn(UnityAction action, float fadeTime)
    {
        StartCoroutine(Fade(action, 1, 0, fadeTime));
    }

    public void FadeOut(UnityAction action, float fadeTime)
    {
        StartCoroutine(Fade(action, 0, 1, fadeTime));
    }

    private IEnumerator Fade(UnityAction action, float start, float end, float fadeTime)
    {
        // action �޼ҵ带 �̺�Ʈ�� ���
        // �� UnityEvent�� UnityAction Ÿ���� �޼��带 ����Ͽ�, Ư�� ������ �����Ǹ� ����
        onFadeEvent.AddListener(action);

        float current = 0.0f;
        float percent = 0.0f; // 0 ~ 1 ������ ������, ���̵� ���൵�� ��Ÿ����. 

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            Color color = fadeImage.color;
            // �� fadeCurve.Evaluate(percent) : percent�� �ش��ϴ� AnimationCurve�� ���� �����´�. 
            color.a = Mathf.Lerp(start, end, fadeCurve.Evaluate(percent));
            fadeImage.color = color;

            yield return null;
        }

        // action �޼ҵ带 ����
        onFadeEvent.Invoke();

        // action �޼ҵ带 �̺�Ʈ���� ����
        onFadeEvent.RemoveListener(action);
    }
}
