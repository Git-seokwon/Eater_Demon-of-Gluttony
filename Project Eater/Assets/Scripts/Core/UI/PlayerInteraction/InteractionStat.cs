using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionStat", menuName = "PlayerInteraction/Stat")]
public class InteractionStat : InteractionPrefab
{
    public override void DoAction()
    {
        try
        {
            GameObject.Find("UI").transform.Find("Stat_Upgrade").gameObject.SetActive(true);
        }
        catch
        {
            Debug.Log("Stat_Upgrade�� ã�� �� �����ϴ�.");
        }
    }

    public override void ConditionCheck()
    {
        condition = true;
    }
}
