using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal; // ReorderableList를 사용하기 위한 namespace

/***************************************************************
                     IdentifiedObjectEditor 
1. IdentifiedObject의 CustomEditor
2. IdentifiedObject를 상속받는 자식 Class들도 해당 CustomEditor를 적용 
※ 물론 IdentifiedObject의 CustomEditor를 그대로 적용받는 것이기 때문에 
   만약 Category가 따로 Serialize 변수를 가졌더라도 그려지지 않는다. 
   이때는 Category도 따로 Custom Editor를 작성해줘야 한다.
 ***************************************************************/
[CustomEditor(typeof(IdentifiedObject), true)] // ← 2번 사항 적용
public class IdentifiedObjectEditor : Editor // 커스텀 에디터므로 Editor를 상속받음
{
    // SerializedProperty
    // → 내가 보고 있는 객체의 public 혹은 [SerializeField] 어트리뷰트를 통해
    //    Serailize된 변수들을 조작하기 위한 class
    // → IdentifiedObject가 가진 Serialize 변수를 찾아와서 값을 조작할 수 있게
    //    해주는 Class
    // → 그래서 IdentifiedObject Class가 가진 Serialize 변수들과 1대1로 대응되게
    //    변수들을 만듬
    #region SerializedProperty
    private SerializedProperty categoriesProperty;
    private SerializedProperty iconProperty;
    private SerializedProperty idProperty;
    private SerializedProperty codeNameProperty;
    private SerializedProperty displayNameProperty;
    private SerializedProperty descriptionProperty;
    #endregion

    // ReorderableList
    // → Inspector 상에서 순서를 편집할 수 있는 List
    #region ReorderableList
    private ReorderableList categories;
    #endregion

    // text를 넓게 보여주는 Style(=Skin) 지정을 위한 변수 - description 변수에 사용
    #region GUIStyle
    private GUIStyle textAreaStyle;
    #endregion

    // Title의 Foldout Expand 상태를 저장하는 변수
    // 해당 Dictionary를 IDictionary로 취급할 수 있다. (그래서 인자값 전달 가능)
    #region Dictionary
    private readonly Dictionary<string, bool> isFoldoutExpandedByTitle = new();
    #endregion

    // GameObject나 ScriptableObject를 눌러서 Inspector 창에 GUI가 그려질 때 호출되는 함수
    // → 나중에 IdentifiedObjectEditor를 상속받아 CustomEditor를 구현하는 자식 Class들에서
    //    OnEnable 함수를 조작할 수 있도록 가상함수로 만듬
    #region OnEnable
    protected virtual void OnEnable()
    {
        // Inspector에서 description을 편집하다가 다른 Inspector View로 넘어가는 경우에,
        // 포커스가 풀리지 않고 이전에 편집하던 desription 내용이 그대로 보이는 문제를
        // 해결하기위해 포커스를 풀어줌
        // → 필수 코드는 아니고 CustomEditor를 만들 때, 위의 문제가 발생하면 작성
        GUIUtility.keyboardControl = 0;

        // https://nforbidden-fruit.tistory.com/30
        #region serializedObject
        // serializedObject는 현재 내가 유니티 Editor에서 Click해서 인스펙터창에서 보고 있는 에셋(IdentifiedObject)를 뜻함
        // → 객체에서 Serialize 변수들을 찾아옴 (SerializedProperty 초기화)
        categoriesProperty = serializedObject.FindProperty("categories");
        iconProperty = serializedObject.FindProperty("icon");
        idProperty = serializedObject.FindProperty("id");
        codeNameProperty = serializedObject.FindProperty("codeName");
        displayNameProperty = serializedObject.FindProperty("displayName");
        descriptionProperty = serializedObject.FindProperty("description");
        #endregion

        // categories 배열이 ReorderableList 형태로 그려진다.
        categories = new(serializedObject, categoriesProperty);

        // List를 어떻게 그려줄지 Event를 통해서 정함
        #region List 그리기 
        // List의 Prefix Label 그리기 
        #region List Prefix Label
        // drawHeaderCallback 콜백 함수 정의 
        // → EditorGUI.LabelField() 함수를 사용하여 List의 Prefix Label를 그림
        // ※ EditorGUI.LabelField() : Editor Window에 레이블을 그리는 함수
        categories.drawHeaderCallback = rect => EditorGUI.LabelField(rect, categoriesProperty.displayName);
        // rect: 매개변수, 레이블이 그려질 사각형의 위치와 크기를 지정 (현재는 default 값 적용)
        // categoriesProperty.displayName : rect 위치에 categoriesProperty.displayName를 그림
        #endregion

        // List의 Element를 어떻게 그릴지 정함
        #region List Element
        // isFocused : 현재 요소가 Click을 통해 Focus된 상태인지를 나타내는 bool형
        categories.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            // rect.y + 2f : 위쪽에 여유공간을 줌 (header와 붙여지는 것을 막기 위함)
            // EditorGUIUtility.singleLineHeight : 기본적으로 Unity에 설정되어 있는 Property의 높이
            // → 이 값을 바꾸게 되면 인스펙터창에서 보는 변수들의 간격도 바뀌게 된다.
            rect = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);

