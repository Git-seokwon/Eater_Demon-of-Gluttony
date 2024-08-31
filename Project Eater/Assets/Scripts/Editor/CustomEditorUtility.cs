using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// Style은 공식이 있는 것이 아니라 수치를 직접 조절하여 예쁘게 보이는 것으로 하기 
public static class CustomEditorUtility
{
    #region GUIStyle
    // GUIStyle : 그릴(Draw) GUI가 어떤 Design을 가졌는지 정의하는 Class
    private readonly static GUIStyle titleStyle;
    #endregion

    #region Constructor
    static CustomEditorUtility()
    {
        // 유니티 내부에 정의되어있는 ShurikenModuleTitle Style을 Base로 함
        // ShurikenModuleTitle GUI Style : Particle System에 적용되어 있는 GUI Style
        // (깔끔한 GUI라 따로 스킨을 사용하는게 아니면 많이 사용함)
        titleStyle = new GUIStyle("ShurikenModuleTitle")
        {
            // 유니티 Default Label의 font를 가져옴
            font = new GUIStyle(EditorStyles.label).font,
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            // title을 그릴 공간에 좌우 위아래 여백을 줌
            border = new RectOffset(15, 7, 4, 4), // Left, Right, Top, Bottom
            // 높이 
            fixedHeight = 26f,
            // 내부 Text의 위치를 조절 ex) Label, Property
            contentOffset = new Vector2(20f, -2f) // 텍스트가 오른쪽으로 20, 위로 2 위치가 옮겨진다.
        };
    }
    #endregion

