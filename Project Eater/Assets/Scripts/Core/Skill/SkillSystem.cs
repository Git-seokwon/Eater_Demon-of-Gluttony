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
    // SkillSystem�� Skill�� ��ϵ��� ��, ȣ��Ǵ� Event
    // 1) skillSystem : ���� skillSystem
    // 2) skill       : ��ϵ� skill 
    public delegate void SkillRegisteredHandler(SkillSystem skillSystem, Skill skill);
    // Skill�� skillSystem���� ��� ���� �Ǿ��� �� ȣ��Ǵ� Event
    public delegate void SkillUnregisteredHandler(SkillSystem skillSystem, Skill skill);
    // Skill�� �������� �� ȣ��Ǵ� Event 
    public delegate void SkillEquippedHandler(SkillSystem skillSystem, Skill skill, int keyNumbder);
    // Skill�� �������� �� ȣ��Ǵ� Event
    public delegate void SkillDisarmHandler(SkillSystem skillSystem, Skill skill, int keyNumbder);

    // Skill�� Event�� Wrapping �ϴ� Event�� (Skill���� �� �۾��� �Ͼ�� ��, ȣ��Ǵ� Event) 
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

    // Effect�� Event�� Wrapping �ϴ� Event�� (Effect���� �� �۾����� �Ͼ�� ��, ȣ��Ǵ� Event)
    public delegate void EffectStartedHandler(SkillSystem skillSystem, Effect effect);
    public delegate void EffectAppliedHandler(SkillSystem skillSystem, Effect effect, int currentApplyCount, int prevApplyCount);
    public delegate void EffectReleasedHandler(SkillSystem skillSystem, Effect effect);
    public delegate void EffectStackChangedHandler(SkillSystem skillSystem, Effect effect, int currentStack, int prevStack);
    #endregion

    #region �ع� ��ų 
    // 0 : basic attack
    // 1 : basic trait
    // 2 : super skill
    public Skill[] defaultSkills;
    #endregion

    #region Skill & Effect
    [SerializeField]
    private SkillCombination skillCombination;

    // ���� ������ Skill��
    // �� ���ʿ� ���� DefaultSkills�� ownSkills�� ��ϵȴ�. 
    private List<Skill> ownSkills = new();

    // ���� ������ Skill�� 
    // �� �ع� ��ų�� �����ϰ� Active Skill�� �ִ� 4������ ���� �����ϴ�. 
    // �� �ع� ��ų�� �����ϰ� Passive Skill�� �ִ� 4������ ���� �����ϴ�. 
    private List<Skill> equippedSkills = new();

    // ���� ������ Active Skill�� 
    private List<Skill> activeSkills = new();
    // ���� ������ Passive Skill�� 
    private List<Skill> passiveSkills = new();

    // ��� ���� Skill�� �����ϴ� ����
    // �� Skill�� ����ߴµ� �˻��� �������� Skill�� ���� �ۿ� ���� ��� Skill�� ����� ��ҵǰ�
    //    reservedSkill�� Setting �ż� ����� Skill ���� �ȿ� ������ �ڵ����� ����� Skill�� ����ϰ� �Ѵ�.
    private Skill reservedSkill;

    // ���� ���� ���� Skill��
    private List<Skill> runningSkills = new();

    // ���� ����� Effect��
    private List<Effect> runningEffects = new();

    // �ߵ��� ����� Effect�� �ٸ� Effect ȿ���� ���ؼ� ���ŵǴ� Effect���� ����Ǵ� Queue
    // �� Effect�� Update�� ���� ��, destroyEffectQueue�� �ִ� Effect���� Destroy �Լ��� �ı��ȴ�. 
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

    #region Event ���� 
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

    // ��ϵ� Skill��� ����� Effect���� Update�� �Ͼ��. 
    private void Update()
    {
        // runningSkills Update
        UpdateSkills();
        // runningEffects Update
        UpdateRunningEffects();
        // Release�� Effect���� ���� 
        DestroyReleasedEffects();
        // ����� Skill�� Update
        UpdateReservedSkill();
    }

    // �ʱ�ȭ �Լ� : SkillSystem�� �����ڸ� Setting 
    public void Setup(Entity entity)
    {
        Owner = entity;
        Debug.Assert(Owner != null, "SkillSystem::Awake - Owner�� null�� �� �� �����ϴ�.");
    }

    // LatentSkill���� SkillSystem�� ����ϴ� �Լ� 
    public void SetupLatentSkills(List<Skill> latentskills)
    {
        for (int i = 0; i < latentskills.Count; i++)
        {
            var clone = Register(latentskills[i]);
            Equip(clone);

            defaultSkills[i] = clone;
        }
    }

    // SkillSystem�� Skill�� ����ϴ� �Լ� 
    public Skill Register(Skill skill, int level = 0)
    {
        Debug.Assert(!ownSkills.Exists(x => x.ID == skill.ID), "SkillSystem::Register - �̹� �����ϴ� Skill�Դϴ�.");

        // �纻 ����� (Clone)
        var clone = skill.Clone() as Skill;

        // ��ų Setup
        if (level > 0)
            clone.Setup(Owner, level);
        else
            clone.Setup(Owner);

        // ��ų ���� ���� �����ϰ� �ִ� ��� ��ų�� ����
        clone.onLevelChanged += OnSkillLevelChanged;

        // ������ ��ų ����� ownSkills�� Skill�� �߰����ְ�, onSkillRegistered Event�� ȣ���Ͽ� ���ο� Skill�� ��ϵ� ���� �˸���. 
        ownSkills.Add(clone);
        onSkillRegistered?.Invoke(this, clone);

        // �ܺο��� ��ϵ� Skill�� �����ϰ� ���� ���� ������ ��ϵ� Skill�� return
        return clone;
    }

    // Skill�� �����ϴ� �Լ� 
    public bool Unregister(Skill skill)
    {
        // Find �Լ��� ownSkills ��Ͽ��� ���ڷ� ���� Skill�� ã�ƿ´�. 
        skill = FindOwnSkill(skill);
        if (skill ==  null)
            return false;

        // ownSkills���� Skill ���� 
        ownSkills.Remove(skill);

        // Event�� Skill�� ����� �����Ǿ����� �˷��ֱ� 
        onSkillUnregistered?.Invoke(this, skill);

        // skill ���� 
        Destroy(skill);

        // ��� ���� ���� 
        return true;
    }

    // ��ų�� �����ϴ� �Լ� 
    // �� Skill ���� UI���� ownSkills�� �ִ� Skill�� ���ڷ� �־� ȣ���Ѵ�. 
    // �� ���ڷ� ���� Skill�� ownSkills�� �ִ� Skill�̱� ������ ���� Skill�� �纻�̴�. (�ٽ� �纻 ����� �� �ʿ� X)
    // �� keyCode : ���� �� Ű���� ���� ��ư 1 ~ 4 (�ƽ�Ű �ڵ�)
    //            : �нú� ��ų�� ��� 5 ~ 8���� ����Ѵ�. (�Է�ó���� ����)
    //            : �ع� ��ų�� ���, keyNumber ������ -1�� �־� ���� �����Ѵ�. 
    //            : �ñ� ��ų�� ���, keyNumber ������ -2�� �ش�. 
    public Skill Equip(Skill skill, int keyNumbder = -1)
    {
        Debug.Assert(!(equippedSkills.Count > 8), "SkillSystem::Equip - ���̻� Skill�� ������ �� �����ϴ�.");
        Debug.Assert(!(activeSkills.Count > 4), "SkillSystem::Equip - ���̻� Active Skill�� ������ �� �����ϴ�.");
        Debug.Assert(!(passiveSkills.Count > 4), "SkillSystem::Equip - ���̻� Passive Skill�� ������ �� �����ϴ�.");

        skill.SetupStateMachine();

        // Skill�� Event�� SkillSystem�� CallBack �Լ����� ���
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

    // Skill�� �����ϴ� �Լ� 
    public bool Disarm(Skill skill, int keyNumbder = -1)
    {
        // Find �Լ��� equippedSkills ��Ͽ��� ���ڷ� ���� Skill�� ã�ƿ´�. 
        skill = FindEquippedSkill(skill);
        if (skill == null) return false;

        // Skill�� ã�Ҵٸ�, ���� ��� ���� ���� ������ Cancle �Լ��� ����� ����Ѵ�. 
        // �� isForce�� true�� �Ѱ��༭ ���� ���(SkillExecuteCommand.CancelImmediately)�� �Ѵ�. 
        // �� Skill ��Ұ��� ������ 
        // �� ���� ���ӵ��� ��ų�� ��� ���̰ų� Cooldown ���� ��, ��ų�� �����ϴ� �� ������� �ʴ´�. 
        //    �׷��� ������ ���ʿ� ��ų�� ��� ���̸� ������� ���ϰ� �ϴ� �͵� �ϳ��� ����� �� �� �ִ�.
        if (!skill.IsFinished && skill.Type == SkillType.Active)
            skill.Cancel(true);
        // Passive ��ų�� ���, ��ų�� �����ϸ� ��ų ȿ����(Effect)�� Remove �Ѵ�.
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
        // ������ ������ ��ų���� foreach������ ���鼭 Update �Լ��� �����Ѵ�. 
        // �� Skill ���ο��� StateMachine�� Update�� �Ͼ State�� ���� Skill���� �����Ѵ�. 
        int count = equippedSkills.Count;
        for (int i = 0; i < count; i++)
        {
            var skill = equippedSkills[i];
            skill.Update();
        }
    }

    // Entity�� ����� Effect���� Update �ϴ� �Լ� 
    private void UpdateRunningEffects()
    {
        // Update�� Effect�� ���ؼ� ���ο� Effect�� runningEffects�� �߰��� ���� �����Ƿ�,
        // foreach���� �ƴ� for������ ��ȸ��
        // �� foreach������ runningEffects�� ���� �Ǹ� ���� ���� �ٲ���ٰ� Error�� ����. 
        // �� �ܼ��� runningEffects.Count�� �ϰ� �Ǹ�, ���� �߰��� Effect���� Update ���ѹ����� ��Ȳ�� �߻��� �� �־� ��ϰ� ���ÿ� Update���� 
        //    �� �� �ִ�. �׷��� Apply�� �� ������ ���� �ȴٰų� ȿ���� �� ������ ���� �������� �ϴ� ������ �߻��Ѵ�. 
        // �� �̷� ������ ������ count�� �����ص� for������ ����ϴ� ���̴�. 
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

    // ���� ��Ͽ� �� Effect���� ������ �������ִ� �Լ� 
    private void DestroyReleasedEffects()
    {
        while (destroyEffectQueue.Count > 0)
        {
            var effect = destroyEffectQueue.Dequeue();
            runningEffects.Remove(effect);
            Destroy(effect);
        }
    }

    // ��� ����� Skill�� Update�ϴ� �Լ� 
    // Ex) ���� ���� �����带 ���� ���� �ۿ� �ִ� ���� ������� Skill�� ����ϸ� �� ��󿡰� �پ��, ����� Skill ���� �ȿ� 
    //     ������ Skill�� ����Ѵ�. 
    // �� ��󿡰� �ɾ�� Code�� PlayerController Ȥ�� ������ AI�� ���õ� �κ��̶� SkillSystem Script�� �߰����� ����
    private void UpdateReservedSkill()
    {
        // ��� ����� ��ų�� ���ٸ� ���������� 
        if (!reservedSkill)
            return;

        // ����� Skill�� TargetSelectionResult�� �������� 
        var selectionResult = reservedSkill.TargetSelectionResult;
        // selectionResult�� Target�̸� �ش� Target�� ��ġ��, �ƴϸ� ���õ� ��ġ�� ������.
        // �� ?? ������ : ���� �ǿ����ڰ� null���� �˻��ϰ�, null�� ��� ������ �ǿ������� ���� ��ȯ 
        var targetPosition = selectionResult.selectedTarget?.transform.position ?? selectionResult.selectedPosition;

        // Target�� Skill�� ���� �ȿ� ������ ��,
        // Skill�� ��� ������ ���¸� ��� ���, ����� �Ұ����ϴٸ� ��� ������ �����
        if (reservedSkill.IsInRange(targetPosition))
        {
            Debug.Log("reservedSkill �ߵ�");

            if (reservedSkill.IsUseable)
                reservedSkill.UseImmediately(targetPosition);

            reservedSkill = null; // ��� ���� ��� 
        }
    }

    // Skil ����� �����ϴ� �Լ� 
    public void ReserveSkill(Skill skill) => reservedSkill = skill;

    // Skill ��� ������ ����ϴ� �Լ� 
    public void CancelReservedSkill() => reservedSkill = null;

    // ��� ����� Skill�� �ִ� �� üũ�ϴ� �Լ� 
    public bool IsReservedSkill() => reservedSkill != null;

    // Skillsystem�� Owner���� Effect�� �����Ű�� �Լ� 
    // �� Skillsystem�� Effect�� ����ϴ� �Լ� 
    private void ApplyNewEffect(Effect effect)
    {
        // ���ڷ� ���� Effect�� Clone�� �����. 
        var newEffect = effect.Clone() as Effect;

        // SetTarget �Լ��� SkillSystem�� Owner�� Effect�� Target���� ���� 
        newEffect.SetTarget(Owner);

        // Effect�� CallBack �Լ����� ��� 
        newEffect.onStarted += OnEffectStarted;
        newEffect.onApplied += OnEffectApplied;
        newEffect.onReleased += OnEffectReleased;
        newEffect.onStackChanged += OnEffectStackChanged;

        // Effect�� Start �Լ� ���� 
        newEffect.Start();

        // Effect�� ���� ���� ������ IsApplicable�� true��� Effect�� Apply �Լ��� ���� 
        if (newEffect.IsApplicable)
            newEffect.Apply();

        // ���� Effect�� ���� Apply �Լ��� ���� ���ٸ� Effect�� Release���ְ� Destroy ���ش�.
        if (newEffect.IsFinished)
        {
            newEffect.Release();
            Destroy(newEffect);
        }
        // ������ �ʾҴٸ� runningEffects�� �߰��ؼ� Update�ǰ� ���ش�. 
        else
            runningEffects.Add(newEffect);
    }

    // Effect�� Owner���� �����Ű�� �Լ� 
    // �� �̹� ����� Effect�� �ִٸ�, Effect Option�� ���� �ٸ� �۾� Ȥ�� �߰����� �۾��� �Ѵ�. 
    // �� ApplyNewEffect �Լ��� private �Լ��̰� Apply �Լ��� public �Լ��̱� ������ �⺻������ Effect�� �����ų ���� �� �Լ��� ����. 
    public void Apply(Effect effect)
    {
        // ���ڷ� ���� effect�� �̹� Entity�� ����Ǿ� �ִ��� ã�ƿ´�. 
        Effect runningEffect = Find(effect);

        // ���ο� Effect�ų� Effect�� �ߺ� ������ ���ȴٸ� Effect�� ������
        if (runningEffect == null || effect.IsAllowDuplicate)
            ApplyNewEffect(effect);
        else
        {
            // Stack�� ���̴� Effect��� Stack�� ����
            // �� Stack�� ��� ���� ���ȭ �ؼ� ���� ���� 
            if (runningEffect.MaxStack > 1)
                return;
            // Effect�� RemoveDuplicateTargetOption�� Old(�̹� ���� ���� Effect�� ����)��� ���� Effect�� �����, Effect�� ���� ������
            else if (runningEffect.RemoveDuplicateTargetOption == EffectRemoveDuplicateTargetOption.Old)
            {
                RemoveEffect(runningEffect);
                ApplyNewEffect(effect);
            }
            // �� ���� ���� RemoveDuplicateTargetOption�� New(���� ���� Effect�� ����)��� �ǹ��̹Ƿ� ���� ���� Effect�� ������
        }
    }

    // Effect List�� ���ڷ� �޴� Apply Overloading �Լ� 
    public void Apply(IReadOnlyList<Effect> effects)
    {
        foreach (var effect in effects)
            Apply(effect);
    }

    // Skill�� ���ڷ� �޴� Apply Overloading �Լ� 
    public void Apply(Skill skill)
    {
        Apply(skill.currentEffects);
    }

    // ���ڷ� ���� Skill�� equippedSkills ��Ͽ��� ã�ƿ� Skill�� Use �Լ��� �����ϴ� �Լ� 
    public bool Use(Skill skill)
    {
        skill = FindEquippedSkill(skill);

        Debug.Assert(skill != null,
            $"SkillSystem::IncreaseStack({skill.CodeName}) - Skill�� System�� ��ϵ��� �ʾҽ��ϴ�.");

        return skill.Use();
    }

    // runnuingSkills ��Ͽ��� Skill�� ã�ƿ� Cancel �Լ��� �����ϴ� �Լ� 
    public bool Cancel(Skill skill, bool isForce = false)
    {
        skill = FindEquippedSkill(skill);
        return skill?.Cancel(isForce) ?? false;
    }

    // ���� ���� ��� Skill�� ������ִ� �Լ� 
    // 1) isForce : ������ ��ų�� ����� �� ���� 
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

    // ���ڷ� ���� ������ Skill�� ã�� �Լ� 
    // �� skill�� Owner(Entity)�� SkillSystem�� Owner(Entity)�� ���ٴ� �Ҹ��� �̹� ������ Skill�̶�� �ǹ��̱� ������ �״�� return
    // �� �ƴ϶�� ownSkills���� ���ڷ� ���� Skill�� ���� ID�� ���� Skill�� ã�ƿ´�. 
    public Skill FindOwnSkill(Skill skill)
        => skill.Owner == Owner ? skill : ownSkills.Find(x => x.ID == skill.ID);
    // Find�� Overloading �Լ� 
    // �� Linq���� Lamda�� ���ǹ��� ���� ���� ���ǹ��� ���� Skill�� ã�ƿ� �� �ֵ��� Delegate�� ���ڷ� �޴´�. 
    //    ���ڷ� ���� ���ǹ��� ownSkills�� Find �Լ��� �־ ���ǿ� �´� Skill�� ã�ƿ´�. 
    public Skill FindOwnSkill(System.Predicate<Skill> match)
        => ownSkills.Find(match);

    // ���ڷ� ���� ������ Skill�� ã�� �Լ� 
    // �� equippedSkills���� ���ڷ� ���� Skill�� ���� ID�� ���� Skill�� ã�ƿ´�. 
    public Skill FindEquippedSkill(Skill skill)
        => equippedSkills.Find(x => x.ID == skill.ID);

    // ���ڷ� ���� effect�� ã�� �Լ� 
    // �� effect�� Target�� Owner(Entity)��� ���� �� Effect�� Owner���� ����� Effect��� �� 
    //    �״�� return ���ش�. 
    // �� �ƴ϶��, runningEffects���� ���ڷ� ���� effect�� ���� ID�� ���� Effect�� ã�� return �Ѵ�. 
    public Effect Find(Effect effect)
        => effect.Target == Owner ? effect : runningEffects.Find(x => x.ID ==  effect.ID);
    public Effect Find(System.Predicate<Effect> match)
        => runningEffects.Find(match);

    // Effect�� Skill ���, ���ǿ� �´� Data�� ��� ã�ƿ��� �Լ� 
    public List<Skill> FindOwnAll(System.Predicate<Skill> match)
        => ownSkills.FindAll(match);
    public List<Skill> FindEquippedAll(System.Predicate<Skill> match)
        => equippedSkills.FindAll(match);
    public List<Effect> FindAll(System.Predicate<Effect> match)
        => runningEffects.FindAll(match);

    // ���ڷ� ���� Skill�� Effect�� SkillSystem�� ��ϵǾ��ִ��� Ȯ���ϴ� �Լ� 
    public bool ContainsInownskills(Skill skill)
        => FindOwnSkill(skill) != null;
    public bool ContainsInequippedskills(Skill skill)
        => FindEquippedSkill(skill) != null;
    public bool Contains(Effect effect)
        => Find(effect) != null;
    public bool ContainsCategory(Category category)
        => Find(x => x.HasCategory(category));

    // Effect ���� �Լ� 
    public bool RemoveEffect(Effect effect)
    {
        // Find �Լ��� ���ڷ� ���� effect�� ã�ƿ´�. 
        effect = Find(effect);

        if (effect == null || destroyEffectQueue.Contains(effect))
            return false;

        effect.Release();

        // �� Effect�� �ٷ� �������� �ʰ�, ���� Queue�� �־�δ� ����
        // �� UpdateRunningEffects �Լ����� for������ runningEffects�� ���� �ִµ� Effect�� �߰��� �����ع�����
        //    List�� �߰��� �����ż� Index�� �� �¾ƹ����� ������ �߻��ϱ� �����̴�.
        // �� ��ü�� Queue�� �־�ΰ� �������� �����ϴ� ����� Unity�� Unreal�� �⺻���� ��ü ó�� ����̱⵵ �ϴ�. 
        //    Unity�� ���, Destroy �Լ��� ȣ���ߴٰ� �ؼ� ��ü�� �ٷ� �ı��Ǵ� ���� �ƴ϶�, ���� Flag�� ���� Null Reference���� �񱳸�
        //    true�� return ���ִ� ���� ��, Queue�� �����ϰ� �ִٰ� �������� ���� Destroy�� �۾��� �Ͼ�� �ȴ�. 
        //    Unreal�� ��쿡�� ���������̴�. ��ü�� Kill�ϸ� ��ü�� Queue�� �����Ǿ� PendingKill ���¿� ���̰� �ǰ�,
        //    �������� ���� Destroy �۾��� �Ͼ�� �ȴ�. �׷��� ��ü�� PendingKill ���������� Ȯ���� �� �ִ�
        //    IsPendingKill, IsValid�� ���� �Լ����� �����Ѵ�. 
        //    Effect�� ��� Unreal�� ���� ����̶�� �����ϸ� �ȴ�. 
        destroyEffectQueue.Enqueue(effect);

        return true;
    }

    // RemoveEffect�� Overloading �Լ� 
    // �� ������ ������ Effect�� ������ �� �ֵ��� Delegate�� ���ڷ� �޴´�. 
    public bool RemoveEffect(System.Predicate<Effect> predicate)
    {
        var target = runningEffects.Find(predicate);
        // null�� �ƴ϶�� RemoveEffect �Լ��� effect�� �����Ѵ�. 
        return target != null && RemoveEffect(target);
    }

    // RemoveEffect�� Overloading �Լ� 
    // �� Ư�� Category�� ���� Effect�� �����ϴ� RemoveEffect Overloading �Լ� 
    public bool RemoveEffect(Category category)
        => RemoveEffect(x => x.HasCategory(category));

    // ����� ��� Effect�� ����� �Լ� 
    public void RemoveEffectAll()
    {
        foreach (var target in runningEffects)
            RemoveEffect(target);
    }

    // RemoveEffectAll�� Overloading �Լ� 
    // �� Delegate�� ���ڷ� �޾Ƽ� �ش��ϴ� ��� Effect�� ����� �Լ� 
    // �� Linq.Where�� ����ϱ� ���� Predicate�� �ƴ� Func�� ����� 
    public void RemoveEffectAll(System.Func<Effect, bool> predicate)
    {
        var targets = runningEffects.Where(predicate);
        foreach (var target in targets)
            RemoveEffect(target);
    }

    // RemoveEffectAll�� Overloading �Լ� 
    // �� Ư�� Effect Ȥ�� Ư�� Category�� ���� Effect�� ����� �Լ�
    public void RemoveEffectAll(Effect effect) => RemoveEffectAll(x => x.ID ==  effect.ID);
    public void RemoveEffectAll(Category category) => RemoveEffectAll(x => x.HasCategory(category));

    // ���� �˻� ���� Skill�� �ִٸ� ����ϴ� �Լ� 
    public void CancelTargetSearching()
        => equippedSkills.Find(x => x.IsInState<SearchingTargetState>())?.Cancel();

    public void CancelTargetSearchingInActive()
    => equippedSkills.Find(x => x.IsInState<SearchingTargetState>() && x.Type == SkillType.Active)?.Cancel();

    public void SkillLevelUp(Skill skill)
    {
        // ��ų ��ȭ �������� OwnSkill�鸸 �߱� ������ null Check ���� ���ص� ��
        var target = FindOwnSkill(skill);
        
        if (ContainsInequippedskills(target))
        {
            int keyNumber = target.skillKeyNumber;
            Disarm(target);
            target.LevelUp();
            Equip(target, keyNumber);
        }
        // �������� ���� ��ų�� �׳� Level�� �����ָ� �ȴ�. 
        else
            target.LevelUp();
    }

    // Animation���� ȣ��� CallBack �Լ� 
    // �� Animation�� Ư�� Frame���� �� �Լ��� ȣ���ϴ� ������ Skill�� �ߵ� ������ �����Ѵ�. 
    private void ApplyCurrentRunningSkill()
    {
        if (Owner.IsPlayer)
        {
            var statemachine = (Owner as PlayerEntity).StateMachine;
            if (statemachine.GetCurrentState() is InSkillActionState ownerState)
            {
                // State���� ���� ���� Skill�� �������� 
                var runningSkill = ownerState.RunningSkill;

                // Skill�� Apply �Լ��� ȣ�� 
                // �� ���ڷ� SkillExecutionType�� InputType�� �ƴ϶�� True�� InputType�̶�� False�� �Ѱ��ش�. 
                //    Input Type�� ���� Skill�� ���� ���� Ƚ���� �������� �ʰ� ���ش�. 
                // �� Input Type�� ��쿡�� InSkillActionState���� ���� ���� Ƚ���� �����Ǳ� ������ ���⼭�� �������� �ʴ´�. 
                runningSkill.Apply(runningSkill.ExecutionType != SkillExecutionType.Input);
            }
        }
        else
        {
            var statemachine = (Owner as EnemyEntity).StateMachine;
            if (!statemachine && statemachine.GetCurrentState() is EnemyInSkillActionState ownerState)
            {
                // State���� ���� ���� Skill�� �������� 
                var runningSkill = ownerState.RunningSkill;

                // Skill�� Apply �Լ��� ȣ�� 
                // �� ���ڷ� SkillExecutionType�� InputType�� �ƴ϶�� True�� InputType�̶�� False�� �Ѱ��ش�. 
                //    Input Type�� ���� Skill�� ���� ���� Ƚ���� �������� �ʰ� ���ش�. 
                // �� Input Type�� ��쿡�� InSkillActionState���� ���� ���� Ƚ���� �����Ǳ� ������ ���⼭�� �������� �ʴ´�. 
                runningSkill.Apply(runningSkill.ExecutionType != SkillExecutionType.Input);
            }
        }
    }

    #region Event CallBack
    // SkillSystem�� onSkillStateChanged Event�� ȣ�� 
    private void OnSkillStateChanged(Skill skill, State<Skill> newState, State<Skill> prevState, int layer)
        => onSkillStateChanged?.Invoke(this, skill, newState, prevState, layer);

    // �� OnSkillActivated�� OnSkillDeactivated
    // �� runningSkills�� ������ Update �Լ����� ���� ���ִ� ���� �ƴ϶� Event ȣ�⿡ ���� �����ȴ�. 
    // runningSkills�� Activated�� Skill�� �߰����ְ�, SkillSystem�� onSkillActivated Event�� ȣ���Ѵ�. 
    private void OnSkillActivated(Skill skill)
    {
        runningSkills.Add(skill);

        onSkillActivated?.Invoke(this, skill);
    }

    // Deactivated�� Skill�� runningSkills���� �������ְ� SkillSystem�� onSkillDeactivated Event�� ȣ���Ѵ�. 
    private void OnSkillDeActivated(Skill skill)
    {
        runningSkills.Remove(skill);

        onSkillDeactivated?.Invoke(this, skill);    
    }

    // SkillSystem�� onSkillUsed Event�� ȣ���Ѵ�. 
    private void OnSkillUsed(Skill skill) => onSkillUsed?.Invoke(this, skill);

    // SkillSystem�� onSkillCanceled Event�� ȣ���Ѵ�. 
    private void OnSkillCanceled(Skill skill) => onSkillCanceled?.Invoke(this, skill);

    // SkillSystem�� onSkillApplied Event�� ȣ���Ѵ�. 
    private void OnSkillApplied(Skill skill, int currentApplyCount) 
        => onSkillApplied?.Invoke(this, skill, currentApplyCount);

    // result�� Ȯ���ؼ� �������� ã�Ҵٸ�, ����� Skill�� reservedSkill�� null�� �����, onSkillTargetSelectionCompleted Event�� ȣ��
    private void OnSkillTargetSelectionCompleted(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        if (result.resultMessage == SearchResultMessage.FindTarget || result.resultMessage == SearchResultMessage.FindPosition)
            // ��� ����� Skill�� ��� 
            // �� � Skill�� ��� ����� ���¿��� �ٸ� Skill�� ����� ���, ����� Skill�� ����� ��ҵȴ�. 
            reservedSkill = null;

        onSkillTargetSelectionCompleted?.Invoke(this, skill, targetSearcher, result);
    }

    private void OnSkillLevelChanged(Skill skill, int currentLevel, int prevLevel)
    {
        onSkillLevelChanged?.Invoke(this, skill, currentLevel, prevLevel);
    }

    // SkillSystem�� onEffectStarted Event ȣ�� 
    private void OnEffectStarted(Effect effect) => onEffectStarted?.Invoke(this, effect);

    // SkillSystem�� onEffectApplied Event ȣ�� 
    private void OnEffectApplied(Effect effect, int currentApplyCount, int prevApplyCount) 
        => onEffectApplied?.Invoke(this, effect, currentApplyCount, prevApplyCount);

    // SkillSystem�� onEffectReleased Event ȣ�� 
    private void OnEffectReleased(Effect effect) => onEffectReleased?.Invoke(this, effect);

    // SkillSystem�� onEffectStackChanged Event ȣ�� 
    private void OnEffectStackChanged(Effect effect, int currentStack, int prevStack)
        => onEffectStackChanged?.Invoke(this, effect, currentStack, prevStack);

    #endregion
}
