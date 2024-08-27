using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageRoom_", menuName = "Scriptable Object/Room")]
public class StageRoomTemplateSO : ScriptableObject
{
    // �� prefab
    public GameObject prefab;

    // ���߿� �߰� 
    #region HEADER ROOM MUSIC
    // [Space(10)]
    // [Header("ROOM MUSIC")]
    #endregion

    #region ENEMY SPAWN
    public Transform[] spawnPositionArray;
    #endregion
}
