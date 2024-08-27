using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 명중 시 Skill을 적용시키는 Projectile을 Spawn 하는 Module
[System.Serializable]
public class SpawnProjectileAction : SkillAction
{
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private string spawnPointSocketName; // Projectile이 Spawn될 위치 
    [SerializeField]
    private float speed;

    public override void Apply(Skill skill)
    {
        // Projectile을 Spawn할 위치를 가져온다. 
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        var projectile = PoolManager.Instance.ReuseGameObject(projectilePrefab, socket.position, Quaternion.identity);

        // Projectile이 Socket 기준으로 마우스 방향으로 날아가서 Entity에게 맞으면 Skill의 효과를 적용
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
