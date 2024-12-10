using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect : IdentifiedObject // Effect는 Database로 관리할 것이기 때문에 IdentifiedObject를 상속
{
    #region 기본 변수 
    // 몇몇 변수는 값이 0이면 무한을 의미한다. 
    // → 그냥 0이라는 상수와 비교하는 것보다 의미 있는 이름을 가지는 변수와 비교하는 게 훨씬 직관적
    private const int kInfinity = 0;

    #region Event
    // Effect가 Start 되었을 때 호출 하는 event
    // ※ effect : 이벤트를 호출한 현재 effect
    public delegate void StartedHandler(Effect effect);
    // Effect가 Apply 되었을 때 호출 하는 event
    // ※ currentApplyCount : 현재 applyCount
    // ※ prevApplyCount : 이전 applyCount
    public delegate void AppliedHandler(Effect effect, int currentApplyCount, int prevApplyCount);
    // Effect가 Release 되었을 때 호출 하는 event
    public delegate void ReleaseHandler(Effect effect);
    // Effect의 Stack이 바뀌었을 때 호출되는 Event
    public delegate void StackChangedHandler(Effect effect, int currentStack, int prevStack);
    #endregion

    [SerializeField]
    private EffectType type;

    // Effect의 중복 적용 가능 여부 
    [SerializeField]
    private bool isAllowDuplicate = true;
    // Effect 중복 적용 상황일 때, 어떻게 처리할 것인지를 나타내는 옵션
    [SerializeField]
    private EffectRemoveDuplicateTargetOption removeDuplicateTargetOption;

    // UI로 Effect 정보를 보여줄지에 대한 여부
    // ex) 버프, 디버프 스킬은 UI를 보여주고, 단순히 데미지를 주는 스킬이라면 UI를 보여줄 필요가 없다. 
    [SerializeField]
    private bool isShowInUI;

    [SerializeField]
    private int maxLevel;

    // Level별 Data
    // → Level 별로 EffectData가 존재하고 EffectData 안에 Stack별 Action Data가 존재한다. 
    // → Level은 1부터 시작하고 Array의 Index는 0부터 시작하므로 Level에 맞는 Data를 가져오려면 [현재 Level - 1]번째 Data를 가져와야함
    [SerializeField]
    private EffectData[] effectDatasAllLevel;

    // 현재 Data
    private EffectData currentLevelData;
    // 현재 Level
    private int level = 0;

    // 현재 Stack
    private int currentStack = 0;
    // 현재 지속시간
    private float currentDuration;
    // 현재 적용 횟수
    private int currentApplyCount;
    // 현재 적용 Cycle
    private float currentApplyCycle;

    // Action의 Apply 함수를 실행하려 시도한 적이 있는지 여부
    // → Action의 Apply 함수가 실행될 때 true가되고, Apply 함수가 true를 return하면 false로 초기화 됨
    // → 다음 Apply를 시도할 때, 이 변수가 true이면 이전에 Apply를 시도했는데 실패해서 Apply가 false를 반환했다는 걸 알 수 있음
    // → 이 값에 따라 Apply 성공시에 currentApplyCycle 변수의 값을 다르게 초기화 함
    private bool isApplyTried;

    // Stack에 따른 현재 적용된 Stack Actions List
    private readonly List<EffectStackAction> appliedStackActions = new();
    #endregion

    #region 단순한 Get 프로퍼티
    public EffectType Type => type;
    public bool IsAllowDuplicate => isAllowDuplicate;
    public EffectRemoveDuplicateTargetOption RemoveDuplicateTargetOption => removeDuplicateTargetOption;

    public bool IsShowInUI => isShowInUI;

    public IReadOnlyList<EffectData> EffectDatas => effectDatasAllLevel;
    // StackActions는 currentData의 stackActions를 반환한다. 

    // Effect가 가지고 있는 Stack별 Action
    // Ex) 1번 Stack Effect 1
    //     1번 Stack Effect 2
    //     1번 Stack Effect 3
    //     2번 Stack Effect 1
    //     2번 Stack Effect 3
    //     2번 Stack Effect 5
    //     ...
    public IReadOnlyList<EffectStackAction> StackActions => currentLevelData.stackActions;
    #endregion

    #region Level 관련 프로퍼티 
    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value > 0 && value <= MaxLevel, $"Effect.Rank = {value} - value는 0보다 크고 MaxLevel보다 같거나 작아야합니다.");

            if (value == level)
                return;

            level = value;

            // ※ Last(Func<T, bool> predicate) : 데이터 집합에서 특정 조건을 가지는 마지막 요소를 반환
            // 현재 Effect Level보다 작으면서 가장 가까운 Level인 Data를 찾아옴
            // ex) Data가 Level 1, 3, 5 이렇게 있을 때, Effect의 Level이 4일 경우, Level 3의 Data를 찾아옴
            var newData = effectDatasAllLevel.Last(x => x.level <= level);
            // ex) 위의 경우에서 현재 Level이 3이고 Level Up하여 4 Level이 되었다 하자. 
            //     newData에는 3 Level Data를 참조하고 있고, 현재 Level도 3 Level이기 때문에 currentLevelData는 그대로 있는다. 
            //     여기서, 한 번더 Level Up하여 5 Level이 된다고 하면, newData는 이제 5 Level Data를 참조하고 있고, 이제야 if문을
            //     통과하여 currentLevelData가 5 Level Data를 가리키게 된다. 
            if (newData.level != currentLevelData.level)
                currentLevelData = newData;
        }
    }

    // 현재 level인 maxLevel인가?
    public bool IsMaxLevel => level == maxLevel;

    // 현재 Effect와 EffectData의 Level 차이
    // Ex) EffectData의 Level이 1, 3, 5가 있고, 현재 level이 4인 경우, Level 프로퍼티에 의해 
    //     currentData.level은 3이 된다. 이때 DataBonusLevel의 값은 1이다. 
    // → Action 쪽에서 Bonus Value를 주는데 활용
    // → 스킬 Level 증가에 따라 단순한 수치 강화만 있는 경우, 굳이 Level 구조체를 늘리지 않고, DataBonusLevel 기능을 사용
    // Ex) 1, 3, 5 Level EffectData에만 특수한 능력이 있고, 2, 4 Level 에는 단순한 수치강화만 있는 경우, DataBonusLevel를 사용하여 
    //     자동으로 수치 강화가 이루어 지도록 한다. 
    public int DataBonusLevel => Mathf.Max(level - currentLevelData.level, 0);
    #endregion

    #region 지속 시간 관련 프로퍼티
    // public StatScaleFloat duration은 public float defaultValue, public Stat scaleStat로 선언되었기 때문에 
    // Effect 생성 모듈에서 default 값과 Stat을 설정할 수 있다. 
    public float Duration => currentLevelData.duration.GetValue(User.Stats);

    // Duration이 0이면 무한 지속
    public bool IsTimeless => Mathf.Approximately(Duration, kInfinity);

    public float CurrentDuration
    {
        get => currentDuration;
        set => currentDuration = Mathf.Clamp(value, 0f, Duration);
    }

    public float RemainDuration => Mathf.Max(0f, Duration - currentDuration);
    #endregion

    #region Stack 관련 프로퍼티
    public int MaxStack => currentLevelData.maxStack;

    public int CurrentStack
    {
        get => currentStack;
        set
        {
            var prevStack = currentStack;
            currentStack = Mathf.Clamp(value, 0, MaxStack);

            // Stack이 쌓이면 currentDuration을 초기화하여 Effect의 지속 시간을 늘려줌
            // ※ Stack이 변화 없을 때도 지속 시간을 초기화하는 이유 ( '>=' )
            // 최대 스택에 도달한 경우, Stack Update가 일어나면 기존과 동일하게 지속시간을 초기화 시켜줘야 하기 때문이다. 
            // Ex) 잭스 Passive
            if (currentStack >= prevStack)
                currentDuration = 0f;

            if (currentStack != prevStack)
            {
                // Stack 변동에 따라 OnEffectStackChanged 함수를 실행 
                // ※ null 체크 이유 ( '?' )
                // 모든 Effect가 Action을 가지진 않기 때문이다. 게임들을 보면 특별한 효과 없이 오로지 Stack만 쌓이는 Effect들이 있다. 
                // 이런 Effect들은 자기의 Action은 없지만, 다른 Skill과 시너지를 일으키게 된다. 
                // Ex) FireBall 스킬, 적에게 화상 Effect가 100 Stack 쌓여 있으면 2배의 데미지를 준다.
                //     화상 Effect는 아무런 Action도 없지만, FireBall 스킬과 시너지를 일으킨다. (사신의 낫)
                Action?.OnEffectStackChanged(this, User, Target, level, currentStack, Scale);

                // 바뀐, Stack에 따라 기존에 적용된 Stack 효과를 Release하고, 현재 Stack에 맞는 새로운 효과를 Applys
                TryApplyStackAction();
                
                // Stack 변경 Event 실행
                onStackChanged?.Invoke(this, currentStack, prevStack);
            }
        }
    }
    #endregion

    #region Apply와 관련된 프로퍼티
    public int ApplyCount => currentLevelData.applyCount;

    // ApplyCount가 kInfinity(=0)이면 무한 적용(= 매 프레임마다 적용)
    public bool IsInfiniteApplicable => ApplyCount == kInfinity;

    public int CurrentApplyCount
    {
        get => currentApplyCount;
        // 1) 무한 적용 : value 그대로 set
        // 2) 아닌 경우 : Mathf.Clamp(value, 0, ApplyCount)
        set => currentApplyCount = IsInfiniteApplicable ? value : Mathf.Clamp(value, 0, ApplyCount);
    }

    // ※ ApplyCycle : 적용 간격(시간)
    // → ApplyCycle이 0이고 ApplyCount가 1보다 크면 ApplyCount와 Duration을 이용해서 ApplyCycle을 계산
    // Ex) ApplyCycle이 0인 상황, Duration이 10초고 ApplyCount가 11번이면, 처음 Effect가 적용될 때, Apply가 1번 이뤄져서
    //     남은 ApplyCount인 ApplyCount - 1 = 10이다. 그래서 10(Duration) / 10(ApplyCount - 1) = 1, 즉 ApplyCycle = 1초
    public float ApplyCycle => Mathf.Approximately(currentLevelData.applyCycle, 0f) && ApplyCount > 1
        ? (Duration / (ApplyCount - 1)) : currentLevelData.applyCycle;

    public float CurrentApplyCycle
    {
        get => currentApplyCycle;
        set => currentApplyCycle = Mathf.Clamp(value, 0f, ApplyCycle);
    }
    #endregion

    #region Action, Owner, Target, Scale, Description
    // Effect 자체가 가지고 자체 효과(Action)
    // → 특정 스택형 스킬의 경우, Action이 없고 StackActions만 있을 수 있다. 
    private EffectAction Action => currentLevelData.action;

    private CustomAction[] CustomActions => currentLevelData.customActions;

    // Effect를 가지고 있던 소유주 : Skill, Item 등 (기본적으로 Skill)
    public object Owner { get; private set; }

    // Effect의 사용자 (Skill을 소유하고 있는 Enitity)
    public Entity User { get; private set; }

    // Effect를 적용받는 Target Entity
    public Entity Target { get; private set; }

    // Scale 조절을 통해 Effect의 위력을 조절할 수 있음
    // → Charge처럼 Casting 시간에 따라 위력이 달라지는 Skill에 활용할 수 있음
    public float Scale { get; set; }

    // ※ 0 : 원래는 현재 Effect가 Skill이 가진 Effect 중 몇 번째인지를 나타내는 것인데, Description 프로퍼티는
    //        순수하게 현재 Effect 자체만을 설명하는 Text라 무조건 첫번 째를 의미하는 0을 넣음
    public override string Description => BuildDescription(base.Description, 0, 0);
    #endregion

    #region 상태 여부 체크 함수
    private bool IsApplyAllWhenDurationExpires => currentLevelData.isApplyAllWhenDurationExpires;

    // Effect의 지속시간이 끝났는지 여부 
    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);

    // 적용 횟수를 다 채웠는지 여부 
    private bool IsApplyCompleted => !IsInfiniteApplicable && CurrentApplyCount == ApplyCount;

    // Effect의 완료 여부
    // 1) 지속 시간이 끝났거나 
    // 2) 적용 횟수를 다 채웠거나
    public bool IsFinished => IsDurationEnded ||
        (currentLevelData.runningFinishOption == EffectRunningFinishOption.FinishWhenApplyComplted && IsApplyCompleted);

    // ※ 완료 여부와 상관없이 순수히 Effect가 종료되었는지 확인하기 위한 Property (Release 함수가 호출되었는지 여부)
    // → Effect의 Release 함수가 실행되면(= Effect가 종료되면) True
    // → IsFinished Property가 Effect가 온전히 완료되어야만 True인반면, IsReleased는 무언가에 의해 Effect가 제거되어도 True
    public bool IsReleased {  get; private set; }

    // Effect가 현재 Apply 가능한 상태인지에 대한 여부 
    // → 적용 횟수가 남았고, 적용할 Time이 됐으면 true
    public bool IsApplicable => Action != null &&
        (CurrentApplyCount < ApplyCount || ApplyCount == kInfinity) &&
         CurrentApplyCycle >= ApplyCycle;

    public event StartedHandler onStarted;
    public event AppliedHandler onApplied;
    public event ReleaseHandler onReleased;
    public event StackChangedHandler onStackChanged;
    #endregion

    #region Set 함수
    public void Setup(object owner, Entity user, int level, float scale = 1f)
    {
        Owner = owner;
        User = user;
        Level = level;

        // CurrentApplyCycle에 ApplyCycle를 대입하여 스킬 사용시 즉시 Effect가 적용되도록 한다.
        if (currentLevelData.startDelayByApplyCycle == EffectStartDelayByApplyCycle.Instant)
            CurrentApplyCycle = ApplyCycle;

        Scale = scale;
    }

    public void SetTarget(Entity target) => Target = target;
    #endregion

    #region Release & Apply 함수
    // 현재 적용된 모든 StackAction들을 Release + Clear
    private void ReleaseStackActionAll()
    {
        appliedStackActions.ForEach(x => x.Release(this, level, User, Target, Scale));
        appliedStackActions.Clear();
    }

    // 현재 적용된 StackAction들에서 조건에 맞는 StackAction들을 찾아 Release + Remove
    private void ReleaseStackActions(Func<EffectStackAction, bool> action)
    {
        // ※ Linq.Where : https://developer-talk.tistory.com/565
        var stackActions = appliedStackActions.Where(action).ToList();
        foreach (var stackAction in stackActions)
        {
            stackAction.Release(this, level, User, Target, Scale);
            appliedStackActions.Remove(stackAction);
        }
    }

    // 현재 적용된 StackAction들 중 더 이상 조건에 맞지 않는 StackAction들은 Release하고
    // 조건에 맞는 StackAction들을 Apply하는 함수 
    private void TryApplyStackAction()
    {
        // 적용된 StackAction들 중 현재 Stack보다 더 큰 Stack을 요구하는 StackAction들을 Release함.
        // → 어떤 이유에 의해 Stack 수가 떨어졌을 때를 위한 처리
        ReleaseStackActions(x => x.Stack > currentStack);

        // ※ 적용 가능한 StackAction 목록
        // 1) StackAction들 중에서 필요한 Stack 수가 충족되고 (필요 Stack이 현재 Stack보다 작거나 같고)
        // 2) 현재 적용중이지 않고
        // 3) Effect 자체가 현재 적용 가능하다면
        var stackActions = StackActions.Where(x => x.Stack <= currentStack && !appliedStackActions.Contains(x) && IsApplicable);

        // 현재 적용된 StackActions와 적용 가능한 StackActions 중에서 가장 높은 Stack 값을 가져온다.
        // → 둘 다 currentStack 이하이다. 즉, 최소 1 ~ 최대 currentStack
        // ※ ~.Any() : 자료구조에 요소가 있는지 없는지 검사, 없으면 ':'로 이동하여 0이라는 값이 할당된다. 
        int appliedStackHighestStack = appliedStackActions.Any() ? appliedStackActions.Max(x => x.Stack) : 0;
        int stackActionsHighestStack = stackActions.Any() ? stackActions.Max(x => x.Stack) : 0;

        // 둘 중 더 큰 값을 highestStack이라는 이름으로 가져오기 
        // → 현재 적용할 수 있는 StackAction의 최대 Stack 값이다. 즉, 해당 값 이하로만 StackAction이 적용될 수 있다. 
        var highestStack = Mathf.Max(appliedStackHighestStack, stackActionsHighestStack);
        if (highestStack > 0)
        {
            // 적용 가능한 StackActions 중 Stack이 highestStack보다 낮고, IsReleaseOnNextApply가 true인 StackAction들을 제외
            // → IsReleaseOnNextApply가 true인 StackAction은 더 높은 Stack을 가진 StackAction이 존재한다면 Release 되야 하기 때문에 
            //    애초에 적용 목록에서 제거해 준다. 
            // → stackActions에는 x.Stack <= currentStack인 모든 StackAction이 있기 때문에 currentStack 이하의 Stack을 가진 
            //    같은 종류의 StackAction들도 있다. 
            // Ex) currentStack = 3인 상황에서 Stack이 1, 2, 3인 EffectStackAction 정보가 다 존재하다. 이때 해당 EffectStackAction에서
            //     IsReleaseOnNextApply가 true이면 1, 2 EffectStackAction을 삭제해야 한다. 
            var except = stackActions.Where(x => x.Stack < highestStack && x.IsReleaseOnNextApply);
            stackActions = stackActions.Except(except);
        }

        if (stackActions.Any())
        {
            ReleaseStackActions(x => x.Stack < currentStack && x.IsReleaseOnNextApply);

            foreach (var stackAction in stackActions)
                stackAction.Apply(this, level, User, Target, Scale);

            // ※ List<T>.AddRange : 주어진 모든 List의 요소를 기존 목록 끝에 추가
            appliedStackActions.AddRange(stackActions);
        }
    }
    #endregion

    #region Action 관련 함수 ex) Start, Update ...
    // Effect가 처음 적용되기 전에 실행되는 함수 
    public void Start()
    {
        Debug.Assert(!IsReleased, "Effect::Start - 이미 종료된 Effect입니다.");

        // Effect가 시작될 때, 호출되는 시작 함수 
        Action?.Start(this, User, Target, Level, Scale);

        // 필요 Stack이 1인 StackAction들을 적용
        TryApplyStackAction();

        foreach (var customAction in CustomActions)
            customAction.Start(this);

        onStarted?.Invoke(this);
    }

    // Effect가 매 프레임마다 실행하는 함수 
    public void Update()
    {
        CurrentDuration += Time.deltaTime;
        currentApplyCycle += Time.deltaTime;

        if (IsApplicable)
            Apply();

        // ※ IsApplyAllWhenDurationExpires : Effect의 지속 시간이 만료되었을 때, 남은 적용 횟수가 있다면 모두 적용할 것인지 여부
        // 지속 시간이 끝났고 무한 적용 설정이 아니라면 남은 적용 횟수만큼 효과를 적용
        if (IsApplyAllWhenDurationExpires && IsDurationEnded && !IsInfiniteApplicable)
        {
            for (int i = currentApplyCount; i < ApplyCount; i++)
                Apply();
        }
    }
    
    // 효과 적용
    public void Apply()
    {
        Debug.Assert(!IsReleased, "Effect::Apply - 이미 종료된 Effect입니다.");

        if (Action == null)
            return;

        if (Action.Apply(this, User, Target, level, currentStack, Scale))
        {
            foreach (var customAction in CustomActions)
            {
                customAction.Run(this);
            }

            var prevApplyCount = CurrentApplyCount++;

            // ※ Time Sync 문제 
            // → Skill의 Duration이 1초고, ApplyCycle이 0.5초고, ApplyCount가 3회라면
            //    처음에 한 번 적용되고, 0.5초 뒤에 한 번 적용되고, 다시 0.5초 뒤에 적용되어야 한다. 
            // → 하지만, Update 함수에서 CurrentDuration과 currentApplyCycle은 Time.deltaTime을 
            //    더하기 때문에 약간의 오차가 존재할 수 있다. 
            // Ex) CurrentDuration, currentApplyCycle : 0.4988
            //     += Time.deltaTime
            //     CurrentDuration, currentApplyCycle : 0.5012
            // 이때, currentApplyCycle을 단순히 0으로 초기화 시켜주면 
            //     CurrentDuration : 0.5012
            //     currentApplyCycle : 0
            // 이 되고, 이 상태에서 CurrentDuration이 1이 되면 currentApplyCycle은 0.4988이 되어 적용되어야 할 Action이 적용되지
            // 못하고, 끝나게 된다. 그래서 오차 값인 0.0012로 초기화 해줘야 한다. ( currentApplyCycle %= ApplyCycle )
            if (isApplyTried)
                // isApplyTried가 true이면 이전에 Apply를 시도했지만, 어떠한 조건을 만족하지 못해 실패했다는 것
                // 즉, 주기적으로 계속 적용되는 Effect가 아니라 특정한 조건을 만족할 때 적용되는 Effect라는 것
                // → 이 경우에는 조건을 만족해서 Effect가 발동했을 때, 0으로 초기화시켜주는 것이 맞다.
                currentApplyCycle = 0f;
            else
                currentApplyCycle %= ApplyCycle;

            isApplyTried = false;

            onApplied?.Invoke(this, CurrentApplyCount, prevApplyCount);
        }
        else
            isApplyTried = true;
        // Ex) 캐릭터가 죽었을 때 자동 부활시키는 버프 효과의 경우, 캐릭터가 죽어야지 부활을 시킬 수 있기 때문에
        //     캐릭터가 죽을 때 까지 계속 false((Action.Apply(...))를 반환하다가 캐릭터가 죽으면 true를 반환
    }

    // Effect가 끝났을 때, 호출되는 함수
    public void Release()
    {
        Debug.Assert(!IsReleased, "Effect::Release - 이미 종료된 Effect입니다.");

        Debug.Log("실행 4");

        Action?.Release(this, User, Target, level, Scale);
        ReleaseStackActionAll();

        foreach (var customAction in CustomActions)
            customAction.Release(this);

        IsReleased = true;

        onReleased?.Invoke(this);
    }
    #endregion

    #region GetData, BuildDescription 함수
    public EffectData GetData(int level) => effectDatasAllLevel[level - 1];

    // 인자로 받은 Text에서 Mark들을 Effect의 Data로 Replace하여 return 해주는 함수 
    // ※ effectIndex : 해당 Effect가 Skill의 Effect 목록에 있는 순번
    public string BuildDescription(string description, int skillIndex, int effectIndex)
    {
        Dictionary<string, string> stringByKeyword = new Dictionary<string, string>()
        {
            { "duration", Duration.ToString("0.##") },
            { "applyCount", ApplyCount.ToString() },
            { "applyCycle", ApplyCycle.ToString("0.##") }
        };

        // prefix로 skillIndex를 붙여줌 + suffix(접미어)로 effectIndex를 붙여줌
        description = TextReplacer.Replace(description, skillIndex.ToString(), stringByKeyword, effectIndex.ToString());   

        // Effect의 기본 Action의 설명을 EffectAction.BuildDescription 함수로 Replace 하기
        // → 기본 Action이기 때문에 stackActionIndex와 stack을 0으로 set
        description = Action.BuildDescription(this, description, skillIndex, 0, 0, effectIndex);

        // StackActions을 Stack을 기준으로 Group화 한다.
        // → 같은 Stack을 가진 StackAction들 끼리 묶는다.
        // → Key가 Stack, Value가 EffectStackAction List인 Dictionary로 정리된다.
        var stackGroups = StackActions.GroupBy(x => x.Stack);
        foreach (var group in stackGroups)
        {
            int i = 0;

            // Group안에 있는 StackAction의 BuildDescription 함수를 실행
            // Ex) Stack 3인 StackAction 3개, Stack 4인 StackAction 3개가 있다고 가정
            //     Key가 3인 Group과 Key가 4인 Group, 총 2개의 Group이 생긴다.
            //     Ket가 3인 Group에는 차례대로 stackActionIndex에 0, 1, 2라는 값이 들어가고 
            //     Ket가 4인 Group에도 차례대로 stackActionIndex에 0, 1, 2라는 값이 들어간다 
            foreach (var stackAction in group)
                description = stackAction.BuildDescription(this, description, skillIndex, i++, effectIndex);
        }

        return description;
    }
    #endregion

    public override object Clone()
    {
        var clone = Instantiate(this);

        // Owner가 존재한다면 clone을 현재 Effect의 Data들로 Setup 해주고 return 한다.
        if (Owner != null)
            clone.Setup(Owner, User, Level, Scale);

        return clone;
    }
}
