// �� ��ų ��� �� �̵��� �����Ѱ�? 
// 1) Move : �̵� ���� 
// 2) Stop : �̵� �Ұ��� 
public enum MovementInSkill
{
    Move,
    Stop
}

// �� ��ų ��� 
public enum SkillGrade
{
    Latent,
    Common, 
    Rare,
    Unique
}

// �� Skill�� ���� ���� ���ΰ�? 
// 1) FinishWhenApplyCompleted : applyCount��ŭ ��� �����̵Ǿ����� ����
// 2) FinishWhenDurationEnded  : applyCount��ŭ �����ߵ�, ���ߵ� �ð��� ������ ���� 
public enum SkillRunningFinishOption
{
    FinishWhenApplyCompleted,
    FinishWhenDurationEnded
}

// �� Skill�� Charge�� ������(���� �ð��� ������) � �ൿ�� ���� ���ΰ�? 
// 1) Use : Skill�� �ڵ����� ����� 
// 2) Cancel : Skill�� ����� ��ҵ� 
public enum SkillChargeFinishActionOption
{
    Use,
    Cancel
}

// �� ��ų Ÿ�� 
// 1) ��Ƽ��
// 2) �нú� 
public enum SkillType
{
    Active,
    Passive
}

// �� ��ų�� �ʿ��� TargetSearcher �˻� ��� Type�� �����ΰ�? 
public enum NeedSelectionResultType
{
    Target,
    Position
}

// �� ��ų ��� Ÿ�� 
// 1) Instant : �ܹ߼� ��ų
// �� Passive ��ų�� Instant�� ����
// 2) Tooggle : ����, �״� �� �� �ִ� Toggle�� ��ų ex) �ƹ��� W
public enum SkillUseType
{
    Instant,
    Toggle
}

// �� TargetSearcher�� ���� Target�� Search �ϴ°�
// 1) TargetSelectionCompleted : Target�� ����(Selection) �Ǿ��� �� �ٷ� Search�� ����
// �� Skill�� Target�� ������ �ʰ� ������ �� ���
// 2) Apply : Skill�� ����� ������ Search�� ����
// �� Skill�� ����� ������ Target�� �޶��� �� ���� ��� ���
// Ex) ���� �� ���� �����ļ� �ֺ��� ���� �����ϴ� Skill�� ���, ���� ����ĥ ������ �ֺ��� ���� �ִ��� �˻� 
// �� �� ���� ���� ���� �� ������ �ϴ� Skill�� TargetSelectionCompleted��,
//    ���� �� ������� �ϴ� ��, �׶����� �� �� �տ� �ִ� ���� ������ �ϴ� Skill�� Apply�� ����
public enum TargetSearchTimingOption
{
    TargetSelectionCompleted,
    Apply,
    Both
}

// �� Skill ���� �� CustomAction 
public enum SkillCustomActoinType
{
    Cast,
    Charge,
    PrecedingAction,
    Action
}

// �� Skill�� � ������� �����ų��
// 1) Auto  : Skill�� ApplyCount��ŭ �ڵ� ����
// 2) Input : ������ ���� Ư�� Key�� ������ ApplyCount��ŭ Skill�� �ߵ�
// Ex) ������ Q ��ų - Q Button�� ������ ��ų�� #�� ���� �ִ� 3�� ����� �� ����
public enum SkillExecutionType
{
    Auto,
    Input
}

// �� ���� TargetSearcher�� Select �Լ��� ������ ��
// 1) Use         : ó�� Skill�� ����� �� ����
// 2) UseInAction : SkillExecutionType�� Input�� �� Skill�� �����ų ������ ����
// �� SkillExecutionType�� Auto�̸� ��� X
public enum TargetSelectionTimingOption
{
    Use,
    UseInAction,
    Both
}

// �� Skill�� ����(�ߵ�) ����
// 1) Instant   : ApplyCycle�� ���� Skill�� ����Ǹ� �ٷ� ����
// �� SkillExecutionType�� Input�� ���, ������ Skill�� �ߵ���ų ������ ����
// 2) Animation : Skill�� ���� ������ Animation���� �����Ͽ� ����
// �� �̸� ���� ĳ���Ͱ� �ָ��� �������� ����, ���� ���� �� �� �� Skill�� ���� ������ �����ϰ� ���� �� ����
public enum SkillApplyType
{
    Instant,
    Animation
}

// �� Skill�� ����ϴ� Entity�� InSkillActionState�� ���� ���� ���ΰ�? 
// 1) FinishOnceApplied        : Skill�� �ѹ� ������ڸ��� �ٷ� ĳ���͸� ������ �� ����
// 2) FinishWhenFullyApplied   : Skill�� ApplyCount��ŭ ��� ����Ǿ� ĳ���͸� ������ �� ����
// 3) FinishWhenAnimationEnded : Skill�� ���� ���ο� ������� ���� �������� Animation�� ������ ĳ���͸� ������ �� ����
public enum InSkillActionFinishOption
{
    FinishOnceApplied,
    FinishWhenFullyApplied,
    FinishWhenAnimationEnded
}