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
            // Skill 첫 번째 Target의 이름 출력 
            Debug.Log($"Action Target: {skill.Targets[0].name}");
            // 어떤 Effect들이 적용되는지 출력 
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