    #region Enum
    public static void DrawEnumToolbar(SerializedProperty enumProperty)
    {
        // 가로 정렬 시작
        EditorGUILayout.BeginHorizontal();

        // enumProperty의 변수명을 PrefixLabel로 그림
        EditorGUILayout.PrefixLabel(enumProperty.displayName);

        // ※ Toolbar 그리기 
        // 1) enumProperty.enumValueIndex : 현재 enum 값
        // 2) enumProperty.enumDisplayNames : 해당 enum Type이 가진 모든 enum 값의 이름 
        // 3) return : 선택한 enum 값이 int로 반환
        enumProperty.enumValueIndex = GUILayout.Toolbar(enumProperty.enumValueIndex, enumProperty.enumDisplayNames);
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    // FoldoutTitle를 그려주는 메서드 (Information 옆에 있는 여닫기 버튼 ▼/▶)
    // 인자값 : Title 명, Foldout 펼친 여부, Title 위쪽 여백
    // 반환값 : 여닫기 여부 반환
    #region Foldout
    public static bool DrawFoldoutTitle(string title, bool isExpanded, float space = 15f)
    {
        // space만큼 윗 줄을 띄움 - 위쪽 GUI와 여유 공간 두기 
        EditorGUILayout.Space(space);

        // titleStyle의 정보를 가지고 Inspector상에서 올바른 위치를 가져옴
        var rect = GUILayoutUtility.GetRect(16f, titleStyle.fixedHeight, titleStyle);
        // TitleStyle을 적용시킨 Box를 그려줌
        GUI.Box(rect, title, titleStyle);

        // Foldout 기능 구현 //

        // 현재 Editor의 Event를 가져옴
        // Editor Event는 마우스 입력, GUI 새로 그리기(Repaint), 키보드 입력 등 Editor 상에서 일어나는 일
        var currentEvent = Event.current;

        // Toggle Button의 위치와 크기를 정함
        // 위치는 방금 그린 박스의 좌표에서 살짝 오른쪽 아래, 즉 Button이 좌, 가운데 정렬이 된 형태가 됨
        var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);

        // Event가 Repaint(=GUI를 그린다 혹은 다시 그린다)면 단순히 foldout button을 보여줌
        if (currentEvent.type == EventType.Repaint)
        {
            // Mouse가 Button 위에 있는지 여부, Foldout 버튼 활성화 여부는 큰 의미가 없기 때문에 false로 설정
            // hasKetBoardFocus : keyboard 조작이 이 Button에 가 있는지 여부 -> 이것도 의미 없어서 false
            EditorStyles.foldout.Draw(toggleRect, // 그려줄 위치
                                      false,      // Mouse가 Button 위에 있는지 여부
                                      false,      // Foldout 버튼 활성화 여부
                                      isExpanded, // foldout expand 여부
                                      false);     // hasKetBoardFocus
        }
        // Event가 MouseDown이고 mousePosition이 rect와 겹쳐있으면(=Mouse Pointer가 위에서 그려준 Box안에 있음) Click 판정
        if (currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
        {
            isExpanded = !isExpanded;
            // // Use 함수를 사용하지 않으면 아직 Event가 처리되지 않은 것으로 판단되어 같은 위치에 있는 다른 GUI도 같이 동작될 수 있음.
            // // ★★★★★ event 처리를 했으면 항상 Use를 통해 event에 대한 처리를 했음을 Unity에 알려주는게 좋음 ★★★★★
            currentEvent.Use();
        }

        // 지금 그린 Foldout Title과 다음에 그려질 GUI가 겹치지 않게 Space 함수로 10 pixel 정도 띄워주기
        EditorGUILayout.Space(10f);

        return isExpanded;
    }

    // ** DrawFoldoutTitle 오버로딩 **
    // FoldoutTitle을 그림과 동시에 인자로 받은 Dictionary에 Expand 상황을 저장
    // → Editor에서 Foldout Title을 여러 개 그릴 때, 그린 Title마다 Expand 여부를 관리해줘야 하는데 그걸 쉽게 해주기 위한 오버로딩
    // → Dictionary의 key값은 Title Text, value는 expand 여부
    // ** IDictionary **
    // 모든 Dictionary<string, bool>을 IDictionary<string, bool>로 취급할 수 있다. → 다형성
    // 그래서 IDictionary처럼 일반적으로 선언하면 나중에 Dictionary를 변경할 때 편리하다.
    // ex) 
    // IDictionary<string> dictionary = new Dictionary<string>();
    // ↓                    손쉽게 변형
    // IDictionary<string> dictionary = new RedBlackTree<string>(); ※ RedBlackTree : 사용자 정의 Dictionary
    public static bool DrawFoldoutTitle(IDictionary<string, bool> isFoldoutExpandedByTitle, string title, float space = 15f)
    {
        // Dictionary에 Title 정보가 없다면 Expand를 true로 하여 Title 정보를 만들어주기
        // (처음 Title foldout을 펼친 경우)
        if (!isFoldoutExpandedByTitle.ContainsKey(title))
            isFoldoutExpandedByTitle[title] = true;

        // IDictionary를 통해서 Title마다 Foldout 정보가 기록되는 것
        isFoldoutExpandedByTitle[title] = DrawFoldoutTitle(title, isFoldoutExpandedByTitle[title], space);
        return isFoldoutExpandedByTitle[title];
    }
    #endregion

    #region UnderLine
    public static void DrawUnderLine(float height = 1f)
    {
        // 마지막으로 그린 GUI의 위치와 크기 정보를 가진 Rect 구조체를 가져온다. 
        var lastRect = GUILayoutUtility.GetLastRect();

        // rect의 값을 이전 GUI의 높이만큼 내린다. (즉, y 값은 이전 GUI 바로 아래에 위치하게 된다.)
        lastRect.y += lastRect.height;
        lastRect.height = height;

        // rect 값을 이용해서 지정된 위치에 height크기의 Box를 그림
        // → height가 1이라면 Line이 그려지게됨
        EditorGUI.DrawRect(lastRect, Color.grey);
    }
    #endregion

    #region DeepCopySerializeReference
    public static void DeepCopySerializeReference(SerializedProperty property)
    {
        // 직렬화된 참조 타입의 객체 정보를 다룸 
        // Ex) int형 Property는 intValue 변수로 값에 접근하듯
        //     SerializedProperty 자체는 managedReferenceValue 변수로 접근
        // → 다형성 직렬화를 사용하기 위해 더 많이 사용된다. 
        if (property.managedReferenceValue == null)
            return;

        // 모든 Module이 기본적으로 ICloneable를 상속받고 있기 때문에
        // ICloneable로 캐스팅하여 Clone 함수를 실행해서 복사된 객체를 managedReferenceValue에 Setting
        // → ICloneable를 사용해서 간단하게 DeepCopy를 구현
        property.managedReferenceValue = (property.managedReferenceValue as ICloneable).Clone();
    }

    // 배열을 순회하면서 각 Element마다 Deep Copy를 수행해주는 함수
    // ※ fieldName
    // 1) fieldName이 Empty인 경우      : 인자로 받은 Property가 SerializeReference 배열 변수
    // → 바로 각 Element마다 Deep Copy를 진행
    // 2) fieldName이 Empty가 아닌 경우 : 인자로 받은 property가 일반 Struct 배열이나 Class 배열
    // → 각 Element에서 fieldName을 이름으로 가진 SerializeReference 변수를 찾아온 다음 Deep Copy를 진행
    public static void DeepCopySerializeReferenceArray(SerializedProperty property, string fieldName = "")
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            var elementProperty = property.GetArrayElementAtIndex(i);

            // Element가 일반 class나 struct라서 Element 내부에 SerializedReference 변수가 있을 수 있으므로,
            // fieldName이 Empty가 아니라면 Elenemt에서 fieldName 변수 정보를 찾아옴
            if (!string.IsNullOrEmpty(fieldName))
                elementProperty = elementProperty.FindPropertyRelative(fieldName);

            if (elementProperty.managedReferenceValue == null)
                continue;

            elementProperty.managedReferenceValue = (elementProperty.managedReferenceValue as ICloneable).Clone();
        }
    }
    #endregion
}
