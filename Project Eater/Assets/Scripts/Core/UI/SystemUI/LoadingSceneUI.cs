using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneUI : MonoBehaviour
{
    static string nextScene;

    [SerializeField] Image progressBar;
    [SerializeField] TextMeshProUGUI loadingText;

    private string loading = "Loading";

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }


    private void Start()
    {
        loadingText.text = "Loading";
        StartCoroutine(LoadSceneProcess());

        MusicManager.Instance.StopMusic(0f);
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        float dottimer = 0f;
        int dotCount = 0;

        while(!op.isDone)
        {
            yield return null;

            dottimer += Time.deltaTime;

            if (dottimer >= 0.5f)
            {
                dottimer = 0f;
                dotCount = (dotCount + 1) % 4;
                loadingText.text = loading + new string('.', dotCount);
            }

            if(op.progress < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime/2;
                
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if(progressBar.fillAmount >= 1f)
                {
                    SaveSystem.Instance.Save();
                    //CursorManager.Instance.ChangeCursor(0);
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
