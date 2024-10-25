using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QTask/Action/PositiveCount", fileName = "Positive Count")]
public class PositiveCount : QTaskAction
{
    public override int Run(QTask task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : currentSuccess;
    }
}
