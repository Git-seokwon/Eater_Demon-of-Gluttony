using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QTask/Action/ContinuousCount", fileName = "Continuous Count")]
public class ContinuousCount : QTaskAction
{
    public override int Run(QTask task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : 0;
    }
}
