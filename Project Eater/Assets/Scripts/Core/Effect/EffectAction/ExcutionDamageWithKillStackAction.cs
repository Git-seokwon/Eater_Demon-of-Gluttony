using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사신의 낫 스킬 효과 
[System.Serializable]
public class ExcutionDamageWithKillStackAction : EffectAction
{
    #region 데미지 관련 변수 
    // 기본적으로 줄 Damage
    [SerializeField]
    private float defaultDamage;
    // Bonus 값으로 줄 Stat
    // → Target을 때린 Entity의 Stat 중 어느 Stat을 Bonus Damage로 줄지 
    [SerializeField]
    private Stat bonusDamageStat;
    // Bonus 값을 줄 Stat에 적용할 Factor
    // → Stat이 주는 Bonus 값 : bonusDamageStat.Value * bonusDamageStatFactor
    // Ex) 공격력의 30% 피해 : bonusDamageStatFactor = 0.3
    [SerializeField]
    private float bonusDamageStatFactor;
    // Bonus Level마다 몇의 Bonus Damage를 줄 지
    [SerializeField]
    private float bonusDamagePerLevel;
    // Effect Stack당 줄 Bonus Damage
    // → Stack이 1일 때는 아직 Stack이 추가로 쌓인 상태가 아니니 Bonus Damage를 주지 않는다.
    [SerializeField]
    private float bonusDamagePerStack;
    [SerializeField]
    private bool isTrueDamage;
    #endregion

    #region 처형 관련 변수
    [Space(10)]
    [SerializeField]
    private float executionThreshold;
    [SerializeField]
    private GameObject executionImpact;
    #endregion

    #region 각각의 Damage들을 계산해서 가져오는 함수들 
    private float GetDefaultDamage(Effect effect)
        => defaultDamage + (effect.DataBonusLevel * bonusDamagePerLevel);

    // Stack의 시작 값은 1이므로 (stack - 1)을 곱한 것
    private float GetStackDamage(int stack)
        => (stack - 1) * bonusDamagePerStack;

    // 인자로 받은 user Entity에서 Stat 값을 찾아와서 Factor와 곱해 반환
    private float GetBonusStatDamage(Entity user)
        => user.Stats.GetValue(bonusDamageStat) * bonusDamageStatFactor;

    private float GetTotalDamage(Effect effect, Entity user, int stack, float scale)
    {
        // ※ Damage 계산 공식
        // (defaultValue + (bonusLevel * bonusDamageByLevel)) + ((stack - 1) * bonusDamageByStack) + (bonusDamageStat.Value * bonuDamageStatFactor);
        var totalDamage = GetDefaultDamage(effect) + GetStackDamage(stack);
        if (bonusDamageStat)
            totalDamage += GetBonusStatDamage(user);

        // 마지막으로 Effect의 Scale로 Damage를 Scaling함
        // ex) Charge가 덜 됐다거나 특정 이유로 Effect가 약해졌다면 Damage도 약해지고 
        //     Over Charge가 됐다거나 특정 이유로 Effect가 강화됐다면 그만큼 Damage도 강해진다. 
        totalDamage *= scale;

        return totalDamage;
    }
    #endregion

    public override void Start(Effect effect, Entity user, Entity target, int level, float scale)
        => target.onKilled += OnKilled;

    // 실제로 데미지를 주는 효과
    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        bool isCrit = false;

        float totalDamage = GetTotalDamage(effect, user, stack, scale);

        // 크리티컬 Apply
        float prevTotalDamage = totalDamage;
        totalDamage = HelperUtilities.GetApplyCritDamage(totalDamage, user.Stats.CritRateStat.Value, user.Stats.CritDamageStat.Value);
        if (!Mathf.Approximately(totalDamage, prevTotalDamage))
            isCrit = true;

        // 상대 체력 정보 가져오기 
        float targetFullness = target.Stats.FullnessStat.Value;
        float targetMaxFullness = target.Stats.FullnessStat.MaxValue;

        float fullnessPercentage = (targetFullness - totalDamage) / targetMaxFullness;
        // 보스 몬스터의 경우 50% 증가된 피해를 준다. 
        if (fullnessPercentage < executionThreshold && target.gameObject.tag == "Boss")
            totalDamage *= (1.5f);
        // 일반 몬스터의 경우 적을 처형시킨다.
        else if (fullnessPercentage < executionThreshold && target.gameObject.tag != "Boss")
        {
            float rendererSize = target.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
            var position = (Vector2)target.transform.position + Vector2.up * rendererSize;

            PoolManager.Instance.ReuseGameObject(executionImpact, position, Quaternion.identity);
            // 실수 계산 이므로 오차가 발생할 수 있기 때문에 10이라는 값을 더해 확실하게 처형 시킨다. 
            // → 망멸의 낫 스킬의 경우 100% 피 상태여도 처형하기 때문에 해당 상황을 고려하여 10을 더함
            target.TakeDamage(user, effect, targetMaxFullness + 10f, false, false, true);

            return true;
        }

        // 데미지를 준 Causer는 Action을 소유한 Effect를 넘겨준다. 
        // → 어떤 Entity가 어떤 Effect로 얼마나 Damage를 줬는지 알 수 있다.
        target.TakeDamage(user, effect, totalDamage, isCrit, true, isTrueDamage);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
        => target.onKilled -= OnKilled;

    // 해당 Effect(DEATHSCYTHE_DAMAGE)로 적이 처치되면 Stack을 1 증가 
    public void OnKilled(Entity instigator, object causer, Entity target)
    {

        if ((causer as Effect).CodeName == "DEATHSCYTHE_DAMAGE")
        {
            (instigator as PlayerEntity).DeathStack += 1;
        }
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>
        {
            // .## : 소수점 둘 째 자리까지 문자열로 표시함
            ["defaultDamage"] = GetDefaultDamage(effect).ToString(".##"),
            // bonusDamageStat이 있다면 Stat의 DisplayName이 되고 없다면 빈 문자열
            ["bonusDamageStat"] = bonusDamageStat?.DisplayName ?? string.Empty,
            // bonusDamageStatFactor를 곱하기 100한 다음 %를 붙인 문자열 
            ["bonusDamageStatFactor"] = (bonusDamageStatFactor * 100f).ToString() + "%",
            ["bonusDamagePerLevel"] = bonusDamagePerLevel.ToString(),
            ["bonusDamagePerStack"] = bonusDamagePerStack.ToString(),
            ["executionThreshold"] = (executionThreshold * 100f).ToString() + "%"
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
        return new ExcutionDamageWithKillStackAction()
        {
            defaultDamage = defaultDamage,
            bonusDamageStat = bonusDamageStat,
            bonusDamageStatFactor = bonusDamageStatFactor,
            bonusDamagePerLevel = bonusDamagePerLevel,
            bonusDamagePerStack = bonusDamagePerStack,
            isTrueDamage = isTrueDamage,
            executionThreshold = executionThreshold,
            executionImpact = executionImpact
        };
    }
}
