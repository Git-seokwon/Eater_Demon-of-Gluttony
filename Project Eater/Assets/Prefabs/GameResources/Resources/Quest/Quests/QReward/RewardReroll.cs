using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardReroll", menuName = "Quest/QReward/RewardReroll")]
public class RewardReroll : QReward
{
    public override void Give(Quest quest)
    {
        GameManager.Instance.player.Stats.ReRollStat.DefaultValue += Quantity;
    }
}
