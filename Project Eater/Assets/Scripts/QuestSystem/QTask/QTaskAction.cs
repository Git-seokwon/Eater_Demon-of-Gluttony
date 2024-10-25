using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class QTaskAction : ScriptableObject
{
    public abstract int Run(QTask task, int currentSuccess, int successCount);
}
