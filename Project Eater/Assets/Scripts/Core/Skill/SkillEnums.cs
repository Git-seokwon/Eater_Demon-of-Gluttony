// ※ 스킬 사용 중 이동이 가능한가? 
// 1) Move : 이동 가능 
// 2) Stop : 이동 불가능 
public enum MovementInSkill
{
    Move,
    Stop
}

// ※ Skill을 언제 끝낼 것인가? 
// 1) FinishWhenApplyCompleted : applyCount만큼 모두 적용이되었으면 종료
// 2) FinishWhenDurationEnded  : applyCount만큼 적용했든, 안했든 시간이 지나면 종료 
public enum SkillRunningFinishOption
{
    FinishWhenApplyCompleted,
    FinishWhenDurationEnded
}

// ※ 스킬이 필요한 TargetSearcher 검색 결과 Type은 무엇인가? 
public enum NeedSelectionResultType
{
    Target,
    Position
}

// ※ Skill을 사용하는 Entity의 InSkillActionState를 언제 끝낼 것인가? 
// 1) FinishOnceApplied        : Skill이 한번 적용되자마자 바로 캐릭터를 제어할 수 있음
// 2) FinishWhenFullyApplied   : Skill이 ApplyCount만큼 모두 적용되야 캐릭터를 제어할 수 있음
// 3) FinishWhenAnimationEnded : Skill의 적용 여부와 상관없이 현재 실행중인 Animation이 끝나야 캐릭터를 제어할 수 있음
public enum InSkillActionFinishOption
{
    FinishOnceApplied,
    FinishWhenFullyApplied,
    FinishWhenAnimationEnded
}