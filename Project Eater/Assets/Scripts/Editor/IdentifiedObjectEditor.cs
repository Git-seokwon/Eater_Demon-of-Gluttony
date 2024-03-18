using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal; // ReorderableList�� ����ϱ� ���� namespace

/***************************************************************
                     IdentifiedObjectEditor 
1. IdentifiedObject�� CustomEditor
2. IdentifiedObject�� ��ӹ޴� �ڽ� Class�鵵 �ش� CustomEditor�� ���� 
�� ���� IdentifiedObject�� CustomEditor�� �״�� ����޴� ���̱� ������ 
   ���� Category�� ���� Serialize ������ �������� �׷����� �ʴ´�. 
   �̶��� Category�� ���� Custom Editor�� �ۼ������ �Ѵ�.
 ***************************************************************/
[CustomEditor(typeof(IdentifiedObject), true)] // �� 2�� ���� ����
public class IdentifiedObjectEditor : Editor // Ŀ���� �����͹Ƿ� Editor�� ��ӹ���
{
    // SerializedProperty
    // �� ���� ���� �ִ� ��ü�� public Ȥ�� [SerializeField] ��Ʈ����Ʈ�� ����
    //    Serailize�� �������� �����ϱ� ���� class
    // �� IdentifiedObject�� ���� Serialize ������ ã�ƿͼ� ���� ������ �� �ְ�
    //    ���ִ� Class
    // �� �׷��� IdentifiedObject Class�� ���� Serialize ������� 1��1�� �����ǰ�
    //    �������� ����
    #region SerializedProperty
    private SerializedProperty categoriesProperty;
    private SerializedProperty iconProperty;
    private SerializedProperty idProperty;
    private SerializedProperty codeNameProperty;
    private SerializedProperty displayNameProperty;
    private SerializedProperty descriptionProperty;
    #endregion

    // ReorderableList
    // �� Inspector �󿡼� ������ ������ �� �ִ� List
    #region ReorderableList
    private ReorderableList categories;
    #endregion

    // text�� �а� �����ִ� Style(=Skin) ������ ���� ���� - description ������ ���
    #region GUIStyle
    private GUIStyle textAreaStyle;
    #endregion

    // Title�� Foldout Expand ���¸� �����ϴ� ����
    // �ش� Dictionary�� IDictionary�� ����� �� �ִ�. (�׷��� ���ڰ� ���� ����)
    #region Dictionary
    private readonly Dictionary<string, bool> isFoldoutExpandedByTitle = new();
    #endregion

    // GameObject�� ScriptableObject�� ������ Inspector â�� GUI�� �׷��� �� ȣ��Ǵ� �Լ�
    // �� ���߿� IdentifiedObjectEditor�� ��ӹ޾� CustomEditor�� �����ϴ� �ڽ� Class�鿡��
    //    OnEnable �Լ��� ������ �� �ֵ��� �����Լ��� ����
    #region OnEnable
    protected virtual void OnEnable()
    {
        // Inspector���� description�� �����ϴٰ� �ٸ� Inspector View�� �Ѿ�� ��쿡,
        // ��Ŀ���� Ǯ���� �ʰ� ������ �����ϴ� desription ������ �״�� ���̴� ������
        // �ذ��ϱ����� ��Ŀ���� Ǯ����
        // �� �ʼ� �ڵ�� �ƴϰ� CustomEditor�� ���� ��, ���� ������ �߻��ϸ� �ۼ�
        GUIUtility.keyboardControl = 0;

        // https://nforbidden-fruit.tistory.com/30
        #region serializedObject
        // serializedObject�� ���� ���� ����Ƽ Editor���� Click�ؼ� �ν�����â���� ���� �ִ� ����(IdentifiedObject)�� ����
        // �� ��ü���� Serialize �������� ã�ƿ� (SerializedProperty �ʱ�ȭ)
        categoriesProperty = serializedObject.FindProperty("categories");
        iconProperty = serializedObject.FindProperty("icon");
        idProperty = serializedObject.FindProperty("id");
        codeNameProperty = serializedObject.FindProperty("codeName");
        displayNameProperty = serializedObject.FindProperty("displayName");
        descriptionProperty = serializedObject.FindProperty("description");
        #endregion

        // categories �迭�� ReorderableList ���·� �׷�����.
        categories = new(serializedObject, categoriesProperty);

        // List�� ��� �׷����� Event�� ���ؼ� ����
        #region List �׸��� 
        // List�� Prefix Label �׸��� 
        #region List Prefix Label
        // drawHeaderCallback �ݹ� �Լ� ���� 
        // �� EditorGUI.LabelField() �Լ��� ����Ͽ� List�� Prefix Label�� �׸�
        // �� EditorGUI.LabelField() : Editor Window�� ���̺��� �׸��� �Լ�
        categories.drawHeaderCallback = rect => EditorGUI.LabelField(rect, categoriesProperty.displayName);
        // rect: �Ű�����, ���̺��� �׷��� �簢���� ��ġ�� ũ�⸦ ���� (����� default �� ����)
        // categoriesProperty.displayName : rect ��ġ�� categoriesProperty.displayName�� �׸�
        #endregion

        // List�� Element�� ��� �׸��� ����
        #region List Element
        // isFocused : ���� ��Ұ� Click�� ���� Focus�� ���������� ��Ÿ���� bool��
        categories.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            // rect.y + 2f : ���ʿ� ���������� �� (header�� �ٿ����� ���� ���� ����)
            // EditorGUIUtility.singleLineHeight : �⺻������ Unity�� �����Ǿ� �ִ� Property�� ����
            // �� �� ���� �ٲٰ� �Ǹ� �ν�����â���� ���� �������� ���ݵ� �ٲ�� �ȴ�.
            rect = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);

