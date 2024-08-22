using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill : IdentifiedObject
{
    private const int kInfinity = 0;

    #region Events
    // Skill�� Level�� ������ �� ����Ǵ� Event
    public delegate void LevelChangedHandler(Skill skill, int currentLevel, int prevLevel);

    // Skill�� StateMachine�� ���� Event�� ������ Event (�׷��� �̸��� �Ű������� �Ȱ���)
    public delegate void StateChangedHandler(Skill skill, State<Skill> newState, State<Skill> prevState, int layer);

    // Skill�� Apply�Ǹ� ����Ǵ� Event
    // �� currentApplyCount : ���� Apply Ƚ�� 
    public delegate void AppliedHandler(Skill skill, int currentApplyCount);

    // Skill�� ����ϸ� ����Ǵ� Event
    // �� Skill ��ư�� ���� �� ����
    public delegate void UseHandler(Skill skill);

    // Skill�� ���(Use)�� ���� ���� ���� ���¿� ���� ����Ǵ� Event
    // �� Skill ��ư�� ������ Skill�� ������ Target�� �������� �� ���� 
    public delegate void ActivatedHandler(Skill skill);

    // Skill�� ����(Action���� ��� ������ ����ħ)�� ���� ����Ǵ� Event
    public delegate void DeactivatedHandler(Skill skill);

    // Skill ����� �����Ǹ� ����Ǵ� Event
    public delegate void CancelHandler(Skill skill);

    // TargetSearcher�� Select �۾��� �Ϸ��ϸ� ����Ǵ� Event
    public delegate void TargetSelectionCompletedHandler(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result);

    // currentApplyCount�� ���� �ٲ�� ����Ǵ� Event
    public delegate void CurrentApplyCountChangedHandler(Skill skill, int currentApplyCount, int prevApplyCount);
    #endregion

    [SerializeField]
    private SkillType type;
    [SerializeField]
    private SkillUseType useType; // �ܹ߼� or Toggle
    [SerializeField]
    private SkillGrade grade;
    [SerializeField]
    private int skillIdentifier; // skill �ĺ��� 

    [SerializeField]
    private MovementInSkill movement;
    [SerializeField]
    private SkillExecutionType executionType; // Auto Or Input
    [SerializeField]
    private SkillApplyType applyType; // ��� ���� or Animatoin ����

    [SerializeField]
    private NeedSelectionResultType needSelectionResultType; // �ʿ��� TargetSearcher �˻� ��� : Target or Position
    [SerializeField]
    private TargetSelectionTimingOption targetSelectionTimingOption; // TargetSearcher Select �Լ� ���� ���� : Use or UseInAction
    [SerializeField]
    private TargetSearchTimingOption targetSearchTimingOption; // TargetSearcher Search �Լ� ���� ���� : TargetSelectionCompleted or Apply

    // Skill�� ����ϱ� ���� ���ǵ�
    // Ex) Entity�� ���� �̻� �ɷ��� ���� ���, Jump ���� ���� ��� �� 
    [SerializeReference, SubclassSelector]
    private SkillCondition[] useConditions;

    // Data ���� ���� 
    [SerializeField]
    private int maxLevel;
    [SerializeField, Min(1f)]
    private int defaultLevel = 1;
    // ���� ���� ��ų �ĺ��ڸ� ����
    // �� �Ŀ� ��ų ���� ��, Skill�� childSkills �ĺ��ڿ� ������ �����ϰ� �ִ� ��ų�� �ĺ��ڸ� ���Ͽ� 
    //    ������ ������ �� �ƴ��� �Ǵ��Ѵ�. 
    [SerializeField]
    private int[] childSkills;
    [SerializeField]
    private SkillData[] skillDatas;

    // ���� Level�� �����Ǵ� SkillData
    private SkillData currentData;

    // ��ų�� ���� Level
    private int level;

    // �� ��ġ ���� current ������
    private int currentApplyCount;       // ���� ��ų ���� Ƚ�� 
    private float currentCastTime;       // ���� Cast Time
    private float currentCooldown;       // ���� ���� ���ð� 
    private float currentDuration;       // ���� ��ų�� ���� �ð�
    private float currentChargePower;    // ���� Charge ����
    private float currentChargeDuration; // ���� Charge ���� �ð� 

    // CustomAction���� Type�� ���� �з��� ���� Dictionary
    private readonly Dictionary<SkillCustomActoinType, CustomAction[]> customActionsByType = new();

    // ��ų ���� Entity
    public Entity Owner { get; private set; }
    public StateMachine<Skill> StateMachine { get; private set; }

    public SkillType Type => type;
    public SkillUseType UseType => useType;
    public SkillGrade Grade => grade;
    public int SkillIdentifier => skillIdentifier;

    public MovementInSkill Movement => movement;
    public SkillExecutionType ExecutionType => executionType;
    public SkillApplyType ApplyType => applyType;

    public IReadOnlyList<SkillCondition> UseConditions => useConditions;

    // �� Effects
    // �� Skill�� Level�� ������ ��, Level�� �ش��ϴ� SkillData�� �����ͼ� SkillData�� ���� EffectSelector�� ���� ������
    //    Effect���� Setting�ϴ� Property
    public IReadOnlyList<Effect> Effects {  get; private set; } = Array.Empty<Effect>();

    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value >= 1 && value <= MaxLevel,
               $"Skill.Rank = {value} - value�� 1�� MaxLevel({MaxLevel}) ���� ���̿����մϴ�.");

            if (level == value)
                return;

            int prevLevel = level;
            level = value;

            // ���ο� Level�� ���� ����� Level Data�� ã�ƿ�
            var newData = skillDatas.Last(x => x.level <= level);
            // ã�ƿ� Data�� Level�� ���� Data�� Level�� �ٸ��ٸ� 
            // currentData�� ã�ƿ� Data�� ����  
            if (newData.level != currentData.level)
                ChangeData(newData);

            onLevelChanged?.Invoke(this, level, prevLevel);
        }
    }

    public bool IsMaxLevel => level == maxLevel;

    // �� ��ų�� Level Up ������ �����ΰ�?
    // Skill�� �ִ� Level�� �ƴϰ�, Level Up ������ �����ϰ�, Level Up�� ���� Costs�� ����ϴٸ� True


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

    // TargetSearcher �����
    public IReadOnlyList<Entity> Targets { get; private set; }
    public IReadOnlyList<Vector2> TargetPositions { get; private set; }

    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount == ApplyCount;

    // Skill�� �ߵ��� ����Ǿ��°�? 
    public bool IsFinished => currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded
        ? IsDurationEnded
        : IsApplyCompleted;

    #region Event ������
    public event LevelChangedHandler onLevelChanged;
    public event AppliedHandler onApplied;
    public event CurrentApplyCountChangedHandler onCurrentApplyCountChanged;
    #endregion

    // Skill Ȱ��ȭ �Լ� 
    public void Activate()
    {

    }

    // customActionsByType�� Data�� ���� CunstomAction���� �����ϴ� �Լ� 
    private void UpdateCustomActions()
    {
        customActionsByType[SkillCustomActoinType.Cast] = currentData.customActionsOnCast;
        customActionsByType[SkillCustomActoinType.Charge] = currentData.customActionsOnCharge;
        customActionsByType[SkillCustomActoinType.PrecedingAction] = currentData.customActionsOnPrecedingAction;
        customActionsByType[SkillCustomActoinType.Action] = currentData.customActionsOnAction;
    }

    private void ChangeData(SkillData newData)
    {
        // ������ ������ �ִ� Effect���� �μ��� 
        foreach (var effect in Effects)
            Destroy(effect);

        // ���� Data�� ���ο� Data�� ���� 
        currentData = newData;

        // ���ο� Data�� effectSelectors�� Linq.Select �Լ��� ��ȸ�ϸ鼭 CreateEffect �Լ��� Effect���� �纻�� ���� 
        // Effects Property�� �Ҵ��ϱ� 
        Effects = currentData.effectSelectors.Select(x => x.CreateEffect(this)).ToArray();

        UpdateCustomActions();
    }

    // Skill�� T �������� Ȯ���ϴ� �Լ� 
    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();
    public bool IsInState<T>(int layer) where T : State<Skill> => StateMachine.IsInState<T>(layer);
}
