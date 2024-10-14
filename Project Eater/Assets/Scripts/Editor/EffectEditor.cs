using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Effect))]
public class EffectEditor : IdentifiedObjectEditor
{
	#region SerializeProperty
	private SerializedProperty typeProperty;
	private SerializedProperty isAllDuplicateProperty;
	private SerializedProperty removeDuplicateTargetOptionProperty;

	private SerializedProperty isShowInUIProperty;

	private SerializedProperty maxLevelProperty;
	private SerializedProperty effectDatasProperty;
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

		typeProperty = serializedObject.FindProperty("type");
		isAllDuplicateProperty = serializedObject.FindProperty("isAllowDuplicate");
		removeDuplicateTargetOptionProperty = serializedObject.FindProperty("removeDuplicateTargetOption");

		isShowInUIProperty = serializedObject.FindProperty("isShowInUI");

		maxLevelProperty = serializedObject.FindProperty("maxLevel");
		effectDatasProperty = serializedObject.FindProperty("effectDatasAllLevel");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		// 객체의 Serialize 변수들의 값을 Update한다. 
		serializedObject.Update();

		// ※ labelWidth : 인스펙터 창에 보여지는 변수 이름 칸의 넓이 
		// Label(=Inspector 창에 표시되는 변수의 이름)의 길이를 늘림
		// → 변수 명이 긴 Serialize 변수의 경우, 기본 넓이로는 인스펙터창에서 변수명이 많이 잘려서 나오기 때문에 길이를 늘린다. 
		float preLevelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 260f;

        // Foldout Title과 Title에 맞는 SerializeProperty를 그려주는 함수들 
        DrwaSettings();
        DrawOptions();
		DrawEffectDatas();

		EditorGUIUtility.labelWidth = preLevelWidth;

