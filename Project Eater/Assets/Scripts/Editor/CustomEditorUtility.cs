using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    // FoldoutTitle를 그려주는 메서드 (Information 옆에 있는 여닫기 버튼)
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
}
