using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillInfo
{
    [SerializeField]
    private int tier;
    [SerializeField]
    private int index;

    public int Tier => tier;
    public int Index => index;  
}

public class MonsterDNA : MonoBehaviour
{
    [SerializeField]
    private SkillInfo[] skillInfos; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            var player = collision.GetComponent<PlayerEntity>();

            foreach (var skillInfo in skillInfos)
                player.SkillSystem.AddAcquirableSkills(skillInfo.Tier, skillInfo.Index);
        }
    }
}
