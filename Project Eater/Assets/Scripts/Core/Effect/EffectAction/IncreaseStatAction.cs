using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ IncreaseStatAction : Target의 Stat 수치를 증가시켜주는 효과를 구현한 Action
// → Stat에는 현재 HP도 존재한다. 현재 HP를 증가시킨다는 건 '회복'의 의미이기 때문에 치유 효과도 IncreaseStatAction에 포함
[System.Serializable]
public class IncreaseStatAction : EffectAction
{
    // 수치를 증가시킬 Target Stats
    [SerializeField]
    private Stat stat;

    // 기본적으로 수치를 얼마나 증가시킬 것인가 
    // → 음수 값을 넣는다면 Stat의 수치가 떨어지게 된다.
    [SerializeField]
    private float defaultValue;

    // DealDamageAction에 있는 같은 이름의 변수들과 동일한 역할
    [SerializeField]
    private Stat bonusValueStat;
    [SerializeField]
    private float bonusValueStatFactor;
    [SerializeField]
    private float bonusValuePerLevel;
    [SerializeField]
    private float bonusValuePerStack;

    // 적용할 값을 Stat의 DefaultValue에 더할 것인가? Bonus Value로 추가할 것인가? 
    // ※ HP나 MP 같은 소모성 Stat : DefaultValue에 더함
    // ※ 그 외 힘, 민첩, 지능같은 Stat : Bonus Value로 추가
    [SerializeField]
    private bool isBonusType = true;

    // 적용한 값을 Release할 때, 되돌릴 것인가?
    // ex) 힘을 증가시키는 Buff라면, Buff가 끝났을 때, Bonus가 사라지고 원래 힘 수치로 돌아온다.
    [SerializeField]
    private bool isUndoOnRelease = true;

    // Stat을 증가시킨 수치를 저장해두기 위한 변수 
    // → 수치를 저장해뒀다가 Release할 때, 이 수치를 다시 빼주는 걸로 Undo를 실행할 수 있다. 
    private float totalValue;

    #region 각각의 Stat Value들을 계산해서 가져오는 함수들 
    private float GetDefaultValue(Effect effect)
       => defaultValue + (effect.DataBonusLevel * bonusValuePerLevel);

    private float GetStackValue(int stack)
        => (stack - 1) * bonusValuePerStack;

    private float GetBonusStatValue(Entity user)
        => user.Stats.GetValue(bonusValueStat) * bonusValueStatFactor;

    private float GetTotalValue(Effect effect, Entity user, int stack, float scale)
    {
        totalValue = GetDefaultValue(effect) + GetStackValue(stack);
        if (bonusValueStat)
            totalValue += GetBonusStatValue(user);

        totalValue *= scale;

        return totalValue;
    }
    #endregion

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        totalValue = GetTotalValue(effect, user, stack, scale);

        if (isBonusType)
            // Stat의 BonusValue로 추가 
            // → key 값은 해당 Action
            target.Stats.SetBonusValue(stat, this, totalValue);
        else
            // Stat의 DefaultValue로 추가 
            target.Stats.IncreaseDefaultValue(stat, totalValue);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        if (!isUndoOnRelease)
            return;

        // BonusValue 삭제 
        if (isBonusType)
            target.Stats.RemoveBonusValue(stat, this);
        // 증가시켰던 DefaultValue 감소 
        else
            target.Stats.IncreaseDefaultValue(stat, -totalValue);
    }

    public override void OnEffectStackChanged(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        // Bonus Type일시 Release 실행 
        if (!isBonusType)
            Release(effect, user, target, level, scale);

        // 새로운 Stack 값으로 Apply 하기 
        // → Default 값인 경우, 증가시키고 그 상태에서 다시 증가시킴
        Apply(effect, user, target, level, stack, scale);
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValuesByKeyword = new Dictionary<string, string>
        {
            { "stat", stat.DisplayName },
            { "defaultValueInt", GetDefaultValue(effect).ToString("0.##") },
            { "defaultValueFloat", (GetDefaultValue(effect) * 100f).ToString() + "%" },
            { "bonusDamageStat", bonusValueStat?.DisplayName ?? string.Empty },
            { "bonusDamageStatFactor", (bonusValueStatFactor * 100f).ToString() + "%" },
            { "bonusDamageByLevel", bonusValuePerLevel.ToString() },
            { "bonusDamageByStack", bonusValuePerStack.ToString() },
        };

        if (effect.Owner != null)
        {
            descriptionValuesByKeyword.Add("totalValue",
                GetTotalValue(effect, effect.User, effect.CurrentStack, effect.Scale).ToString("0.##"));
        }

        return descriptionValuesByKeyword;
    }

    public override object Clone()
    {
        return new IncreaseStatAction()
        {
            stat = stat,
            defaultValue = defaultValue,
            bonusValueStat = bonusValueStat,
            bonusValueStatFactor = bonusValueStatFactor,
            bonusValuePerLevel = bonusValuePerLevel,
            bonusValuePerStack = bonusValuePerStack,
            isBonusType = isBonusType,
            isUndoOnRelease = isUndoOnRelease
        };
    }
}
