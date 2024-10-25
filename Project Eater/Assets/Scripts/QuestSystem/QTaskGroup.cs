using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum QTaskGroupState // TaskGroup 전체의 state이다. QTaskState와 별개임
{
    Inactive,
    Running,
    Complete
}

[System.Serializable]

public class QTaskGroup
{
    [SerializeField] private QTask[] tasks;

    public IReadOnlyList<QTask> Tasks => tasks;
    public Quest Owner { get; private set; }
    public bool IsAllTaskComplete => tasks.All(x => x.IsComplete);
    public bool IsComplete => State == QTaskGroupState.Complete;

    public QTaskGroupState State { get; private set; }

    public QTaskGroup(QTaskGroup copyTarget) // 다른 TaskGroup을 복사하는 생성자
    {
        tasks = copyTarget.Tasks.Select(x => Object.Instantiate(x)).ToArray();
    }

    public void Setup(Quest owner)
    {
        Owner = owner;
        foreach(var task in tasks)
            task.Setup(owner);
    }

    public void Start()
    {
        State = QTaskGroupState.Running;
        foreach(var task in tasks)
            task.Start();
    }

    public void End()
    {
        State = QTaskGroupState.Complete;
        foreach (var task in tasks)
            task.End();
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        foreach(var task in tasks)
        {
            if(task.IsTarget(category, target))
                task.ReceiveReport(successCount);
        }
    }

    public void Complete()
    {
        if (IsComplete)
            return;

        State = QTaskGroupState.Complete;
        foreach(var task in tasks)
        {
            if(!task.IsComplete)
                task.Complete();
        }
    }
}
