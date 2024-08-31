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

            // skillClone이 null이라면 Start Skill Test라는 GUI Button을 그려주고 Button을 누르면 Test 함수를 실행하여 Test를 진행한다. 
            if (!skillClone)
            {
                if (GUI.Button(new Rect(boxRect.x + 50f, 4f, 100f, 30f), "Start Skill Test"))
                    Test();
            }
            // skillClone이 null이 아니라면 Test를 시작한 것 
            else
            {
                var labelRect = new Rect(boxRect.x + 2f, 4f, 200f, 30f);
                GUI.Label(labelRect, $"Skill: {skillClone.CodeName}");

                labelRect.y += 17f;
                GUI.Label(labelRect, $"Level: {skillClone.Level}");

                // 스킬이 MaxLevel이 아니라면 방금 그린 Level Label 우측에 Up Button을 그려준다. 
                if (!skillClone.IsMaxLevel)
                {
                    var buttonRect = new Rect(labelRect.x + 165f, labelRect.y, 30f, 17f);
                    if (GUI.Button(buttonRect, "Up"))
                    {
                        // Up Button을 누르면 Skill의 Level을 1 올려주고 Effect들의 Level을 Log로 띄워준다. 
                        skillClone.Level++;
                        foreach (var effect in skillClone.Effects)
                            Debug.Log($"Effect: {effect.CodeName}, Level {effect.Level}");
                    }
                }

                labelRect.y += 17f;
                // Skill의 현재 State의 이름을 그려준다. 
                GUI.Label(labelRect, skillClone.GetCurrentStateType().Name);

                labelRect.y += 17f;

                // Skill State에 따라서 Label을 그려준다. 
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

            // Skill의 사본을 만들어서 Setup을 시킨다. 
            skillClone = skill.Clone() as Skill;
            skillClone.Setup(GetComponent<Entity>());

            // Skill의 정보와 안내문을 Log로 띄운다. 
            Debug.Log($"Skill Code Name: {skillClone.CodeName}");
            Debug.Log($"Description: {skillClone.Description}");

            Debug.Log("Skill을 사용하려면 O를 누르세요.");
            Debug.Log("Test를 종료하려면 P를 누르세요.");

            while (true)
            {
                // Skill의 IsUseable이 true라면 Use 함수를 실행해서 동작시키기 
                if (Input.GetKeyDown(KeyCode.O))
                {
                    if (skillClone.IsUseable)
                        skillClone.Use();
                    else
                        Debug.Log("Skill을 사용할 수 없습니다.");
                }
                else if (Input.GetKeyDown(KeyCode.P))
                    break;

                // Update 함수가 계속 실행되면서 Skill의 Logic, 정확히는 StateMachine이 Update 되는 것
                skillClone.Update();
                yield return null;
            }

            Destroy(skillClone);
            skillClone = null;

            Debug.Log($"<color=green>[SkillTest] Success</color>");
        }
    }
}

