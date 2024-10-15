using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageRoom_", menuName = "Scriptable Object/StageRoom")]
public class StageRoomTemplateSO : ScriptableObject
{
    // 방 prefab
    public GameObject stageRoom;

    // 나중에 추가 
    #region HEADER ROOM MUSIC
    // [Space(10)]
    // [Header("ROOM MUSIC")]
    #endregion

    #region ENEMY SPAWN
    public Vector3[] spawnPositionArray;
    #endregion

    #region Header ENEMY DETAILS
    [Space(10)]
    [Header("ENEMY DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Populate the list with all the enemies that can be spawned in this room by wave, including the ratio (random) of this " +
        "enemy type that will be spawned")]
    #endregion
    public List<SpawnableObjectsByWave<GameObject>> enemiesByWaveList;

    #region Tooltip
    [Tooltip("Populate the list with the spawn parameters for the enemies")]
    #endregion
    public List<WaveEnemySpawnParameters> waveEnemySpawnParametersList;

    #region Tolltip
    [Tooltip("Populate the field boss and stage boss that can be spawned in this room in specific wave")]
    #endregion
    public GameObject stageBoss;
}