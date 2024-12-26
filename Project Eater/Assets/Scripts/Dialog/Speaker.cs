using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Speaker", menuName = "Dialog/Speaker")]
public class Speaker : ScriptableObject
{
    public Sprite spriteRenderer;            // 캐릭터 이미지
    public string textName;        // 현재 대사중인 캐릭터 이름 출력 Text UI
}
