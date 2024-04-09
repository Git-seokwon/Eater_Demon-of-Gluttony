using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// �� StatOverrideDrawer : isUseOverride�� Check�Ǿ� �־�߸� overrideDefaultValue�� �����ֵ��� �ϱ� 
// �� StatOverride���� Override�� ����, ������ ������ �� �ִ�. 
//    ���⼭ Override�� �� �ҰŸ� overrideDefaultValue�� ������ �ʿ䰡 ���� ������ �����, 
//    Override�� �ҽÿ��� overrideDefaultValue�� �����ֵ��� �����Ѵ�. 
[CustomPropertyDrawer(typeof(StatOverride))] // DecoratorDrawer�� PropertyDrawer�� ���� ��, �ش� Attribute�� Draw�� Class�� �־���� �Ѵ�. 
public class StatOverrideDrawer : PropertyDrawer // ����� ���� �Ӽ��� �����Ϳ��� ǥ���ϴ� �� ���Ǵ� ���
                                                 // �� Ư�� �Ӽ��� ����� �����Ͽ� ǥ�� (Custom Editor)
{
    // position : GUI�� �׸� ��ġ 
    // property : �׸� Target Property �� ���⼭�� statoverride�� �ǹ�
    // label : �ν����Ϳ��� �������� property�� �̸� 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Property Drawing ����
        EditorGUI.BeginProperty(position, label, property);

        // �� FindPropertyRelative : SerializedProperty�� �ִ� ���� �Ӽ��� ã��
        // �� property�� ���� �Ӽ� �� �̸��� "stat"�� �Ӽ��� ã�Ƽ� statProperty�� �Ҵ�
        var statProperty = property.FindPropertyRelative("stat");

        var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // �� objectReferenceValue : SerializedProperty(�纻)�� �ƴ� statProperty�� �����ϴ� ��ü�� ���� ��(����)
        // �� ?. : null ���Ǻ� ������ �� ���� �ǿ����ڰ� null�� �ƴ� ���� ������ �ǿ����ڸ� ��
        //                            �� objectReferenceValue�� null�̸� null�� ��ȯ
        // �� name.Replace("STAT_", "") : ������ ��ü�� �̸����� "STAT_"�� �� ���ڿ��� ��ü
        //                              : "STAT_" �κ��� ���� �� Stat�� CodeName�� ���
        // �� ?? : null ���� ������ �� ���� �ǿ����ڰ� null�� �ƴϸ� ���� �ǿ����ڸ� ��ȯ�ϰ�, null�̸� ������ �ǿ����ڸ� ��ȯ
        string labelName = statProperty.objectReferenceValue?.name.Replace("STAT_", "") ?? label.text;

        // �� EditorGUI.Foldout : ���� �ƿ� UI�� ����
        // �� SerializedProperty.isExpanded : SerializedProperty�� Foldout ���θ� �����ϱ� ���� ������� �ִ� ���� 
        // �� ���� Foldout ������ �����ϱ� ���� ������ ���� �ʿ䰡 ����. (���� ����)
        property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, labelName);
        if (property.isExpanded) // Foldout�� Ȯ��Ǿ��ٸ� ������ �κ� �׷��ֱ� 
        {
            var boxRect = new Rect(position.x,
                                   position.y + EditorGUIUtility.singleLineHeight, // ������ Foldout�� �׷ȱ� ������ �׸�ŭ ������
                                   position.width,
                                   GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight);
                                   // �� ���� ���� 
                                   // �� GetPropertyHeight : GUI �� ����
                                   // �� ���� Foldout�� Box�� �ȵ��� ������ Foldout �з� ��ŭ ���ش�.

            // HelpBox �׷��ֱ� 
            // �� HelpBox : � Message�� ����ִ� GUI�� ���ڷ� Message�� MessageType�� �޴µ� ���⼭�� HelpBox�� ��Ÿ�ϸ� �̿��� �ű� ������ 
            //              Message�� MessageType ��� none���� �־��ش�. 
            EditorGUI.HelpBox(boxRect, "", MessageType.None);

            // �� StatOverride ���� �׸��� 
            // HelpBox �ȿ� ������ ��ǥ, ũ�� ���� 
            var propertyRect = new Rect(boxRect.x + 4f, boxRect.y + 2f, boxRect.width - 8f, EditorGUIUtility.singleLineHeight);

            // �� statProperty �׸���
            // �� FindPropertyRelative : property �Ӽ����� ���ڰ�(SerializedProperty) �������� 
            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("stat"));

            // �� �� ������ 
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            var isUseOverrideProperty = property.FindPropertyRelative("isUseOverride");
            EditorGUI.PropertyField(propertyRect, isUseOverrideProperty);

            // �� isUseOverrideProperty.boolValue : isUseOverrideProperty�� bool ���� �������� 
            if (isUseOverrideProperty.boolValue)
            {
                propertyRect.y += EditorGUIUtility.singleLineHeight;
                // overrideDefaultValue ���� �׸��� 
                EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("overrideDefaultValue"));
            }
        }

        // Property Drawing ��
        EditorGUI.EndProperty();
    }

    // �� GetPropertyHeight : GUI�� �� ���̸� ��Ÿ���� �Լ� 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Property�� Ȯ��Ǿ� ���� ������ Foldout�� �׷����״� ���̴� �� ��(EditorGUIUtility.singleLineHeight)
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;
        else
        {
            // Foldout�� Ȯ��� ���
            bool isUseOverride = property.FindPropertyRelative("isUseOverride").boolValue;

            // isUseOverride ���ο� ���� overrideDefaultValue�� ������ ��(4) �ƴ���(3) ����
            int propertyLine = isUseOverride ? 4 : 3;

            return (EditorGUIUtility.singleLineHeight * propertyLine) + propertyLine;
        }
    }
}
