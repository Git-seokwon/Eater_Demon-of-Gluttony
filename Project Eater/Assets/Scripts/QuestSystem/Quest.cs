using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;
using System.Linq;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Diagnostics;

public enum QuestState // ����Ʈ�� ���¸� ��Ÿ���� Enum
{
    Inactive,
    Running,
    Complete,
    Cancel,
    WaitingForCompletion
} // watingForCompletion�� ������ Complete�� ������ ��ٸ��� ������

[CreateAssetMenu(menuName ="Quest/Quest", fileName ="Quest_")]
public class Quest : ScriptableObject
{
    // �븮�ڵ�
    #region Events
    public delegate void TaskSuccessChangeHandler(Quest quest, QTask task, int currentSuccess, int prevSuccess);
    public delegate void CompletedHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);
    public delegate void NewQTaskGroupHandler(Quest quest, QTaskGroup currentTaskGroup, QTaskGroup prevTaskGroup);
    #endregion

    [SerializeField] private QCategory category;
    [SerializeField] private Sprite icon;

    [Header("Text")]
    [SerializeField] private string codeName;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;

    [Header("Reward")]
    [SerializeField] private QReward[] rewards; // ����Ʈ�� ����
    [SerializeField] private bool isRewardGiven; // ���� ���� ���� 25.2. 9 �߰�.

    [Header("Task")]
    [SerializeField] private QTaskGroup[] taskGroups; // ����Ʈ���� �ؾ��ϴ� �ϵ��� �׷� ex) ��Ȳ���� 3���� ��� + �Ķ����� 3���� ��� 

    [Header("Option")]
    [SerializeField] private bool useAutoComplete;
    [SerializeField] private bool isCancelable;


    [Header("Condition")] // ����Ʈ�� �����ϰų� ����� �� �ִ� ������ -> Ŭ�����ϴ� ������ �ƴ�.
    [SerializeField] private QCondition[] acceptionConditions; // �������� (����Ʈ �������� ����)
    [SerializeField] private QCondition[] cancelConditions; // �������
    [SerializeField] private bool isSavable; // ��ȸ�� ����Ʈ ���� ���, ���̺� �� �ʿ䰡 ����

    private int currentTaskGroupIndex; // ���� �����ϰ� �ִ� TaskGroup�� �ε���

    #region Property
    public QCategory Category => category;
    public Sprite Icon => icon;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    public string Description => description;
    public QuestState State { get; private set; }
    public QTaskGroup CurrentTaskGroup => taskGroups[currentTaskGroupIndex];
    public IReadOnlyList<QTaskGroup> TaskGroups => taskGroups;
    public IReadOnlyList<QReward> Rewards => rewards;
    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsCompletable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCancelable => isCancelable && cancelConditions.All(x => x.IsPass(this));
    public bool IsAcceptable => acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => isSavable;
    public bool IsRewardGiven => isRewardGiven;
    #endregion

    public event TaskSuccessChangeHandler onTaskSuccessChanged;
    public event CompletedHandler onCompleted;
    public event CanceledHandler onCanceled;
    public event NewQTaskGroupHandler onNewQTaskGroup;

    public void OnRegister() // Quest�� system�� ��ϵǾ��� �� awake�Լ��μ� �۵��ϴ� �޼���
    {
        Debug.Assert(!IsRegistered, $"Quest - OnRegistered - �̹� ��ϵ� ����Ʈ ��ü�Դϴ�. : {this.CodeName}");

        foreach(var taskGroup in taskGroups )
        {
            taskGroup.Setup(this);
            foreach (var task in taskGroup.Tasks)
                task.onSuccessChanged += OnSuccessChanged;
        }

        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        // Debug.Log($"ReceiveReport({category}, {target}, {successCount}) / {State}"); // ������.

        Debug.Assert(IsRegistered, $"Quest -  ReceiveReport - �̹� ��ϵ� ����Ʈ ��ü�Դϴ�. : {this.CodeName}");
        Debug.Assert(!IsCancel, $"��ҵ� ����Ʈ�Դϴ�. : {this.CodeName}");

        if (IsComplete)
            return;

        CurrentTaskGroup.ReceiveReport(category, target, successCount);

        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            if (currentTaskGroupIndex + 1 == taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;
                if (useAutoComplete)
                    Complete();
            }
            else
            {
                var prevTaskGroup = taskGroups[currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                onNewQTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        else
            State = QuestState.Running;
    }

    public void Complete()
    {
        CheckIsRunning();

        foreach(var taskgroup in taskGroups)
            taskgroup.Complete();

        State = QuestState.Complete;

        /* 250214 ����. => Give ȣ�� ���ϰ� Give ���ϵ���
        foreach (var reward in rewards)
            reward.Give(this);
        */

        onCompleted?.Invoke(this);

        onTaskSuccessChanged = null;
        onCompleted = null;
        onCanceled = null;
        onNewQTaskGroup = null;
    }

    public virtual void Cancel()
    {
        CheckIsRunning();
        Debug.Assert(IsCancelable, $"�� ����Ʈ�� ��ҵ� �� �����ϴ�. : {this.CodeName}");

        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }

    public Quest Clone()
    {
        var clone = Instantiate(this);
        clone.taskGroups = taskGroups.Select(x => new QTaskGroup(x)).ToArray();

        return clone;
    }


    public QuestSaveData ToSaveData()
    {
        return new QuestSaveData
        {
            codeName = codeName,
            state = State,
            taskGroupIndex = currentTaskGroupIndex,
            taskSuccessCounts = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray(),
            isRewardGiven = isRewardGiven
        };
    }

    public void LoadFrom(QuestSaveData saveData)
    {
        State = saveData.state;
        currentTaskGroupIndex = saveData.taskGroupIndex;
        isRewardGiven = saveData.isRewardGiven;

        for (int i = 0; i < currentTaskGroupIndex; i++)
        {
            var taskGroup = taskGroups[i];
            taskGroup.Start();
            taskGroup.Complete();
        }
        for (int i = 0; i < saveData.taskSuccessCounts.Length; i++)
        {
            CurrentTaskGroup.Start();
            CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.taskSuccessCounts[i];
        }
    }


    private void OnSuccessChanged(QTask task, int currentSuccess, int prevSuccess)
        => onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);

    [Conditional("UNITY_EDITOR")]
    public void CheckIsRunning()
    {
        Debug.Assert(IsRegistered, "�̹� ��ϵ� ����Ʈ�Դϴ�. : ");
        Debug.Assert(!IsCancel, "��ҵ� ����Ʈ�Դϴ�. : ");
        Debug.Assert(!IsComplete, "�̹� �Ϸ�� ����Ʈ�Դϴ�. : ");
    }

    public void SetIsReward(bool val) => isRewardGiven = val;
}
