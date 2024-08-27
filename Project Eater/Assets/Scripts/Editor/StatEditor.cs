using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Stat�� Serialize ������ ��µǵ��� StatEditor �ۼ�
[CustomEditor(typeof(Stat))]
public class StatEditor : IdentifiedObjectEditor
{
    #region Variable
    // serializedProperty : Serialize ������ ������ ��� ���� Class
    private SerializedProperty isPercentTypeProperty;
	private SerializedProperty maxValueProperty;
	private SerializedProperty minValueProperty;
	private SerializedProperty defaultValueProperty;
    #endregion

    // GameObject�� ScriptableObject�� ������ Inspector â�� GUI�� �׷��� �� ȣ��Ǵ� �Լ�
    protected override void OnEnable()
    {
        base.OnEnable();

        isPercentTypeProperty = serializedObject.FindProperty("isPercentType");
        maxValueProperty = serializedObject.FindProperty("maxValue");
        minValueProperty = serializedObject.FindProperty("minValue");
        defaultValueProperty = serializedObject.FindProperty("defaultValue");
    }

    // OnInspectorGUI�� �ڵ�� Unity�� �ν����Ϳ� �����͸� ǥ���� ������ ����
    // �� Editor �Լ��� GUI�� �׸��� �Լ��� �� �Լ����� ȣ���ؾ� �Ѵ�
    // �� Editor�� �ٹ̴� �۾��� ����
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // Setting�̶�� ��� �ڽ� �׸���
        if (DrawFoldoutTitle("Setting"))
        {
            EditorGUILayout.PropertyField(isPercentTypeProperty);
            EditorGUILayout.PropertyField(maxValueProperty);
            EditorGUILayout.PropertyField(minValueProperty);
            EditorGUILayout.PropertyField(defaultValueProperty);
        }

        // Serialize �������� �� ��ȭ�� ������(=��ũ�� ������)
        // �� �۾��� ������ ������ �ٲ� ���� ������� �ʾƼ� ���� ������ ���ư�
        serializedObject.ApplyModifiedProperties();
    }
}
