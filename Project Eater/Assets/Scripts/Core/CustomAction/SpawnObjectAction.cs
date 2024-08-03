using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObjectAction : CustomAction
{
    // ���� Object�� Spawn ��ų ���ΰ�? 
    // 1) Start �Լ�
    // 2) Run �Լ� 
    private enum MethodType { Start, Run }

    // ������ �������� Object�� Spawn�� ���ΰ�? 
    // 1) OwnerOrUser : SpawnObjectAction�� �����Ų ��ü (Skill Or User)
    // 2) Target      : Skill�� Target (Entity or Position)
    private enum TargetType { OwnerOrUser, Target }

    [SerializeField]
    private TargetType targetType;
    [SerializeField]
    private MethodType methodType;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Vector3 offset;

    // Spawn�� Object�� ũ�⸦ ������ �� ���
    [SerializeField]
    private Vector2 scaleFactor = Vector2.one;
    // Spawn�� Object�� �������� �ڽ� Trasform���� ������ ���� 
    [SerializeField]
    private bool isAttachToTarget;
    // Action�� Release �� ��, Spawn�� Object�� Destroy�� �� ���� 
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

    // ���� Object Spawn �Լ� 
    private GameObject Spawn(Vector3 position)
    {
        spawnedObject = PoolManager.Instance.ReuseGameObject(prefab, position + offset, Quaternion.identity);
        var localScale = spawnedObject.transform.localScale;
        // �� Vector3.Scale : �� ������ �� ��ҵ��� ���ϴ� �޼��� 
        spawnedObject.transform.localScale = Vector3.Scale(localScale, scaleFactor);

        return spawnedObject;
    }

    // Transform�� ���� ���� Object�� Spawn�ϴ� �Լ� 
    private void Spawn(Transform transform)
    {
        GameObject spawnedObject = Spawn(transform.position);
        if (isAttachToTarget)
            spawnedObject.transform.SetParent(transform);
    }

    // data�� Skill�� ��� Spawn Overloading �Լ� 
    private void Spawn(Skill data)
    {
        // �� Owner : Skill�� ����� Entity 
        if (targetType == TargetType.OwnerOrUser)
            Spawn(data.Owner.transform);
        else
            Spawn(data.TargetSelectionResult.selectedPosition);
    }

    // data�� Effect�� ��� Spawn Overloading �Լ� 
    private void Spawn(Effect data)
    {
        // �� User : Effect�� ����� Entity 
        var targetTransform = targetType == TargetType.OwnerOrUser ? data.User.transform : data.Target.transform;
        Spawn(targetTransform);
    }

    // ���ڷ� ���� data�� Type�� ���� Spawn Overloading �Լ� ���� 
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
