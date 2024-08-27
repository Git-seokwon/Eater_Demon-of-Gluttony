using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageRoom_", menuName = "Scriptable Object/Room")]
public class StageRoomTemplateSO : ScriptableObject
{
    // 방 prefab
    public GameObject prefab;

    // 나중에 추가 
    #region HEADER ROOM MUSIC
    // [Space(10)]
    // [Header("ROOM MUSIC")]
    #endregion

    #region ENEMY SPAWN
    public Transform[] spawnPositionArray;
    #endregion
}
