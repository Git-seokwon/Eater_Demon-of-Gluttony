using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum QTaskState
{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName ="Quest/QTask/QTask", fileName ="QTask_")]

public class QTask : ScriptableObject
{
    #region Events
    public delegate void StateChangedHandler(QTask task, QTaskState state, QTaskState prevState);
    public delegate void SuccessChangedHandler(QTask task, int currentSuccess, int prevSuccess);
    #endregion

    [Header("Category")]
    [SerializeField] private QCategory category;

    [Header("Text")]
    [SerializeField] private string codeName;
    [SerializeField] private string description;

    [Header("Action")]
    [SerializeField] QTaskAction action;

    [Header("Target")]
    [SerializeField] private QTaskTarget[] targets;

    [Header("Setting")]
    [SerializeField] private InitialSuccessValue initialSuccessValue;
    [SerializeField] private int needSuccessToComplete;
    [SerializeField] private bool canReceiveReportsDuringCompletion;
    // Complete ���¿��� ����Ʈ ������ �������� ���ϴ� ����(Running)�� ��ȯ�� �� �ִ����� ���� ����.

    private QTaskState state; 
    private int currentSuccess;

    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    public int CurrentSuccess
    {
        get => currentSuccess;
        set
        {
            int prevSuccess = currentSuccess;
            currentSuccess = Mathf.Clamp(value, 0, needSuccessToComplete);
            if(currentSuccess != prevSuccess)
            {
                State = currentSuccess == needSuccessToComplete ? QTaskState.Complete : QTaskState.Running;
                // Debug.Log($"call log - {this.codeName} : state changed - {this.state}");
                onSuccessChanged?.Invoke(this, currentSuccess, prevSuccess);
            }
        }
    }

    public QCategory Category => category;
    public string CodeName => codeName;
    public string Description => description;
    public int NeedSuccessToComplete => needSuccessToComplete;

    public QTaskState State
    {
        get => state;
        set
        {
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
        } 
    }

    public bool IsComplete => State == QTaskState.Complete;
    public Quest Owner { get; private set; }

    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    public void Start()
    {
        State = QTaskState.Running;
        if(initialSuccessValue)
            currentSuccess = initialSuccessValue.GetValue(this);
    }

    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;
    }

    public void ReceiveReport(int successCount)
    {
        //Debug.Log($"QTask - ReceiveReport - {this.codeName}, {this.state}");
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    public void Complete()
    {
        CurrentSuccess = needSuccessToComplete;
    }

    public bool IsTarget(string category, object target)
        => this.Category == category && targets.Any(x => x.IsEqual(target)) && (!IsComplete || (IsComplete && canReceiveReportsDuringCompletion));
    // Ÿ���� �´��� Ȯ���ϴ� �޼���
    // 1. category�� �´��� Ȯ��,
    // 2. target�� �ִ� target�� �������� Ȯ��
    // 3. �Ϸ���� �ʾҰų�, �Ϸ�Ǿ�����, �Ϸᰡ ��ҵ� �� �ִ� ������� Ȯ��.
}
