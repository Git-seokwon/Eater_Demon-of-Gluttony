using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectSelector // ������ ������ Effect�� �纻�� �������ִ� Class
                            // �� Skill�� level���� ������ �� Effect���� �����ϴµ� ���ȴ�. 
{
    [SerializeField]
    private int level;
    [SerializeField]
    private Effect effect;

    public int Level => level;
    public Effect Effect => effect;

    public Effect CreateEffect(Skill owner)
    {
        var clone = effect.Clone() as Effect;

        // �� owner       : Skill
        // �� owner.Owner : Skill�� Owner(Entity)
        clone.Setup(owner, owner.Owner, level);
        return clone;
    }
}
