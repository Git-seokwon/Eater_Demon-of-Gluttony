using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    // ※ UnityEvent : Unity에서 제공하는 이벤트 시스템 → System.Action 또는 System.Delegate와 비슷한 역할
    //               : UnityEditor의 인스펙터에서 직접 이벤트 리스너를 등록할 수 있다는 점이 특징
    // 페이드 효과가 끝났을 때 호출하고 싶은 메소드를 등록, 호출하는 이벤트 클래스
    [System.Serializable]
    private class FadeEvent : UnityEvent { }            // 
    private FadeEvent onFadeEvent = new FadeEvent();    // 

    [SerializeField]
    [Range(0.01f, 10f)]
    private float fadeTime;                 // 페이드 되는 시간
    [SerializeField]
    private AnimationCurve fadeCurve;       // 페이드 효과가 적용되는 알파 값을 곡선의 값으로 설정
                                            // → 곡선을 기반으로 값을 보간하는 기능을 제공
                                            //    0 ~ 1 사이의 입력 값을 기반으로 곡선의 특정 시점에서의 출력 값을 가져올 수 있다.
    private Image fadeImage;		        // 페이드 효과에 사용되는 검은 바탕 이미지

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public void FadeIn(UnityAction action)
    {
        StartCoroutine(Fade(action, 1, 0));
    }

    public void FadeOut(UnityAction action)
    {
        StartCoroutine(Fade(action, 0, 1));
    }

    private IEnumerator Fade(UnityAction action, float start, float end)
    {
        // action 메소드를 이벤트에 등록
        // → UnityEvent에 UnityAction 타입의 메서드를 등록하여, 특정 조건이 충족되면 실행
        onFadeEvent.AddListener(action);

        float current = 0.0f;
        float percent = 0.0f; // 0 ~ 1 사이의 값으로, 페이드 진행도를 나타낸다. 

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            Color color = fadeImage.color;
            // ※ fadeCurve.Evaluate(percent) : percent에 해당하는 AnimationCurve의 값을 가져온다. 
            color.a = Mathf.Lerp(start, end, fadeCurve.Evaluate(percent));
            fadeImage.color = color;

            yield return null;
        }

        // action 메소드를 실행
        onFadeEvent.Invoke();

        // action 메소드를 이벤트에서 제거
        onFadeEvent.RemoveListener(action);
    }
}
