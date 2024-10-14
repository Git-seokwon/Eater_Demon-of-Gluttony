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

		// ��ü�� Serialize �������� ���� Update�Ѵ�. 
		serializedObject.Update();

		// �� labelWidth : �ν����� â�� �������� ���� �̸� ĭ�� ���� 
		// Label(=Inspector â�� ǥ�õǴ� ������ �̸�)�� ���̸� �ø�
		// �� ���� ���� �� Serialize ������ ���, �⺻ ���̷δ� �ν�����â���� �������� ���� �߷��� ������ ������ ���̸� �ø���. 
		float preLevelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 260f;

        // Foldout Title�� Title�� �´� SerializeProperty�� �׷��ִ� �Լ��� 
        DrwaSettings();
        DrawOptions();
		DrawEffectDatas();

		EditorGUIUtility.labelWidth = preLevelWidth;

		serializedObject.ApplyModifiedProperties();
    }

    private void DrwaSettings()
    {
		// Foldout Title("Setting")�� �׷��ְ�, Ȯ��� ���°� �ƴ϶�� �ٷ� return �Ѵ�. 
		if (!DrawFoldoutTitle("Setting"))
			return;

		CustomEditorUtility.DrawEnumToolbar(typeProperty);

		// �ٸ� ������Ƽ�� �������� �ֱ� ���� ������ �׸���. 
		EditorGUILayout.Space();
		CustomEditorUtility.DrawUnderLine();
		EditorGUILayout.Space();

        // isAllowDuplicateProperty ������Ƽ �׷��ֱ� 
        EditorGUILayout.PropertyField(isAllDuplicateProperty);

        // isAllDuplicateProperty�� false��� removeDuplicateTargetOptionProperty Option �׷��ֱ� 
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
		// Effect�� Data�� �ƹ��͵� �������� ������ 1���� �ڵ������� ����� �ش�. 
		if (effectDatasProperty.arraySize == 0)
		{
			effectDatasProperty.arraySize++;

            // �� FindPropertyRelative : serializedObject�� FindProperty�� �Ȱ��� ����
            // �߰��� Data�� Level�� 1�� ����
            effectDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
		}

		if (!DrawFoldoutTitle("Data"))
			return;

        // Property�� �������� ���ϰ� GUI Enable�� false�� �ٲ�
        GUI.enabled = false;

        // ������ EffectData(= ���� ���� Level�� Data)�� ������
        // �� �Ʒ��ʿ��� Level�� �������� Data�� �������� �������ش�. 
        var laseEffectData = effectDatasProperty.GetArrayElementAtIndex(effectDatasProperty.arraySize - 1);

        // maxLevel�� ������ Data�� Level�� ����
        maxLevelProperty.intValue = laseEffectData.FindPropertyRelative("level").intValue;

		EditorGUILayout.PropertyField(maxLevelProperty);
		GUI.enabled = true;

		for (int i = 0; i < effectDatasProperty.arraySize; i++)
		{
			var property = effectDatasProperty.GetArrayElementAtIndex(i);

            var startDelayProperty = property.FindPropertyRelative("startDelayByApplyCycle");

			EditorGUILayout.BeginVertical("HelpBox");
			{
                // Data�� Level�� Data ������ ���� X Button�� �׷��ִ� Foldout Title
                // �� ��, ù��° Data(= index 0) ����� �ȵǱ� ������ X Button�� �׷����� ����
                // �� X Button�� ������ Data�� �������� true�� return
                // �� i != 0 : X ��ư�� �׷����� ����
                // �� i != 0 �̸� 1 level data�� 0�� index�̹Ƿ� ������ false�� �Ǿ� X ��ư�� �׸��� �ʰ�, 
                //    1���ʹ� ������ true�� �Ǿ� X ��ư�� �׷��ش�.
                if (DrawRemovableLevelFoldout(effectDatasProperty, property, i, i != 0))
				{
                    EditorGUILayout.EndVertical();
					break;
                }

                // �������� �ʾҰ�, Foldout�� Ȯ��
                // �� EffectData GUI�� �׷��ش�
                if (property.isExpanded)
				{
					// �鿩���� 
					EditorGUI.indentLevel++;

					var levelProperty = property.FindPropertyRelative("level");

                    // Level Property�� �׷��ֱ�
                    // �� Level ���� �Ұ���
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(levelProperty);
                    GUI.enabled = true;

                    // maxStack �� �׷��ֱ�
                    var maxStackProperty = property.FindPropertyRelative("maxStack");
					EditorGUILayout.PropertyField(maxStackProperty);

                    // maxStack�� �ּ� ���� 1 ���Ϸ� ������ ���ϰ� ��
                    maxStackProperty.intValue = Mathf.Max(maxStackProperty.intValue, 1);

                    // stackActions�� ���� Size�� �����ϰ�, stackActions�� �׷��ش�.
                    var stackActionsProperty = property.FindPropertyRelative("stackActions");
					var prevStrackActionSize = stackActionsProperty.arraySize;
					EditorGUILayout.PropertyField(stackActionsProperty);

                    // stackActions�� Element�� �߰��ƴٸ�(Size�� ����� Size���� Ŀ���ٸ�),
                    // ���� �߰��� Element�� soft copy�� action ������ Deep Copy�� ����
                    // �� SubclassSelectorAttribute ���� �ذ�
                    // �� SubclassSelectorAttribute ����
                    // �� SubclassSelectorAttribute�� ����� ���� Ÿ�� ��ü�� ������ ���, ���� ���簡 �ƴ϶� ���� ���簡
                    //    �̷����. �׷��� ���� ������ Data�� �ֱ� ������ Data�� ����� �Ȱ��� ��ü�� ����Ű�� �ִ� ��Ȳ�� �߻� 
					//    �� ������ 6 level���� 7 level Data�� ���������� 7 level Data�� �����ϸ� 6 level�� ���� �����ȴ�. 
					//    �̸� �ذ��ϱ� ����, ���� ���簡 �ƴ� ���� ���縦 ����Ѵ�. 
                    if (stackActionsProperty.arraySize > prevStrackActionSize)
					{
                        var lastStackActionProperty = stackActionsProperty.GetArrayElementAtIndex(prevStrackActionSize);
						var actionProperty = lastStackActionProperty.FindPropertyRelative("action");
						CustomEditorUtility.DeepCopySerializeReference(actionProperty);
					}

                    // StackAction���� stack ������ �Է� ������ �ִ� ���� MaxStack ������ ����
                    for (int stackActionIndex = 0; stackActionIndex < stackActionsProperty.arraySize; stackActionIndex++)
					{
						var stackActionProperty = stackActionsProperty.GetArrayElementAtIndex(stackActionIndex);
						var stackProperty = stackActionProperty.FindPropertyRelative("stack");
						stackProperty.intValue = Mathf.Clamp(stackProperty.intValue, 1, maxStackProperty.intValue);
					}

                    // ������ Serialized �������� Default ���·� �׷��ֱ� 
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("action"));
					EditorGUILayout.PropertyField(property.FindPropertyRelative("runningFinishOption"));
					EditorGUILayout.PropertyField(property.FindPropertyRelative("isApplyAllWhenDurationExpires"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("duration"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCount"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCycle"));
                    CustomEditorUtility.DrawEnumToolbar(startDelayProperty);
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("customActions"));

					// �鿩���� ���� 
					EditorGUI.indentLevel -= 1;
                }
			}
			EditorGUILayout.EndVertical();
		}

        // ��� EffectData�� �׸����� EffectDatas�� ���ο� level Data�� �߰��ϴ� Button �׸��� 
        if (GUILayout.Button("Add New Level"))
		{
            // �迭 ���̸� �÷��� ���ο� Element�� ����
            var lastArraySize = effectDatasProperty.arraySize++;

			var prevElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
			var newElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize);

            // �� Element�� Level�� ���� Element Level + 1
            var newElementLevel = prevElementProperty.FindPropertyRelative("level").intValue + 1;
			newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;

            // �� Data�� Module���� Deep Copy (SubclassSelector Attribute�� ����� ��ü��)
            // 1) effectDatasProperty ������ EffectStackAction �迭�� stackActions �ȿ� �ִ� action ���� DeppCopy
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("stackActions"), "action");
            // 2) effectDatasProperty�� action ���� DeepCopy
            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));
            // 3) effectDatasProperty�� customActions ���� DeepCopy
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActions"));
        }
    }
}
