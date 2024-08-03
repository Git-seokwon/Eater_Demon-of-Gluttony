using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectSelector // 변수로 설정한 Effect의 사본을 생성해주는 Class
                            // → Skill이 level마다 가지게 될 Effect들을 설정하는데 사용된다. 
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

        // ※ owner       : Skill
        // ※ owner.Owner : Skill의 Owner(Entity)
        clone.Setup(owner, owner.Owner, level);
        return clone;
    }
}
