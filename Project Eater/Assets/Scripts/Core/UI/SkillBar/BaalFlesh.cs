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
        // Event ��� 
        GameManager.Instance.onBaalFleshValueChanged += UpdateBaalFlesh; // ��ȭ(�پ��� ����) ��ġ ��ȭ Event ���

        // �پ��� ���� UI ������Ʈ 
        GameManager.Instance.OnValueChanged();
    }

    private void UpdateBaalFlesh(int currentValue, int prevValue)
        => baalFlesh.text = "x " + currentValue.ToString();
}
