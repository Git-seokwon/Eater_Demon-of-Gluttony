using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� IncreaseStatAction : Target�� Stat ��ġ�� ���������ִ� ȿ���� ������ Action
// �� Stat���� ���� HP�� �����Ѵ�. ���� HP�� ������Ų�ٴ� �� 'ȸ��'�� �ǹ��̱� ������ ġ�� ȿ���� IncreaseStatAction�� ����
[System.Serializable]
public class IncreaseStatAction : EffectAction
{
    // ��ġ�� ������ų Target Stats
    [SerializeField]
    private Stat stat;

    // �⺻������ ��ġ�� �󸶳� ������ų ���ΰ� 
    // �� ���� ���� �ִ´ٸ� Stat�� ��ġ�� �������� �ȴ�.
    [SerializeField]
    private float defaultValue;

    // DealDamageAction�� �ִ� ���� �̸��� ������� ������ ����
    [SerializeField]
    private Stat bonusValueStat;
    [SerializeField]
    private float bonusValueStatFactor;
    [SerializeField]
    private float bonusValuePerLevel;
    [SerializeField]
    private float bonusValuePerStack;

    // ������ ���� Stat�� DefaultValue�� ���� ���ΰ�? Bonus Value�� �߰��� ���ΰ�? 
    // �� HP�� MP ���� �Ҹ� Stat : DefaultValue�� ����
    // �� �� �� ��, ��ø, ���ɰ��� Stat : Bonus Value�� �߰�
    [SerializeField]
    private bool isBonusType = true;

    // ������ ���� Release�� ��, �ǵ��� ���ΰ�?
    // ex) ���� ������Ű�� Buff���, Buff�� ������ ��, Bonus�� ������� ���� �� ��ġ�� ���ƿ´�.
    [SerializeField]
    private bool isUndoOnRelease = true;

    // Stat�� ������Ų ��ġ�� �����صα� ���� ���� 
    // �� ��ġ�� �����ص״ٰ� Release�� ��, �� ��ġ�� �ٽ� ���ִ� �ɷ� Undo�� ������ �� �ִ�. 
    private float totalValue;

    #region ������ Stat Value���� ����ؼ� �������� �Լ��� 
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
            // Stat�� BonusValue�� �߰� 
            // �� key ���� �ش� Action
            target.Stats.SetBonusValue(stat, this, totalValue);
        else
            // Stat�� DefaultValue�� �߰� 
            target.Stats.IncreaseDefaultValue(stat, totalValue);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        if (!isUndoOnRelease)
            return;

        // BonusValue ���� 
        if (isBonusType)
            target.Stats.RemoveBonusValue(stat, this);
        // �������״� DefaultValue ���� 
        else
            target.Stats.IncreaseDefaultValue(stat, -totalValue);
    }

    public override void OnEffectStackChanged(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        // Bonus Type�Ͻ� Release ���� 
        if (!isBonusType)
            Release(effect, user, target, level, scale);

        // ���ο� Stack ������ Apply �ϱ� 
        // �� Default ���� ���, ������Ű�� �� ���¿��� �ٽ� ������Ŵ
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
