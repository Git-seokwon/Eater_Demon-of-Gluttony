using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// Style�� ������ �ִ� ���� �ƴ϶� ��ġ�� ���� �����Ͽ� ���ڰ� ���̴� ������ �ϱ� 
public static class CustomEditorUtility
{
    #region GUIStyle
    // GUIStyle : �׸�(Draw) GUI�� � Design�� �������� �����ϴ� Class
    private readonly static GUIStyle titleStyle;
    #endregion

    #region Constructor
    static CustomEditorUtility()
    {
        // ����Ƽ ���ο� ���ǵǾ��ִ� ShurikenModuleTitle Style�� Base�� ��
        // ShurikenModuleTitle GUI Style : Particle System�� ����Ǿ� �ִ� GUI Style
        // (����� GUI�� ���� ��Ų�� ����ϴ°� �ƴϸ� ���� �����)
        titleStyle = new GUIStyle("ShurikenModuleTitle")
        {
            // ����Ƽ Default Label�� font�� ������
            font = new GUIStyle(EditorStyles.label).font,
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            // title�� �׸� ������ �¿� ���Ʒ� ������ ��
            border = new RectOffset(15, 7, 4, 4), // Left, Right, Top, Bottom
            // ���� 
            fixedHeight = 26f,
            // ���� Text�� ��ġ�� ���� ex) Label, Property
            contentOffset = new Vector2(20f, -2f) // �ؽ�Ʈ�� ���������� 20, ���� 2 ��ġ�� �Ű�����.
        };
    }
    #endregion

