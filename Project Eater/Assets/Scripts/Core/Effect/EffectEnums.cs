public enum EffectType
{
    None, // �Ϲ� ��ų
    Buff,
    Debuff
}

// Effect�� �ߺ� ���� ����� ���� ���� ��, ���� �ߺ� ������ �Ͼ�ٸ� Effect �� �� ������ ������ ���ΰ�? 
public enum EffectRemoveDuplicateTargetOption
{
    Old, // �̹� �������� Effect ���� 
    New // ���� ����� Effect�� ���� 
}

// Effect�� �Ϸ� ������ �����ΰ�? 
public enum EffectRunningFinishOption
{
    // Effect�� ������ ���� Ƚ����ŭ ����ȴٸ� �Ϸ�Ǵ� Option 
    // ��, �ش� Option�� ���� �ð�(Duration)�� ������ �Ϸ�ȴ�. 
    // Ex) ���� Q, ��Ʈ Q
    FinishWhenApplyComplted,
    
    // ���� �ð��� ������ �Ϸ�Ǵ� Option
    // ������ ���� Ƚ�� ��ŭ ����ǵ�, ���� �ð��� ���Ҵٸ� �Ϸᰡ �ȵ�
    // Ex) ������ R
    FinishWhenDurationEnded
}