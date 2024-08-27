using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnderlineTitleAttribute))] // DecoratorDrawer�� PropertyDrawer�� ���� ��, �ش� Attribute�� Draw�� Class�� �־���� �Ѵ�. 
public class UnderlineTitleDrawer : DecoratorDrawer // Attribute�� �׷��ֱ� ���� ���
{
    // position : GUI�� �׷��� ��ġ 
    public override void OnGUI(Rect position)
    {
        // �� ���� �׷��ֱ� ������ base�� �� �޾ƿ´�. ��

        // �� attribute : DecoratorDrawer�� �ִ� ������, �츮�� IdentifiedObject�� �ۼ��� �� 
        //              : ���� IdentifiedObject�� ����ִ� target ����ó�� ���� Attribute ��ü�� ����ִ�. 
        // �� attribute�� UnderlineTitleAttribute�� ĳ����
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;

        // �鿩���Ⱑ ����� Position 
        // �� ����Ƽ�� GUI���� �鿩���� ������ �����ϰ�, �⺻������ Position�� �鿩���� ��ġ�� ������ �ȵǾ� �ֱ� ������ 
        //    �츮�� ���� �鿩���� ��ġ�� ������Ѽ� GUI�� �ùٸ� ��ġ�� �׷����� ���ش�. 
        position = EditorGUI.IndentedRect(position);

        position.height = EditorGUIUtility.singleLineHeight;

        // Y���� attribute�� ������ Space ����ŭ �����ش�. 
        // �� Rect X Y ���� ����Ʈ :  https://ansohxxn.github.io/unitydocs/rect/
        position.y += attributeAsUnderlineTitle.Space;

        // position�� title�� Bold Style�� �׸�
        GUI.Label(position, attributeAsUnderlineTitle.Title, EditorStyles.boldLabel);

        // �� �� �̵�
        position.y += EditorGUIUtility.singleLineHeight;
        // ����(�β�)�� 1
        position.height = 1f;
        // ȸ�� Box�� �׸�
        // �� ���̰� 1�̶� Box�� �ƴ� ȸ�� ���� �׷���
        EditorGUI.DrawRect(position, Color.gray);
    }

    // �츮�� ������ �׸� GUI�� �� ���̸� ��ȯ�ϴ� �Լ�
    // �� �� �Լ��� ��ȯ�ϴ� ���� �������� ���� GUI�� �� �׷����� �� ��, 
    //    ���� GUI�� ��𼭺��� �׷����� ���� �ȴ�. 
    // �� �߸��� ���� ��ȯ�ϸ� ���� �׸� GUI�� ���� GUI�� ���ļ� �׷����ų� 
    //    �ݴ�� �ƿ� Ȯ �������� �׷����� ������ ����
    // �� �׷��⿡ ��ġ ����� ���ؾ� �Ѵ�. (�̰Ŵ� ��� �� �ٲٸ鼭 Ȯ���ؾ� ��)
    public override float GetHeight()
    {
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;

        // �� standardVerticalSpacing : GUI�� ������ ���� ������ ���� 
        //                            : 1�� �ϸ� ���� GUI�� �� �پ �׷����� ������ * 2�� ���� ���̴�. 
        // ������ Attribute Space + �⺻ GUI ���� + (�⺻ GUI ���� * 2)
        return attributeAsUnderlineTitle.Space + EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2);
    }
}
