using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnScaledUpSkillObjectAction : SkillAction
{
    [SerializeField]
    private GameObject skillObjectPrefab;
    [SerializeField]
    private string spawnPointSocketName;
    [SerializeField]
    private float scaleUpSpeed;

    public override void Apply(Skill skill)
    {
        // skillObjectPrefab을 Spawn할 위치를 가져온다. 
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        var skillObject = PoolManager.Instance.ReuseGameObject(skillObjectPrefab, socket.position, Quaternion.identity);

        skillObject.GetComponent<ScaledUpSkillObject>().Setup(skill.Owner, scaleUpSpeed, skill);
    }

    public override object Clone()
    {
        return new SpawnScaledUpSkillObjectAction()
        {
            skillObjectPrefab = skillObjectPrefab,
            spawnPointSocketName = spawnPointSocketName,
            scaleUpSpeed = scaleUpSpeed
        };
    }
}
