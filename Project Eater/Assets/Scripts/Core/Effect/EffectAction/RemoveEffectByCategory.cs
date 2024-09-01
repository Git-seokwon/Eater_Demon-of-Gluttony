using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RemoveEffectByCategory : EffectAction
{
    // Target Effect�� ������ �ִ� category
    [SerializeField]
    private Category category;
    // Category�� ���� Effect�� �� ���� ���� ã�� Effect�� ���� ���ΰ�? ��� Effect�� ���� ���ΰ�?
    [SerializeField]
    private bool isRemoveAll;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        if (isRemoveAll)
            target.SkillSystem.RemoveEffectAll(category);
        else
            target.SkillSystem.RemoveEffect(category);

        return true;
    }

    // category�� DisplayName�� return 
    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
        => new Dictionary<string, string>() { { "category", category.DisplayName } };

    public override object Clone() => new RemoveEffectByCategory() { category = category, isRemoveAll = isRemoveAll };
}
