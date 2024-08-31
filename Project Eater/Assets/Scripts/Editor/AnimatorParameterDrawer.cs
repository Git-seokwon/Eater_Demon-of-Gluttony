using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ※ AnimatorParameter 구조체 Property 그리기 
// Ex) | bool |  | isAttackFirst | 
//       enum         string   
[CustomPropertyDrawer(typeof(AnimatorParameter))]
public class AnimatorParameterDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var nameProperty = property.FindPropertyRelative("parameterName");
        var typeProperty = property.FindPropertyRelative("parameterType");

        position = EditorGUI.PrefixLabel(position, label);

        // GUI가 그려지는 위치를 조정하는 값
        // 여러 Editor가 겹쳐서 그려지다보면 들여쓰기(indent) 좌표가 이상해지는 경우가 있음.
        // AnimatorParameter가 그런 경우에 해당해서 Test를 통해 가장 보기 좋은 수치를 조정 값으로 사용함
        float adjust = EditorGUI.indentLevel * 15f;

        // 전체 넓이에서 왼쪽 15%는 EnumPopup을 그리고 오른쪽 85%는 Text Box를 그림
        // → 그려지는 위치가 x축으로 adjust만큼 당겨지기(들여쓰기) 때문에 당겨진 후의 우측 공백을 채우기 위해 
        //    GUI들의 넓이에 adjust 값을 더해서 더 넓게 그린다. 
        float leftWidth = (position.width * 0.15f) + adjust;
        float rightWidth = (position.width * 0.85f) + adjust;

        // ※ Enum Type DrawRect
        // GUI끼리 사이가 약간 벌어져 있어야하므로 2.5f를 빼서 공간을 만들어준다. 
        var typeRect = new Rect(position.x - adjust, position.y, leftWidth - 2.5f, position.height);
        // 유저가 선택한 Enum 혹은 기본값이 Enum Type으로 반환되어 Int형로 변환한다.
        int enumInt = System.Convert.ToInt32(EditorGUI.EnumPopup(typeRect, (AnimatorParameterType)typeProperty.enumValueIndex));
        // Enum 값 Setting
        typeProperty.enumValueIndex = enumInt;

        // ※ Enum Name DrawRect
        // typeRect.x + typeRect.width - adjust + 2.5f : typeRect의 바로 오른쪽 위치, typeRect와 거리를 벌리기 위해 2.5를 더해줌 
        var nameRect = new Rect(typeRect.x + typeRect.width - adjust + 2.5f, position.y, rightWidth, position.height);
        // label 값에 GUIContent.none을 넣어서 nameProperty의 Label은 그려지지 않도록 한다. → Text Box만 그려진다.
        nameProperty.stringValue = EditorGUI.TextField(nameRect, GUIContent.none, nameProperty.stringValue);

        EditorGUI.EndProperty();
    }
}
