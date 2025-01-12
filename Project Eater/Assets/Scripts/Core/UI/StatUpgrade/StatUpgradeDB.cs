using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class StatUpgradeDB : ScriptableObject 
    //  ScriptableObject�� �Ϲ� �޼��� �߰� �� ������ ó�� ������ �����Ӱ� ������ �� �ִ�.
{
    // level, value �����͸� ������ ����
    public List<StatUpgradeDBEntity> Fullness;
    public List<StatUpgradeDBEntity> Attack;
    public List<StatUpgradeDBEntity> Defence;
    public List<StatUpgradeDBEntity> CritRate;
    public List<StatUpgradeDBEntity> CritDamage;
    public List<StatUpgradeDBEntity> MoveSpeed;
    public List<StatUpgradeDBEntity> AbilityHaste;
    public List<StatUpgradeDBEntity> Absorption;

    // �� �ܰ躰 ��ȭ�� �ʿ��� ��ȭ�� 
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
