using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QTask/Action/NegativeCount", fileName = "Negative Count")]

public class NegativeCount : QTaskAction
{
    public override int Run(QTask task, int currentSuccess, int successCount)
    {
        return successCount < 0 ? currentSuccess - successCount : currentSuccess;
    }
}
