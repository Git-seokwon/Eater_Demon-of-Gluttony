public enum EffectType
{
    None, // 일반 스킬
    Buff,
    Debuff
}

// Effect가 중복 적용 허용을 하지 않을 시, 만약 중복 적용이 일어났다면 Effect 둘 중 무엇을 제거할 것인가? 
public enum EffectRemoveDuplicateTargetOption
{
    Old, // 이미 적용중인 Effect 제거 
    New // 새로 적용된 Effect를 제거 
}

// Effect의 완료 시점이 언제인가? 
public enum EffectRunningFinishOption
{
    // Effect가 설정된 적용 횟수만큼 적용된다면 완료되는 Option 
    // 단, 해당 Option은 지속 시간(Duration)이 끝나도 완료된다. 
    // Ex) 리븐 Q, 아트 Q
    FinishWhenApplyComplted,
    
    // 지속 시간이 끝나면 완료되는 Option
    // 설정된 적용 횟수 만큼 적용되도, 지속 시간이 남았다면 완료가 안됨
    // Ex) 스웨인 R
    FinishWhenDurationEnded
}