            // EditorGUI.PropertyField : ���ڷ� ���� SerializedProperty ������ Type�� ���� �ڵ����� �ùٸ� GUI�� �׷��ִ� �ڵ�ȭ �Լ�
            // ex) SerializedProperty�� int���̸� ������ ���� �� �ִ� GUI�� �׸�
            // �� rect ��ġ�� categories�� ��Ҹ� �׷���
            // categoriesProperty.GetArrayElementAtIndex(index) : categories �迭���� Index�� �ش��ϴ� ��Ҹ� SerializedProperty �������� ��������
            // GUIContent.none : Prefix Label�� �׸��� �ʰڴٴ� �ǹ�
            // �� �迭�� ��Ҹ� �߰��ϸ� �ߴ� Element 0, Element 1 ... Label�� �ȶߵ��� �Ѵ�.
            EditorGUI.PropertyField(rect, categoriesProperty.GetArrayElementAtIndex(index), GUIContent.none);
        };
        #endregion

        // EditorGUILayout�� EditorGUI�� ������
        // 1. EditorGUILayout�� GUI�� �׸��� ������ ���� ��ġ�� �ڵ����� �������� - ��ġ�� �ڵ����� ������
        // 2. EditorGUI�� ����ڰ� ���� GUI�� �׸� ��ġ�� �����������
        // �ڡڡ� ���ڷ� rect�� �ֳ� ���ֳ�
        #endregion
    }
    #endregion

    private void StyleSetup()
    {
        if (textAreaStyle == null)
        {
            // Style�� �⺻ ����� textArea
            textAreaStyle = new(EditorStyles.textArea);
            // ���ڿ��� TextBox ������ �� ���������� ��
            textAreaStyle.wordWrap = true;
        }
    }

    // Foldout Title�� �� �� �����ϰ� �׸� �� �ְ� Utility �Լ��� Wrapping�ϴ� �Լ�
    protected bool DrawFoldoutTitle(string text)
        => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedByTitle, text);

    // OnInspectorGUI�� �ڵ�� Unity�� �ν����Ϳ� �����͸� ǥ���� ������ ����
    // �� Editor �Լ��� GUI�� �׸��� �Լ��� �� �Լ����� ȣ���ؾ� �Ѵ�
    // �� Editor�� �ٹ̴� �۾��� ����
    #region OnInspectorGUI
    public override void OnInspectorGUI()
    {
        StyleSetup();

        // ��ü�� Serialize �������� ���� ������Ʈ��
        // ���������� Custom Editor�� �ۼ��� ��, ���� ���� �׻� ó���� �־� ��� �Ѵ�
        serializedObject.Update();

        // �츮�� ������ ������ ReorderableList�� �׷��� �� DoLayoutList �Լ��� ���� �׷���
        categories.DoLayoutList();

        // Infomation�̶�� Foldout Title�� �׷���
        // �� Expand�� True�� ���� ���� GUI�� �׷��ش�
        // -> DrawFoldout ���ο��� Dictionary�� Foldout ���θ� �����ϰ� �ֱ� ������ ���⼭ ����, Foldout ���θ� ������ �ʿ䰡 ����.
        if (DrawFoldoutTitle("Information"))
        {
            // �⺻������ Unity Editor�� ������ �Ʒ��� GUI�� �׷����� ���� ����

            // EditorGUILayout.BeginHorizontal
            // (1) ���ݺ��� �׸� ��ü�� ���η� ����(����->����)�ϸ�, ����� �׵θ� �ִ� ȸ������ ä��(=HelpBox�� ����Ƽ ���ο� ���ǵǾ� �ִ� Skin��)
            // �� �߰�ȣ�� �ۼ��� �ʿ�� ������ ��Ȯ�� ������ ���� �־��� ���̱� ������ ��Ÿ�Ͽ� ���� �߰�ȣ�� �ȳ־ ��
            EditorGUILayout.BeginHorizontal();
            {
                // IconProperty �׸���
                // �� PropertyField�� �ڵ����� �׸��� �ʰ� ���� Type�� �°� �׸��� -> Icon�� Preview�� �����ִ� ���·� GUI�� ���� �� 
                // �� Sprite�� Preview�� �� �� �ְ� ������ �׷��� - �׸� Icon ������ �׷���
                #region ICON
                // objectReferenceValue : iconProperty�� ������ �ִ� ���� Sprite �� 
                iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, // Prefix Label ������ -> �ʿ����
                                                                                iconProperty.objectReferenceValue, // ���� objectReferenceValue ��
                                                                                typeof(Sprite), // objectReferenceValue Type
                                                                                false, // Sprite�� GameObject�� Component�� �ƴ϶� ���̾��Ű â�� ������ �� ����.
                                                                                GUILayout.Width(65)); // GUI ����
                                                                                                      // ���� �����ϸ� ���̵� �ڵ����� ������
                #endregion

                // EditorGUILayout.BeginVertical
                // �� ���� ������ �׸� �ڽ��� �׷��� ����
                // (2) ���ݺ��� �׸� ��ü�� ���η� �����Ѵ�
                // �� �� icon ������ ���ʿ� �׷�����, ���ݺ��� �׸� �������� �����ʿ� ���η� �׷���
                // ex)
                //          ID                ����
                //   icon   Code Name          ��
                //          Display Name
                EditorGUILayout.BeginVertical();
                {
                    // (3) ���ݺ��� �׸� ��ü�� ���η� �����Ѵ�. 
                    // �� id ������ prefix(= inspector���� ���̴� ������ �̸�)�� ���� �������ֱ� ���� ���� Line�� ���� ����
                    // ex)
                    // ID               |                   |
                    // Code Name        |                   |
                    // Display Name     |                   |
                    EditorGUILayout.BeginHorizontal();
                    {
                        #region ID
                        // ���� ���� Disable, ID�� Database���� ���� Set���� ���̱� ������ ����ڰ� ���� �������� ���ϵ��� ��.
                        GUI.enabled = false; // �������� �׷����� GUI�� ����ڰ� �������� ���ϰ� �Ѵ�. 
                        // ������ ���� ��Ī(Prefix) ����
                        EditorGUILayout.PrefixLabel("ID"); // ID ���ڿ� �׸��� 
                        // ������ ���� GUI.enabled = false; EditorGUILayout.PrefixLabel("ID");
                        // ���׵��� PropertyField�� ���� �׷�����.
                        EditorGUILayout.PropertyField(idProperty, GUIContent.none); // 
                        // ���� ���� Enable
                        GUI.enabled = true;
                        #endregion
                    }
                    // (3) ���� ���� ���� 
                    // Begin�� End�� �׻� ������ ���;� �Ѵ�.
                    EditorGUILayout.EndHorizontal();

                    // ���� ������ ����� ä�� �������� (2) ���η� ���ĵȴ�. 
                    //                 ���� ��
                    // Code Name        |                   |
                    // Display Name     |                   |

                    // ���ݺ��� �׸� Property�� ���� �����Ǿ����� �����ϴ� �Լ� �� codeName, displayName
                    EditorGUI.BeginChangeCheck();
                    #region CODENAME
                    var prevCodeName = codeNameProperty.stringValue; // codeNameProperty�� ���� ���� 
                    // EditorGUILayout.DelayedTextField : codeNameProperty�� ���ڿ� GUI ���·� �׸��µ�,
                    //                                  : ���� Property ���� ������ ����ڰ� Enter Ű�� ���� ������ ������
                    // �� PropertyField �Լ��� �׸��� �ٸ� �������� Edit Box���� ���� �ٲٱ⸸ �ϸ� �ٷ� ���� ���� ���� �ٲ�����
                    //    DelayedTextField�� Edit Box���� ���� �����ϰ� Enter Key�� �����߸� ���� ���� ���� ����ȴ�. 
                    // �� Enter Ű�� ������ �ʰ� �����ų� ESC�� ������ �Ǹ� Edit Box ������ ���� �ԷµǾ��ִ� �������� ���ư���.
                    EditorGUILayout.DelayedTextField(codeNameProperty);
                    #endregion


                    #region CODENAME CHANGE
                    // EditorGUI.BeginChangeCheck()�� ����, ������ �����Ǿ����� Ȯ��
                    // �� codeName ������ �����Ǿ��ٸ� true
                    if (EditorGUI.EndChangeCheck())
                    {
                        // ���� ��ü�� ����Ƽ ������Ʈ���� �ּ�(Asset ���� �ּ�)�� ������
                        // �� target : Editor ����, serializedObject�� targetObject�� ������ �Ȱ��� ����
                        //           : �ν�����â�� �ߴ� serializedObject
                        //           : ���� IdentifiedObject�� ������ �ִ� ����
                        // �� serializeObject.targetObject == target
                        var assetPath = AssetDatabase.GetAssetPath(target);

                        // ���ο� �̸��� '(������ Type)_(codeName)'
                        // �� �ش� Custom Editor�� �Ȱ��� �����ϴ� Category�� ��� CATEGORY_(codeName) ������ �̸��� �ٲ��
                        // ex)
                        // Skill -> Skill_(codeName)
                        // Effect -> EFFECT_(codeName)
                        var newName = $"{target.GetType().Name.ToUpper()}_{codeNameProperty.stringValue}";

                        // Serialize �������� �� ��ȭ�� ������(=��ũ�� ������)
                        // �� �� �۾��� ������ ������ �ٲ� ���� ������� �ʾƼ� ���� ������ ���ư�
                        // �� ���� �� �۾��� �Լ� �� �������� ���ִ� �� �ٷ� �Ʒ��ٿ��� IdentifiedObject�� �̸��� �ٲٸ� 
                        //    Editor���� IdentifiedObject�� �ٽ� �ҷ����� Reload�� �Ͼ��. Reload �������� ������� ���� Data�� 
                        //    ���ư��� ������ ���� ApplyModifiedProperties�� �Ѵ�. 
                        serializedObject.ApplyModifiedProperties();

                        // RenameAsset() : Asset File(IdentifiedObject)�� �̸��� ���� 
                        // �� ���� ���� �̸��� ���� ��ü�� ���� ��� ������.
                        // �� �������� ��� ��ü�� ���� �̸��� �ٲ���.
                        // �� �ܺ� �̸��� ���� �̸��� �ٸ� �� ����Ƽ���� ��� ����,
                        // ���� ������Ʈ������ ������ ����ų ���ɼ��� ���⿡ �׻� �̸��� ��ġ���������
                        var message = AssetDatabase.RenameAsset(assetPath, newName);

                        // �ƹ� message�� ������ ������ ��
                        // ���� �� string.IsNullOrEmpty(message) == true
                        if (string.IsNullOrEmpty(message))
                            target.name = newName; // Object�� ���� �̸��� name ������ �����Ѵ�.
                        else // ���� �߻�
                            codeNameProperty.stringValue = prevCodeName; // ���� �̸����� �ǵ����� 
                    }
                    #endregion

                    // displayName ���� �׷���
                    #region DISPLAYNAME
                    EditorGUILayout.PropertyField(displayNameProperty);
                    #endregion
                }
                // (2) ���� ���� ���� 
                EditorGUILayout.EndVertical();
            }
            // (1) ���� ���� ����
            EditorGUILayout.EndHorizontal();

            #region Description
            // ���� ���� ����
            // �� �⺻������ ���� ������ Default �����̱� ������ ���� ���� ���ο� ����ϴ°� �ƴ϶��
            //    ���� ���� ������ ���� �ʿ䰡 ������ �� ��쿡�� HelpBox�� ���θ� ȸ������ ä������� ���� ���� ������ ��
            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Description�̶�� Lebel�� �����
                EditorGUILayout.LabelField("Description");

                // TextField�� ���� ����(TextArea)�� �׷���
                descriptionProperty.stringValue = EditorGUILayout.TextArea(descriptionProperty.stringValue, // ���� Text
                                                                           textAreaStyle, // TextArea�� ��Ÿ��
                                                                           GUILayout.Height(60)); // TextArea�� ����
            }
            // ���� ���� ����
            EditorGUILayout.EndVertical();
            #endregion
        }

        // Serialize �������� �� ��ȭ�� ������(=��ũ�� ������)
        // �� �۾��� ������ ������ �ٲ� ���� ������� �ʾƼ� ���� ������ ���ư�
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
