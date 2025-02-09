using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName ="RewardBaal", menuName ="Quest/QReward/RewardBaal")]
public class RewardBaal : QReward
{
    public override void Give(Quest quest)
    {
        GameManager.Instance.BaalFlesh += 1;
    }
}
