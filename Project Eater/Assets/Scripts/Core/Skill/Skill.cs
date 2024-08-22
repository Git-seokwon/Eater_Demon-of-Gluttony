using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill : IdentifiedObject
{
    private const int kInfinity = 0;

    #region Events
    // Skill의 Level이 변했을 때 실행되는 Event
    public delegate void LevelChangedHandler(Skill skill, int currentLevel, int prevLevel);

    // Skill의 StateMachine이 가진 Event에 연결할 Event (그래서 이름과 매개변수가 똑같음)
    public delegate void StateChangedHandler(Skill skill, State<Skill> newState, State<Skill> prevState, int layer);

    // Skill이 Apply되면 실행되는 Event
    // ※ currentApplyCount : 현재 Apply 횟수 
    public delegate void AppliedHandler(Skill skill, int currentApplyCount);

    // Skill을 사용하면 실행되는 Event
    // → Skill 버튼을 누를 때 실행
    public delegate void UseHandler(Skill skill);

    // Skill이 사용(Use)된 직후 실제 동작 상태에 들어가면 실행되는 Event
    // → Skill 버튼을 누르고 Skill을 적용할 Target을 선택했을 때 실행 
    public delegate void ActivatedHandler(Skill skill);

    // Skill이 종료(Action까지 모든 동작을 끝마침)된 직후 실행되는 Event
    public delegate void DeactivatedHandler(Skill skill);

    // Skill 사용이 중지되면 실행되는 Event
    public delegate void CancelHandler(Skill skill);

    // TargetSearcher가 Select 작업을 완료하면 실행되는 Event
    public delegate void TargetSelectionCompletedHandler(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result);

    // currentApplyCount의 값이 바뀌면 실행되는 Event
    public delegate void CurrentApplyCountChangedHandler(Skill skill, int currentApplyCount, int prevApplyCount);
    #endregion

    [SerializeField]
    private SkillType type;
    [SerializeField]
    private SkillUseType useType; // 단발성 or Toggle
    [SerializeField]
    private SkillGrade grade;
    [SerializeField]
    private int skillIdentifier; // skill 식별자 

    [SerializeField]
    private MovementInSkill movement;
    [SerializeField]
    private SkillExecutionType executionType; // Auto Or Input
    [SerializeField]
    private SkillApplyType applyType; // 즉시 적용 or Animatoin 적용

    [SerializeField]
    private NeedSelectionResultType needSelectionResultType; // 필요한 TargetSearcher 검색 결과 : Target or Position
    [SerializeField]
    private TargetSelectionTimingOption targetSelectionTimingOption; // TargetSearcher Select 함수 실행 시점 : Use or UseInAction
    [SerializeField]
    private TargetSearchTimingOption targetSearchTimingOption; // TargetSearcher Search 함수 실행 시점 : TargetSelectionCompleted or Apply

    // Skill을 사용하기 위한 조건들
    // Ex) Entity가 상태 이상에 걸렸을 때만 사용, Jump 중일 때만 사용 등 
    [SerializeReference, SubclassSelector]
    private SkillCondition[] useConditions;

    // Data 관련 변수 
    [SerializeField]
    private int maxLevel;
    [SerializeField, Min(1f)]
    private int defaultLevel = 1;
    // 하위 조합 스킬 식별자를 저장
    // → 후에 스킬 조합 시, Skill의 childSkills 식별자와 유저가 소유하고 있는 스킬의 식별자를 비교하여 
    //    조합이 가능한 지 아닌지 판단한다. 
    [SerializeField]
    private int[] childSkills;
    [SerializeField]
    private SkillData[] skillDatas;

    // 현재 Level에 대응되는 SkillData
    private SkillData currentData;

    // 스킬의 현재 Level
    private int level;

    // ※ 수치 관련 current 변수들
    private int currentApplyCount;       // 현재 스킬 적용 횟수 
    private float currentCastTime;       // 현재 Cast Time
    private float currentCooldown;       // 현재 재사용 대기시간 
    private float currentDuration;       // 현재 스킬의 지속 시간
    private float currentChargePower;    // 현재 Charge 정도
    private float currentChargeDuration; // 현재 Charge 지속 시간 

    // CustomAction들을 Type에 따라 분류해 놓은 Dictionary
    private readonly Dictionary<SkillCustomActoinType, CustomAction[]> customActionsByType = new();

    // 스킬 소유 Entity
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

    // ※ Effects
    // → Skill의 Level이 설정될 때, Level에 해당하는 SkillData를 가져와서 SkillData가 가진 EffectSelector에 의해 생성된
    //    Effect들을 Setting하는 Property
    public IReadOnlyList<Effect> Effects {  get; private set; } = Array.Empty<Effect>();

    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value >= 1 && value <= MaxLevel,
               $"Skill.Rank = {value} - value는 1과 MaxLevel({MaxLevel}) 사이 값이여야합니다.");

            if (level == value)
                return;

            int prevLevel = level;
            level = value;

            // 새로운 Level과 가장 가까운 Level Data를 찾아옴
            var newData = skillDatas.Last(x => x.level <= level);
            // 찾아온 Data의 Level과 현재 Data의 Level이 다르다면 
            // currentData를 찾아온 Data로 변경  
            if (newData.level != currentData.level)
                ChangeData(newData);

            onLevelChanged?.Invoke(this, level, prevLevel);
        }
    }

    public bool IsMaxLevel => level == maxLevel;

    // ※ 스킬이 Level Up 가능한 상태인가?
    // Skill이 최대 Level이 아니고, Level Up 조건을 만족하고, Level Up을 위한 Costs가 충분하다면 True


    // 현재 SkillData의 InSkillActionFinishOption을 반환
    public InSkillActionFinishOption InSkillActionFinishOption => currentData.inSkillActionFinishOption;

    public TargetSearcher TargetSearcher => currentData.targetSearcher;
    public bool IsSearchingTarget => TargetSearcher.IsSearching;
    public TargetSelectionResult TargetSelectionResult => TargetSearcher.SelectionResult;
    public TargetSearchResult TargetSearchResult => TargetSearcher.SearchResult;

    // Skill이 필요로 하는 기준점 Type과 TargetSearcher가 검색한 기준점의 Type이 일치하는가? 
    public bool HasValidTargetSelectionResult
    {
        get
        {
            // switch 문 활용 
            // → TargetSelectionResult.resultMessage의 값에 따라 다른 조건을 평가 
            return TargetSelectionResult.resultMessage switch
            {
                // case 조건 1 : SearchResultMessage가 FindTarget일 때, needSelectionResultType도 Target이여야 한다.
                SearchResultMessage.FindTarget => needSelectionResultType == NeedSelectionResultType.Target,
                // case 조건 2 : SearchResultMessage가 FindPosition일 때, needSelectionResultType도 Position이여야 한다. 
                SearchResultMessage.FindPosition => needSelectionResultType == NeedSelectionResultType.Position,
                // default 조건 : false 
                _ => false
            };
        }
    }

    // ※ Target Select를 성공했는지 여부 
    // → 스킬이 기준점 검색 중이 아니고, 검색한 기준점이 스킬이 필요로 하는 Type이라면 True
    public bool IsTargetSelectSuccessful => !IsSearchingTarget && HasValidTargetSelectionResult;

    // ※ Duration Property
    public float Duration => currentData.duration;

    // 스킬의 지속시간이 무한인가? 
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

    // ※ Apply Cycle Property
    // 1)
    // currentData의 applyCycle이 0이고 applyCount가 1보다 크면 (여러번 적용 가능하면)
    // Skill의 duration을 (ApplyCount - 1)로 나눠서 ApplyCycle을 계산하여 return 함.
    // ※ ApplyCount - 1 : 처음 한 번은 적용되고 시작하기 때문에 1번을 빼준다. 
    // 2)
    // 아니라면 설정된 currentData의 applyCycle을 그대로 return 함.
    public float ApplyCycle
        => Mathf.Approximately(currentData.applyCycle, 0f) && ApplyCount > 1
        ? Duration / (ApplyCount - 1)
        : currentData.applyCycle;

    // CurrentApplyCycle 값이 ApplyCycle에 도달하면 Skill이 발동
    public float CurrentApplyCycle { get; set; }

    // TargetSearcher 결과들
    public IReadOnlyList<Entity> Targets { get; private set; }
    public IReadOnlyList<Vector2> TargetPositions { get; private set; }

    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount == ApplyCount;

    // Skill의 발동이 종료되었는가? 
    public bool IsFinished => currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded
        ? IsDurationEnded
        : IsApplyCompleted;

    #region Event 변수들
    public event LevelChangedHandler onLevelChanged;
    public event AppliedHandler onApplied;
    public event CurrentApplyCountChangedHandler onCurrentApplyCountChanged;
    #endregion

    // Skill 활성화 함수 
    public void Activate()
    {

    }

    // customActionsByType에 Data가 가진 CunstomAction들을 저장하는 함수 
    private void UpdateCustomActions()
    {
        customActionsByType[SkillCustomActoinType.Cast] = currentData.customActionsOnCast;
        customActionsByType[SkillCustomActoinType.Charge] = currentData.customActionsOnCharge;
        customActionsByType[SkillCustomActoinType.PrecedingAction] = currentData.customActionsOnPrecedingAction;
        customActionsByType[SkillCustomActoinType.Action] = currentData.customActionsOnAction;
    }

    private void ChangeData(SkillData newData)
    {
        // 기존에 가지고 있던 Effect들을 부수기 
        foreach (var effect in Effects)
            Destroy(effect);

        // 현재 Data를 새로운 Data로 변경 
        currentData = newData;

        // 새로운 Data의 effectSelectors를 Linq.Select 함수로 순회하면서 CreateEffect 함수로 Effect들의 사본을 만들어서 
        // Effects Property에 할당하기 
        Effects = currentData.effectSelectors.Select(x => x.CreateEffect(this)).ToArray();

        UpdateCustomActions();
    }

    // Skill이 T 상태인지 확인하는 함수 
    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();
    public bool IsInState<T>(int layer) where T : State<Skill> => StateMachine.IsInState<T>(layer);
}
