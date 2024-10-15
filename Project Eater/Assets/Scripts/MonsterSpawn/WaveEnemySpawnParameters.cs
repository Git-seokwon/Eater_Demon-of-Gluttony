using UnityEngine;

// 적 생성 관련된 정보를 담는 class
[System.Serializable]
public class WaveEnemySpawnParameters
{
    #region Tooltip
    [Tooltip("Defines the stage level for this room with regard to how many enemies in total should be spawned")]
    #endregion
    public int stageWave;

    #region Tooltip
    [Tooltip("The minimum number of enemies to spawn in this room for this dungeon level. The actual number will be a random value between " +
        "the minimum and maximum values")]
    #endregion
    public int minTotalEnemiesToSpawn;

    #region Tooltip
    [Tooltip("The maximum number of enemies to spawn in this room for this dungeon level. The actual number will be a random value between " +
        "the minimum and maximum values")]
    #endregion
    public int maxTotalEnemiesToSpawn;

    // 한 번에 생성되는 enemy 수
    #region Tooltip
    [Tooltip("The minimum number of concurrent enemies to spawn in this room for this dungeon level. The actual number will be a random value" +
        "between the minimum and maximum values")]
    #endregion
    public int minConcurrentEnemies;

    #region Tooltip
    [Tooltip("The maximum number of concurrent enemies to spawn in this room for this dungeon level. The actual number will be a random value" +
        "between the minimum and maximum values")]
    #endregion
    public int maxConcurrentEnemies;

    #region Tooltip
    [Tooltip("The minimum spawn interval in seconds for enemies in this room for this dungeon level. The actual number will be a random value " +
        "between the minimum and maximum values.")]
    #endregion
    public int minSpawnInterval;

    #region Tooltip
    [Tooltip("The maximum spawn interval in seconds for enemies in this room for this dungeon level. The actual number will be a random value " +
        "between the minimum and maximum values.")]
    #endregion
    public int maxSpawnInterval;
}
