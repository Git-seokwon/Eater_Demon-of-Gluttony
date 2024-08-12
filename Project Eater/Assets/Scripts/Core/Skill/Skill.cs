using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : IdentifiedObject
{
    private const int kInfinity = 0;

    #region Events
    // Skill이 Apply 되면 실행되는 Event
    // ※ currentApplyCount : 현재 Apply 횟수 
    public delegate void AppliedHandler(Skill skill, int currentApplyCount);
    // currentApplyCount의 값이 바뀌면 실행되는 Event
    public delegate void CurrentApplyCountChangedHandler(Skill skill, int currentApplyCount, int prevApplyCount);
    #endregion

    [SerializeField]
    private MovementInSkill movement;

    [SerializeField]
    private NeedSelectionResultType needSelectionResultType;

    // 현재 Level에 대응되는 SkillData
    private SkillData currentData;

    // 수치 관련 current 변수들
    private int currentApplyCount; // 현재 스킬 적용 횟수 
    private float currentDuration; // 현재 스킬의 지속 시간

    // 스킬 소유 Entity
    public Entity Owner { get; private set; }
    public StateMachine<Skill> StateMachine { get; private set; }

    public MovementInSkill Movement => movement;

    // ※ Effects
    // → Skill의 Level이 설정될 때, Level에 해당하는 SkillData를 가져와서 SkillData가 가진 EffectSelector에 의해 생성된
    //    Effect들을 Setting하는 Property
    public IReadOnlyList<Effect> Effects {  get; private set; } = Array.Empty<Effect>();

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

    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount == ApplyCount;

    // Skill의 발동이 종료되었는가? 
    public bool IsFinished => currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded
        ? IsDurationEnded
        : IsApplyCompleted;

    #region Event 변수들
    public event AppliedHandler onApplied;
    public event CurrentApplyCountChangedHandler onCurrentApplyCountChanged;
    #endregion

    // Skill 활성화 함수 
    public void Activate()
    {

    }

    // Skill이 T 상태인지 확인하는 함수 
    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();
    public bool IsInState<T>(int layer) where T : State<Skill> => StateMachine.IsInState<T>(layer);
}
