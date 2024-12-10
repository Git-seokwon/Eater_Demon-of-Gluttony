using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReaperAttackAction : EffectAction
{
    // �⺻������ �� Damage
    [SerializeField]
    private float defaultDamage;

    // Bonus ������ �� Stat
    // �� Target�� ���� Entity�� Stat �� ��� Stat�� Bonus Damage�� ���� 
    [SerializeField]
    private Stat bonusDamageStat;

    // Bonus ���� �� Stat�� ������ Factor
    // �� Stat�� �ִ� Bonus �� : bonusDamageStat.Value * bonusDamageStatFactor
    // Ex) ���ݷ��� 30% ���� : bonusDamageStatFactor = 0.3
    [SerializeField]
    private float bonusDamageStatFactor;

    [SerializeField]
    private float healthPercentage;

    [SerializeField]
    private float additionalDamagePercentage;

    [SerializeField]
    private bool isTrueDamage;

    #region ������ Damage���� ����ؼ� �������� �Լ��� 
    // ���ڷ� ���� user Entity���� Stat ���� ã�ƿͼ� Factor�� ���� ��ȯ
    private float GetBonusStatDamage(Entity user)
        => user.Stats.GetValue(bonusDamageStat) * bonusDamageStatFactor;

    private float GetTotalDamage(Effect effect, Entity user, float scale)
    {
        var totalDamage = defaultDamage;
        if (bonusDamageStat)
            totalDamage += GetBonusStatDamage(user);

        // ���������� Effect�� Scale�� Damage�� Scaling��
        // ex) Charge�� �� �ƴٰų� Ư�� ������ Effect�� �������ٸ� Damage�� �������� 
        //     Over Charge�� �ƴٰų� Ư�� ������ Effect�� ��ȭ�ƴٸ� �׸�ŭ Damage�� ��������. 
        totalDamage *= scale;

        return totalDamage;
    }
    #endregion

    // ������ �������� �ִ� ȿ��
    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        var totalDamage = GetTotalDamage(effect, user, scale);

        // �߰� ������ ����
        if (HelperUtilities.IsHealthUnderPercentage(user, healthPercentage))
            totalDamage += totalDamage * additionalDamagePercentage;

        // �������� �� Causer�� Action�� ������ Effect�� �Ѱ��ش�. 
        // �� � Entity�� � Effect�� �󸶳� Damage�� ����� �� �� �ִ�.
        target.TakeDamage(user, effect, totalDamage, isTrueDamage);

        return true;
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>
        {
            // .## : �Ҽ��� �� ° �ڸ����� ���ڿ��� ǥ����
            ["defaultDamage"] = defaultDamage.ToString(".##"),
            // bonusDamageStat�� �ִٸ� Stat�� DisplayName�� �ǰ� ���ٸ� �� ���ڿ�
            ["bonusDamageStat"] = bonusDamageStat?.DisplayName ?? string.Empty,
            // bonusDamageStatFactor�� ���ϱ� 100�� ���� %�� ���� ���ڿ� 
            ["bonusDamageStatFactor"] = (bonusDamageStatFactor * 100f).ToString() + "%",
            ["healthPercentage"] = (healthPercentage * 100f).ToString() + "%",
            ["additionalDamagePercentage"] = (additionalDamagePercentage * 100f).ToString() + "%"
        };

        if (effect.User)
        {
            descriptionValueByKeyword["totalDamage"] =
                GetTotalDamage(effect, effect.User, effect.Scale).ToString(".##");
        }

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new ReaperAttackAction()
        {
            defaultDamage = defaultDamage,
            bonusDamageStat = bonusDamageStat,
            bonusDamageStatFactor = bonusDamageStatFactor,
            healthPercentage = healthPercentage,
            additionalDamagePercentage = additionalDamagePercentage,
            isTrueDamage = isTrueDamage
        };
    }
}
