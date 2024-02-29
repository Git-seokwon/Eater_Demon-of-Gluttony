using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MainRoom_", menuName = "Scriptable Object/Room")]
public class MainRoomTemplateSO : ScriptableObject
{
    // 방 prefab
    public GameObject prefab;

    // 나중에 추가 
    #region HEADER ROOM MUSIC
    // [Space(10)]
    // [Header("ROOM MUSIC")]
    #endregion

}
