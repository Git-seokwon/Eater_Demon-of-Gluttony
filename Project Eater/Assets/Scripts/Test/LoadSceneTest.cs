using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
            LoadingSceneUI.LoadScene("MainScene");
    }
}
