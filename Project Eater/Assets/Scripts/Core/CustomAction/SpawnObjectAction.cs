using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObjectAction : CustomAction
{
    // 언제 Object를 Spawn 시킬 것인가? 
    // 1) Start 함수
    // 2) Run 함수 
    private enum MethodType { Start, Run }

    // 누구를 기준으로 Object를 Spawn할 것인가? 
    // 1) OwnerOrUser : SpawnObjectAction을 실행시킨 객체 (Skill Or User)
    // 2) Target      : Skill의 Target (Entity or Position)
    private enum TargetType { OwnerOrUser, Target }

    [SerializeField]
    private TargetType targetType;
    [SerializeField]
    private MethodType methodType;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Vector3 offset;

    // Spawn할 Object의 크기를 조절할 때 사용
    [SerializeField]
    private Vector2 scaleFactor = Vector2.one;
    // Spawn한 Object를 기준점의 자식 Trasform으로 만들지 여부 
    [SerializeField]
    private bool isAttachToTarget;
    // Action이 Release 될 때, Spawn한 Object를 Destroy할 지 여부 
    [SerializeField]
    private bool isDestroyOnRelease;

    private GameObject spawnedObject;

    public override void Start(object data)
    {
        if (methodType == MethodType.Start)
            Spawn(data);
    }

    public override void Run(object data)
    {
        if (methodType == MethodType.Run)
            Spawn(data);
    }

    public override void Release(object data)
    {
        if (spawnedObject && isDestroyOnRelease)
        {
            spawnedObject.SetActive(false);
            spawnedObject = null;
        }
    }

    // 실제 Object Spawn 함수 
    private GameObject Spawn(Vector3 position)
    {
        spawnedObject = PoolManager.Instance.ReuseGameObject(prefab, position + offset, Quaternion.identity);
        var localScale = spawnedObject.transform.localScale;
        // ※ Vector3.Scale : 두 벡터의 각 요소들을 곱하는 메서드 
        spawnedObject.transform.localScale = Vector3.Scale(localScale, scaleFactor);

        return spawnedObject;
    }

    // Transform을 통해 실제 Object를 Spawn하는 함수 
    private void Spawn(Transform transform)
    {
        GameObject spawnedObject = Spawn(transform.position);
        if (isAttachToTarget)
            spawnedObject.transform.SetParent(transform);
    }

    // data가 Skill인 경우 Spawn Overloading 함수 
    private void Spawn(Skill data)
    {
        // ※ Owner : Skill을 사용한 Entity 
        if (targetType == TargetType.OwnerOrUser)
            Spawn(data.Owner.transform);
        else
            Spawn(data.TargetSelectionResult.selectedPosition);
    }

    // data가 Effect인 경우 Spawn Overloading 함수 
    private void Spawn(Effect data)
    {
        // ※ User : Effect를 사용한 Entity 
        var targetTransform = targetType == TargetType.OwnerOrUser ? data.User.transform : data.Target.transform;
        Spawn(targetTransform);
    }

    // 인자로 받은 data에 Type에 따라 Spawn Overloading 함수 실행 
    private void Spawn(object data)
    {
        if (data is Skill)
            Spawn(data as Skill);
        else
            Spawn(data as Effect);
    }

    public override object Clone()
    {
        return new SpawnObjectAction()
        {
            isAttachToTarget = isAttachToTarget,
            isDestroyOnRelease = isDestroyOnRelease,
            methodType = methodType,
            targetType = targetType,
            offset = offset,
            prefab = prefab,
            scaleFactor = scaleFactor
        };
    }
}