		serializedObject.ApplyModifiedProperties();
    }

    private void DrwaSettings()
    {
		// Foldout Title("Setting")을 그려주고, 확장된 상태가 아니라면 바로 return 한다. 
		if (!DrawFoldoutTitle("Setting"))
			return;

		CustomEditorUtility.DrawEnumToolbar(typeProperty);

		// 다른 프로퍼티와 구분점을 주기 위해 밑줄을 그린다. 
		EditorGUILayout.Space();
		CustomEditorUtility.DrawUnderLine();
		EditorGUILayout.Space();

        // isAllowDuplicateProperty 프로퍼티 그려주기 
        EditorGUILayout.PropertyField(isAllDuplicateProperty);

        // isAllDuplicateProperty가 false라면 removeDuplicateTargetOptionProperty Option 그려주기 
        if (!isAllDuplicateProperty.boolValue)
			CustomEditorUtility.DrawEnumToolbar(removeDuplicateTargetOptionProperty);
    }

    private void DrawOptions()
    {
		if (!DrawFoldoutTitle("Option"))
			return;

		EditorGUILayout.PropertyField(isShowInUIProperty);
    }

    private void DrawEffectDatas()
    {
		// Effect의 Data가 아무것도 존재하지 않으면 1개를 자동적으로 만들어 준다. 
		if (effectDatasProperty.arraySize == 0)
		{
			effectDatasProperty.arraySize++;

            // ※ FindPropertyRelative : serializedObject의 FindProperty와 똑같이 동작
            // 추가한 Data의 Level을 1로 설정
            effectDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
		}

		if (!DrawFoldoutTitle("Data"))
			return;

        // Property를 수정하지 못하게 GUI Enable의 false로 바꿈
        GUI.enabled = false;

        // 마지막 EffectData(= 가장 높은 Level의 Data)를 가져옴
        // → 아래쪽에서 Level을 기준으로 Data를 오름차순 정렬해준다. 
        var laseEffectData = effectDatasProperty.GetArrayElementAtIndex(effectDatasProperty.arraySize - 1);

        // maxLevel을 마지막 Data의 Level로 고정
        maxLevelProperty.intValue = laseEffectData.FindPropertyRelative("level").intValue;

		EditorGUILayout.PropertyField(maxLevelProperty);
		GUI.enabled = true;

		for (int i = 0; i < effectDatasProperty.arraySize; i++)
		{
			var property = effectDatasProperty.GetArrayElementAtIndex(i);

            var startDelayProperty = property.FindPropertyRelative("startDelayByApplyCycle");

			EditorGUILayout.BeginVertical("HelpBox");
			{
                // Data의 Level과 Data 삭제를 위한 X Button을 그려주는 Foldout Title
                // → 단, 첫번째 Data(= index 0) 지우면 안되기 때문에 X Button을 그려주지 않음
                // → X Button을 눌러서 Data가 지워지면 true를 return
                // ※ i != 0 : X 버튼을 그려줄지 조건
                // → i != 0 이면 1 level data는 0번 index이므로 조건이 false가 되어 X 버튼을 그리지 않고, 
                //    1부터는 조건이 true가 되어 X 버튼을 그려준다.
                if (DrawRemovableLevelFoldout(effectDatasProperty, property, i, i != 0))
				{
                    EditorGUILayout.EndVertical();
					break;
                }

                // 삭제되지 않았고, Foldout을 확장
                // → EffectData GUI를 그려준다
                if (property.isExpanded)
				{
					// 들여쓰기 
					EditorGUI.indentLevel++;

					var levelProperty = property.FindPropertyRelative("level");

                    // Level Property를 그려주기
                    // → Level 수정 불가능
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(levelProperty);
                    GUI.enabled = true;

                    // maxStack 값 그려주기
                    var maxStackProperty = property.FindPropertyRelative("maxStack");
					EditorGUILayout.PropertyField(maxStackProperty);

                    // maxStack의 최소 값을 1 이하로 내리지 못하게 함
                    maxStackProperty.intValue = Mathf.Max(maxStackProperty.intValue, 1);

                    // stackActions의 현재 Size를 저장하고, stackActions을 그려준다.
                    var stackActionsProperty = property.FindPropertyRelative("stackActions");
					var prevStrackActionSize = stackActionsProperty.arraySize;
					EditorGUILayout.PropertyField(stackActionsProperty);

                    // stackActions에 Element가 추가됐다면(Size가 저장된 Size보다 커졌다면),
                    // 새로 추가된 Element의 soft copy된 action 변수를 Deep Copy로 변경
                    // → SubclassSelectorAttribute 문제 해결
                    // ※ SubclassSelectorAttribute 문제
                    // → SubclassSelectorAttribute를 사용한 참조 타입 객체를 복사할 경우, 깊은 복사가 아니라 얕은 복사가
                    //    이루어짐. 그래서 새로 생성된 Data와 최근 생성된 Data가 사실은 똑같은 객체를 가리키고 있는 상황이 발생 
					//    이 때문에 6 level에서 7 level Data를 생성했으면 7 level Data를 수정하면 6 level도 같이 수정된다. 
					//    이를 해결하기 위해, 얕은 복사가 아닌 깊은 복사를 사용한다. 
                    if (stackActionsProperty.arraySize > prevStrackActionSize)
					{
                        var lastStackActionProperty = stackActionsProperty.GetArrayElementAtIndex(prevStrackActionSize);
						var actionProperty = lastStackActionProperty.FindPropertyRelative("action");
						CustomEditorUtility.DeepCopySerializeReference(actionProperty);
					}

                    // StackAction들의 stack 변수에 입력 가능한 최대 값을 MaxStack 값으로 제한
                    for (int stackActionIndex = 0; stackActionIndex < stackActionsProperty.arraySize; stackActionIndex++)
					{
						var stackActionProperty = stackActionsProperty.GetArrayElementAtIndex(stackActionIndex);
						var stackProperty = stackActionProperty.FindPropertyRelative("stack");
						stackProperty.intValue = Mathf.Clamp(stackProperty.intValue, 1, maxStackProperty.intValue);
					}

                    // 나머지 Serialized 변수들을 Default 형태로 그려주기 
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("action"));
					EditorGUILayout.PropertyField(property.FindPropertyRelative("runningFinishOption"));
					EditorGUILayout.PropertyField(property.FindPropertyRelative("isApplyAllWhenDurationExpires"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("duration"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCount"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCycle"));
                    CustomEditorUtility.DrawEnumToolbar(startDelayProperty);
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("customActions"));

					// 들여쓰기 종료 
					EditorGUI.indentLevel -= 1;
                }
			}
			EditorGUILayout.EndVertical();
		}

        // 모든 EffectData를 그리고나면 EffectDatas에 새로운 level Data를 추가하는 Button 그리기 
        if (GUILayout.Button("Add New Level"))
		{
            // 배열 길이를 늘려서 새로운 Element를 생성
            var lastArraySize = effectDatasProperty.arraySize++;

			var prevElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
			var newElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize);

            // 새 Element의 Level은 이전 Element Level + 1
            var newElementLevel = prevElementProperty.FindPropertyRelative("level").intValue + 1;
			newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;

            // 새 Data의 Module들을 Deep Copy (SubclassSelector Attribute를 사용한 객체만)
            // 1) effectDatasProperty 내부의 EffectStackAction 배열인 stackActions 안에 있는 action 변수 DeppCopy
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("stackActions"), "action");
            // 2) effectDatasProperty의 action 변수 DeepCopy
            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));
            // 3) effectDatasProperty의 customActions 변수 DeepCopy
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActions"));
        }
    }
}
