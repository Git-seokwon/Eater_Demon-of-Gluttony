using System.Collections.Generic;

// class�� ����ȭ�� �ؾ� List<SpawnableObjectsByLevel<T>>�� �ν����� â�� ��� 
[System.Serializable]
public class SpawnableObjectsByWave<T>
{
    public int stageWave;

    public List<SpawnableObjectRatio<T>> spawnableObjectRatioList;
}
