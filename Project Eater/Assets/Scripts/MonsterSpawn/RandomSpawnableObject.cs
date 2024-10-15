using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    // ���� �� low Ratio, High Ratio�� �����ϴ� ����ü
    private struct chanceBoundaries
    {
        public T spawnableObject; // Enemy
        public int lowBoundaryValue; // low Ratio - ���� ����
        public int highBoundaryValue; // High Ratio - ���� ���� 
    }

    // ��ü ���� : ������ ������ highBoundaryValue + 1 (PDF ����)
    private int ratioValueTotal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectsByWave<T>> spawnableObjectsByWaveList;

    // Constructor - ������ : spawnableObjectsByWaveList �޾Ƽ� �Ҵ��� 
    public RandomSpawnableObject(List<SpawnableObjectsByWave<T>> spawnableObjectsByWaveList)
    {
        this.spawnableObjectsByWaveList = spawnableObjectsByWaveList;
    }

    public T GetItem()
    {
        // ���� �ʱ�ȭ 
        int upperBoundary = -1; // highBoundaryValue�� �Ǵ� �� 
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        // ��ȯ�Ǵ� ����
        // �� default(T) : T�� value type�� ���� 0�� reference type�� ��쿡�� null�� ��ȯ
        T spawnableObject = default(T);

        // List<SpawnableObjectsByWave<T>> spawnableObjectsByWaveList���� ��ȸ ���� 
        // �� ���� �������� ���̺�� ���� class ã��
        foreach (SpawnableObjectsByWave<T> spawnableObjectsByWave in spawnableObjectsByWaveList)
        {
            // check for current wave
            if (spawnableObjectsByWave.stageWave == StageManager.Instance.GetCurrentStageWave())
            {
                // ã������ spawnableObjectsByWaveList �� List<SpawnableObjectRatio<T>> spawnableObjectRatioList ��ȸ 
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectsByWave.spawnableObjectRatioList)
                {
                    // ó�� �����ϴ� ��� lowerBoundary == 0
                    // -> ���� upperBoundary + 1���� ���ŵ� 
                    int lowerBoundary = upperBoundary + 1;

                    // -1�� ���� ������ lowerBoundary�� 0���� �����ϱ� ����
                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;

                    // List�� �ִ� spawnableObjectRatio.ratio ��ŭ ���ؼ� ratioValueTotal ���
                    ratioValueTotal += spawnableObjectRatio.ratio;

                    // ����� �������� chanceBoundariesList�� Add�ϱ�
                    // -> PDF ����!
                    chanceBoundariesList.Add(new chanceBoundaries()
                    {
                        spawnableObject = spawnableObjectRatio.stageEnemyObject,
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        // chanceBoundariesList�� 0�̸� ������ ���Ͱ� ���ٴ� �� -> null ���� 
        if (chanceBoundariesList.Count == 0)
        {
            return default(T);
        }

        // ex) Enemy 1�� lowBoundaryValue 0, highBoundaryValue�� 4��� ����
        // Random.Range(0, ratioValueTotal); �� �����Ͽ� 0 ~ 4�� ���� ������ Enemy 1�� ��ȯ�ȴ�. 
        //   �����������������������������������������
        int lookUpValue = Random.Range(0, ratioValueTotal);

        // loop through list to get selected random spawnable object details
        // �� chanceBoundariesList�� ��ȸ�ϸ鼭 lowBoundaryValue�� highBoundaryValue ���̿� lookUpValue�� �ִ����� üũ�ؼ�
        //    �ش��ϴ� chanceBoundaries�� spawnableObject�� ��ȯ�ϱ�
        foreach (chanceBoundaries spawnChance in chanceBoundariesList)
        {
            if (lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue)
            {
                spawnableObject = spawnChance.spawnableObject;
                break;
            }
        }

        Debug.Log(spawnableObject.ToString());

        return spawnableObject;
    }
}
