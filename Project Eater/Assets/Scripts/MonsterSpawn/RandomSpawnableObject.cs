using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    // 몬스터 및 low Ratio, High Ratio를 저장하는 구조체
    private struct chanceBoundaries
    {
        public T spawnableObject; // Enemy
        public int lowBoundaryValue; // low Ratio - 낮은 비율
        public int highBoundaryValue; // High Ratio - 높은 비율 
    }

    // 전체 비율 : 마지막 몬스터의 highBoundaryValue + 1 (PDF 참조)
    private int ratioValueTotal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectsByWave<T>> spawnableObjectsByWaveList;

    // Constructor - 생성자 : spawnableObjectsByWaveList 받아서 할당함 
    public RandomSpawnableObject(List<SpawnableObjectsByWave<T>> spawnableObjectsByWaveList)
    {
        this.spawnableObjectsByWaveList = spawnableObjectsByWaveList;
    }

    public T GetItem()
    {
        // 변수 초기화 
        int upperBoundary = -1; // highBoundaryValue가 되는 값 
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        // 반환되는 몬스터
        // ※ default(T) : T가 value type인 경우는 0을 reference type인 경우에는 null을 반환
        T spawnableObject = default(T);

        // List<SpawnableObjectsByWave<T>> spawnableObjectsByWaveList부터 순회 시작 
        // → 현재 스테이지 웨이브와 같은 class 찾기
        foreach (SpawnableObjectsByWave<T> spawnableObjectsByWave in spawnableObjectsByWaveList)
        {
            // check for current wave
            if (spawnableObjectsByWave.stageWave == StageManager.Instance.GetCurrentStageWave())
            {
                // 찾았으면 spawnableObjectsByWaveList 속 List<SpawnableObjectRatio<T>> spawnableObjectRatioList 순회 
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectsByWave.spawnableObjectRatioList)
                {
                    // 처음 실행하는 경우 lowerBoundary == 0
                    // -> 이후 upperBoundary + 1부터 갱신됨 
                    int lowerBoundary = upperBoundary + 1;

                    // -1을 해준 이유는 lowerBoundary가 0부터 시작하기 때문
                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;

                    // List에 있는 spawnableObjectRatio.ratio 만큼 더해서 ratioValueTotal 계산
                    ratioValueTotal += spawnableObjectRatio.ratio;

                    // 계산이 끝났으면 chanceBoundariesList에 Add하기
                    // -> PDF 참고!
                    chanceBoundariesList.Add(new chanceBoundaries()
                    {
                        spawnableObject = spawnableObjectRatio.stageEnemyObject,
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        // chanceBoundariesList가 0이면 생성할 몬스터가 없다는 뜻 -> null 리턴 
        if (chanceBoundariesList.Count == 0)
        {
            return default(T);
        }

        // ex) Enemy 1의 lowBoundaryValue 0, highBoundaryValue가 4라고 하자
        // Random.Range(0, ratioValueTotal); 를 실행하여 0 ~ 4의 값이 나오면 Enemy 1이 반환된다. 
        //   ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        int lookUpValue = Random.Range(0, ratioValueTotal);

        // loop through list to get selected random spawnable object details
        // → chanceBoundariesList를 순회하면서 lowBoundaryValue와 highBoundaryValue 사이에 lookUpValue가 있는지를 체크해서
        //    해당하는 chanceBoundaries의 spawnableObject는 반환하기
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