    #region Enum
    public static void DrawEnumToolbar(SerializedProperty enumProperty)
    {
        // ���� ���� ����
        EditorGUILayout.BeginHorizontal();

        // enumProperty�� �������� PrefixLabel�� �׸�
        EditorGUILayout.PrefixLabel(enumProperty.displayName);

        // �� Toolbar �׸��� 
        // 1) enumProperty.enumValueIndex : ���� enum ��
        // 2) enumProperty.enumDisplayNames : �ش� enum Type�� ���� ��� enum ���� �̸� 
        // 3) return : ������ enum ���� int�� ��ȯ
        enumProperty.enumValueIndex = GUILayout.Toolbar(enumProperty.enumValueIndex, enumProperty.enumDisplayNames);
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    // FoldoutTitle�� �׷��ִ� �޼��� (Information ���� �ִ� ���ݱ� ��ư ��/��)
    // ���ڰ� : Title ��, Foldout ��ģ ����, Title ���� ����
    // ��ȯ�� : ���ݱ� ���� ��ȯ
    #region Foldout
    public static bool DrawFoldoutTitle(string title, bool isExpanded, float space = 15f)
    {
        // space��ŭ �� ���� ��� - ���� GUI�� ���� ���� �α� 
        EditorGUILayout.Space(space);

        // titleStyle�� ������ ������ Inspector�󿡼� �ùٸ� ��ġ�� ������
        var rect = GUILayoutUtility.GetRect(16f, titleStyle.fixedHeight, titleStyle);
        // TitleStyle�� �����Ų Box�� �׷���
        GUI.Box(rect, title, titleStyle);

        // Foldout ��� ���� //

        // ���� Editor�� Event�� ������
        // Editor Event�� ���콺 �Է�, GUI ���� �׸���(Repaint), Ű���� �Է� �� Editor �󿡼� �Ͼ�� ��
        var currentEvent = Event.current;

        // Toggle Button�� ��ġ�� ũ�⸦ ����
        // ��ġ�� ��� �׸� �ڽ��� ��ǥ���� ��¦ ������ �Ʒ�, �� Button�� ��, ��� ������ �� ���°� ��
        var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);

        // Event�� Repaint(=GUI�� �׸��� Ȥ�� �ٽ� �׸���)�� �ܼ��� foldout button�� ������
        if (currentEvent.type == EventType.Repaint)
        {
            // Mouse�� Button ���� �ִ��� ����, Foldout ��ư Ȱ��ȭ ���δ� ū �ǹ̰� ���� ������ false�� ����
            // hasKetBoardFocus : keyboard ������ �� Button�� �� �ִ��� ���� -> �̰͵� �ǹ� ��� false
            EditorStyles.foldout.Draw(toggleRect, // �׷��� ��ġ
                                      false,      // Mouse�� Button ���� �ִ��� ����
                                      false,      // Foldout ��ư Ȱ��ȭ ����
                                      isExpanded, // foldout expand ����
                                      false);     // hasKetBoardFocus
        }
        // Event�� MouseDown�̰� mousePosition�� rect�� ����������(=Mouse Pointer�� ������ �׷��� Box�ȿ� ����) Click ����
        if (currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
        {
            isExpanded = !isExpanded;
            // // Use �Լ��� ������� ������ ���� Event�� ó������ ���� ������ �ǴܵǾ� ���� ��ġ�� �ִ� �ٸ� GUI�� ���� ���۵� �� ����.
            // // �ڡڡڡڡ� event ó���� ������ �׻� Use�� ���� event�� ���� ó���� ������ Unity�� �˷��ִ°� ���� �ڡڡڡڡ�
            currentEvent.Use();
        }

        // ���� �׸� Foldout Title�� ������ �׷��� GUI�� ��ġ�� �ʰ� Space �Լ��� 10 pixel ���� ����ֱ�
        EditorGUILayout.Space(10f);

        return isExpanded;
    }

    // ** DrawFoldoutTitle �����ε� **
    // FoldoutTitle�� �׸��� ���ÿ� ���ڷ� ���� Dictionary�� Expand ��Ȳ�� ����
    // �� Editor���� Foldout Title�� ���� �� �׸� ��, �׸� Title���� Expand ���θ� ��������� �ϴµ� �װ� ���� ���ֱ� ���� �����ε�
    // �� Dictionary�� key���� Title Text, value�� expand ����
    // ** IDictionary **
    // ��� Dictionary<string, bool>�� IDictionary<string, bool>�� ����� �� �ִ�. �� ������
    // �׷��� IDictionaryó�� �Ϲ������� �����ϸ� ���߿� Dictionary�� ������ �� ���ϴ�.
    // ex) 
    // IDictionary<string> dictionary = new Dictionary<string>();
    // ��                    �ս��� ����
    // IDictionary<string> dictionary = new RedBlackTree<string>(); �� RedBlackTree : ����� ���� Dictionary
    public static bool DrawFoldoutTitle(IDictionary<string, bool> isFoldoutExpandedByTitle, string title, float space = 15f)
    {
        // Dictionary�� Title ������ ���ٸ� Expand�� true�� �Ͽ� Title ������ ������ֱ�
        // (ó�� Title foldout�� ��ģ ���)
        if (!isFoldoutExpandedByTitle.ContainsKey(title))
            isFoldoutExpandedByTitle[title] = true;

        // IDictionary�� ���ؼ� Title���� Foldout ������ ��ϵǴ� ��
        isFoldoutExpandedByTitle[title] = DrawFoldoutTitle(title, isFoldoutExpandedByTitle[title], space);
        return isFoldoutExpandedByTitle[title];
    }
    #endregion

    #region UnderLine
    public static void DrawUnderLine(float height = 1f)
    {
        // ���������� �׸� GUI�� ��ġ�� ũ�� ������ ���� Rect ����ü�� �����´�. 
        var lastRect = GUILayoutUtility.GetLastRect();

        // rect�� ���� ���� GUI�� ���̸�ŭ ������. (��, y ���� ���� GUI �ٷ� �Ʒ��� ��ġ�ϰ� �ȴ�.)
        lastRect.y += lastRect.height;
        lastRect.height = height;

        // rect ���� �̿��ؼ� ������ ��ġ�� heightũ���� Box�� �׸�
        // �� height�� 1�̶�� Line�� �׷����Ե�
        EditorGUI.DrawRect(lastRect, Color.grey);
    }
    #endregion

    #region DeepCopySerializeReference
    public static void DeepCopySerializeReference(SerializedProperty property)
    {
        // ����ȭ�� ���� Ÿ���� ��ü ������ �ٷ� 
        // Ex) int�� Property�� intValue ������ ���� �����ϵ�
        //     SerializedProperty ��ü�� managedReferenceValue ������ ����
        // �� ������ ����ȭ�� ����ϱ� ���� �� ���� ���ȴ�. 
        if (property.managedReferenceValue == null)
            return;

        // ��� Module�� �⺻������ ICloneable�� ��ӹް� �ֱ� ������
        // ICloneable�� ĳ�����Ͽ� Clone �Լ��� �����ؼ� ����� ��ü�� managedReferenceValue�� Setting
        // �� ICloneable�� ����ؼ� �����ϰ� DeepCopy�� ����
        property.managedReferenceValue = (property.managedReferenceValue as ICloneable).Clone();
    }

    // �迭�� ��ȸ�ϸ鼭 �� Element���� Deep Copy�� �������ִ� �Լ�
    // �� fieldName
    // 1) fieldName�� Empty�� ���      : ���ڷ� ���� Property�� SerializeReference �迭 ����
    // �� �ٷ� �� Element���� Deep Copy�� ����
    // 2) fieldName�� Empty�� �ƴ� ��� : ���ڷ� ���� property�� �Ϲ� Struct �迭�̳� Class �迭
    // �� �� Element���� fieldName�� �̸����� ���� SerializeReference ������ ã�ƿ� ���� Deep Copy�� ����
    public static void DeepCopySerializeReferenceArray(SerializedProperty property, string fieldName = "")
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            var elementProperty = property.GetArrayElementAtIndex(i);

            // Element�� �Ϲ� class�� struct�� Element ���ο� SerializedReference ������ ���� �� �����Ƿ�,
            // fieldName�� Empty�� �ƴ϶�� Elenemt���� fieldName ���� ������ ã�ƿ�
            if (!string.IsNullOrEmpty(fieldName))
                elementProperty = elementProperty.FindPropertyRelative(fieldName);

            if (elementProperty.managedReferenceValue == null)
                continue;

            elementProperty.managedReferenceValue = (elementProperty.managedReferenceValue as ICloneable).Clone();
        }
    }
    #endregion
}
