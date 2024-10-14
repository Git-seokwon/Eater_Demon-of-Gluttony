using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DealDamageAndStackOnKillAction : EffectAction
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

    // Bonus Level���� ���� Bonus Damage�� �� ��
    [SerializeField]
    private float bonusDamagePerLevel;

    // Effect Stack�� �� Bonus Damage
    // �� Stack�� 1�� ���� ���� Stack�� �߰��� ���� ���°� �ƴϴ� Bonus Damage�� ���� �ʴ´�.
    [SerializeField]
    private float bonusDamagePerStack;

    // �ش� ������ ���� Mark�� �������� �ʰ� ���� �����ϱ� 
    [SerializeField]
    private bool isTrueDamage;

    #region ������ Damage���� ����ؼ� �������� �Լ��� 
    private float GetDefaultDamage(Effect effect)
        => defaultDamage + (effect.DataBonusLevel * bonusDamagePerLevel);

    // Stack�� ���� ���� 1�̹Ƿ� (stack - 1)�� ���� ��
    private float GetStackDamage(int stack)
        => (stack - 1) * bonusDamagePerStack;

    // ���ڷ� ���� user Entity���� Stat ���� ã�ƿͼ� Factor�� ���� ��ȯ
    private float GetBonusStatDamage(Entity user)
        => user.Stats.GetValue(bonusDamageStat) * bonusDamageStatFactor;

    private float GetTotalDamage(Effect effect, Entity user, int stack, float scale)
    {
        // �� Damage ��� ����
        // (defaultValue + (bonusLevel * bonusDamageByLevel)) + ((stack - 1) * bonusDamageByStack) + (bonusDamageStat.Value * bonuDamageStatFactor);
        var totalDamage = GetDefaultDamage(effect) + GetStackDamage(stack);
        if (bonusDamageStat)
            totalDamage += GetBonusStatDamage(user);

        // ���������� Effect�� Scale�� Damage�� Scaling��
        // ex) Charge�� �� �ƴٰų� Ư�� ������ Effect�� �������ٸ� Damage�� �������� 
        //     Over Charge�� �ƴٰų� Ư�� ������ Effect�� ��ȭ�ƴٸ� �׸�ŭ Damage�� ��������. 
        totalDamage *= scale;

        return totalDamage;
    }
    #endregion

    // onKill Event ����
    public override void Start(Effect effect, Entity user, Entity target, int level, float scale) => target.onKill += OnKill;

    // ������ �������� �ִ� ȿ��
    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        var totalDamage = GetTotalDamage(effect, user, stack, scale);

        // ũ��Ƽ�� Apply
        totalDamage = HelperUtilities.GetApplyCritDamage(totalDamage, user.Stats.CritRateStat.Value, user.Stats.CritDamageStat.Value);

        // �������� �� Causer�� Action�� ������ Effect�� �Ѱ��ش�. 
        // �� � Entity�� � Effect�� �󸶳� Damage�� ����� �� �� �ִ�.
        target.TakeDamage(user, effect, totalDamage, isTrueDamage);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale) => target.onKill -= OnKill;


    // �ش� Effect(DEATHSCYTHE_DAMAGE)�� ���� óġ�Ǹ� Stack�� 1 ���� 
    public void OnKill(Entity instigator, object causer, Entity target)
    {

        if ((causer as Effect).CodeName == "DEATHSCYTHE_DAMAGE")
        {
            (instigator as PlayerEntity).CurrentStackCount += 1;
        }
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>
        {
            // .## : �Ҽ��� �� ° �ڸ����� ���ڿ��� ǥ����
            ["defaultDamage"] = GetDefaultDamage(effect).ToString(".##"),
            // bonusDamageStat�� �ִٸ� Stat�� DisplayName�� �ǰ� ���ٸ� �� ���ڿ�
            ["bonusDamageStat"] = bonusDamageStat?.DisplayName ?? string.Empty,
            // bonusDamageStatFactor�� ���ϱ� 100�� ���� %�� ���� ���ڿ� 
            ["bonusDamageStatFactor"] = (bonusDamageStatFactor * 100f).ToString() + "%",
            ["bonusDamagePerLevel"] = bonusDamagePerLevel.ToString(),
            ["bonusDamagePerStack"] = bonusDamagePerStack.ToString(),
        };

        if (effect.User)
        {
            descriptionValueByKeyword["totalDamage"] =
                GetTotalDamage(effect, effect.User, effect.CurrentStack, effect.Scale).ToString(".##");
        }

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new DealDamageAndStackOnKillAction()
        {
            defaultDamage = defaultDamage,
            bonusDamageStat = bonusDamageStat,
            bonusDamageStatFactor = bonusDamageStatFactor,
            bonusDamagePerLevel = bonusDamagePerLevel,
            bonusDamagePerStack = bonusDamagePerStack,
            isTrueDamage = isTrueDamage,
        };
    }
}
