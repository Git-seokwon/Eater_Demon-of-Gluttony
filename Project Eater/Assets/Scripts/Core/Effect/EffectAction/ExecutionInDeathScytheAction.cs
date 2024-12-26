using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExecutionInDeathScytheAction : EffectAction
{
    [SerializeField]
    private float executionPercentage;
    // �ش� Impact�� ������Ʈ ��ü���� ������Բ� �ڵ带 �ۼ��ؾ� �Ѵ�. 
    [SerializeField]
    private GameObject executionImpact;

    public override void Start(Effect effect, Entity user, Entity target, int level, float scale) => target.onKill += OnKill;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        // ��� ü�� ���� �������� 
        var targetFullness = target.Stats.FullnessStat.Value;
        var targetMaxFullness = target.Stats.FullnessStat.MaxValue;

        var fullnessPercentage = targetFullness / targetMaxFullness;
        if (fullnessPercentage < executionPercentage)
        {
            PoolManager.Instance.ReuseGameObject(executionImpact, target.transform.position, Quaternion.identity);
            target.TakeDamage(user, effect, targetMaxFullness + 10, true, false);
        }

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale) => target.onKill -= OnKill;

    // �ش� Effect(DEATHSCYTHE_DAMAGE)�� ���� óġ�Ǹ� Stack�� 1 ���� 
    public void OnKill(Entity instigator, object causer, Entity target)
    {

        if ((causer as Effect).CodeName == "DEATHSCYTHE_EXECUTION")
        {
            (instigator as PlayerEntity).DeathStack += 1;
        }
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>
        {
            ["executionPercentage"] = (executionPercentage * 100f).ToString() + "%",
        };

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new ExecutionInDeathScytheAction()
        {
            executionPercentage = executionPercentage,
            executionImpact = executionImpact
        };
    }
}
