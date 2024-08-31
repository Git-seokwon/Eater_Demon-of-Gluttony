using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [RequireComponent(typeof(Entity))]
    public class SkillTest : MonoBehaviour
    {
        [SerializeField]
        private Skill skill;

        private Skill skillClone;

        private void OnGUI()
        {
            var boxRect = new Rect(254f, 2f, 200f, 115f);
            GUI.Box(boxRect, string.Empty);

            // skillClone�� null�̶�� Start Skill Test��� GUI Button�� �׷��ְ� Button�� ������ Test �Լ��� �����Ͽ� Test�� �����Ѵ�. 
            if (!skillClone)
            {
                if (GUI.Button(new Rect(boxRect.x + 50f, 4f, 100f, 30f), "Start Skill Test"))
                    Test();
            }
            // skillClone�� null�� �ƴ϶�� Test�� ������ �� 
            else
            {
                var labelRect = new Rect(boxRect.x + 2f, 4f, 200f, 30f);
                GUI.Label(labelRect, $"Skill: {skillClone.CodeName}");

                labelRect.y += 17f;
                GUI.Label(labelRect, $"Level: {skillClone.Level}");

                // ��ų�� MaxLevel�� �ƴ϶�� ��� �׸� Level Label ������ Up Button�� �׷��ش�. 
                if (!skillClone.IsMaxLevel)
                {
                    var buttonRect = new Rect(labelRect.x + 165f, labelRect.y, 30f, 17f);
                    if (GUI.Button(buttonRect, "Up"))
                    {
                        // Up Button�� ������ Skill�� Level�� 1 �÷��ְ� Effect���� Level�� Log�� ����ش�. 
                        skillClone.Level++;
                        foreach (var effect in skillClone.Effects)
                            Debug.Log($"Effect: {effect.CodeName}, Level {effect.Level}");
                    }
                }

                labelRect.y += 17f;
                // Skill�� ���� State�� �̸��� �׷��ش�. 
                GUI.Label(labelRect, skillClone.GetCurrentStateType().Name);

                labelRect.y += 17f;

                // Skill State�� ���� Label�� �׷��ش�. 
                if (skillClone.IsSearchingTarget)
                    GUI.Label(labelRect, $"Searching Target...");
                else if (skillClone.IsInState<CastingState>())
                    GUI.Label(labelRect, $"Casting {skillClone.CurrentCastTime:F2} To {skillClone.CastTime}");
                else if (skillClone.IsInState<ChargingState>())
                {
                    GUI.Label(labelRect, $"Charging Duration {skillClone.CurrentChargeDuration:F2} To {skillClone.ChargeDuration}");

                    labelRect.y += 17f;
                    GUI.Label(labelRect, $"Charging Power: {skillClone.CurrentChargePower:F2}");

                    labelRect.y += 17f;
                    GUI.Label(labelRect, $"Useable: {skillClone.IsMinChargeCompleted}");
                }
                else if (skillClone.IsInState<CooldownState>())
                    GUI.Label(labelRect, $"Cooldown {skillClone.CurrentCooldown:F2} To 0");
            }
        }

        [ContextMenu("Test")]
        private void Test() => StartCoroutine("TestCoroutine");


        private IEnumerator TestCoroutine()
        {
            Debug.Log($"<color=yellow>[SkillTest] Start</color>");

            // Skill�� �纻�� ���� Setup�� ��Ų��. 
            skillClone = skill.Clone() as Skill;
            skillClone.Setup(GetComponent<Entity>());

            // Skill�� ������ �ȳ����� Log�� ����. 
            Debug.Log($"Skill Code Name: {skillClone.CodeName}");
            Debug.Log($"Description: {skillClone.Description}");

            Debug.Log("Skill�� ����Ϸ��� O�� ��������.");
            Debug.Log("Test�� �����Ϸ��� P�� ��������.");

            while (true)
            {
                // Skill�� IsUseable�� true��� Use �Լ��� �����ؼ� ���۽�Ű�� 
                if (Input.GetKeyDown(KeyCode.O))
                {
                    if (skillClone.IsUseable)
                        skillClone.Use();
                    else
                        Debug.Log("Skill�� ����� �� �����ϴ�.");
                }
                else if (Input.GetKeyDown(KeyCode.P))
                    break;

                // Update �Լ��� ��� ����Ǹ鼭 Skill�� Logic, ��Ȯ���� StateMachine�� Update �Ǵ� ��
                skillClone.Update();
                yield return null;
            }

            Destroy(skillClone);
            skillClone = null;

            Debug.Log($"<color=green>[SkillTest] Success</color>");
        }
    }
}

