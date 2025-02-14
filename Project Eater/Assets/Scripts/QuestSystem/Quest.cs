using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;
using System.Linq;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Diagnostics;

public enum QuestState // 퀘스트의 상태를 나타내줄 Enum
{
    Inactive,
    Running,
    Complete,
    Cancel,
    WaitingForCompletion
} // watingForCompletion은 유저가 Complete를 누르길 기다리는 상태임

[CreateAssetMenu(menuName ="Quest/Quest", fileName ="Quest_")]
public class Quest : ScriptableObject
{
    // 대리자들
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
    [SerializeField] private QReward[] rewards; // 퀘스트의 보상
    [SerializeField] private bool isRewardGiven; // 보상 수령 여부 25.2. 9 추가.

    [Header("Task")]
    [SerializeField] private QTaskGroup[] taskGroups; // 퀘스트에서 해야하는 일들의 그룹 ex) 주황버섯 3마리 잡기 + 파랑버섯 3마리 잡기 

    [Header("Option")]
    [SerializeField] private bool useAutoComplete;
    [SerializeField] private bool isCancelable;


    [Header("Condition")] // 퀘스트를 수락하거나 취소할 수 있는 조건임 -> 클리어하는 조건이 아님.
    [SerializeField] private QCondition[] acceptionConditions; // 수락조건 (퀘스트 수락가능 조건)
    [SerializeField] private QCondition[] cancelConditions; // 취소조건
    [SerializeField] private bool isSavable; // 일회용 퀘스트 같은 경우, 세이브 할 필요가 없음

    private int currentTaskGroupIndex; // 현재 수행하고 있는 TaskGroup의 인덱스

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

    public void OnRegister() // Quest가 system에 등록되었을 때 awake함수로서 작동하는 메서드
    {
        Debug.Assert(!IsRegistered, $"Quest - OnRegistered - 이미 등록된 퀘스트 객체입니다. : {this.CodeName}");

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
        // Debug.Log($"ReceiveReport({category}, {target}, {successCount}) / {State}"); // 디버깅용.

        Debug.Assert(IsRegistered, $"Quest -  ReceiveReport - 이미 등록된 퀘스트 객체입니다. : {this.CodeName}");
        Debug.Assert(!IsCancel, $"취소된 퀘스트입니다. : {this.CodeName}");

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

        /* 250214 수정. => Give 호출 안하고 Give 안하도록
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
        Debug.Assert(IsCancelable, $"이 퀘스트는 취소될 수 없습니다. : {this.CodeName}");

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
        Debug.Assert(IsRegistered, "이미 등록된 퀘스트입니다. : ");
        Debug.Assert(!IsCancel, "취소된 퀘스트입니다. : ");
        Debug.Assert(!IsComplete, "이미 완료된 퀘스트입니다. : ");
    }

    public void SetIsReward(bool val) => isRewardGiven = val;
}
