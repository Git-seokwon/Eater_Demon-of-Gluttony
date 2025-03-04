using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1BossPreSpawnEffect : BossPreSpawnEffect
{
    [SerializeField]
    private Animator animator;

    public override void PlayEffect()
    {
        animator.SetBool("IsActive", true);
    }

    public override void BossSpawn()
    {
        PoolManager.Instance.ReuseGameObject(StageManager.Instance.CurrentStage.StageBoss, transform.position, Quaternion.identity);
    }
}
