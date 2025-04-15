 using UnityEngine;

public class MonsterIndicatorManager : SingletonMonobehaviour<MonsterIndicatorManager>
{
    [SerializeField] MonsterIndicator monsterIndicator;
    [SerializeField] RectTransform cameraRect;

    public bool isTimeLimit;
    private bool indicatorActive = false;

    private void Init()
    {
        monsterIndicator.TurnOff();
    }

    private void Start()
    {
        Init();
    }

    private void LateUpdate()
    {
        if(StageManager.Instance.SpawnedEnemyList.Count <= 0 && !indicatorActive)
            return;

        if((StageManager.Instance.SpawnedEnemyList.Count > 10 || !isTimeLimit) && indicatorActive)
        {
            monsterIndicator.TurnOff();
            indicatorActive = false;
        }

        if (isTimeLimit && (StageManager.Instance.SpawnedEnemyList.Count <= 10))
        {
            Transform target = FindNearMonsterDist();
            monsterIndicator.Rotate(target);

            bool inCam = CheckMonsterInCam(target);

            if (!inCam && !indicatorActive)
            {
                monsterIndicator.TurnOn();
                indicatorActive = true;
            }
            else if(inCam && indicatorActive)
            {
                monsterIndicator.TurnOff();
                indicatorActive = false;
            }
        }
    }

    private Transform FindNearMonsterDist()
    {
        if (StageManager.Instance.SpawnedEnemyList == null || StageManager.Instance.SpawnedEnemyList.Count == 0) 
            return null;

        Transform nearest = null;
        float mindist = float.MaxValue;

        foreach (var monster in StageManager.Instance.SpawnedEnemyList) {
            float dist = Vector2.SqrMagnitude(monster.transform.position - gameObject.transform.position);

            if (dist < mindist)
            {
                mindist = dist;
                nearest = monster.transform;
            }
        }
        return nearest;
    }

    private bool CheckMonsterInCam(Transform target)
    {
        if (target == null) return true;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.position);

        // 화면 앞에 있고 (z > 0), 좌우상하 모두 (0~1) 범위 안이면 카메라에 보임
        bool isVisible =
            viewportPos.z > 0 &&
            viewportPos.x >= 0f && viewportPos.x <= 1f &&
            viewportPos.y >= 0f && viewportPos.y <= 1f;

        return isVisible;
    }
}