            // EditorGUI.PropertyField : 인자로 넣은 SerializedProperty 변수의 Type에 따라서 자동으로 올바른 GUI를 그려주는 자동화 함수
            // ex) SerializedProperty가 int형이면 정수만 받을 수 있는 GUI를 그림
            // → rect 위치에 categories의 요소를 그려줌
            // categoriesProperty.GetArrayElementAtIndex(index) : categories 배열에서 Index에 해당하는 요소를 SerializedProperty 형식으로 가져오기
            // GUIContent.none : Prefix Label을 그리지 않겠다는 의미
            // → 배열에 요소를 추가하면 뜨는 Element 0, Element 1 ... Label이 안뜨도록 한다.
            EditorGUI.PropertyField(rect, categoriesProperty.GetArrayElementAtIndex(index), GUIContent.none);
        };
        #endregion

        // EditorGUILayout와 EditorGUI의 차이점
        // 1. EditorGUILayout은 GUI를 그리는 순서에 따라 위치를 자동으로 조정해줌 - 위치가 자동으로 관리됨
        // 2. EditorGUI는 사용자가 직접 GUI를 그릴 위치를 지정해줘야함
        // ★★★ 인자로 rect를 주냐 안주냐
        #endregion
    }
    #endregion

    private void StyleSetup()
    {
        if (textAreaStyle == null)
        {
            // Style의 기본 양식은 textArea
            textAreaStyle = new(EditorStyles.textArea);
            // 문자열이 TextBox 밖으로 못 빠져나가게 함
            textAreaStyle.wordWrap = true;
        }
    }

    // Foldout Title을 좀 더 간편하게 그릴 수 있게 Utility 함수를 Wrapping하는 함수
    protected bool DrawFoldoutTitle(string text)
        => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedByTitle, text);

    // OnInspectorGUI의 코드는 Unity가 인스펙터에 에디터를 표시할 때마다 실행
    // → Editor 함수로 GUI를 그리는 함수를 이 함수에서 호출해야 한다
    // → Editor를 꾸미는 작업을 진행
    #region OnInspectorGUI
    public override void OnInspectorGUI()
    {
        StyleSetup();

        // 객체의 Serialize 변수들의 값을 업데이트함
        // 고정적으로 Custom Editor를 작성할 때, 잊지 말고 항상 처음에 넣어 줘야 한다
        serializedObject.Update();

        // 우리가 위에서 설정한 ReorderableList를 그려줌 → DoLayoutList 함수를 통해 그려줌
        categories.DoLayoutList();

        // Infomation이라는 Foldout Title을 그려줌
        // → Expand가 True일 때만 하위 GUI를 그려준다
        // -> DrawFoldout 내부에서 Dictionary로 Foldout 여부를 관리하고 있기 때문에 여기서 따로, Foldout 여부를 저장할 필요가 없다.
        if (DrawFoldoutTitle("Information"))
        {
            // 기본적으로 Unity Editor는 위에서 아래로 GUI가 그려지는 세로 정렬

            // EditorGUILayout.BeginHorizontal
            // (1) 지금부터 그릴 객체를 가로로 정렬(좌측->우측)하며, 배경을 테두리 있는 회색으로 채움(=HelpBox는 유니티 내부에 정의되어 있는 Skin임)
            // ※ 중괄호는 작성할 필요는 없지만 명확한 구분을 위해 넣어준 것이기 때문에 스타일에 따라 중괄호는 안넣어도 됨
            EditorGUILayout.BeginHorizontal();
            {
                // IconProperty 그리기
                // → PropertyField로 자동으로 그리지 않고 직접 Type에 맞게 그리기 -> Icon의 Preview를 보여주는 형태로 GUI를 만들 것 
                // → Sprite를 Preview로 볼 수 있게 변수를 그려줌 - 네모난 Icon 변수가 그려짐
                #region ICON
                // objectReferenceValue : iconProperty가 가지고 있는 실제 Sprite 값 
                iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, // Prefix Label 변수명 -> 필요없음
                                                                                iconProperty.objectReferenceValue, // 현재 objectReferenceValue 값
                                                                                typeof(Sprite), // objectReferenceValue Type
                                                                                false, // Sprite는 GameObject나 Component가 아니라 하이어라키 창에 생성할 수 없다.
                                                                                GUILayout.Width(65)); // GUI 넓이
                                                                                                      // 넓이 설정하면 높이도 자동으로 맞춰짐
                #endregion

                // EditorGUILayout.BeginVertical
                // → 현재 아이콘 네모 박스는 그려진 상태
                // (2) 지금부터 그릴 객체는 세로로 정렬한다
                // → 위 icon 변수는 왼쪽에 그려지고, 지금부터 그릴 변수들은 오른쪽에 세로로 그려짐
                // ex)
                //          ID                세로
                //   icon   Code Name          ↓
                //          Display Name
                EditorGUILayout.BeginVertical();
                {
                    // (3) 지금부터 그릴 객체는 가로로 정렬한다. 
                    // → id 변수의 prefix(= inspector에서 보이는 변수의 이름)을 따로 지정해주기 위해 변수 Line을 직접 만듬
                    // ex)
                    // ID               |                   |
                    // Code Name        |                   |
                    // Display Name     |                   |
                    EditorGUILayout.BeginHorizontal();
                    {
                        #region ID
                        // 변수 편집 Disable, ID는 Database에서 직접 Set해줄 것이기 때문에 사용자가 직접 편집하지 못하도록 함.
                        GUI.enabled = false; // 이제부터 그려지는 GUI를 사용자가 조작하지 못하게 한다. 
                        // 변수의 선행 명칭(Prefix) 지정
                        EditorGUILayout.PrefixLabel("ID"); // ID 문자열 그리기 
                        // 위에서 정한 GUI.enabled = false; EditorGUILayout.PrefixLabel("ID");
                        // 사항들이 PropertyField을 통해 그려진다.
                        EditorGUILayout.PropertyField(idProperty, GUIContent.none); // 
                        // 변수 편집 Enable
                        GUI.enabled = true;
                        #endregion
                    }
                    // (3) 가로 정렬 종료 
                    // Begin과 End는 항상 쌍으로 나와야 한다.
                    EditorGUILayout.EndHorizontal();

                    // 가로 정렬이 종료된 채로 이제부터 (2) 세로로 정렬된다. 
                    //                 세로 ↓
                    // Code Name        |                   |
                    // Display Name     |                   |

                    // 지금부터 그릴 Property의 값이 수정되었는지 감시하는 함수 → codeName, displayName
                    EditorGUI.BeginChangeCheck();
                    #region CODENAME
                    var prevCodeName = codeNameProperty.stringValue; // codeNameProperty의 값을 저장 
                    // EditorGUILayout.DelayedTextField : codeNameProperty를 문자열 GUI 형태로 그리는데,
                    //                                  : 실제 Property 값의 변경은 사용자가 Enter 키를 누를 때까지 보류함
                    // → PropertyField 함수로 그리는 다른 변수들은 Edit Box에서 값을 바꾸기만 하면 바로 실제 변수 값이 바뀌지만
                    //    DelayedTextField는 Edit Box에서 값을 변경하고 Enter Key를 눌러야만 실제 변수 값이 변경된다. 
                    // → Enter 키를 누르지 않고 나오거나 ESC를 누르게 되면 Edit Box 내용이 원래 입력되어있던 내용으로 돌아간다.
                    EditorGUILayout.DelayedTextField(codeNameProperty);
                    #endregion


                    #region CODENAME CHANGE
                    // EditorGUI.BeginChangeCheck()와 대응, 변수가 수정되었는지 확인
                    // → codeName 변수가 수정되었다면 true
                    if (EditorGUI.EndChangeCheck())
                    {
                        // 현재 객체의 유니티 프로젝트상의 주소(Asset 폴더 주소)를 가져옴
                        // ※ target : Editor 변수, serializedObject의 targetObject와 완전히 똑같은 변수
                        //           : 인스펙터창에 뜨는 serializedObject
                        //           : 실제 IdentifiedObject를 가지고 있는 변수
                        // → serializeObject.targetObject == target
                        var assetPath = AssetDatabase.GetAssetPath(target);

                        // 새로운 이름은 '(변수의 Type)_(codeName)'
                        // → 해당 Custom Editor를 똑같이 공유하는 Category의 경우 CATEGORY_(codeName) 식으로 이름이 바뀐다
                        // ex)
                        // Skill -> Skill_(codeName)
                        // Effect -> EFFECT_(codeName)
                        var newName = $"{target.GetType().Name.ToUpper()}_{codeNameProperty.stringValue}";

                        // Serialize 변수들의 값 변화를 적용함(=디스크에 저장함)
                        // → 이 작업을 해주지 않으면 바뀐 값이 적용되지 않아서 이전 값으로 돌아감
                        // ※ 원래 이 작업은 함수 맨 마지막에 해주는 데 바로 아래줄에서 IdentifiedObject의 이름을 바꾸면 
                        //    Editor에서 IdentifiedObject를 다시 불러오는 Reload가 일어난다. Reload 과정에서 적용되지 못한 Data는 
                        //    날아가기 때문에 먼저 ApplyModifiedProperties을 한다. 
                        serializedObject.ApplyModifiedProperties();

                        // RenameAsset() : Asset File(IdentifiedObject)의 이름을 변경 
                        // → 만약 같은 이름을 가진 객체가 있을 경우 실패함.
                        // → 성공했을 경우 객체의 내부 이름도 바꿔줌.
                        // → 외부 이름과 내부 이름이 다를 시 유니티에서 경고를 띄우고,
                        // 실제 프로젝트에서도 문제를 일으킬 가능성이 높기에 항상 이름을 일치시켜줘야함
                        var message = AssetDatabase.RenameAsset(assetPath, newName);

                        // 아무 message가 없으면 성공한 것
                        // 성공 → string.IsNullOrEmpty(message) == true
                        if (string.IsNullOrEmpty(message))
                            target.name = newName; // Object의 내부 이름인 name 변수도 변경한다.
                        else // 오류 발생
                            codeNameProperty.stringValue = prevCodeName; // 이전 이름으로 되돌리기 
                    }
                    #endregion

                    // displayName 변수 그려줌
                    #region DISPLAYNAME
                    EditorGUILayout.PropertyField(displayNameProperty);
                    #endregion
                }
                // (2) 세로 정렬 종료 
                EditorGUILayout.EndVertical();
            }
            // (1) 가로 정렬 종료
            EditorGUILayout.EndHorizontal();

            #region Description
            // 세로 정렬 시작
            // → 기본적으로 세로 정렬이 Default 정렬이기 때문에 가로 정렬 내부에 사용하는게 아니라면
            //    직접 세로 정렬을 해줄 필요가 없지만 이 경우에는 HelpBox로 내부를 회색으로 채우기위해 직접 세로 정렬을 함
            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Description이라는 Lebel을 띄워줌
                EditorGUILayout.LabelField("Description");

                // TextField를 넓은 형태(TextArea)로 그려줌
                descriptionProperty.stringValue = EditorGUILayout.TextArea(descriptionProperty.stringValue, // 현재 Text
                                                                           textAreaStyle, // TextArea의 스타일
                                                                           GUILayout.Height(60)); // TextArea의 높이
            }
            // 세로 정렬 종료
            EditorGUILayout.EndVertical();
            #endregion
        }

        // Serialize 변수들의 값 변화를 적용함(=디스크에 저장함)
        // 이 작업을 해주지 않으면 바뀐 값이 적용되지 않아서 이전 값으로 돌아감
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
