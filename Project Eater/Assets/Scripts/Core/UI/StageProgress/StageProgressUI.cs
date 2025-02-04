using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageProgressUI : MonoBehaviour
{
    [SerializeField]
    private GameObject progressNoticeWindow;
    [SerializeField]
    private GameObject resultWindow;

    public IEnumerator ShowProgress(float secondsToShow, string progressText)
    {
        progressNoticeWindow.SetActive(true);
        progressNoticeWindow.GetComponentInChildren<TMP_Text>().text = progressText;
        yield return new WaitForSeconds(secondsToShow);
        progressNoticeWindow.SetActive(false);
        yield break;
    }

    public IEnumerator ShowResultWindow(float secondsToShow)
    {
        StartCoroutine(GameManager.Instance.Fade(0f, 1f, secondsToShow, Color.black));
        yield return new WaitForSeconds(secondsToShow);
        StartCoroutine(GameManager.Instance.Fade(1f, 0f, 0f, Color.black));
        resultWindow.SetActive(true);
    }

    public void HideResultWindow()
    {
        resultWindow.SetActive(false);
    }
}
