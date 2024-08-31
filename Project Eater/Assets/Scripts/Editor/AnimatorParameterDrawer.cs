using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// �� AnimatorParameter ����ü Property �׸��� 
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

        // GUI�� �׷����� ��ġ�� �����ϴ� ��
        // ���� Editor�� ���ļ� �׷����ٺ��� �鿩����(indent) ��ǥ�� �̻������� ��찡 ����.
        // AnimatorParameter�� �׷� ��쿡 �ش��ؼ� Test�� ���� ���� ���� ���� ��ġ�� ���� ������ �����
        float adjust = EditorGUI.indentLevel * 15f;

        // ��ü ���̿��� ���� 15%�� EnumPopup�� �׸��� ������ 85%�� Text Box�� �׸�
        // �� �׷����� ��ġ�� x������ adjust��ŭ �������(�鿩����) ������ ����� ���� ���� ������ ä��� ���� 
        //    GUI���� ���̿� adjust ���� ���ؼ� �� �а� �׸���. 
        float leftWidth = (position.width * 0.15f) + adjust;
        float rightWidth = (position.width * 0.85f) + adjust;

        // �� Enum Type DrawRect
        // GUI���� ���̰� �ణ ������ �־���ϹǷ� 2.5f�� ���� ������ ������ش�. 
        var typeRect = new Rect(position.x - adjust, position.y, leftWidth - 2.5f, position.height);
        // ������ ������ Enum Ȥ�� �⺻���� Enum Type���� ��ȯ�Ǿ� Int���� ��ȯ�Ѵ�.
        int enumInt = System.Convert.ToInt32(EditorGUI.EnumPopup(typeRect, (AnimatorParameterType)typeProperty.enumValueIndex));
        // Enum �� Setting
        typeProperty.enumValueIndex = enumInt;

        // �� Enum Name DrawRect
        // typeRect.x + typeRect.width - adjust + 2.5f : typeRect�� �ٷ� ������ ��ġ, typeRect�� �Ÿ��� ������ ���� 2.5�� ������ 
        var nameRect = new Rect(typeRect.x + typeRect.width - adjust + 2.5f, position.y, rightWidth, position.height);
        // label ���� GUIContent.none�� �־ nameProperty�� Label�� �׷����� �ʵ��� �Ѵ�. �� Text Box�� �׷�����.
        nameProperty.stringValue = EditorGUI.TextField(nameRect, GUIContent.none, nameProperty.stringValue);

        EditorGUI.EndProperty();
    }
}
