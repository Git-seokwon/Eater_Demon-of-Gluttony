using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect : IdentifiedObject // Effect�� Database�� ������ ���̱� ������ IdentifiedObject�� ���
{
    #region �⺻ ���� 
    // ��� ������ ���� 0�̸� ������ �ǹ��Ѵ�. 
    // �� �׳� 0�̶�� ����� ���ϴ� �ͺ��� �ǹ� �ִ� �̸��� ������ ������ ���ϴ� �� �ξ� ������
    private const int kInfinity = 0;

    #region Event
    // Effect�� Start �Ǿ��� �� ȣ�� �ϴ� event
    // �� effect : �̺�Ʈ�� ȣ���� ���� effect
    public delegate void StartedHandler(Effect effect);
    // Effect�� Apply �Ǿ��� �� ȣ�� �ϴ� event
    // �� currentApplyCount : ���� applyCount
    // �� prevApplyCount : ���� applyCount
    public delegate void AppliedHandler(Effect effect, int currentApplyCount, int prevApplyCount);
    // Effect�� Release �Ǿ��� �� ȣ�� �ϴ� event
    public delegate void ReleaseHandler(Effect effect);
    // Effect�� Stack�� �ٲ���� �� ȣ��Ǵ� Event
    public delegate void StackChangedHandler(Effect effect, int currentStack, int prevStack);
    #endregion

    [SerializeField]
    private EffectType type;

    // Effect�� �ߺ� ���� ���� ���� 
    [SerializeField]
    private bool isAllowDuplicate = true;
    // Effect �ߺ� ���� ��Ȳ�� ��, ��� ó���� �������� ��Ÿ���� �ɼ�
    [SerializeField]
    private EffectRemoveDuplicateTargetOption removeDuplicateTargetOption;

    // UI�� Effect ������ ���������� ���� ����
    // ex) ����, ����� ��ų�� UI�� �����ְ�, �ܼ��� �������� �ִ� ��ų�̶�� UI�� ������ �ʿ䰡 ����. 
    [SerializeField]
    private bool isShowInUI;

    [SerializeField]
    private int maxLevel;

    // Level�� Data
    // �� Level ���� EffectData�� �����ϰ� EffectData �ȿ� Stack�� Action Data�� �����Ѵ�. 
    // �� Level�� 1���� �����ϰ� Array�� Index�� 0���� �����ϹǷ� Level�� �´� Data�� ���������� [���� Level - 1]��° Data�� �����;���
    [SerializeField]
    private EffectData[] effectDatasAllLevel;

    // ���� Data
    private EffectData currentLevelData;
    // ���� Level
    private int level = 0;

    // ���� Stack
    private int currentStack = 0;
    // ���� ���ӽð�
    private float currentDuration;
    // ���� ���� Ƚ��
    private int currentApplyCount;
    // ���� ���� Cycle
    private float currentApplyCycle;

    // Action�� Apply �Լ��� �����Ϸ� �õ��� ���� �ִ��� ����
    // �� Action�� Apply �Լ��� ����� �� true���ǰ�, Apply �Լ��� true�� return�ϸ� false�� �ʱ�ȭ ��
    // �� ���� Apply�� �õ��� ��, �� ������ true�̸� ������ Apply�� �õ��ߴµ� �����ؼ� Apply�� false�� ��ȯ�ߴٴ� �� �� �� ����
    // �� �� ���� ���� Apply �����ÿ� currentApplyCycle ������ ���� �ٸ��� �ʱ�ȭ ��
    private bool isApplyTried;

    // Stack�� ���� ���� ����� Stack Actions List
    private readonly List<EffectStackAction> appliedStackActions = new();
    #endregion

    #region �ܼ��� Get ������Ƽ
    public EffectType Type => type;
    public bool IsAllowDuplicate => isAllowDuplicate;
    public EffectRemoveDuplicateTargetOption RemoveDuplicateTargetOption => removeDuplicateTargetOption;

    public bool IsShowInUI => isShowInUI;

    public IReadOnlyList<EffectData> EffectDatas => effectDatasAllLevel;
    // StackActions�� currentData�� stackActions�� ��ȯ�Ѵ�. 

    // Effect�� ������ �ִ� Stack�� Action
    // Ex) 1�� Stack Effect 1
    //     1�� Stack Effect 2
    //     1�� Stack Effect 3
    //     2�� Stack Effect 1
    //     2�� Stack Effect 3
    //     2�� Stack Effect 5
    //     ...
    public IReadOnlyList<EffectStackAction> StackActions => currentLevelData.stackActions;
    #endregion

    #region Level ���� ������Ƽ 
    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value > 0 && value <= MaxLevel, $"Effect.Rank = {value} - value�� 0���� ũ�� MaxLevel���� ���ų� �۾ƾ��մϴ�.");

            if (value == level)
                return;

            level = value;

            // �� Last(Func<T, bool> predicate) : ������ ���տ��� Ư�� ������ ������ ������ ��Ҹ� ��ȯ
            // ���� Effect Level���� �����鼭 ���� ����� Level�� Data�� ã�ƿ�
            // ex) Data�� Level 1, 3, 5 �̷��� ���� ��, Effect�� Level�� 4�� ���, Level 3�� Data�� ã�ƿ�
            var newData = effectDatasAllLevel.Last(x => x.level <= level);
            // ex) ���� ��쿡�� ���� Level�� 3�̰� Level Up�Ͽ� 4 Level�� �Ǿ��� ����. 
            //     newData���� 3 Level Data�� �����ϰ� �ְ�, ���� Level�� 3 Level�̱� ������ currentLevelData�� �״�� �ִ´�. 
            //     ���⼭, �� ���� Level Up�Ͽ� 5 Level�� �ȴٰ� �ϸ�, newData�� ���� 5 Level Data�� �����ϰ� �ְ�, ������ if����
            //     ����Ͽ� currentLevelData�� 5 Level Data�� ����Ű�� �ȴ�. 
            if (newData.level != currentLevelData.level)
                currentLevelData = newData;
        }
    }

    // ���� level�� maxLevel�ΰ�?
    public bool IsMaxLevel => level == maxLevel;

    // ���� Effect�� EffectData�� Level ����
    // Ex) EffectData�� Level�� 1, 3, 5�� �ְ�, ���� level�� 4�� ���, Level ������Ƽ�� ���� 
    //     currentData.level�� 3�� �ȴ�. �̶� DataBonusLevel�� ���� 1�̴�. 
    // �� Action �ʿ��� Bonus Value�� �ִµ� Ȱ��
    // �� ��ų Level ������ ���� �ܼ��� ��ġ ��ȭ�� �ִ� ���, ���� Level ����ü�� �ø��� �ʰ�, DataBonusLevel ����� ���
    // Ex) 1, 3, 5 Level EffectData���� Ư���� �ɷ��� �ְ�, 2, 4 Level ���� �ܼ��� ��ġ��ȭ�� �ִ� ���, DataBonusLevel�� ����Ͽ� 
    //     �ڵ����� ��ġ ��ȭ�� �̷�� ������ �Ѵ�. 
    public int DataBonusLevel => Mathf.Max(level - currentLevelData.level, 0);
    #endregion

    #region ���� �ð� ���� ������Ƽ
    // public StatScaleFloat duration�� public float defaultValue, public Stat scaleStat�� ����Ǿ��� ������ 
    // Effect ���� ��⿡�� default ���� Stat�� ������ �� �ִ�. 
    public float Duration => currentLevelData.duration.GetValue(User.Stats);

    // Duration�� 0�̸� ���� ����
    public bool IsTimeless => Mathf.Approximately(Duration, kInfinity);

    public float CurrentDuration
    {
        get => currentDuration;
        set => currentDuration = Mathf.Clamp(value, 0f, Duration);
    }

    public float RemainDuration => Mathf.Max(0f, Duration - currentDuration);
    #endregion

    #region Stack ���� ������Ƽ
    public int MaxStack => currentLevelData.maxStack;

    public int CurrentStack
    {
        get => currentStack;
        set
        {
            var prevStack = currentStack;
            currentStack = Mathf.Clamp(value, 0, MaxStack);

            // Stack�� ���̸� currentDuration�� �ʱ�ȭ�Ͽ� Effect�� ���� �ð��� �÷���
            // �� Stack�� ��ȭ ���� ���� ���� �ð��� �ʱ�ȭ�ϴ� ���� ( '>=' )
            // �ִ� ���ÿ� ������ ���, Stack Update�� �Ͼ�� ������ �����ϰ� ���ӽð��� �ʱ�ȭ ������� �ϱ� �����̴�. 
            // Ex) �轺 Passive
            if (currentStack >= prevStack)
                currentDuration = 0f;

            if (currentStack != prevStack)
            {
                // Stack ������ ���� OnEffectStackChanged �Լ��� ���� 
                // �� null üũ ���� ( '?' )
                // ��� Effect�� Action�� ������ �ʱ� �����̴�. ���ӵ��� ���� Ư���� ȿ�� ���� ������ Stack�� ���̴� Effect���� �ִ�. 
                // �̷� Effect���� �ڱ��� Action�� ������, �ٸ� Skill�� �ó����� ����Ű�� �ȴ�. 
                // Ex) FireBall ��ų, ������ ȭ�� Effect�� 100 Stack �׿� ������ 2���� �������� �ش�.
                //     ȭ�� Effect�� �ƹ��� Action�� ������, FireBall ��ų�� �ó����� ����Ų��. (����� ��)
                Action?.OnEffectStackChanged(this, User, Target, level, currentStack, Scale);

                // �ٲ�, Stack�� ���� ������ ����� Stack ȿ���� Release�ϰ�, ���� Stack�� �´� ���ο� ȿ���� Applys
                TryApplyStackAction();
                
                // Stack ���� Event ����
                onStackChanged?.Invoke(this, currentStack, prevStack);
            }
        }
    }
    #endregion

    #region Apply�� ���õ� ������Ƽ
    public int ApplyCount => currentLevelData.applyCount;

    // ApplyCount�� kInfinity(=0)�̸� ���� ����(= �� �����Ӹ��� ����)
    public bool IsInfiniteApplicable => ApplyCount == kInfinity;

    public int CurrentApplyCount
    {
        get => currentApplyCount;
        // 1) ���� ���� : value �״�� set
        // 2) �ƴ� ��� : Mathf.Clamp(value, 0, ApplyCount)
        set => currentApplyCount = IsInfiniteApplicable ? value : Mathf.Clamp(value, 0, ApplyCount);
    }

    // �� ApplyCycle : ���� ����(�ð�)
    // �� ApplyCycle�� 0�̰� ApplyCount�� 1���� ũ�� ApplyCount�� Duration�� �̿��ؼ� ApplyCycle�� ���
    // Ex) ApplyCycle�� 0�� ��Ȳ, Duration�� 10�ʰ� ApplyCount�� 11���̸�, ó�� Effect�� ����� ��, Apply�� 1�� �̷�����
    //     ���� ApplyCount�� ApplyCount - 1 = 10�̴�. �׷��� 10(Duration) / 10(ApplyCount - 1) = 1, �� ApplyCycle = 1��
    public float ApplyCycle => Mathf.Approximately(currentLevelData.applyCycle, 0f) && ApplyCount > 1
        ? (Duration / (ApplyCount - 1)) : currentLevelData.applyCycle;

    public float CurrentApplyCycle
    {
        get => currentApplyCycle;
        set => currentApplyCycle = Mathf.Clamp(value, 0f, ApplyCycle);
    }
    #endregion

    #region Action, Owner, Target, Scale, Description
    // Effect ��ü�� ������ ��ü ȿ��(Action)
    // �� Ư�� ������ ��ų�� ���, Action�� ���� StackActions�� ���� �� �ִ�. 
    private EffectAction Action => currentLevelData.action;

    private CustomAction[] CustomActions => currentLevelData.customActions;

    // Effect�� ������ �ִ� ������ : Skill, Item �� (�⺻������ Skill)
    public object Owner { get; private set; }

    // Effect�� ����� (Skill�� �����ϰ� �ִ� Enitity)
    public Entity User { get; private set; }

    // Effect�� ����޴� Target Entity
    public Entity Target { get; private set; }

    // Scale ������ ���� Effect�� ������ ������ �� ����
    // �� Chargeó�� Casting �ð��� ���� ������ �޶����� Skill�� Ȱ���� �� ����
    public float Scale { get; set; }

    // �� 0 : ������ ���� Effect�� Skill�� ���� Effect �� �� ��°������ ��Ÿ���� ���ε�, Description ������Ƽ��
    //        �����ϰ� ���� Effect ��ü���� �����ϴ� Text�� ������ ù�� °�� �ǹ��ϴ� 0�� ����
    public override string Description => BuildDescription(base.Description, 0, 0);
    #endregion

    #region ���� ���� üũ �Լ�
    private bool IsApplyAllWhenDurationExpires => currentLevelData.isApplyAllWhenDurationExpires;

    // Effect�� ���ӽð��� �������� ���� 
    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);

    // ���� Ƚ���� �� ä������ ���� 
    private bool IsApplyCompleted => !IsInfiniteApplicable && CurrentApplyCount == ApplyCount;

    // Effect�� �Ϸ� ����
    // 1) ���� �ð��� �����ų� 
    // 2) ���� Ƚ���� �� ä���ų�
    public bool IsFinished => IsDurationEnded ||
        (currentLevelData.runningFinishOption == EffectRunningFinishOption.FinishWhenApplyComplted && IsApplyCompleted);

    // �� �Ϸ� ���ο� ������� ������ Effect�� ����Ǿ����� Ȯ���ϱ� ���� Property (Release �Լ��� ȣ��Ǿ����� ����)
    // �� Effect�� Release �Լ��� ����Ǹ�(= Effect�� ����Ǹ�) True
    // �� IsFinished Property�� Effect�� ������ �Ϸ�Ǿ�߸� True�ιݸ�, IsReleased�� ���𰡿� ���� Effect�� ���ŵǾ True
    public bool IsReleased {  get; private set; }

    // Effect�� ���� Apply ������ ���������� ���� ���� 
    // �� ���� Ƚ���� ���Ұ�, ������ Time�� ������ true
    public bool IsApplicable => Action != null &&
        (CurrentApplyCount < ApplyCount || ApplyCount == kInfinity) &&
         CurrentApplyCycle >= ApplyCycle;

    public event StartedHandler onStarted;
    public event AppliedHandler onApplied;
    public event ReleaseHandler onReleased;
    public event StackChangedHandler onStackChanged;
    #endregion

    #region Set �Լ�
    public void Setup(object owner, Entity user, int level, float scale = 1f)
    {
        Owner = owner;
        User = user;
        Level = level;

        // CurrentApplyCycle�� ApplyCycle�� �����Ͽ� ��ų ���� ��� Effect�� ����ǵ��� �Ѵ�.
        if (currentLevelData.startDelayByApplyCycle == EffectStartDelayByApplyCycle.Instant)
            CurrentApplyCycle = ApplyCycle;

        Scale = scale;
    }

    public void SetTarget(Entity target) => Target = target;
    #endregion

    #region Release & Apply �Լ�
    // ���� ����� ��� StackAction���� Release + Clear
    private void ReleaseStackActionAll()
    {
        appliedStackActions.ForEach(x => x.Release(this, level, User, Target, Scale));
        appliedStackActions.Clear();
    }

    // ���� ����� StackAction�鿡�� ���ǿ� �´� StackAction���� ã�� Release + Remove
    private void ReleaseStackActions(Func<EffectStackAction, bool> action)
    {
        // �� Linq.Where : https://developer-talk.tistory.com/565
        var stackActions = appliedStackActions.Where(action).ToList();
        foreach (var stackAction in stackActions)
        {
            stackAction.Release(this, level, User, Target, Scale);
            appliedStackActions.Remove(stackAction);
        }
    }

    // ���� ����� StackAction�� �� �� �̻� ���ǿ� ���� �ʴ� StackAction���� Release�ϰ�
    // ���ǿ� �´� StackAction���� Apply�ϴ� �Լ� 
    private void TryApplyStackAction()
    {
        // ����� StackAction�� �� ���� Stack���� �� ū Stack�� �䱸�ϴ� StackAction���� Release��.
        // �� � ������ ���� Stack ���� �������� ���� ���� ó��
        ReleaseStackActions(x => x.Stack > currentStack);

        // �� ���� ������ StackAction ���
        // 1) StackAction�� �߿��� �ʿ��� Stack ���� �����ǰ� (�ʿ� Stack�� ���� Stack���� �۰ų� ����)
        // 2) ���� ���������� �ʰ�
        // 3) Effect ��ü�� ���� ���� �����ϴٸ�
        var stackActions = StackActions.Where(x => x.Stack <= currentStack && !appliedStackActions.Contains(x) && IsApplicable);

        // ���� ����� StackActions�� ���� ������ StackActions �߿��� ���� ���� Stack ���� �����´�.
        // �� �� �� currentStack �����̴�. ��, �ּ� 1 ~ �ִ� currentStack
        // �� ~.Any() : �ڷᱸ���� ��Ұ� �ִ��� ������ �˻�, ������ ':'�� �̵��Ͽ� 0�̶�� ���� �Ҵ�ȴ�. 
        int appliedStackHighestStack = appliedStackActions.Any() ? appliedStackActions.Max(x => x.Stack) : 0;
        int stackActionsHighestStack = stackActions.Any() ? stackActions.Max(x => x.Stack) : 0;

        // �� �� �� ū ���� highestStack�̶�� �̸����� �������� 
        // �� ���� ������ �� �ִ� StackAction�� �ִ� Stack ���̴�. ��, �ش� �� ���Ϸθ� StackAction�� ����� �� �ִ�. 
        var highestStack = Mathf.Max(appliedStackHighestStack, stackActionsHighestStack);
        if (highestStack > 0)
        {
            // ���� ������ StackActions �� Stack�� highestStack���� ����, IsReleaseOnNextApply�� true�� StackAction���� ����
            // �� IsReleaseOnNextApply�� true�� StackAction�� �� ���� Stack�� ���� StackAction�� �����Ѵٸ� Release �Ǿ� �ϱ� ������ 
            //    ���ʿ� ���� ��Ͽ��� ������ �ش�. 
            // �� stackActions���� x.Stack <= currentStack�� ��� StackAction�� �ֱ� ������ currentStack ������ Stack�� ���� 
            //    ���� ������ StackAction�鵵 �ִ�. 
            // Ex) currentStack = 3�� ��Ȳ���� Stack�� 1, 2, 3�� EffectStackAction ������ �� �����ϴ�. �̶� �ش� EffectStackAction����
            //     IsReleaseOnNextApply�� true�̸� 1, 2 EffectStackAction�� �����ؾ� �Ѵ�. 
            var except = stackActions.Where(x => x.Stack < highestStack && x.IsReleaseOnNextApply);
            stackActions = stackActions.Except(except);
        }

        if (stackActions.Any())
        {
            ReleaseStackActions(x => x.Stack < currentStack && x.IsReleaseOnNextApply);

            foreach (var stackAction in stackActions)
                stackAction.Apply(this, level, User, Target, Scale);

            // �� List<T>.AddRange : �־��� ��� List�� ��Ҹ� ���� ��� ���� �߰�
            appliedStackActions.AddRange(stackActions);
        }
    }
    #endregion

    #region Action ���� �Լ� ex) Start, Update ...
    // Effect�� ó�� ����Ǳ� ���� ����Ǵ� �Լ� 
    public void Start()
    {
        Debug.Assert(!IsReleased, "Effect::Start - �̹� ����� Effect�Դϴ�.");

        // Effect�� ���۵� ��, ȣ��Ǵ� ���� �Լ� 
        Action?.Start(this, User, Target, Level, Scale);

        // �ʿ� Stack�� 1�� StackAction���� ����
        TryApplyStackAction();

        foreach (var customAction in CustomActions)
            customAction.Start(this);

        onStarted?.Invoke(this);
    }

    // Effect�� �� �����Ӹ��� �����ϴ� �Լ� 
    public void Update()
    {
        CurrentDuration += Time.deltaTime;
        currentApplyCycle += Time.deltaTime;

        if (IsApplicable)
            Apply();

        // �� IsApplyAllWhenDurationExpires : Effect�� ���� �ð��� ����Ǿ��� ��, ���� ���� Ƚ���� �ִٸ� ��� ������ ������ ����
        // ���� �ð��� ������ ���� ���� ������ �ƴ϶�� ���� ���� Ƚ����ŭ ȿ���� ����
        if (IsApplyAllWhenDurationExpires && IsDurationEnded && !IsInfiniteApplicable)
        {
            for (int i = currentApplyCount; i < ApplyCount; i++)
                Apply();
        }
    }
    
    // ȿ�� ����
    public void Apply()
    {
        Debug.Assert(!IsReleased, "Effect::Apply - �̹� ����� Effect�Դϴ�.");

        if (Action == null)
            return;

        if (Action.Apply(this, User, Target, level, currentStack, Scale))
        {
            foreach (var customAction in CustomActions)
            {
                customAction.Run(this);
            }

            var prevApplyCount = CurrentApplyCount++;

            // �� Time Sync ���� 
            // �� Skill�� Duration�� 1�ʰ�, ApplyCycle�� 0.5�ʰ�, ApplyCount�� 3ȸ���
            //    ó���� �� �� ����ǰ�, 0.5�� �ڿ� �� �� ����ǰ�, �ٽ� 0.5�� �ڿ� ����Ǿ�� �Ѵ�. 
            // �� ������, Update �Լ����� CurrentDuration�� currentApplyCycle�� Time.deltaTime�� 
            //    ���ϱ� ������ �ణ�� ������ ������ �� �ִ�. 
            // Ex) CurrentDuration, currentApplyCycle : 0.4988
            //     += Time.deltaTime
            //     CurrentDuration, currentApplyCycle : 0.5012
            // �̶�, currentApplyCycle�� �ܼ��� 0���� �ʱ�ȭ �����ָ� 
            //     CurrentDuration : 0.5012
            //     currentApplyCycle : 0
            // �� �ǰ�, �� ���¿��� CurrentDuration�� 1�� �Ǹ� currentApplyCycle�� 0.4988�� �Ǿ� ����Ǿ�� �� Action�� �������
            // ���ϰ�, ������ �ȴ�. �׷��� ���� ���� 0.0012�� �ʱ�ȭ ����� �Ѵ�. ( currentApplyCycle %= ApplyCycle )
            if (isApplyTried)
                // isApplyTried�� true�̸� ������ Apply�� �õ�������, ��� ������ �������� ���� �����ߴٴ� ��
                // ��, �ֱ������� ��� ����Ǵ� Effect�� �ƴ϶� Ư���� ������ ������ �� ����Ǵ� Effect��� ��
                // �� �� ��쿡�� ������ �����ؼ� Effect�� �ߵ����� ��, 0���� �ʱ�ȭ�����ִ� ���� �´�.
                currentApplyCycle = 0f;
            else
                currentApplyCycle %= ApplyCycle;

            isApplyTried = false;

            onApplied?.Invoke(this, CurrentApplyCount, prevApplyCount);
        }
        else
            isApplyTried = true;
        // Ex) ĳ���Ͱ� �׾��� �� �ڵ� ��Ȱ��Ű�� ���� ȿ���� ���, ĳ���Ͱ� �׾���� ��Ȱ�� ��ų �� �ֱ� ������
        //     ĳ���Ͱ� ���� �� ���� ��� false((Action.Apply(...))�� ��ȯ�ϴٰ� ĳ���Ͱ� ������ true�� ��ȯ
    }

    // Effect�� ������ ��, ȣ��Ǵ� �Լ�
    public void Release()
    {
        Debug.Assert(!IsReleased, "Effect::Release - �̹� ����� Effect�Դϴ�.");

        Debug.Log("���� 4");

        Action?.Release(this, User, Target, level, Scale);
        ReleaseStackActionAll();

        foreach (var customAction in CustomActions)
            customAction.Release(this);

        IsReleased = true;

        onReleased?.Invoke(this);
    }
    #endregion

    #region GetData, BuildDescription �Լ�
    public EffectData GetData(int level) => effectDatasAllLevel[level - 1];

    // ���ڷ� ���� Text���� Mark���� Effect�� Data�� Replace�Ͽ� return ���ִ� �Լ� 
    // �� effectIndex : �ش� Effect�� Skill�� Effect ��Ͽ� �ִ� ����
    public string BuildDescription(string description, int skillIndex, int effectIndex)
    {
        Dictionary<string, string> stringByKeyword = new Dictionary<string, string>()
        {
            { "duration", Duration.ToString("0.##") },
            { "applyCount", ApplyCount.ToString() },
            { "applyCycle", ApplyCycle.ToString("0.##") }
        };

        // prefix�� skillIndex�� �ٿ��� + suffix(���̾�)�� effectIndex�� �ٿ���
        description = TextReplacer.Replace(description, skillIndex.ToString(), stringByKeyword, effectIndex.ToString());   

        // Effect�� �⺻ Action�� ������ EffectAction.BuildDescription �Լ��� Replace �ϱ�
        // �� �⺻ Action�̱� ������ stackActionIndex�� stack�� 0���� set
        description = Action.BuildDescription(this, description, skillIndex, 0, 0, effectIndex);

        // StackActions�� Stack�� �������� Groupȭ �Ѵ�.
        // �� ���� Stack�� ���� StackAction�� ���� ���´�.
        // �� Key�� Stack, Value�� EffectStackAction List�� Dictionary�� �����ȴ�.
        var stackGroups = StackActions.GroupBy(x => x.Stack);
        foreach (var group in stackGroups)
        {
            int i = 0;

            // Group�ȿ� �ִ� StackAction�� BuildDescription �Լ��� ����
            // Ex) Stack 3�� StackAction 3��, Stack 4�� StackAction 3���� �ִٰ� ����
            //     Key�� 3�� Group�� Key�� 4�� Group, �� 2���� Group�� �����.
            //     Ket�� 3�� Group���� ���ʴ�� stackActionIndex�� 0, 1, 2��� ���� ���� 
            //     Ket�� 4�� Group���� ���ʴ�� stackActionIndex�� 0, 1, 2��� ���� ���� 
            foreach (var stackAction in group)
                description = stackAction.BuildDescription(this, description, skillIndex, i++, effectIndex);
        }

        return description;
    }
    #endregion

    public override object Clone()
    {
        var clone = Instantiate(this);

        // Owner�� �����Ѵٸ� clone�� ���� Effect�� Data��� Setup ���ְ� return �Ѵ�.
        if (Owner != null)
            clone.Setup(Owner, User, Level, Scale);

        return clone;
    }
}
