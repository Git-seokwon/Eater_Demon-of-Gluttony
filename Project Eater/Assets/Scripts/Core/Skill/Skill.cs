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

    #region Skill Character
    [SerializeField]
    private SkillType type;
    [SerializeField]
    private SkillUseType useType; // �ܹ߼� or Toggle
    [SerializeField]
    private SkillGrade grade;
    [SerializeField]
    private bool stackCountDisplay; // ��ų�� Stack�� UI�� ǥ���� ���ΰ�

    [SerializeField]
    private MovementInSkill movement;
    [SerializeField]
    private SkillExecutionType executionType; // Auto Or Input

    [SerializeField]
    private TargetSelectionTimingOption targetSelectionTimingOption; // TargetSearcher Select �Լ� ���� ���� : Use or UseInAction
    [SerializeField]
    private TargetSearchTimingOption targetSearchTimingOption; // TargetSearcher Search �Լ� ���� ���� : TargetSelectionCompleted or Apply

    // Skill�� ����ϱ� ���� ���ǵ�
    // Ex) Entity�� ���� �̻� �ɷ��� ���� ���, Jump ���� ���� ��� �� 
    [SerializeReference, SubclassSelector]
    private SkillCondition[] useConditions;

    [SerializeField]
    private SoundEffectSO[] inPrecedingActionSkillSFXs;
    [SerializeField]
    private SoundEffectSO[] inActionSkillSFXs;
    #endregion

    #region DATA
    // Data ���� ���� 
    [SerializeField]
    private int maxLevel;
    [SerializeField, Min(1f)]
    private int defaultLevel = 1;
    [SerializeField]
    private SkillData[] skillDatas;
    #endregion

    #region Current DATA
    // ���� Level�� �����Ǵ� SkillData
    private SkillData currentData;

    // ��ų�� ���� Level
    private int level;

    // �� ��ġ ���� current ������
    private int currentApplyCount;        // ���� ��ų ���� Ƚ�� 
    private float currentCastTime;        // ���� Cast Time
    private float currentCooldown;        // ���� ���� ���ð� 
    private float currentDuration;        // ���� ��ų�� ���� �ð�
    private float currentChargePower;     // ���� Charge ����
    private float currentChargeDuration;  // ���� Charge ���� �ð� 
    #endregion

    // CustomAction���� Type�� ���� �з��� ���� Dictionary
    private readonly Dictionary<SkillCustomActoinType, CustomAction[]> customActionsByType = new();
    // ��ų ���� Entity
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

    public int PrecedingSFXIndex = 0; // ���� ���� ȿ���� Index
    public int SFXIndex = 0;          // ��ų ���� ȿ���� Index
    #endregion

    // �� Effects
    // �� Skill�� Level�� ������ ��, Level�� �ش��ϴ� SkillData�� �����ͼ� SkillData�� ���� EffectSelector�� ���� ������
    //    Effect���� Setting�ϴ� Property
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
               $"Skill.Rank = {value} - value�� 1�� MaxLevel({MaxLevel}) ���� ���̿����մϴ�.");

            if (level == value || level >= MaxLevel)
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
    #endregion

    #region PrecedingAction & Action
    private SkillPrecedingAction PrecedingAction => ApplyActions[ApplyActionIndex].precedingAction;
    private SkillAction Action => ApplyActions[ApplyActionIndex].action;
    public bool HasPrecedingAction => PrecedingAction != null;
    // ���� SkillData�� InSkillActionFinishOption�� ��ȯ
    public InSkillActionFinishOption InSkillActionFinishOption => ApplyActions[ApplyActionIndex].inSkillActionFinishOption;
    #endregion

    #region AnimatorParameter Property
    // ���� ������ �̿��ؼ� return�� Parameter ���� �� �ٲٰ� �� (return�� AnimatorParameter ���� �����ϴ� ���� ����)
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
    // Skill�� �ʿ�� �ϴ� ������ Type�� TargetSearcher�� �˻��� ������(Select)�� Type�� ��ġ�ϴ°�? 
    public bool HasValidTargetSelectionResult
    {
        get
        {
            // switch �� Ȱ�� 
            // �� TargetSelectionResult.resultMessage�� ���� ���� �ٸ� ������ �� 
            return TargetSelectionResult.resultMessage switch
            {
                // case ���� 1 : SearchResultMessage�� FindTarget�� ��, needSelectionResultType�� Target�̿��� �Ѵ�.
                SearchResultMessage.FindTarget => ApplyActions[ApplyActionIndex].needSelectionResultType == NeedSelectionResultType.Target,
                // case ���� 2 : SearchResultMessage�� FindPosition�� ��, needSelectionResultType�� Position�̿��� �Ѵ�. 
                SearchResultMessage.FindPosition => ApplyActions[ApplyActionIndex].needSelectionResultType == NeedSelectionResultType.Position,
                // default ���� : false 
                _ => false
            };
        }
    }
    // �� Target Select�� �����ߴ��� ���� 
    // �� ��ų�� ������ �˻� ���� �ƴϰ�, �˻��� �������� ��ų�� �ʿ�� �ϴ� Type�̶�� True
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
    // CurrentCooldown�� 0�̸� true�� ��ȯ
    public bool IsCooldownCompleted => Mathf.Approximately(CurrentCooldown, 0f);
    #endregion

    #region Duration Property
    public float Duration => currentData.duration;
    // ��ų�� ���ӽð��� �����ΰ�? 
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

            // ����� currentChargePower�� TargetSearcher�� Scale�� �����Ͽ�
            // Skill�� ������ �����Ѵ�. 
            if (currentData.isApplyRangeScale)
                TargetSearcher.Scale = currentChargePower;

            // Effect���� Scale ���� ���� ChargePower ������ Setting
            // �� Charge ���¿� ���� Skill���� ȿ���鵵 �������ų� ��������. 
            if (currentData.isApplyEffectScale)
            {
                foreach (var effect in currentEffects)
                    effect.Scale = currentChargePower;
            }
        }
    }
    // Charge �ð�
    public float ChargeDuration => currentData.chargeDuration;
    // IsUseCharge�� false�� 1�� ����,
    // true��� Lerp�� ���ؼ� StartChargePower���� 1���� currentChargeDuration / ChargeTime���� ����
    // �� ChargePower�� ���� ChargeDuration�� ���� �ٲ�� �ȴ�.
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
    // ����� ���� ����(�ʿ���) ChargeTime�� �����ߴ°�?
    public bool IsMinChargeCompleted => currentChargeDuration >= NeedChargeTimeToUse;
    // �ִ� ������ �����ߴ°�?
    public bool IsMaxChargeCompleted => currentChargeDuration >= ChargeTime;
    // Charge�� ���� �ð��� �����°�?
    public bool IsChargeDurationEnded => Mathf.Approximately(ChargeDuration, CurrentChargeDuration);
    #endregion

    #region Skill State Property (Not StateMachine State)
    public bool IsPassive => type == SkillType.Passive;
    public bool IsToggleType => useType == SkillUseType.Toggle;

    // ��ų�� ��� ������ ��Ÿ���� Property
    public bool IsActivated {  get; private set; }

    // ��ų�� ��� �غ� �Ǿ������� ��Ÿ���� Property
    // �� ��ų�� StateMachine�� ReadyState��� true�� ��ȯ
    public bool IsReady => StateMachine.IsInState<ReadyState>();

    // ������ ��ų�� ����� �� �ִ��� ���� (= ����� �Է��� ���� �� �ִ� ����)
    // �� �ߵ� Ƚ���� ���Ұ�, ApplyCycle��ŭ �ð��� �������� true�� return
    public bool IsApplicable => (CurrentApplyCount < ApplyCount || IsInfinitelyApplicable) &&
        (CurrentApplyCycle >= ApplyCycle);

    // ��ų�� �ߵ� ���������� ��Ÿ���� Property
    public bool IsUseable
    {
        get
        {
            if (IsReady)
            {
                // useConditions.All(x => x.IsPass(this)) : ��� ��� ������ ���������� true return
                return useConditions.All(x => x.IsPass(this));
            }
            else if (StateMachine.IsInState<InActionState>())
                // SkillExecutionType�� Input�� ��, ������� �Է��� ���� �� �ִ� ���¶�� true
                return ExecutionType == SkillExecutionType.Input && IsApplicable && useConditions.All(x => x.IsPass(this));
            else if (StateMachine.IsInState<ChargingState>())
                return IsMinChargeCompleted;
            else
                return false;
        }
    }
    #endregion

    // TargetSearcher �����
    public IReadOnlyList<Entity> Targets { get; private set; }
    public IReadOnlyList<Vector2> TargetPositions { get; private set; }

    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount >= ApplyCount;

    // Skill�� �ߵ��� ����Ǿ��°�? 
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

            // �̸� ����� ���� BuildDescription �Լ� ���п� �ڵ� ���� ��������. 
            description = TextReplacer.Replace(description, stringByKeyWord);

            // �� Apply ��ų���� �����ϴ� targetSearcher
            // ex) targetSearcher.selectAction.range or 0.targetSearcher.searchAction.range
            for (int i = 0; i < ApplyCount; i++)
                description = ApplyActions[i].targetSearcher.BuildDescription(description, i.ToString());

            // ���� ������ precedingAction�� TextReplace�� ����.
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

            // ��ų ���� ����.(Key String).(��ų ������ ������ �ִ� Effect�� ����)
            // Key String ex) duration, applyCount, applyCycle
            // �� 0.duration.1
            // �� �� ������ effectAction 
            // Ex) 0.effectAction.totalDamage.0 : ��ų�� ù ��° ����(0.effectAction)�� ù ��° Effect(0)�� �� ������ ��(totalDamage)
            // Ex) 1.effectAction.executionThreshold.0 : ��ų�� �� ��° ����(1.effectAction)�� ù ��° Effect(0)�� ó�� ��ġ %(executionThreshold)
            // �� ������ effectAction�� ����
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

    #region Event ������
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

    // �� �Ʒ� �׸� Setting
    // 1) Skill ������ 
    // 2) Level
    // 3) Data (Level ����� Setting)
    // 4) StateMachine
    public void Setup(Entity owner, int level)
    {
        // �Ʒ� ���ǽ��� �����ϸ� ��� 
        Debug.Assert(owner != null, $"Skill::Setup - Owner�� Null�� �� �� �����ϴ�.");
        Debug.Assert(level >= 1 && level <= maxLevel, $"Skill::Setup - {level}�� 1���� �۰ų� {maxLevel}���� Ů�ϴ�.");
        Debug.Assert(Owner == null, $"Skill::Setup - �̹� Setup�Ͽ����ϴ�.");

        Owner = owner;
        // level�� ������ ��, Data Setting �۾��� ���� ����ȴ�.
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

        // StateMachine<Skill>�� onStateChanged Event�� Skill�� onStateChanged Event�� �����Ѵ�. 
        // �� �⺻������ �̺�Ʈ�� �޼��带 ����ϴ� ���� �Ϲ���������, ���� Ÿ���� �̺�Ʈ�� �ٸ� �̺�Ʈ�� �����ϴ� �͵� ����
        //    ��, ���� �Ű������� ������ �־�� �Ѵ�. 
        StateMachine.onStateChanged += (_, newState, prevState, layer)
            => onStateChanged?.Invoke(this, newState, prevState, layer);
    }

    // Current Property���� 0���� Reset ��Ű�� �Լ� 
    // �� Skill�� ���ǰ�, Cooldown�� ������ �� ȣ��
    public void ResetProperties()
    {
        CurrentCastTime = 0f;
        CurrentCooldown = 0f;
        CurrentDuration = 0f;
        CurrentApplyCycle = 0f;
        CurrentChargeDuration = 0f;
        CurrentApplyCount = 0;

        // ��ų ȿ���� Index �ʱ�ȭ
        PrecedingSFXIndex = 0;
        SFXIndex = 0;

        Targets = null;
        TargetPositions = null;
    }

    public void Update() => StateMachine.Update();

    public void FixedUpdate() => StateMachine.FixedUpdate();

    // customActionsByType�� Data�� ���� CunstomAction���� �����ϴ� �Լ� 
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
        // ������ ������ �ִ� Effect���� �μ��� 
        foreach (var effect in currentEffects)
            Destroy(effect);
        Effects.Clear();

        // ���� Data�� ���ο� Data�� ���� 
        currentData = newData;

        // ���ο� Data�� effectSelectors�� Linq.Select �Լ��� ��ȸ�ϸ鼭 CreateEffect �Լ��� Effect���� �纻�� ���� 
        // Effects Property�� �Ҵ��ϱ� 
        currentEffects = ApplyActions[0].effectSelectors.Select(x => x.CreateEffect(this)).ToArray();

        for (int i = 0; i < ApplyCount; i++)
            Effects.Add(ApplyActions[i].effectSelectors.Select(x => x.CreateEffect(this)).ToArray());

        UpdateCustomActions();
    }

    public void LevelUp(bool temporaryAcquisition = false)
    {
        // Level ���� 
        Level++;

        if (temporaryAcquisition)
            return;

        // ��ų�� �ִ� ��ȭ�� ������ ���, �ش� ��ų�� ���� ��ų�� ��ȸ�ϸ鼭 ��ų ȹ���� �������� �Ǵ��Ͽ� 
        // �����ϸ� combinableSkills List�� �߰��ϱ� 
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

    // TargetSearcher�� SelectTarget�� ȣ���ϴ� �Լ� 
    // 1) onSelectCompletedOrNull : Select�� �Ϸ�Ǿ��� ��, ȣ���� Action
    // 2) Indicator�� ������ �� ���� 
    public void SelectTarget(Action<Skill, TargetSearcher, TargetSelectionResult> onSelectCompletedOrNull, bool isShowIndicator = true)
    {
        CancelSelectTarget();

        if (isShowIndicator)
            ShowIndicator();

        TargetSearcher.SelectTarget(Owner, Owner.gameObject, (targetSearcher, result) =>
        {
            // �������� �������� ���� Indicator�� ���� 
            if (isShowIndicator)
                HideIndicator();

            // 1) Skill�� �ʿ�� �ϴ� Type�� ������ �˻��� �����߰�,
            // 2) SearchTiming�� ������ �˻� ���Ķ��(TargetSelectionCompleted) Target �˻� ����
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

    // SelectTarget�� Action ���ڰ� ���� �����ε� �Լ�  
    public void SelectTarget(bool isShowIndicator = true) => SelectTarget(null, isShowIndicator);

    // �� isHideIndicator : Indicator Hide ���� 
    public void CancelSelectTarget(bool isHideIndicator = true)
    {
        if (!TargetSearcher.IsSearching)
            return;

        Debug.Log("CancelSelectTarget ����");

        TargetSearcher.CancelSelect();

        if (isHideIndicator)
            HideIndicator();
    }

    private void SearchTargets()
    {
        var result = TargetSearcher.SearchTargets(Owner, Owner.gameObject);

        // TargetSearcher.SearchTargets�� ����� Targets, TargetPositions�� �Ҵ��Ѵ�. 
        Targets = result.targets.Select(x => x.GetComponent<Entity>()).ToArray();

        TargetPositions = result.positions;
    }

    // Target�� ��� Select
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

    // ��ų ���� �Լ� (�÷��̾ ��ų ��ư�� ���� ��� ����)
    // �� Skill�� State�� ���� �� ������ �����Ѵٸ�(IsUseable) Skill State�� ���� �ܰ�(Preceding, InAction ...)�� �����ϰų�
    //    Ư�� State�� ����� Animation�� �����ϵ��� �Ѵ�. 
    // �� ���� Skill�� ȿ���� Action�� ����Ǳ� �� �غ��ϴ� �����̴�. 
    // �� ���� ȿ���� Action�� ������ Apply �Լ����� �����Ѵ�. 
    public bool Use()
    {
        if (!IsUseable)
        {
            return false;
        }

        // Command�� Message ���·� 'Use'�� �����Ѵ�. 
        // ( Command VS Message ���� )
        bool isUsed = StateMachine.ExecuteCommand(SkillExecuteCommand.Use) || StateMachine.SendMessage(SkillStateMessage.Use);
        // Use�� �޾Ƶ鿩����, ���ƴٴ� �ǹ̷� onUsed Event�� ȣ���ϰ�, ��� ���θ� ��ȯ
        if (isUsed)
            onUsed?.Invoke(this);

        return isUsed;
    }

    // ��ų ��� �ߵ� �Լ� 
    // �� Use �Լ��� ������ 
    // �� ���� Use�� ����ϸ� Skill�� �ٷ� ���Ǵ� �� �ƴ϶�, TargetSearcher�� SelectTarget �Լ��� �̿��ؼ� �������� ã�� �۾��� ���� �Ѵ�. 
    //    ������, UseImmediately�� ����ϸ� �������� �ٷ� Select �Ѵ�. (�������� ã�� �۾��� �پ�Ѱ� �ٷ� ���)
    // �� SkillSystem���� Skill ���� �ۿ� �ִ� ����� Skill�� ���������� ���� ��, �ڵ����� �� ��󿡰� �ٰ����� Skill�� ����ϴ� 
    //    Skill ��� ���� ��ɿ� ���ȴ�. 
    public bool UseImmediately(Vector2 position)
    {
        Debug.Assert(IsUseable, "Skill::UseImmediately - ��� ������ �������� ���߽��ϴ�.");

        SelectTargetImmediate(position);

        // UseImmediately(��� ��ų ���) ����� ����
        bool isUsed = StateMachine.ExecuteCommand(SkillExecuteCommand.UseImmediately) || StateMachine.SendMessage(SkillStateMessage.Use);
        if (isUsed)
            onUsed?.Invoke(this);

        return isUsed;
    }

    // Skill ��� ��� 
    // �� isForce : ������ ��� ���� (��� ���)
    // �� ���� ��ų�� ���� 
    public bool Cancel(bool isForce = false)
    {
        if (IsPassive)
            return false;

        // Debug.Assert(!IsPassive, "Skill::Cancel - Passive Skill�� Cancel �� �� �����ϴ�.");

        var isCanceled = isForce ? StateMachine.ExecuteCommand(SkillExecuteCommand.CancelImmediately) :
            StateMachine.ExecuteCommand(SkillExecuteCommand.Cancel);

        if (isCanceled)
            onCancelled?.Invoke(this);

        return isCanceled;
    }

    // Skill Ȱ��ȭ �Լ� 
    public void Activate()
    {
        Debug.Assert(!IsActivated, "Skill::Activate - �̹� Ȱ��ȭ�Ǿ� �ֽ��ϴ�.");

        IsActivated = true;
        onActivated?.Invoke(this);
    }

    // Skill ��Ȱ��ȭ �Լ� 
    public void Deactivate()
    {
        Debug.Assert(IsActivated, "Skill::Activate - Skill�� Ȱ��ȭ�Ǿ����� �ʽ��ϴ�.");

        IsActivated = false;
        onDeactivated?.Invoke(this);
    }

    // ���ڷ� ���� Type�� �ش�Ǵ� CustomAction���� �Լ��� ȣ�� 
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

    // �����Ǵ� CustomAction �Լ��� PrecedingAction �Լ��� ȣ�� 
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

    // �����Ǵ� CustomAction �Լ��� Action �Լ��� ȣ�� 
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

    // Skill ���� ȿ�� �ߵ� �Լ� 
    // �� isConsumeApplyCount : Apply Ƚ���� �Ҹ����� �䱸 
    // 1) false : Apply�� ����, ���� ApplyCount ���� �������� �ʰ� �״�� ���� 
    // 2) true  : Apply ����, ���� ApplyCount ���� ���� 
    public void Apply(bool isConsumeApplyCount = true)
    {
        Debug.Assert(IsInfinitelyApplicable || !isConsumeApplyCount || (CurrentApplyCount < ApplyCount),
            $"Skill({CodeName})�� �ִ� ���� Ƚ��({ApplyCount})�� �ʰ��ؼ� ������ �� �����ϴ�.");

        // ExecutionType�� Input�̰�, ApplyType�� Animation�� Select�� ���Ƿ� ����� �Ѵ�. 
        if (executionType == SkillExecutionType.Input && ApplyType == SkillApplyType.Animation)
            SelectTarget();

        // TargetSearch Ÿ�̹��� Apply��� SearchTargets ����
        if (targetSearchTimingOption == TargetSearchTimingOption.Apply ||
            targetSearchTimingOption == TargetSearchTimingOption.Both)
            SearchTargets();

        // ��ų ȿ��(Action) ���� 
        RunCustomActions(SkillCustomActoinType.Action);
        Action.Apply(this);

        // Auto�� ���� Duration���� ���� ���� ����� ���� ApplyCycle�� ���� �������� ���� ������
        // Ex. Duration = 1.001, CurrentApplyCycle = 1.001
        //     => Duration = 1.001, CurrentApplyCycle = 0.001
        if (executionType == SkillExecutionType.Auto)
            CurrentApplyCycle %= ApplyCycle;
        // Input�� ��, ������ ������ Skill�� �ߵ� Timing�� ���� ���ϱ� ������ ������ �������� �����Ƿ� 0���� �ʱ�ȭ
        else
            CurrentApplyCycle = 0f;

        if (isConsumeApplyCount)
            CurrentApplyCount++;

        onApplied?.Invoke(this, CurrentApplyCount);
    }

    // Skill�� T �������� Ȯ���ϴ� �Լ� 
    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();
    public bool IsInState<T>(int layer) where T : State<Skill> => StateMachine.IsInState<T>(layer);

    // Skill�� StateMachine���� Layer�� State Type�� �������� �Լ� 
    public Type GetCurrentStateType(int layer = 0) => StateMachine.GetCurrentStateType(layer);

    // Skill�� targetSelectionTimingOption�� ���� ��(option)�� ������ Ȯ���ϴ� �Լ�  
    // �� targetSelectionTimingOption�� Both�̰ų� ���� ���� ���ٸ� True�� ��ȯ
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
