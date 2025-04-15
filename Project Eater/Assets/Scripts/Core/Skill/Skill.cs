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

    #region Skill Character
    [SerializeField]
    private SkillType type;
    [SerializeField]
    private SkillUseType useType; // 단발성 or Toggle
    [SerializeField]
    private SkillGrade grade;
    [SerializeField]
    private bool stackCountDisplay; // 스킬의 Stack을 UI에 표시할 것인가

    [SerializeField]
    private MovementInSkill movement;
    [SerializeField]
    private SkillExecutionType executionType; // Auto Or Input

    [SerializeField]
    private TargetSelectionTimingOption targetSelectionTimingOption; // TargetSearcher Select 함수 실행 시점 : Use or UseInAction
    [SerializeField]
    private TargetSearchTimingOption targetSearchTimingOption; // TargetSearcher Search 함수 실행 시점 : TargetSelectionCompleted or Apply

    // Skill을 사용하기 위한 조건들
    // Ex) Entity가 상태 이상에 걸렸을 때만 사용, Jump 중일 때만 사용 등 
    [SerializeReference, SubclassSelector]
    private SkillCondition[] useConditions;

    [SerializeField]
    private SoundEffectSO[] inPrecedingActionSkillSFXs;
    [SerializeField]
    private SoundEffectSO[] inActionSkillSFXs;
    #endregion

    #region DATA
    // Data 관련 변수 
    [SerializeField]
    private int maxLevel;
    [SerializeField, Min(1f)]
    private int defaultLevel = 1;
    [SerializeField]
    private SkillData[] skillDatas;
    #endregion

    #region Current DATA
    // 현재 Level에 대응되는 SkillData
    private SkillData currentData;

    // 스킬의 현재 Level
    private int level;

    // ※ 수치 관련 current 변수들
    private int currentApplyCount;        // 현재 스킬 적용 횟수 
    private float currentCastTime;        // 현재 Cast Time
    private float currentCooldown;        // 현재 재사용 대기시간 
    private float currentDuration;        // 현재 스킬의 지속 시간
    private float currentChargePower;     // 현재 Charge 정도
    private float currentChargeDuration;  // 현재 Charge 지속 시간 
    #endregion

    // CustomAction들을 Type에 따라 분류해 놓은 Dictionary
    private readonly Dictionary<SkillCustomActoinType, CustomAction[]> customActionsByType = new();
    // 스킬 소유 Entity
    public Entity Owner { get; private set; }
    public StateMachine<Skill> StateMachine { get; private set; }
    public int skillKeyNumber;

    #region Skill Character Property
    public SkillType Type => type;
    public SkillUseType UseType => useType;
    public SkillGrade Grade => grade;
    public bool StackCountDisplay => stackCountDisplay;

    public MovementInSkill Movement => movement;
    public SkillExecutionType ExecutionType => executionType;
    public SkillApplyType ApplyType => ApplyActions[ApplyActionIndex].applyType;

    public IReadOnlyList<SkillCondition> UseConditions => useConditions;

    public IReadOnlyList<SoundEffectSO> InPrecedingActionSkillSFXs => inPrecedingActionSkillSFXs;
    public IReadOnlyList<SoundEffectSO> InActionSkillSFXs => inActionSkillSFXs;

    public int PrecedingSFXIndex = 0; // 사전 동작 효과음 Index
    public int SFXIndex = 0;          // 스킬 동작 효과음 Index
    #endregion

    // ※ Effects
    // → Skill의 Level이 설정될 때, Level에 해당하는 SkillData를 가져와서 SkillData가 가진 EffectSelector에 의해 생성된
    //    Effect들을 Setting하는 Property
    public List<Effect[]> Effects { get; private set; } = new List<Effect[]>();
    public IReadOnlyList<Effect> currentEffects {  get; private set; } = Array.Empty<Effect>();
    public IReadOnlyList<SkillApplyAction> ApplyActions => currentData.applyActions;

    #region LEVEL
    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {

            Debug.Assert(value >= 1 && value <= MaxLevel,
               $"Skill.Rank = {value} - value는 1과 MaxLevel({MaxLevel}) 사이 값이여야합니다.");

            if (level == value || level >= MaxLevel)
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
    #endregion

    #region PrecedingAction & Action
    private SkillPrecedingAction PrecedingAction => ApplyActions[ApplyActionIndex].precedingAction;
    private SkillAction Action => ApplyActions[ApplyActionIndex].action;
    public bool HasPrecedingAction => PrecedingAction != null;
    // 현재 SkillData의 InSkillActionFinishOption을 반환
    public InSkillActionFinishOption InSkillActionFinishOption => ApplyActions[ApplyActionIndex].inSkillActionFinishOption;
    #endregion

    #region AnimatorParameter Property
    // 지역 변수를 이용해서 return된 Parameter 값을 못 바꾸게 함 (return된 AnimatorParameter 값을 수정하는 것은 금지)
    public AnimatorParameter CastAnimatorParameter
    {
        get
        {
            var constValue = currentData.castAnimatorParameter;
            return constValue;
        }
    }
    public AnimatorParameter ChargeAnimationParameter
    {
        get
        {
            var constValue = currentData.chargeAnimatorParameter;
            return constValue;
        }
    }
    public AnimatorParameter PrecedingActionAnimationParameter
    {
        get
        {
            var constValue = ApplyActions[ApplyActionIndex].precedingActionAnimatorParameter;

            return constValue;
        }
    }
    public AnimatorParameter ActionAnimationParameter
    {
        get
        {
            var constValue = ApplyActions[ApplyActionIndex].actionAnimatorParameter;

            return constValue;
        }
    }
    #endregion

    #region Target Searcher
    public TargetSearcher TargetSearcher => ApplyActions[ApplyActionIndex].targetSearcher;
    public bool IsSearchingTarget => TargetSearcher.IsSearching;
    public TargetSelectionResult TargetSelectionResult => TargetSearcher.SelectionResult;
    public TargetSearchResult TargetSearchResult => TargetSearcher.SearchResult;
    // Skill이 필요로 하는 기준점 Type과 TargetSearcher가 검색한 기준점(Select)의 Type이 일치하는가? 
    public bool HasValidTargetSelectionResult
    {
        get
        {
            // switch 문 활용 
            // → TargetSelectionResult.resultMessage의 값에 따라 다른 조건을 평가 
            return TargetSelectionResult.resultMessage switch
            {
                // case 조건 1 : SearchResultMessage가 FindTarget일 때, needSelectionResultType도 Target이여야 한다.
                SearchResultMessage.FindTarget => ApplyActions[ApplyActionIndex].needSelectionResultType == NeedSelectionResultType.Target,
                // case 조건 2 : SearchResultMessage가 FindPosition일 때, needSelectionResultType도 Position이여야 한다. 
                SearchResultMessage.FindPosition => ApplyActions[ApplyActionIndex].needSelectionResultType == NeedSelectionResultType.Position,
                // default 조건 : false 
                _ => false
            };
        }
    }
    // ※ Target Select를 성공했는지 여부 
    // → 스킬이 기준점 검색 중이 아니고, 검색한 기준점이 스킬이 필요로 하는 Type이라면 True
    public bool IsTargetSelectSuccessful => !IsSearchingTarget && HasValidTargetSelectionResult;
    #endregion

    #region Cool Down Property
    public float Cooldown => currentData.coolDown.GetValue(Owner.Stats);
    public bool HasCooldown => Cooldown > 0f;
    public float CurrentCooldown
    {
        get => currentCooldown;
        set => currentCooldown = Mathf.Clamp(value, 0f, Cooldown);
    }
    // CurrentCooldown이 0이면 true를 반환
    public bool IsCooldownCompleted => Mathf.Approximately(CurrentCooldown, 0f);
    #endregion

    #region Duration Property
    public float Duration => currentData.duration;
    // 스킬의 지속시간이 무한인가? 
    private bool IsTimeless => Mathf.Approximately(Duration, kInfinity);
    public float CurrentDuration
    {
        get => currentDuration;
        set => currentDuration = !IsTimeless ? Mathf.Clamp(value, 0f, Duration) : value;
    }
    #endregion

    #region Apply Count Property
    public SkillRunningFinishOption RunningFinishOption => currentData.runningFinishOption;
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

            UpdateSkillApplyAction();

            onCurrentApplyCountChanged?.Invoke(this, currentApplyCount, prevApplyCount);
        }
    }

    public int ApplyActionIndex => Mathf.Min(CurrentApplyCount, ApplyCount - 1);
    #endregion

    #region Apply Cycle Property
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
    #endregion

    #region Cast Property
    public bool IsUseCast => currentData.isUseCast;
    public float CastTime => currentData.isUseCast ? currentData.castTime.GetValue(Owner.Stats) : 0f;
    public float CurrentCastTime
    {
        get => currentCastTime;
        set => currentCastTime = Mathf.Clamp(value, 0f, CastTime);
    }
    public bool IsCastCompleted => Mathf.Approximately(CastTime, CurrentCastTime);
    #endregion

    #region Charge Property
    public bool IsUseCharge => currentData.isUseCharge;
    public SkillChargeFinishActionOption ChargeFinishActionOption => currentData.chargeFinishActionOption;
    public float ChargeTime => currentData.chargeTime;
    public float StartChargePower => currentData.startChargePower;
    public float CurrentChargePower
    {
        get => currentChargePower;
        set
        {
            var prevChargePower = currentChargePower;
            currentChargePower = Mathf.Clamp01(value);

            if (Mathf.Approximately(prevChargePower, currentChargePower))
                return;

            // 변경된 currentChargePower를 TargetSearcher의 Scale에 적용하여
            // Skill의 범위를 조절한다. 
            if (currentData.isApplyRangeScale)
                TargetSearcher.Scale = currentChargePower;

            // Effect들의 Scale 값도 현재 ChargePower 값으로 Setting
            // → Charge 상태에 따라 Skill들의 효과들도 강해지거나 약해진다. 
            if (currentData.isApplyEffectScale)
            {
                foreach (var effect in currentEffects)
                    effect.Scale = currentChargePower;
            }
        }
    }
    // Charge 시간
    public float ChargeDuration => currentData.chargeDuration;
    // IsUseCharge가 false면 1로 고정,
    // true라면 Lerp를 통해서 StartChargePower부터 1까지 currentChargeDuration / ChargeTime으로 보간
    // → ChargePower가 현재 ChargeDuration에 따라서 바뀌게 된다.
    public float CurrentChargeDuration
    {
        get => currentChargeDuration;
        set
        {
            currentChargeDuration = Mathf.Clamp(value, 0f, ChargeDuration);
            CurrentChargePower = !IsUseCharge
                ? 1f
                : Mathf.Lerp(StartChargePower, 1f, currentChargeDuration / ChargeTime);
        }
    }
    public float NeedChargeTimeToUse => currentData.needChargeTimeToUse;
    // 사용을 위한 최저(필요한) ChargeTime에 도달했는가?
    public bool IsMinChargeCompleted => currentChargeDuration >= NeedChargeTimeToUse;
    // 최대 충전에 도달했는가?
    public bool IsMaxChargeCompleted => currentChargeDuration >= ChargeTime;
    // Charge의 지속 시간이 끝났는가?
    public bool IsChargeDurationEnded => Mathf.Approximately(ChargeDuration, CurrentChargeDuration);
    #endregion

    #region Skill State Property (Not StateMachine State)
    public bool IsPassive => type == SkillType.Passive;
    public bool IsToggleType => useType == SkillUseType.Toggle;

    // 스킬이 사용 중임을 나타내는 Property
    public bool IsActivated {  get; private set; }

    // 스킬이 사용 준비가 되었는지를 나타내는 Property
    // → 스킬의 StateMachine이 ReadyState라면 true를 반환
    public bool IsReady => StateMachine.IsInState<ReadyState>();

    // 유저가 스킬을 사용할 수 있는지 여부 (= 사용자 입력을 받을 수 있는 상태)
    // → 발동 횟수가 남았고, ApplyCycle만큼 시간이 지났으면 true를 return
    public bool IsApplicable => (CurrentApplyCount < ApplyCount || IsInfinitelyApplicable) &&
        (CurrentApplyCycle >= ApplyCycle);

    // 스킬이 발동 가능한지를 나타내는 Property
    public bool IsUseable
    {
        get
        {
            if (IsReady)
            {
                // useConditions.All(x => x.IsPass(this)) : 모든 사용 조건을 만족했으면 true return
                return useConditions.All(x => x.IsPass(this));
            }
            else if (StateMachine.IsInState<InActionState>())
                // SkillExecutionType이 Input일 때, 사용자의 입력을 받을 수 있는 상태라면 true
                return ExecutionType == SkillExecutionType.Input && IsApplicable && useConditions.All(x => x.IsPass(this));
            else if (StateMachine.IsInState<ChargingState>())
                return IsMinChargeCompleted;
            else
                return false;
        }
    }
    #endregion

    // TargetSearcher 결과들
    public IReadOnlyList<Entity> Targets { get; private set; }
    public IReadOnlyList<Vector2> TargetPositions { get; private set; }

    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount >= ApplyCount;

    // Skill의 발동이 종료되었는가? 
    public bool IsFinished => currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded
        ? IsDurationEnded
        : IsApplyCompleted || IsDurationEnded;

    public override string Description => base.Description;

    public override string SpecificDescription
    {
        get
        {
            string description = base.SpecificDescription;

            var stringByKeyWord = new Dictionary<string, string>()
            {
                { "duration", Duration.ToString("0.##") },
                { "applyCount", ApplyCount.ToString() },
                { "applyCycle", ApplyCycle.ToString("0.##") },
                { "castTime", CastTime.ToString("0.##") },
                { "chargeDuration", ChargeDuration.ToString("0.##") },
                { "chargeTime", ChargeTime.ToString("0.##") },
                { "needChargeTimeToUse", NeedChargeTimeToUse.ToString("0.##") },
                { "coolTime", Cooldown.ToString("0") }
            };

            // 미리 만들어 놓은 BuildDescription 함수 덕분에 코드 수가 적어진다. 
            description = TextReplacer.Replace(description, stringByKeyWord);

            // 각 Apply 스킬마다 존재하는 targetSearcher
            // ex) targetSearcher.selectAction.range or 0.targetSearcher.searchAction.range
            for (int i = 0; i < ApplyCount; i++)
                description = ApplyActions[i].targetSearcher.BuildDescription(description, i.ToString());

            // 현재 설정한 precedingAction의 TextReplace는 없다.
            if (PrecedingAction != null)
            {
                for (int i = 0; i < ApplyCount; i++)
                {
                    if (ApplyActions[i].precedingAction != null)
                        description = ApplyActions[i].precedingAction.BuildDescription(description, i);

                    continue;
                }
            }

            // ex) skillAction.duration.0, skillAction.range.1, etc...
            for (int i = 0; i < ApplyCount; i++)
            {
                description = Action.BuildDescription(description, i);
            }

            // 스킬 동작 순번.(Key String).(스킬 동작이 가지고 있는 Effect들 순번)
            // Key String ex) duration, applyCount, applyCycle
            // → 0.duration.1
            // ※ 비 스택형 effectAction 
            // Ex) 0.effectAction.totalDamage.0 : 스킬의 첫 번째 동작(0.effectAction)의 첫 번째 Effect(0)의 총 데미지 값(totalDamage)
            // Ex) 1.effectAction.executionThreshold.0 : 스킬의 두 번째 동작(1.effectAction)의 첫 번째 Effect(0)의 처형 수치 %(executionThreshold)
            // ※ 스택형 effectAction은 없음
            int skillIndex = 0;
            foreach (var effects in Effects)
            {
                for (int i = 0; i < effects.Length; i++)
                {
                    description = effects[i].BuildDescription(description, skillIndex, i);
                }
                skillIndex++;
            }

            return description;
        }
    }

    #region Event 변수들
    public event LevelChangedHandler onLevelChanged;
    public event StateChangedHandler onStateChanged;
    public event AppliedHandler onApplied;
    public event ActivatedHandler onActivated;
    public event DeactivatedHandler onDeactivated;
    public event UseHandler onUsed;
    public event CancelHandler onCancelled;
    public event TargetSelectionCompletedHandler onTargetSelectionCompleted;
    public event CurrentApplyCountChangedHandler onCurrentApplyCountChanged;
    #endregion

    #region Method
    public void OnDestroy()
    {
        foreach (var effect in currentEffects)
            Destroy(effect);
    }

    // ※ 아래 항목 Setting
    // 1) Skill 소유주 
    // 2) Level
    // 3) Data (Level 변경시 Setting)
    // 4) StateMachine
    public void Setup(Entity owner, int level)
    {
        // 아래 조건식을 만족하면 통과 
        Debug.Assert(owner != null, $"Skill::Setup - Owner는 Null이 될 수 없습니다.");
        Debug.Assert(level >= 1 && level <= maxLevel, $"Skill::Setup - {level}이 1보다 작거나 {maxLevel}보다 큽니다.");
        Debug.Assert(Owner == null, $"Skill::Setup - 이미 Setup하였습니다.");

        Owner = owner;
        // level을 설정할 때, Data Setting 작업도 같이 실행된다.
        Level = level;
    }

    public void Setup(Entity owner)
        => Setup(owner, defaultLevel);

    public void SetupStateMachine()
    {
        if (Type == SkillType.Passive)
            StateMachine = new PassiveSkillStateMachine();
        else if (useType == SkillUseType.Toggle)
            StateMachine = new ToggleSkillStateMachine();
        else
            StateMachine = new InstantSkillStateMachine();

        StateMachine.Setup(this);

        // StateMachine<Skill>의 onStateChanged Event에 Skill의 onStateChanged Event를 연결한다. 
        // → 기본적으로 이벤트에 메서드를 등록하는 것이 일반적이지만, 같은 타입의 이벤트를 다른 이벤트에 연결하는 것도 가능
        //    단, 같은 매개변수를 가지고 있어야 한다. 
        StateMachine.onStateChanged += (_, newState, prevState, layer)
            => onStateChanged?.Invoke(this, newState, prevState, layer);
    }

    // Current Property들을 0으로 Reset 시키는 함수 
    // → Skill이 사용되고, Cooldown이 끝났을 때 호출
    public void ResetProperties()
    {
        CurrentCastTime = 0f;
        CurrentCooldown = 0f;
        CurrentDuration = 0f;
        CurrentApplyCycle = 0f;
        CurrentChargeDuration = 0f;
        CurrentApplyCount = 0;

        // 스킬 효과음 Index 초기화
        PrecedingSFXIndex = 0;
        SFXIndex = 0;

        Targets = null;
        TargetPositions = null;
    }

    public void Update() => StateMachine.Update();

    public void FixedUpdate() => StateMachine.FixedUpdate();

    // customActionsByType에 Data가 가진 CunstomAction들을 저장하는 함수 
    private void UpdateCustomActions()
    {
        customActionsByType[SkillCustomActoinType.Cast] = currentData.customActionsOnCast;
        customActionsByType[SkillCustomActoinType.Charge] = currentData.customActionsOnCharge;
        customActionsByType[SkillCustomActoinType.PrecedingAction] = ApplyActions[0].customActionsOnPrecedingAction;
        customActionsByType[SkillCustomActoinType.Action] = ApplyActions[0].customActionsOnAction;
    }

    private void UpdateSkillApplyAction()
    {
        currentEffects = Effects[ApplyActionIndex];
        // currentEffects = ApplyActions[ApplyActionIndex].effectSelectors.Select(x => x.CreateEffect(this)).ToArray();

        customActionsByType[SkillCustomActoinType.PrecedingAction] = ApplyActions[ApplyActionIndex].customActionsOnPrecedingAction;
        customActionsByType[SkillCustomActoinType.Action] = ApplyActions[ApplyActionIndex].customActionsOnAction;
    }

    private void ChangeData(SkillData newData)
    {
        // 기존에 가지고 있던 Effect들을 부수기 
        foreach (var effect in currentEffects)
            Destroy(effect);
        Effects.Clear();

        // 현재 Data를 새로운 Data로 변경 
        currentData = newData;

        // 새로운 Data의 effectSelectors를 Linq.Select 함수로 순회하면서 CreateEffect 함수로 Effect들의 사본을 만들어서 
        // Effects Property에 할당하기 
        currentEffects = ApplyActions[0].effectSelectors.Select(x => x.CreateEffect(this)).ToArray();

        for (int i = 0; i < ApplyCount; i++)
            Effects.Add(ApplyActions[i].effectSelectors.Select(x => x.CreateEffect(this)).ToArray());

        UpdateCustomActions();
    }

    public void LevelUp(bool temporaryAcquisition = false)
    {
        // Level 증가 
        Level++;

        if (temporaryAcquisition)
            return;

        // 스킬이 최대 강화에 도달한 경우, 해당 스킬의 상위 스킬을 순회하면서 스킬 획득이 가능한지 판단하여 
        // 가능하면 combinableSkills List에 추가하기 
        if (Level >= MaxLevel)
        {
            var skill = Owner.SkillSystem.RemoveUpgradableSkills(this);
            var topSkills = skill.GetTopSkillSlotNodes();

            foreach (var topSkill in topSkills)
            {
                if (topSkill.IsSkillAcquirable(Owner) && !Owner.SkillSystem.OwnSkills.Exists(x => x.ID == topSkill.Skill.ID))
                    Owner.SkillSystem.AddCombinableSkills(topSkill);
            }
        }
    }

    public void ShowIndicator()
        => TargetSearcher.ShowIndicator(Owner.gameObject);

    public void HideIndicator()
        => TargetSearcher.HideIndicator();

    // TargetSearcher의 SelectTarget을 호출하는 함수 
    // 1) onSelectCompletedOrNull : Select가 완료되었을 때, 호출할 Action
    // 2) Indicator를 보여줄 지 여부 
    public void SelectTarget(Action<Skill, TargetSearcher, TargetSelectionResult> onSelectCompletedOrNull, bool isShowIndicator = true)
    {
        CancelSelectTarget();

        if (isShowIndicator)
            ShowIndicator();

        TargetSearcher.SelectTarget(Owner, Owner.gameObject, (targetSearcher, result) =>
        {
            // 기준점을 정했으면 이제 Indicator를 꺼줌 
            if (isShowIndicator)
                HideIndicator();

            // 1) Skill이 필요로 하는 Type의 기준점 검색에 성공했고,
            // 2) SearchTiming이 기준점 검색 직후라면(TargetSelectionCompleted) Target 검색 실행
            if (IsTargetSelectSuccessful &&
                (targetSearchTimingOption == TargetSearchTimingOption.TargetSelectionCompleted ||
                 targetSearchTimingOption == TargetSearchTimingOption.Both))
            {
                SearchTargets();
            }

            onSelectCompletedOrNull?.Invoke(this, targetSearcher, result);
            onTargetSelectionCompleted?.Invoke(this, targetSearcher, result);
        });
    }

    // SelectTarget의 Action 인자가 없는 오버로딩 함수  
    public void SelectTarget(bool isShowIndicator = true) => SelectTarget(null, isShowIndicator);

    // ※ isHideIndicator : Indicator Hide 여부 
    public void CancelSelectTarget(bool isHideIndicator = true)
    {
        if (!TargetSearcher.IsSearching)
            return;

        Debug.Log("CancelSelectTarget 실행");

        TargetSearcher.CancelSelect();

        if (isHideIndicator)
            HideIndicator();
    }

    private void SearchTargets()
    {
        var result = TargetSearcher.SearchTargets(Owner, Owner.gameObject);

        // TargetSearcher.SearchTargets의 결과를 Targets, TargetPositions에 할당한다. 
        Targets = result.targets.Select(x => x.GetComponent<Entity>()).ToArray();

        TargetPositions = result.positions;
    }

    // Target을 즉시 Select
    public TargetSelectionResult SelectTargetImmediate(Vector2 position)
    {
        CancelSelectTarget();

        var result = TargetSearcher.SelectImmediate(Owner, Owner.gameObject, position);
        if (IsTargetSelectSuccessful && 
            (targetSearchTimingOption == TargetSearchTimingOption.TargetSelectionCompleted ||
             targetSearchTimingOption == TargetSearchTimingOption.Both))
            SearchTargets();

        return result;
    }

    public bool IsInRange(Vector2 position)
        => TargetSearcher.IsInRange(Owner, Owner.gameObject, position);

    // 스킬 시작 함수 (플레이어가 스킬 버튼을 누른 경우 실행)
    // → Skill의 State에 따라 각 조건을 만족한다면(IsUseable) Skill State를 다음 단계(Preceding, InAction ...)로 전이하거나
    //    특정 State에 예약된 Animation을 실행하도록 한다. 
    // → 실제 Skill의 효과인 Action이 실행되기 전 준비하는 과정이다. 
    // → 실제 효과인 Action의 실행은 Apply 함수에서 실행한다. 
    public bool Use()
    {
        if (!IsUseable)
        {
            return false;
        }

        // Command나 Message 형태로 'Use'를 전달한다. 
        // ( Command VS Message 참고 )
        bool isUsed = StateMachine.ExecuteCommand(SkillExecuteCommand.Use) || StateMachine.SendMessage(SkillStateMessage.Use);
        // Use가 받아들여지면, 사용됐다는 의미로 onUsed Event를 호출하고, 사용 여부를 반환
        if (isUsed)
            onUsed?.Invoke(this);

        return isUsed;
    }

    // 스킬 즉시 발동 함수 
    // ※ Use 함수와 차이점 
    // → 보통 Use를 사용하면 Skill이 바로 사용되는 게 아니라, TargetSearcher의 SelectTarget 함수를 이용해서 기준점을 찾는 작업을 먼저 한다. 
    //    하지만, UseImmediately를 사용하면 기준점을 바로 Select 한다. (기준점을 찾는 작업을 뛰어넘고 바로 사용)
    // → SkillSystem에서 Skill 범위 밖에 있는 대상을 Skill의 기준점으로 삼을 때, 자동으로 그 대상에게 다가가서 Skill을 사용하는 
    //    Skill 사용 예약 기능에 사용된다. 
    public bool UseImmediately(Vector2 position)
    {
        Debug.Assert(IsUseable, "Skill::UseImmediately - 사용 조건을 만족하지 못했습니다.");

        SelectTargetImmediate(position);

        // UseImmediately(즉시 스킬 사용) 명령을 보냄
        bool isUsed = StateMachine.ExecuteCommand(SkillExecuteCommand.UseImmediately) || StateMachine.SendMessage(SkillStateMessage.Use);
        if (isUsed)
            onUsed?.Invoke(this);

        return isUsed;
    }

    // Skill 사용 취소 
    // ※ isForce : 강제로 사용 종료 (즉시 취소)
    // → 자폭 스킬은 제외 
    public bool Cancel(bool isForce = false)
    {
        if (IsPassive)
            return false;

        // Debug.Assert(!IsPassive, "Skill::Cancel - Passive Skill은 Cancel 할 수 없습니다.");

        var isCanceled = isForce ? StateMachine.ExecuteCommand(SkillExecuteCommand.CancelImmediately) :
            StateMachine.ExecuteCommand(SkillExecuteCommand.Cancel);

        if (isCanceled)
            onCancelled?.Invoke(this);

        return isCanceled;
    }

    // Skill 활성화 함수 
    public void Activate()
    {
        Debug.Assert(!IsActivated, "Skill::Activate - 이미 활성화되어 있습니다.");

        IsActivated = true;
        onActivated?.Invoke(this);
    }

    // Skill 비활성화 함수 
    public void Deactivate()
    {
        Debug.Assert(IsActivated, "Skill::Activate - Skill이 활성화되어있지 않습니다.");

        IsActivated = false;
        onDeactivated?.Invoke(this);
    }

    // 인자로 받은 Type에 해당되는 CustomAction들의 함수를 호출 
    public void StartCustomActions(SkillCustomActoinType type)
    {
        foreach (var customAction in customActionsByType[type])
            customAction.Start(this);
    }
    public void RunCustomActions(SkillCustomActoinType type)
    {
        foreach (var customAction in customActionsByType[type])
            customAction.Run(this);
    }
    public void ReleaseCustomActions(SkillCustomActoinType type)
    {
        foreach (var customAction in customActionsByType[type])
            customAction.Release(this);
    }

    // 대응되는 CustomAction 함수와 PrecedingAction 함수를 호출 
    public void StartPrecedingAction()
    {
        StartCustomActions(SkillCustomActoinType.PrecedingAction);
        PrecedingAction.Start(this);
    }
    public bool RunPrecedingAction()
    {
        RunCustomActions(SkillCustomActoinType.PrecedingAction);
        return PrecedingAction.Run(this);
    }
    public void FixedRunPrecedingAction() => PrecedingAction.FixedRun(this);
    public void ReleasePrecedingAction()
    {
        ReleaseCustomActions(SkillCustomActoinType.PrecedingAction);
        PrecedingAction.Release(this);
    }

    // 대응되는 CustomAction 함수와 Action 함수를 호출 
    public void StartActoin()
    {
        StartCustomActions(SkillCustomActoinType.Action);
        Action.Start(this);
    }
    public void ReleaseActoin()
    {
        ReleaseCustomActions(SkillCustomActoinType.Action);
        Action.Release(this);
    }

    // Skill 실제 효과 발동 함수 
    // ※ isConsumeApplyCount : Apply 횟수를 소모할지 요구 
    // 1) false : Apply가 실행, 현재 ApplyCount 값이 증가하지 않고 그대로 유지 
    // 2) true  : Apply 실행, 현재 ApplyCount 값이 증가 
    public void Apply(bool isConsumeApplyCount = true)
    {
        Debug.Assert(IsInfinitelyApplicable || !isConsumeApplyCount || (CurrentApplyCount < ApplyCount),
            $"Skill({CodeName})의 최대 적용 횟수({ApplyCount})를 초과해서 적용할 수 없습니다.");

        // ExecutionType이 Input이고, ApplyType이 Animation이 Select를 임의로 해줘야 한다. 
        if (executionType == SkillExecutionType.Input && ApplyType == SkillApplyType.Animation)
            SelectTarget();

        // TargetSearch 타이밍이 Apply라면 SearchTargets 실행
        if (targetSearchTimingOption == TargetSearchTimingOption.Apply ||
            targetSearchTimingOption == TargetSearchTimingOption.Both)
            SearchTargets();

        // 스킬 효과(Action) 실행 
        RunCustomActions(SkillCustomActoinType.Action);
        Action.Apply(this);

        // Auto일 때는 Duration과의 오차 값을 남기기 위해 ApplyCycle로 나눈 나머지로 값을 설정함
        // Ex. Duration = 1.001, CurrentApplyCycle = 1.001
        //     => Duration = 1.001, CurrentApplyCycle = 0.001
        if (executionType == SkillExecutionType.Auto)
            CurrentApplyCycle %= ApplyCycle;
        // Input일 때, 어차피 유저가 Skill의 발동 Timing을 직접 정하기 때문에 오차가 존재하지 않으므로 0으로 초기화
        else
            CurrentApplyCycle = 0f;

        if (isConsumeApplyCount)
            CurrentApplyCount++;

        onApplied?.Invoke(this, CurrentApplyCount);
    }

    // Skill이 T 상태인지 확인하는 함수 
    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();
    public bool IsInState<T>(int layer) where T : State<Skill> => StateMachine.IsInState<T>(layer);

    // Skill의 StateMachine에서 Layer의 State Type을 가져오는 함수 
    public Type GetCurrentStateType(int layer = 0) => StateMachine.GetCurrentStateType(layer);

    // Skill의 targetSelectionTimingOption이 인자 값(option)과 같은지 확인하는 함수  
    // → targetSelectionTimingOption이 Both이거나 인자 값과 같다면 True를 반환
    public bool IsTargetSelectionTiming(TargetSelectionTimingOption option)
        => targetSelectionTimingOption == TargetSelectionTimingOption.Both || targetSelectionTimingOption == option;

    public override object Clone()
    {
        var clone = Instantiate(this);

        if (Owner != null)
            clone.Setup(Owner, level);

        return clone;
    }
    #endregion
}
