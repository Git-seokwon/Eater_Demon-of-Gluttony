// ※ 스킬 사용 중 이동이 가능한가? 
// 1) Move : 이동 가능 
// 2) Stop : 이동 불가능 
public enum MovementInSkill
{
    Move,
    Stop
}

// ※ 스킬 등급 
public enum SkillGrade
{
    Latent,
    Common, 
    Rare,
    Unique
}

// ※ Skill을 언제 끝낼 것인가? 
// 1) FinishWhenApplyCompleted : applyCount만큼 모두 적용이되었으면 종료
// 2) FinishWhenDurationEnded  : applyCount만큼 적용했든, 안했든 시간이 지나면 종료 
public enum SkillRunningFinishOption
{
    FinishWhenApplyCompleted,
    FinishWhenDurationEnded
}

// ※ Skill의 Charge가 끝나면(지속 시간이 끝나면) 어떤 행동을 취할 것인가? 
// 1) Use : Skill을 자동으로 사용함 
// 2) Cancel : Skill의 사용이 취소됨 
public enum SkillChargeFinishActionOption
{
    Use,
    Cancel
}

// ※ 스킬 타입 
// 1) 액티브
// 2) 패시브 
public enum SkillType
{
    Active,
    Passive
}

// ※ 스킬이 필요한 TargetSearcher 검색 결과 Type은 무엇인가? 
public enum NeedSelectionResultType
{
    Target,
    Position
}

// ※ 스킬 사용 타입 
// 1) Instant : 단발성 스킬
// → Passive 스킬은 Instant로 고정
// 2) Tooggle : 껐다, 켰다 할 수 있는 Toggle형 스킬 ex) 아무무 W
public enum SkillUseType
{
    Instant,
    Toggle
}

// ※ TargetSearcher가 언제 Target을 Search 하는가
// 1) TargetSelectionCompleted : Target이 선택(Selection) 되었을 때 바로 Search를 실행
// → Skill의 Target이 변하지 않고 고정일 때 사용
// 2) Apply : Skill이 적용될 때마다 Search를 실행
// → Skill이 적용될 때마다 Target이 달라질 수 있을 경우 사용
// Ex) 여러 번 땅을 내리쳐서 주변의 적을 공격하는 Skill의 경우, 땅을 내려칠 때마다 주변에 누가 있는지 검색 
// ★ 한 명의 적을 여러 번 때려야 하는 Skill은 TargetSelectionCompleted를,
//    여러 번 때리기는 하는 데, 그때마다 내 눈 앞에 있는 적을 때려야 하는 Skill은 Apply를 선택
public enum TargetSearchTimingOption
{
    TargetSelectionCompleted,
    Apply,
    Both
}

// ※ Skill 상태 별 CustomAction 
public enum SkillCustomActoinType
{
    Cast,
    Charge,
    PrecedingAction,
    Action
}

// ※ Skill을 어떤 방식으로 실행시킬지
// 1) Auto  : Skill이 ApplyCount만큼 자동 실행
// 2) Input : 유저가 직접 특정 Key를 눌러서 ApplyCount만큼 Skill을 발동
// Ex) 리븐의 Q 스킬 - Q Button을 눌러서 스킬을 #초 동안 최대 3번 사용할 수 있음
public enum SkillExecutionType
{
    Auto,
    Input
}

// ※ 언제 TargetSearcher의 Select 함수를 실행할 지
// 1) Use         : 처음 Skill을 사용할 때 선택
// 2) UseInAction : SkillExecutionType이 Input일 때 Skill을 실행시킬 때마다 선택
// → SkillExecutionType이 Auto이면 사용 X
public enum TargetSelectionTimingOption
{
    Use,
    UseInAction,
    Both
}

// ※ Skill의 적용(발동) 시점
// 1) Instant   : ApplyCycle에 맞춰 Skill이 실행되면 바로 적용
// → SkillExecutionType이 Input일 경우, 유저가 Skill을 발동시킬 때마다 적용
// 2) Animation : Skill의 적용 시점을 Animation에서 결정하여 적용
// → 이를 통해 캐릭터가 주먹을 내질렀을 때나, 검이 적을 벨 때 등 Skill의 적용 시점을 정교하게 정할 수 있음
public enum SkillApplyType
{
    Instant,
    Animation
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