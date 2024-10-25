using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QTask/Action/SimpleCount", fileName = "Simple Count")]
public class SimpleCount : QTaskAction
{
    public override int Run(QTask task, int currentSuccess, int successCount)
    {
        return currentSuccess + successCount;
    }
}
