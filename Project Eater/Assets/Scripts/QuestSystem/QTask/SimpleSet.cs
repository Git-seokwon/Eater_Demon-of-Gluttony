using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QTask/Action/SimpleSet", fileName = "Simple Set")]
public class SimpleSet : QTaskAction
{
    public override int Run(QTask task, int currentSuccess, int successCount)
    {
        return successCount;
    }
}
