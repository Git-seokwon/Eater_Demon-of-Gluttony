using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaalFlesh : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI baalFlesh;

    private void Start()
    {
        // Event 등록 
        GameManager.Instance.onBaalFleshValueChanged += UpdateBaalFlesh; // 재화(바알의 살점) 수치 변화 Event 등록

        // 바알의 살점 UI 업데이트 
        GameManager.Instance.OnValueChanged();
    }

    private void UpdateBaalFlesh(int currentValue, int prevValue)
        => baalFlesh.text = "x " + currentValue.ToString();
}
