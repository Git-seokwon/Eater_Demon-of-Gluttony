using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : IdentifiedObject
{
    private const int kInfinity = 0;

    #region Events
    // Skill�� Apply �Ǹ� ����Ǵ� Event
    // �� currentApplyCount : ���� Apply Ƚ�� 
    public delegate void AppliedHandler(Skill skill, int currentApplyCount);
    // currentApplyCount�� ���� �ٲ�� ����Ǵ� Event
    public delegate void CurrentApplyCountChangedHandler(Skill skill, int currentApplyCount, int prevApplyCount);
    #endregion

    [SerializeField]
    private MovementInSkill movement;

    [SerializeField]
    private NeedSelectionResultType needSelectionResultType;

    // ���� Level�� �����Ǵ� SkillData
    private SkillData currentData;

    // ��ġ ���� current ������
    private int currentApplyCount; // ���� ��ų ���� Ƚ�� 
    private float currentDuration; // ���� ��ų�� ���� �ð�

    // ��ų ���� Entity
    public Entity Owner { get; private set; }
    public StateMachine<Skill> StateMachine { get; private set; }

    public MovementInSkill Movement => movement;

    // �� Effects
    // �� Skill�� Level�� ������ ��, Level�� �ش��ϴ� SkillData�� �����ͼ� SkillData�� ���� EffectSelector�� ���� ������
    //    Effect���� Setting�ϴ� Property
    public IReadOnlyList<Effect> Effects {  get; private set; } = Array.Empty<Effect>();

    // ���� SkillData�� InSkillActionFinishOption�� ��ȯ
    public InSkillActionFinishOption InSkillActionFinishOption => currentData.inSkillActionFinishOption;

    public TargetSearcher TargetSearcher => currentData.targetSearcher;
    public bool IsSearchingTarget => TargetSearcher.IsSearching;
    public TargetSelectionResult TargetSelectionResult => TargetSearcher.SelectionResult;
    public TargetSearchResult TargetSearchResult => TargetSearcher.SearchResult;

    // Skill�� �ʿ�� �ϴ� ������ Type�� TargetSearcher�� �˻��� �������� Type�� ��ġ�ϴ°�? 
    public bool HasValidTargetSelectionResult
    {
        get
        {
            // switch �� Ȱ�� 
            // �� TargetSelectionResult.resultMessage�� ���� ���� �ٸ� ������ �� 
            return TargetSelectionResult.resultMessage switch
            {
                // case ���� 1 : SearchResultMessage�� FindTarget�� ��, needSelectionResultType�� Target�̿��� �Ѵ�.
                SearchResultMessage.FindTarget => needSelectionResultType == NeedSelectionResultType.Target,
                // case ���� 2 : SearchResultMessage�� FindPosition�� ��, needSelectionResultType�� Position�̿��� �Ѵ�. 
                SearchResultMessage.FindPosition => needSelectionResultType == NeedSelectionResultType.Position,
                // default ���� : false 
                _ => false
            };
        }
    }

    // �� Target Select�� �����ߴ��� ���� 
    // �� ��ų�� ������ �˻� ���� �ƴϰ�, �˻��� �������� ��ų�� �ʿ�� �ϴ� Type�̶�� True
    public bool IsTargetSelectSuccessful => !IsSearchingTarget && HasValidTargetSelectionResult;

    // �� Duration Property
    public float Duration => currentData.duration;

    // ��ų�� ���ӽð��� �����ΰ�? 
    private bool IsTimeless => Mathf.Approximately(Duration, kInfinity);

    public float CurrentDuration
    {
        get => currentDuration;
        set => currentDuration = !IsTimeless ? Mathf.Clamp(value, 0f, Duration) : value;
    }

    public int ApplyCount => currentData.applyCount;
    private bool IsInfinitelyApplicable => ApplyCount == kInfinity;
    public int CurrentApplyCount
    {
        get => currentApplyCount;
        set
        {
            if (currentApplyCount == value)
                return;

            var prevApplyCount = currentApplyCount;
            currentApplyCount = Mathf.Clamp(value, 0, ApplyCount);

            onCurrentApplyCountChanged?.Invoke(this, currentApplyCount, prevApplyCount);
        }
    }

    // �� Apply Cycle Property
    // 1)
    // currentData�� applyCycle�� 0�̰� applyCount�� 1���� ũ�� (������ ���� �����ϸ�)
    // Skill�� duration�� (ApplyCount - 1)�� ������ ApplyCycle�� ����Ͽ� return ��.
    // �� ApplyCount - 1 : ó�� �� ���� ����ǰ� �����ϱ� ������ 1���� ���ش�. 
    // 2)
    // �ƴ϶�� ������ currentData�� applyCycle�� �״�� return ��.
    public float ApplyCycle
        => Mathf.Approximately(currentData.applyCycle, 0f) && ApplyCount > 1
        ? Duration / (ApplyCount - 1)
        : currentData.applyCycle;

    // CurrentApplyCycle ���� ApplyCycle�� �����ϸ� Skill�� �ߵ�
    public float CurrentApplyCycle { get; set; }

    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount == ApplyCount;

    // Skill�� �ߵ��� ����Ǿ��°�? 
    public bool IsFinished => currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded
        ? IsDurationEnded
        : IsApplyCompleted;

    #region Event ������
    public event AppliedHandler onApplied;
    public event CurrentApplyCountChangedHandler onCurrentApplyCountChanged;
    #endregion

    // Skill Ȱ��ȭ �Լ� 
    public void Activate()
    {

    }

    // Skill�� T �������� Ȯ���ϴ� �Լ� 
    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();
    public bool IsInState<T>(int layer) where T : State<Skill> => StateMachine.IsInState<T>(layer);
}
