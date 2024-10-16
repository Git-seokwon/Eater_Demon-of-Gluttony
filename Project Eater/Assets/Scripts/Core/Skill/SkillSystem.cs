using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class SkillSystem : MonoBehaviour
{
    #region Event
    // SkillSystem에 Skill이 등록됐을 때, 호출되는 Event
    // 1) skillSystem : 현재 skillSystem
    // 2) skill       : 등록된 skill 
    public delegate void SkillRegisteredHandler(SkillSystem skillSystem, Skill skill);
    // Skill이 skillSystem에서 등록 해제 되었을 때 호출되는 Event
    public delegate void SkillUnregisteredHandler(SkillSystem skillSystem, Skill skill);
    // Skill을 장착했을 때 호출되는 Event 
    public delegate void SkillEquippedHandler(SkillSystem skillSystem, Skill skill, int keyNumbder);
    // Skill을 해제했을 때 호출되는 Event
    public delegate void SkillDisarmHandler(SkillSystem skillSystem, Skill skill, int keyNumbder);

    // Skill의 Event를 Wrapping 하는 Event들 (Skill에서 각 작업이 일어났을 때, 호출되는 Event) 
    public delegate void SkillStateChangedHandler(SkillSystem skillSystem, Skill skill,
        State<Skill> newState, State<Skill> prevState, int layer);
    public delegate void SkillActivatedHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillDeactivatedHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillAppliedHandler(SkillSystem skillSystem, Skill skill, int currentApplyCount);
    public delegate void SkillUsedHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillCanceledHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillTargetSelectionCompleted(SkillSystem skillSystem, Skill skill,
        TargetSearcher targetSearcher, TargetSelectionResult result);
    public delegate void SkillLevelChangedHandler(SkillSystem skillSystem, Skill skill, int currentLevel, int prevLevel);

    // Effect의 Event를 Wrapping 하는 Event들 (Effect에서 각 작업들이 일어났을 때, 호출되는 Event)
    public delegate void EffectStartedHandler(SkillSystem skillSystem, Effect effect);
    public delegate void EffectAppliedHandler(SkillSystem skillSystem, Effect effect, int currentApplyCount, int prevApplyCount);
    public delegate void EffectReleasedHandler(SkillSystem skillSystem, Effect effect);
    public delegate void EffectStackChangedHandler(SkillSystem skillSystem, Effect effect, int currentStack, int prevStack);
    #endregion

    #region 해방 스킬 
    // 0 : basic attack
    // 1 : basic trait
    // 2 : super skill
    public Skill[] defaultSkills;
    #endregion

    #region Skill & Effect
    [SerializeField]
    private SkillCombination skillCombination;

    // 현재 소유한 Skill들
    // → 최초에 위의 DefaultSkills이 ownSkills에 등록된다. 
    private List<Skill> ownSkills = new();

    // 현재 장착한 Skill들 
    // → 해방 스킬을 제외하고 Active Skill을 최대 4개까지 장착 가능하다. 
    // → 해방 스킬을 제외하고 Passive Skill을 최대 4개까지 장착 가능하다. 
    private List<Skill> equippedSkills = new();

    // 현재 장착한 Active Skill들 
    private List<Skill> activeSkills = new();
    // 현재 장착한 Passive Skill들 
    private List<Skill> passiveSkills = new();

    // 사용 예약 Skill을 저장하는 변수
    // → Skill을 사용했는데 검색된 기준점이 Skill의 범위 밖에 있을 경우 Skill의 사용이 취소되고
    //    reservedSkill에 Setting 돼서 대상이 Skill 범위 안에 들어오면 자동으로 예약된 Skill을 사용하게 한다.
    private Skill reservedSkill;

    // 현재 실행 중인 Skill들
    private List<Skill> runningSkills = new();

    // 현재 적용된 Effect들
    private List<Effect> runningEffects = new();

    // 발동이 종료된 Effect나 다른 Effect 효과에 의해서 제거되는 Effect들이 저장되는 Queue
    // → Effect의 Update가 끝난 뒤, destroyEffectQueue에 있는 Effect들은 Destroy 함수로 파괴된다. 
    private Queue<Effect> destroyEffectQueue = new();   
    #endregion

    public Entity Owner { get; private set; }
    public SkillCombination SkillCombination => skillCombination;
    public IReadOnlyList<Skill> OwnSkills => ownSkills;
    public IReadOnlyList<Skill> EquippedSkills => equippedSkills;
    public IReadOnlyList<Skill> ActiveSkills => activeSkills;
    public IReadOnlyList<Skill> PassiveSkills => passiveSkills;
    public IReadOnlyList<Skill> RunningSkills => runningSkills;
    public IReadOnlyList<Effect> RunningEffects => runningEffects;

    #region Event 변수 
    public event SkillRegisteredHandler onSkillRegistered;
    public event SkillUnregisteredHandler onSkillUnregistered;
    public event SkillEquippedHandler onSkillEquipped;   
    public event SkillDisarmHandler onSkillDisarm;

    public event SkillStateChangedHandler onSkillStateChanged;
    public event SkillActivatedHandler onSkillActivated;
    public event SkillDeactivatedHandler onSkillDeactivated;
    public event SkillAppliedHandler onSkillApplied;
    public event SkillUsedHandler onSkillUsed;
    public event SkillCanceledHandler onSkillCanceled;
    public event SkillTargetSelectionCompleted onSkillTargetSelectionCompleted;
    public event SkillLevelChangedHandler onSkillLevelChanged;

    public event EffectStartedHandler onEffectStarted;
    public event EffectAppliedHandler onEffectApplied;
    public event EffectReleasedHandler onEffectReleased;
    public event EffectStackChangedHandler onEffectStackChanged;
    #endregion

    private void OnDestroy()
    {
        foreach (var skill in ownSkills)
            Destroy(skill);

        foreach (var effect in runningEffects) 
            Destroy(effect);
    }

    // 등록된 Skill들과 적용된 Effect들의 Update가 일어난다. 
    private void Update()
    {
        // runningSkills Update
        UpdateSkills();
        // runningEffects Update
        UpdateRunningEffects();
        // Release된 Effect들을 제거 
        DestroyReleasedEffects();
        // 예약된 Skill이 Update
        UpdateReservedSkill();
    }

    // 초기화 함수 : SkillSystem의 소유자를 Setting 
    public void Setup(Entity entity)
    {
        Owner = entity;
        Debug.Assert(Owner != null, "SkillSystem::Awake - Owner는 null이 될 수 없습니다.");
    }

    // LatentSkill들을 SkillSystem에 등록하는 함수 
    public void SetupLatentSkills(List<Skill> latentskills)
    {
        for (int i = 0; i < latentskills.Count; i++)
        {
            var clone = Register(latentskills[i]);
            Equip(clone);

            defaultSkills[i] = clone;
        }
    }

    // SkillSystem에 Skill을 등록하는 함수 
    public Skill Register(Skill skill, int level = 0)
    {
        Debug.Assert(!ownSkills.Exists(x => x.ID == skill.ID), "SkillSystem::Register - 이미 존재하는 Skill입니다.");

        // 사본 만들기 (Clone)
        var clone = skill.Clone() as Skill;

        // 스킬 Setup
        if (level > 0)
            clone.Setup(Owner, level);
        else
            clone.Setup(Owner);

        // 스킬 레벨 업은 소유하고 있는 모든 스킬이 가능
        clone.onLevelChanged += OnSkillLevelChanged;

        // 소유한 스킬 목록인 ownSkills에 Skill을 추가해주고, onSkillRegistered Event를 호출하여 새로운 Skill이 등록된 것을 알린다. 
        ownSkills.Add(clone);
        onSkillRegistered?.Invoke(this, clone);

        // 외부에서 등록된 Skill을 조작하고 싶을 수도 있으니 등록된 Skill을 return
        return clone;
    }

    // Skill을 해제하는 함수 
    public bool Unregister(Skill skill)
    {
        // Find 함수로 ownSkills 목록에서 인자로 받은 Skill을 찾아온다. 
        skill = FindOwnSkill(skill);
        if (skill ==  null)
            return false;

        // ownSkills에서 Skill 제거 
        ownSkills.Remove(skill);

        // Event로 Skill의 등록이 해제되었음을 알려주기 
        onSkillUnregistered?.Invoke(this, skill);

        // skill 삭제 
        Destroy(skill);

        // 등록 해제 성공 
        return true;
    }

    // 스킬을 장착하는 함수 
    // → Skill 장착 UI에서 ownSkills에 있는 Skill을 인자로 넣어 호출한다. 
    // → 인자로 들어온 Skill은 ownSkills에 있는 Skill이기 때문에 원본 Skill의 사본이다. (다시 사본 만들어 줄 필요 X)
    // → keyCode : 장착 된 키보드 숫자 버튼 1 ~ 4 (아스키 코드)
    //            : 패시브 스킬의 경우 5 ~ 8번을 사용한다. (입력처리는 무시)
    //            : 해방 스킬의 경우, keyNumber 값으로 -1을 주어 따로 설정한다. 
    //            : 궁극 스킬의 경우, keyNumber 값으로 -2를 준다. 
    public Skill Equip(Skill skill, int keyNumbder = -1)
    {
        Debug.Assert(!(equippedSkills.Count > 8), "SkillSystem::Equip - 더이상 Skill을 장착할 수 없습니다.");
        Debug.Assert(!(activeSkills.Count > 4), "SkillSystem::Equip - 더이상 Active Skill을 장착할 수 없습니다.");
        Debug.Assert(!(passiveSkills.Count > 4), "SkillSystem::Equip - 더이상 Passive Skill을 장착할 수 없습니다.");

        skill.SetupStateMachine();

        // Skill의 Event에 SkillSystem의 CallBack 함수들을 등록
        skill.onStateChanged += OnSkillStateChanged;
        skill.onActivated += OnSkillActivated;
        skill.onDeactivated += OnSkillDeActivated;
        skill.onApplied += OnSkillApplied;
        skill.onUsed += OnSkillUsed;
        skill.onCancelled += OnSkillCanceled;
        skill.onTargetSelectionCompleted += OnSkillTargetSelectionCompleted;

        equippedSkills.Add(skill);
        onSkillEquipped?.Invoke(this, skill, keyNumbder);

        if (skill.Type == SkillType.Active)
            activeSkills.Add(skill);
        else
            passiveSkills.Add(skill);

        skill.skillKeyNumber = keyNumbder;

        return skill;
    }

    // Skill을 해제하는 함수 
    public bool Disarm(Skill skill, int keyNumbder = -1)
    {
        // Find 함수로 equippedSkills 목록에서 인자로 받은 Skill을 찾아온다. 
        skill = FindEquippedSkill(skill);
        if (skill == null) return false;

        // Skill을 찾았다면, 현재 사용 중일 수도 있으니 Cancle 함수로 사용을 취소한다. 
        // → isForce를 true로 넘겨줘서 강제 취소(SkillExecuteCommand.CancelImmediately)를 한다. 
        // ※ Skill 취소관련 유의점 
        // → 많은 게임들이 스킬이 사용 중이거나 Cooldown 중일 때, 스킬을 해제하는 걸 허용하지 않는다. 
        //    그렇기 때문에 애초에 스킬이 사용 중이면 취소하지 못하게 하는 것도 하나의 방법이 될 수 있다.
        if (!skill.IsFinished && skill.Type == SkillType.Active)
            skill.Cancel(true);
        // Passive 스킬의 경우, 스킬을 해제하면 스킬 효과들(Effect)을 Remove 한다.
        else if (skill.Type == SkillType.Passive)
            RemoveEffectAll(x => skill.currentEffects.Contains(x));

        equippedSkills.Remove(skill);

        onSkillDisarm?.Invoke(this, skill, keyNumbder);

        if (skill.Type == SkillType.Active)
            activeSkills.Remove(skill);
        else
            passiveSkills.Remove(skill);

        skill.skillKeyNumber = 0;

        return true;
    }

    private void UpdateSkills()
    {
        // 간단히 장착한 스킬들을 foreach문으로 돌면서 Update 함수를 실행한다. 
        // → Skill 내부에서 StateMachine의 Update가 일어나 State에 따라 Skill들이 동작한다. 
        int count = equippedSkills.Count;
        for (int i = 0; i < count; i++)
        {
            var skill = equippedSkills[i];
            skill.Update();
        }
    }

    // Entity에 적용된 Effect들을 Update 하는 함수 
    private void UpdateRunningEffects()
    {
        // Update된 Effect에 의해서 새로운 Effect가 runningEffects에 추가될 수도 있으므로,
        // foreach문이 아닌 for문으로 순회함
        // → foreach문으로 runningEffects를 돌게 되면 내부 값이 바뀌었다고 Error를 띄운다. 
        // → 단순히 runningEffects.Count로 하게 되면, 새로 추가된 Effect까지 Update 시켜버리는 상황이 발생할 수 있어 등록과 동시에 Update까지 
        //    될 수 있다. 그러면 Apply가 한 프레임 빨리 된다거나 효과가 한 프레임 빨리 끝나던가 하는 문제가 발생한다. 
        // → 이런 문제들 때문에 count로 저장해둔 for문으로 사용하는 것이다. 
        int count = runningEffects.Count;
        for (int i = 0; i < count; i++)
        {
            var effect = runningEffects[i];
            if (effect.IsReleased)
                continue;

            effect.Update();

            if (effect.IsFinished)
                RemoveEffect(effect);
        }
    }

    // 제거 목록에 들어간 Effect들을 실제로 제거해주는 함수 
    private void DestroyReleasedEffects()
    {
        while (destroyEffectQueue.Count > 0)
        {
            var effect = destroyEffectQueue.Dequeue();
            runningEffects.Remove(effect);
            Destroy(effect);
        }
    }

    // 사용 예약된 Skill을 Update하는 함수 
    // Ex) 리그 오브 레전드를 보면 범위 밖에 있는 적을 대상으로 Skill을 사용하면 그 대상에게 뛰어가고, 대상이 Skill 범위 안에 
    //     들어오면 Skill을 사용한다. 
    // → 대상에게 걸어가는 Code는 PlayerController 혹은 몬스터의 AI와 관련된 부분이라 SkillSystem Script에 추가하지 않음
    private void UpdateReservedSkill()
    {
        // 사용 예약된 스킬이 없다면 빠져나가기 
        if (!reservedSkill)
            return;

        // 예약된 Skill의 TargetSelectionResult를 가져오기 
        var selectionResult = reservedSkill.TargetSelectionResult;
        // selectionResult가 Target이면 해당 Target의 위치를, 아니면 선택된 위치를 가져옴.
        // ※ ?? 연산자 : 왼쪽 피연산자가 null인지 검사하고, null인 경우 오른쪽 피연산자의 값을 반환 
        var targetPosition = selectionResult.selectedTarget?.transform.position ?? selectionResult.selectedPosition;

        // Target이 Skill의 범위 안에 들어왔을 때,
        // Skill이 사용 가능한 상태면 즉시 사용, 사용이 불가능하다면 사용 예약을 취소함
        if (reservedSkill.IsInRange(targetPosition))
        {
            Debug.Log("reservedSkill 발동");

            if (reservedSkill.IsUseable)
                reservedSkill.UseImmediately(targetPosition);

            reservedSkill = null; // 사용 예약 취소 
        }
    }

    // Skil 사용을 예약하는 함수 
    public void ReserveSkill(Skill skill) => reservedSkill = skill;

    // Skill 사용 예약을 취소하는 함수 
    public void CancelReservedSkill() => reservedSkill = null;

    // 사용 예약된 Skill이 있는 지 체크하는 함수 
    public bool IsReservedSkill() => reservedSkill != null;

    // Skillsystem의 Owner에게 Effect를 적용시키는 함수 
    // → Skillsystem에 Effect를 등록하는 함수 
    private void ApplyNewEffect(Effect effect)
    {
        // 인자로 받은 Effect의 Clone을 만든다. 
        var newEffect = effect.Clone() as Effect;

        // SetTarget 함수로 SkillSystem의 Owner를 Effect의 Target으로 설정 
        newEffect.SetTarget(Owner);

        // Effect에 CallBack 함수들을 등록 
        newEffect.onStarted += OnEffectStarted;
        newEffect.onApplied += OnEffectApplied;
        newEffect.onReleased += OnEffectReleased;
        newEffect.onStackChanged += OnEffectStackChanged;

        // Effect의 Start 함수 실행 
        newEffect.Start();

        // Effect의 적용 가능 여부인 IsApplicable이 true라면 Effect의 Apply 함수를 실행 
        if (newEffect.IsApplicable)
            newEffect.Apply();

        // 만약 Effect가 위의 Apply 함수로 끝이 났다면 Effect를 Release해주고 Destroy 해준다.
        if (newEffect.IsFinished)
        {
            newEffect.Release();
            Destroy(newEffect);
        }
        // 끝나지 않았다면 runningEffects에 추가해서 Update되게 해준다. 
        else
            runningEffects.Add(newEffect);
    }

    // Effect를 Owner에게 적용시키는 함수 
    // → 이미 적용된 Effect가 있다면, Effect Option에 따라 다른 작업 혹은 추가적인 작업을 한다. 
    // → ApplyNewEffect 함수는 private 함수이고 Apply 함수는 public 함수이기 때문에 기본적으로 Effect를 적용시킬 때는 이 함수를 쓴다. 
    public void Apply(Effect effect)
    {
        // 인자로 받은 effect가 이미 Entity에 적용되어 있는지 찾아온다. 
        Effect runningEffect = Find(effect);

        // 새로운 Effect거나 Effect의 중복 적용이 허용된다면 Effect를 적용함
        if (runningEffect == null || effect.IsAllowDuplicate)
            ApplyNewEffect(effect);
        else
        {
            // Stack이 쌓이는 Effect라면 Stack을 쌓음
            // → Stack의 경우 이제 모듈화 해서 따로 빼줌 
            if (runningEffect.MaxStack > 1)
                return;
            // Effect의 RemoveDuplicateTargetOption이 Old(이미 적용 중인 Effect를 제거)라면 기존 Effect를 지우고, Effect를 새로 적용함
            else if (runningEffect.RemoveDuplicateTargetOption == EffectRemoveDuplicateTargetOption.Old)
            {
                RemoveEffect(runningEffect);
                ApplyNewEffect(effect);
            }
            // 그 외의 경우는 RemoveDuplicateTargetOption이 New(새로 들어온 Effect를 제거)라는 의미이므로 새로 들어온 Effect를 무시함
        }
    }

    // Effect List를 인자로 받는 Apply Overloading 함수 
    public void Apply(IReadOnlyList<Effect> effects)
    {
        foreach (var effect in effects)
            Apply(effect);
    }

    // Skill을 인자로 받는 Apply Overloading 함수 
    public void Apply(Skill skill)
    {
        Apply(skill.currentEffects);
    }

    // 인자로 들어온 Skill을 equippedSkills 목록에서 찾아와 Skill의 Use 함수를 실행하는 함수 
    public bool Use(Skill skill)
    {
        skill = FindEquippedSkill(skill);

        Debug.Assert(skill != null,
            $"SkillSystem::IncreaseStack({skill.CodeName}) - Skill이 System에 등록되지 않았습니다.");

        return skill.Use();
    }

    // runnuingSkills 목록에서 Skill을 찾아와 Cancel 함수를 실행하는 함수 
    public bool Cancel(Skill skill, bool isForce = false)
    {
        skill = FindEquippedSkill(skill);
        return skill?.Cancel(isForce) ?? false;
    }

    // 실행 중인 모든 Skill을 취소해주는 함수 
    // 1) isForce : 강제로 스킬을 취소할 지 여부 
    public void CancelAll(bool isForce = false)
    {
        CancelTargetSearching();

        foreach (var skill in runningSkills.ToArray())
            skill.Cancel(isForce);
    }

    public void CancelAllActiveSkill(bool isForce = false)
    {
        CancelTargetSearchingInActive();

        foreach (var skill in activeSkills.ToArray())
            skill.Cancel(isForce);
    }

    // 인자로 받은 소유한 Skill을 찾는 함수 
    // → skill의 Owner(Entity)와 SkillSystem의 Owner(Entity)가 같다는 소리는 이미 소유한 Skill이라는 의미이기 때문에 그대로 return
    // → 아니라면 ownSkills에서 인자로 받은 Skill과 같은 ID를 가진 Skill을 찾아온다. 
    public Skill FindOwnSkill(Skill skill)
        => skill.Owner == Owner ? skill : ownSkills.Find(x => x.ID == skill.ID);
    // Find의 Overloading 함수 
    // → Linq에서 Lamda로 조건문을 만들 듯이 조건문을 통해 Skill을 찾아올 수 있도록 Delegate를 인자로 받는다. 
    //    인자로 받은 조건문을 ownSkills의 Find 함수에 넣어서 조건에 맞는 Skill을 찾아온다. 
    public Skill FindOwnSkill(System.Predicate<Skill> match)
        => ownSkills.Find(match);

    // 인자로 받은 장착된 Skill을 찾는 함수 
    // → equippedSkills에서 인자로 받은 Skill과 같은 ID를 가진 Skill을 찾아온다. 
    public Skill FindEquippedSkill(Skill skill)
        => equippedSkills.Find(x => x.ID == skill.ID);

    // 인자로 받은 effect를 찾는 함수 
    // → effect의 Target이 Owner(Entity)라는 것은 이 Effect가 Owner에게 적용된 Effect라는 뜻 
    //    그대로 return 해준다. 
    // → 아니라면, runningEffects에서 인자로 받은 effect와 같은 ID를 가진 Effect를 찾아 return 한다. 
    public Effect Find(Effect effect)
        => effect.Target == Owner ? effect : runningEffects.Find(x => x.ID ==  effect.ID);
    public Effect Find(System.Predicate<Effect> match)
        => runningEffects.Find(match);

    // Effect와 Skill 모두, 조건에 맞는 Data를 모두 찾아오는 함수 
    public List<Skill> FindOwnAll(System.Predicate<Skill> match)
        => ownSkills.FindAll(match);
    public List<Skill> FindEquippedAll(System.Predicate<Skill> match)
        => equippedSkills.FindAll(match);
    public List<Effect> FindAll(System.Predicate<Effect> match)
        => runningEffects.FindAll(match);

    // 인자로 들어온 Skill과 Effect가 SkillSystem에 등록되어있는지 확인하는 함수 
    public bool ContainsInownskills(Skill skill)
        => FindOwnSkill(skill) != null;
    public bool ContainsInequippedskills(Skill skill)
        => FindEquippedSkill(skill) != null;
    public bool Contains(Effect effect)
        => Find(effect) != null;
    public bool ContainsCategory(Category category)
        => Find(x => x.HasCategory(category));

    // Effect 제거 함수 
    public bool RemoveEffect(Effect effect)
    {
        // Find 함수로 인자로 받은 effect를 찾아온다. 
        effect = Find(effect);

        if (effect == null || destroyEffectQueue.Contains(effect))
            return false;

        effect.Release();

        // ※ Effect를 바로 제거하지 않고, 따로 Queue에 넣어두는 이유
        // → UpdateRunningEffects 함수에서 for문으로 runningEffects를 돌고 있는데 Effect를 중간에 제거해버리면
        //    List가 중간에 수정돼서 Index가 안 맞아버리는 문제가 발생하기 때문이다.
        // → 객체를 Queue에 넣어두고 마지막에 정리하는 방식은 Unity와 Unreal의 기본적인 객체 처리 방법이기도 하다. 
        //    Unity의 경우, Destroy 함수를 호출했다고 해서 객체가 바로 파괴되는 것이 아니라, 내부 Flag를 통해 Null Reference와의 비교를
        //    true로 return 해주는 것일 뿐, Queue에 보관하고 있다가 마지막에 실제 Destroy이 작업이 일어나게 된다. 
        //    Unreal의 경우에도 마찬가지이다. 객체를 Kill하면 객체가 Queue에 보관되어 PendingKill 상태에 놓이게 되고,
        //    마지막에 실제 Destroy 작업이 일어나게 된다. 그래서 객체가 PendingKill 상태인지를 확인할 수 있는
        //    IsPendingKill, IsValid와 같은 함수들을 제공한다. 
        //    Effect의 경우 Unreal과 같은 방식이라고 생각하면 된다. 
        destroyEffectQueue.Enqueue(effect);

        return true;
    }

    // RemoveEffect의 Overloading 함수 
    // → 조건을 만족한 Effect를 제거할 수 있도록 Delegate를 인자로 받는다. 
    public bool RemoveEffect(System.Predicate<Effect> predicate)
    {
        var target = runningEffects.Find(predicate);
        // null이 아니라면 RemoveEffect 함수로 effect를 제거한다. 
        return target != null && RemoveEffect(target);
    }

    // RemoveEffect의 Overloading 함수 
    // → 특정 Category를 가진 Effect를 제거하는 RemoveEffect Overloading 함수 
    public bool RemoveEffect(Category category)
        => RemoveEffect(x => x.HasCategory(category));

    // 적용된 모든 Effect를 지우는 함수 
    public void RemoveEffectAll()
    {
        foreach (var target in runningEffects)
            RemoveEffect(target);
    }

    // RemoveEffectAll의 Overloading 함수 
    // → Delegate를 인자로 받아서 해당하는 모든 Effect를 지우는 함수 
    // → Linq.Where을 사용하기 위해 Predicate가 아닌 Func를 사용함 
    public void RemoveEffectAll(System.Func<Effect, bool> predicate)
    {
        var targets = runningEffects.Where(predicate);
        foreach (var target in targets)
            RemoveEffect(target);
    }

    // RemoveEffectAll의 Overloading 함수 
    // → 특정 Effect 혹은 특정 Category를 가진 Effect를 지우는 함수
    public void RemoveEffectAll(Effect effect) => RemoveEffectAll(x => x.ID ==  effect.ID);
    public void RemoveEffectAll(Category category) => RemoveEffectAll(x => x.HasCategory(category));

    // 현재 검색 중인 Skill이 있다면 취소하는 함수 
    public void CancelTargetSearching()
        => equippedSkills.Find(x => x.IsInState<SearchingTargetState>())?.Cancel();

    public void CancelTargetSearchingInActive()
    => equippedSkills.Find(x => x.IsInState<SearchingTargetState>() && x.Type == SkillType.Active)?.Cancel();

    public void SkillLevelUp(Skill skill)
    {
        // 스킬 강화 선택지에 OwnSkill들만 뜨기 때문에 null Check 굳이 안해도 됨
        var target = FindOwnSkill(skill);
        
        if (ContainsInequippedskills(target))
        {
            int keyNumber = target.skillKeyNumber;
            Disarm(target);
            target.LevelUp();
            Equip(target, keyNumber);
        }
        // 장착하지 않은 스킬은 그냥 Level만 업해주면 된다. 
        else
            target.LevelUp();
    }

    // Animation에서 호출될 CallBack 함수 
    // → Animation의 특정 Frame에서 이 함수를 호출하는 것으로 Skill의 발동 시점을 제어한다. 
    private void ApplyCurrentRunningSkill()
    {
        if (Owner.IsPlayer)
        {
            var statemachine = (Owner as PlayerEntity).StateMachine;
            if (statemachine.GetCurrentState() is InSkillActionState ownerState)
            {
                // State에서 실행 중인 Skill을 가져오고 
                var runningSkill = ownerState.RunningSkill;

                // Skill의 Apply 함수를 호출 
                // → 인자로 SkillExecutionType이 InputType이 아니라면 True를 InputType이라면 False를 넘겨준다. 
                //    Input Type일 때는 Skill의 남은 적용 횟수가 차감되지 않게 해준다. 
                // → Input Type일 경우에는 InSkillActionState에서 남은 적용 횟수가 차감되기 때문에 여기서는 차감되지 않는다. 
                runningSkill.Apply(runningSkill.ExecutionType != SkillExecutionType.Input);
            }
        }
        else
        {
            var statemachine = (Owner as EnemyEntity).StateMachine;
            if (!statemachine && statemachine.GetCurrentState() is EnemyInSkillActionState ownerState)
            {
                // State에서 실행 중인 Skill을 가져오고 
                var runningSkill = ownerState.RunningSkill;

                // Skill의 Apply 함수를 호출 
                // → 인자로 SkillExecutionType이 InputType이 아니라면 True를 InputType이라면 False를 넘겨준다. 
                //    Input Type일 때는 Skill의 남은 적용 횟수가 차감되지 않게 해준다. 
                // → Input Type일 경우에는 InSkillActionState에서 남은 적용 횟수가 차감되기 때문에 여기서는 차감되지 않는다. 
                runningSkill.Apply(runningSkill.ExecutionType != SkillExecutionType.Input);
            }
        }
    }

    #region Event CallBack
    // SkillSystem의 onSkillStateChanged Event를 호출 
    private void OnSkillStateChanged(Skill skill, State<Skill> newState, State<Skill> prevState, int layer)
        => onSkillStateChanged?.Invoke(this, skill, newState, prevState, layer);

    // ※ OnSkillActivated와 OnSkillDeactivated
    // → runningSkills의 관리는 Update 함수에서 따로 해주는 것이 아니라 Event 호출에 의해 관리된다. 
    // runningSkills에 Activated된 Skill을 추가해주고, SkillSystem의 onSkillActivated Event를 호출한다. 
    private void OnSkillActivated(Skill skill)
    {
        runningSkills.Add(skill);

        onSkillActivated?.Invoke(this, skill);
    }

    // Deactivated된 Skill을 runningSkills에서 제거해주고 SkillSystem의 onSkillDeactivated Event를 호출한다. 
    private void OnSkillDeActivated(Skill skill)
    {
        runningSkills.Remove(skill);

        onSkillDeactivated?.Invoke(this, skill);    
    }

    // SkillSystem의 onSkillUsed Event를 호출한다. 
    private void OnSkillUsed(Skill skill) => onSkillUsed?.Invoke(this, skill);

    // SkillSystem의 onSkillCanceled Event를 호출한다. 
    private void OnSkillCanceled(Skill skill) => onSkillCanceled?.Invoke(this, skill);

    // SkillSystem의 onSkillApplied Event를 호출한다. 
    private void OnSkillApplied(Skill skill, int currentApplyCount) 
        => onSkillApplied?.Invoke(this, skill, currentApplyCount);

    // result를 확인해서 기준점을 찾았다면, 예약된 Skill인 reservedSkill을 null로 만들고, onSkillTargetSelectionCompleted Event를 호출
    private void OnSkillTargetSelectionCompleted(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        if (result.resultMessage == SearchResultMessage.FindTarget || result.resultMessage == SearchResultMessage.FindPosition)
            // 사용 예약된 Skill을 취소 
            // → 어떤 Skill이 사용 예약된 상태에서 다른 Skill을 사용할 경우, 예약된 Skill의 사용은 취소된다. 
            reservedSkill = null;

        onSkillTargetSelectionCompleted?.Invoke(this, skill, targetSearcher, result);
    }

    private void OnSkillLevelChanged(Skill skill, int currentLevel, int prevLevel)
    {
        onSkillLevelChanged?.Invoke(this, skill, currentLevel, prevLevel);
    }

    // SkillSystem의 onEffectStarted Event 호출 
    private void OnEffectStarted(Effect effect) => onEffectStarted?.Invoke(this, effect);

    // SkillSystem의 onEffectApplied Event 호출 
    private void OnEffectApplied(Effect effect, int currentApplyCount, int prevApplyCount) 
        => onEffectApplied?.Invoke(this, effect, currentApplyCount, prevApplyCount);

    // SkillSystem의 onEffectReleased Event 호출 
    private void OnEffectReleased(Effect effect) => onEffectReleased?.Invoke(this, effect);

    // SkillSystem의 onEffectStackChanged Event 호출 
    private void OnEffectStackChanged(Effect effect, int currentStack, int prevStack)
        => onEffectStackChanged?.Invoke(this, effect, currentStack, prevStack);

    #endregion
}
