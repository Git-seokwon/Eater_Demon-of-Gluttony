using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �� Skill�� �����Ű�� Projectile�� Spawn �ϴ� Module
[System.Serializable]
public class SpawnProjectileAction : SkillAction
{
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private string spawnPointSocketName; // Projectile�� Spawn�� ��ġ 
    [SerializeField]
    private float speed;

    public override void Apply(Skill skill)
    {
        // Projectile�� Spawn�� ��ġ�� �����´�. 
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        var projectile = PoolManager.Instance.ReuseGameObject(projectilePrefab, socket.position, Quaternion.identity);

        // Projectile�� Socket �������� ���콺 �������� ���ư��� Entity���� ������ Skill�� ȿ���� ����
        projectile.GetComponent<Projectile>().Setup(skill.Owner, speed, HelperUtilities.GetMouseWorldPosition(), skill);
    }

    public override object Clone()
    {
        return new SpawnProjectileAction()
        {
            projectilePrefab = projectilePrefab,
            spawnPointSocketName = spawnPointSocketName,
            speed = speed
        };
    }
}
