using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ※ StatOverrideDrawer : isUseOverride가 Check되어 있어야만 overrideDefaultValue를 보여주도록 하기 
// → StatOverride에서 Override를 할지, 안할지 선택할 수 있다. 
//    여기서 Override를 안 할거면 overrideDefaultValue를 보여줄 필요가 없기 때문에 숨기고, 
//    Override를 할시에만 overrideDefaultValue를 보여주도록 설정한다. 
[CustomPropertyDrawer(typeof(StatOverride))] // DecoratorDrawer나 PropertyDrawer를 만들 때, 해당 Attribute에 Draw할 Class를 넣어줘야 한다. 
public class StatOverrideDrawer : PropertyDrawer // 사용자 지정 속성을 에디터에서 표시하는 데 사용되는 기능
                                                 // → 특정 속성을 사용자 지정하여 표시 (Custom Editor)
{
    // position : GUI를 그릴 위치 
    // property : 그릴 Target Property → 여기서는 statoverride를 의미
    // label : 인스펙터에서 보여지는 property의 이름 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Property Drawing 시작
        EditorGUI.BeginProperty(position, label, property);

        // ※ FindPropertyRelative : SerializedProperty에 있는 하위 속성을 찾음
        // → property의 하위 속성 중 이름이 "stat"인 속성을 찾아서 statProperty에 할당
        var statProperty = property.FindPropertyRelative("stat");

        var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // ※ objectReferenceValue : SerializedProperty(사본)가 아닌 statProperty가 참조하는 객체의 실제 값(원본)
        // ※ ?. : null 조건부 연산자 → 왼쪽 피연산자가 null이 아닐 때만 오른쪽 피연산자를 평가
        //                            → objectReferenceValue가 null이면 null을 반환
        // ※ name.Replace("STAT_", "") : 가져온 객체의 이름에서 "STAT_"를 빈 문자열로 대체
        //                              : "STAT_" 부분을 제거 → Stat의 CodeName만 출력
        // ※ ?? : null 병합 연산자 → 왼쪽 피연산자가 null이 아니면 왼쪽 피연산자를 반환하고, null이면 오른쪽 피연산자를 반환
        string labelName = statProperty.objectReferenceValue?.name.Replace("STAT_", "") ?? label.text;

        // ※ EditorGUI.Foldout : 폴더 아웃 UI를 생성
        // ※ SerializedProperty.isExpanded : SerializedProperty에 Foldout 여부를 저장하기 위해 만들어져 있는 변수 
        // → 따로 Foldout 유무를 저장하기 위해 변수를 만들 필요가 없다. (변수 한정)
        property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, labelName);
        if (property.isExpanded) // Foldout이 확장되었다면 나머지 부분 그려주기 
        {
            var boxRect = new Rect(position.x,
                                   position.y + EditorGUIUtility.singleLineHeight, // 위에서 Foldout을 그렸기 때문에 그만큼 내려옴
                                   position.width,
                                   GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight);
                                   // ※ 높이 설정 
                                   // ※ GetPropertyHeight : GUI 총 높이
                                   // → 위에 Foldout은 Box에 안들어가기 때문에 Foldout 분량 만큼 빼준다.

            // HelpBox 그려주기 
            // ※ HelpBox : 어떤 Message를 띄워주는 GUI라 인자로 Message와 MessageType을 받는데 여기서는 HelpBox의 스타일만 이용할 거기 때문에 
            //              Message와 MessageType 모두 none으로 넣어준다. 
            EditorGUI.HelpBox(boxRect, "", MessageType.None);

            // ★ StatOverride 변수 그리기 
            // HelpBox 안에 들어가도록 좌표, 크기 설정 
            var propertyRect = new Rect(boxRect.x + 4f, boxRect.y + 2f, boxRect.width - 8f, EditorGUIUtility.singleLineHeight);

            // ★ statProperty 그리기
            // ※ FindPropertyRelative : property 속성에서 인자값(SerializedProperty) 가져오기 
            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("stat"));

            // 한 줄 내리기 
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            var isUseOverrideProperty = property.FindPropertyRelative("isUseOverride");
            EditorGUI.PropertyField(propertyRect, isUseOverrideProperty);

            // ※ isUseOverrideProperty.boolValue : isUseOverrideProperty의 bool 값을 가져오기 
            if (isUseOverrideProperty.boolValue)
            {
                propertyRect.y += EditorGUIUtility.singleLineHeight;
                // overrideDefaultValue 변수 그리기 
                EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("overrideDefaultValue"));
            }
        }

        // Property Drawing 끝
        EditorGUI.EndProperty();
    }

    // ※ GetPropertyHeight : GUI의 총 높이를 나타내는 함수 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Property가 확장되어 있지 않으면 Foldout만 그려질테니 높이는 한 줄(EditorGUIUtility.singleLineHeight)
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;
        else
        {
            // Foldout이 확장된 경우
            bool isUseOverride = property.FindPropertyRelative("isUseOverride").boolValue;

            // isUseOverride 여부에 따라 overrideDefaultValue를 포함할 지(4) 아닐지(3) 정함
            int propertyLine = isUseOverride ? 4 : 3;

            return (EditorGUIUtility.singleLineHeight * propertyLine) + propertyLine;
        }
    }
}
