[System.Serializable]
public class SpawnableObjectRatio<T>
{
    public T stageEnemyObject; // T : Enemy Object Type    // dungeonObject -> stageObject로 변경

    public int ratio; // 몬스터 비율 (비율이 상대적으로 높을수록 해당 몬스터가 소환될 확률이 높다)
}
