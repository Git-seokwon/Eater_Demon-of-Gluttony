// �� ��ų ��� �� �̵��� �����Ѱ�? 
// 1) Move : �̵� ���� 
// 2) Stop : �̵� �Ұ��� 
public enum MovementInSkill
{
    Move,
    Stop
}

// �� Skill�� ���� ���� ���ΰ�? 
// 1) FinishWhenApplyCompleted : applyCount��ŭ ��� �����̵Ǿ����� ����
// 2) FinishWhenDurationEnded  : applyCount��ŭ �����ߵ�, ���ߵ� �ð��� ������ ���� 
public enum SkillRunningFinishOption
{
    FinishWhenApplyCompleted,
    FinishWhenDurationEnded
}

// �� ��ų�� �ʿ��� TargetSearcher �˻� ��� Type�� �����ΰ�? 
public enum NeedSelectionResultType
{
    Target,
    Position
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