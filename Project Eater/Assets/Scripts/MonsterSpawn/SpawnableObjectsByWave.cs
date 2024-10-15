using System.Collections.Generic;

// class를 직렬화를 해야 List<SpawnableObjectsByLevel<T>>가 인스펙터 창에 뜬다 
[System.Serializable]
public class SpawnableObjectsByWave<T>
{
    public int stageWave;

    public List<SpawnableObjectRatio<T>> spawnableObjectRatioList;
}
