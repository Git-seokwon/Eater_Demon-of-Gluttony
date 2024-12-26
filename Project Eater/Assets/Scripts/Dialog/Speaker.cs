using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Speaker", menuName = "Dialog/Speaker")]
public class Speaker : ScriptableObject
{
    public Sprite spriteRenderer;            // ĳ���� �̹���
    public string textName;        // ���� ������� ĳ���� �̸� ��� Text UI
}
