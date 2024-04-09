using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Stat의 Serialize 변수가 출력되도록 StatEditor 작성
[CustomEditor(typeof(Stat))]
public class StatEditor : IdentifiedObjectEditor
{
    #region Variable
    // serializedProperty : Serialize 변수의 정보를 담기 위한 Class
    private SerializedProperty isPercentTypeProperty;
	private SerializedProperty maxValueProperty;
	private SerializedProperty minValueProperty;
	private SerializedProperty defaultValueProperty;
    #endregion

    // GameObject나 ScriptableObject를 눌러서 Inspector 창에 GUI가 그려질 때 호출되는 함수
    protected override void OnEnable()
    {
        base.OnEnable();

        isPercentTypeProperty = serializedObject.FindProperty("isPercentType");
        maxValueProperty = serializedObject.FindProperty("maxValue");
        minValueProperty = serializedObject.FindProperty("minValue");
        defaultValueProperty = serializedObject.FindProperty("defaultValue");
    }

    // OnInspectorGUI의 코드는 Unity가 인스펙터에 에디터를 표시할 때마다 실행
    // → Editor 함수로 GUI를 그리는 함수를 이 함수에서 호출해야 한다
    // → Editor를 꾸미는 작업을 진행
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // Setting이라는 토글 박스 그리기
        if (DrawFoldoutTitle("Setting"))
        {
            EditorGUILayout.PropertyField(isPercentTypeProperty);
            EditorGUILayout.PropertyField(maxValueProperty);
            EditorGUILayout.PropertyField(minValueProperty);
            EditorGUILayout.PropertyField(defaultValueProperty);
        }

        // Serialize 변수들의 값 변화를 적용함(=디스크에 저장함)
        // 이 작업을 해주지 않으면 바뀐 값이 적용되지 않아서 이전 값으로 돌아감
        serializedObject.ApplyModifiedProperties();
    }
}
