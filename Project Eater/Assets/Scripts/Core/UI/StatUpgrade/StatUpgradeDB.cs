using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class StatUpgradeDB : ScriptableObject 
    //  ScriptableObject는 일반 메서드 추가 및 데이터 처리 로직을 자유롭게 구현할 수 있다.
{
    // level, value 데이터를 가지고 있음
    public List<StatUpgradeDBEntity> Fullness;
    public List<StatUpgradeDBEntity> Attack;
    public List<StatUpgradeDBEntity> Defence;
    public List<StatUpgradeDBEntity> CritRate;
    public List<StatUpgradeDBEntity> CritDamage;
    public List<StatUpgradeDBEntity> MoveSpeed;
    public List<StatUpgradeDBEntity> AbilityHaste;
    public List<StatUpgradeDBEntity> Absorption;

    // 각 단계별 강화에 필요한 재화량 
    public List<StatUpgradeDBEntity> NeedBaalFlesh;

    public List<StatUpgradeDBEntity> GetStatUpgradeList(UpgradeStats statType)
    {
        return statType switch
        {
            UpgradeStats.Fullness => Fullness,
            UpgradeStats.Attack => Attack,
            UpgradeStats.Defence => Defence,
            UpgradeStats.CritRate => CritRate,
            UpgradeStats.CritDamage => CritDamage,
            UpgradeStats.MoveSpeed => MoveSpeed,
            UpgradeStats.AbilityHaste => AbilityHaste,
            UpgradeStats.Absorption => Absorption,
            _ => null
        };
    }
}
