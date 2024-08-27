using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// StatScaleFloat(구조체)이 한 줄로 그려지게 만든다. 
[CustomPropertyDrawer(typeof(StatScaleFloat))]
public class StatScaleFloatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var defaultValueProperty = property.FindPropertyRelative("defaultValue");
        var scaleStatProperty = property.FindPropertyRelative("scaleStat");

        // 변수명 그려주기 
        // ※ EditorGUI.PrefixLabel : 반환값 Rect → label을 그리고 난 뒤 GUI 좌표 
        position = EditorGUI.PrefixLabel(position, label);

        // ※ EditorGUI.indentLevel : 들여쓰기 단계를 설정하는 변수
        // → 들여쓰기 단계에 따라서 property의 x좌표 틀어짐 문제가 있어서 이를 조정해줌
        // → 이는 꼭 해줘야하는게 아니라 CustomEditor를 작성하다보면 여러 GUI들이 얽혀서 좌표 문제를 일으키는 경우가 있음
        //    지금이 그 상황이라 적절한 수치를 찾아서 조정해주는 것
        float adjust = EditorGUI.indentLevel * 15f;

        // 변수들이 들어갈 넓이의 절반 크기 구하기 
        // → 조정값 더하기 
        float halfWidth = (position.width * 0.5f) + adjust;

        // ★ 변수 그리기 
        // ※ halfWidth - 2.5f : 아래에 그릴 scaleStatRect와 겹치지 않도록 2.5f를 빼준다. 
        var defaultValueRect = new Rect(position.x - adjust, position.y, halfWidth - 2.5f, position.height);

        // Float Field 그리기
        // ※ GUIContent.none : 변수명을 쓰지 않겠다는 의미 
        defaultValueProperty.floatValue = EditorGUI.FloatField(defaultValueRect, GUIContent.none, defaultValueProperty.floatValue);

        // ※ defaultValueRect.x + defaultValueRect.width - adjust + 2.5f : defaultValue 옆에 그리기 (간격 추가 : 2.5f)
        var scaleStatRect = new Rect(defaultValueRect.x + defaultValueRect.width - adjust + 2.5f, position.y, halfWidth, position.height);

        // Object Field 그리기 
        // ※ GUIContent.none : 변수명을 쓰지 않겠다는 의미 
        scaleStatProperty.objectReferenceValue = EditorGUI.ObjectField(scaleStatRect, GUIContent.none, scaleStatProperty.objectReferenceValue,
                                                                       typeof(Stat), 
                                                                       false);

        EditorGUI.EndProperty();

        // 현재 그린 GUI의 높이가 Unity Default Height에서 벗어나지 않고 GetHeight 함수는 Override를 하지 않으면 
        // Default Height를 반환하기 때문에 굳이 Override 하지 않음 
    }
}
