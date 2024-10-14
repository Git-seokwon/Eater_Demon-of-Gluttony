using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [System.Serializable]
    public class TestSkillAction : SkillAction
    {
        public override void Release(Skill skill)
        {
            Debug.Log("Action Release");
        }

        public override void Apply(Skill skill)
        {
            // Skill ù ��° Target�� �̸� ��� 
            Debug.Log($"Action Target: {skill.Targets[0].name}");
            // � Effect���� ����Ǵ��� ��� 
            foreach (var effect in skill.currentEffects)
                Debug.Log($"Apply: {effect.CodeName} Lv.{effect.Level}");
        }

        public override void Start(Skill skill)
        {
            Debug.Log("Action Start");
        }

        public override object Clone() => new TestSkillAction();
    }
}
