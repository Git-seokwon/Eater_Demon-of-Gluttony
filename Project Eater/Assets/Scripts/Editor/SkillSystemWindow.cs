using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

public class SkillSystemWindow : EditorWindow // 에디터 창을 상속받음 ex) Animator 창
{
    // Static 변수 : Window를 닫고 다시 열었을 때, Data들을 이전에 설정한 값 그대로 다시 쓰기 위함
    #region Static 변수
    // 현재 보고 있는 database의 index
    private static int toolbarIndex = 0;

    // Database List의 Scroll Position (SkillSystemWindow 좌측 Editor Scroll)
    // → Database별 현재 Scroll이 어느 위치에 있는지 좌표값을 저장할 변수
    // → 키 값은 IdentifiedObejct를 상속받는 Data의 Type이다. ex) Category, Stat 등
    private static Dictionary<Type, Vector2> scrollPositionByType = new();

    // 현재 보여주고 있는 data의 Scroll Posiiton
    // → 우측 Editor(IdentifiedObjectEditor)의 Scroll Position
    private static Vector2 drawingEditorScrollPosition;

    //                   스크롤 위치
    /* ------------------------------------------------------
     *                        |
     *  scrollPositionByType  |  drawingEditorScrollPosition
     *                        |
       ------------------------------------------------------ */

    // 현재 선택한 Data
    // → Database 별 사용자가 목록에서 선택한 Data가 뭔지 저장하는 변수 
    private static Dictionary<Type, IdentifiedObject> selectedObjectsByType = new();
    #endregion

    #region 일반 변수
    // Type별 Database(Category, Stat, Skill 등등...)
    private readonly Dictionary<Type, IODatabase> databasesByType = new();
    
    // Database Data들의 Type들
    private Type[] databaseTypes;
    
    // 위 Type들의 string 이름
    private string[] databaseTypeNames;
    
    // 현재 보여주고 있는 data의 Editor class
    // → 목록에서 선택한 Data의 Editor를 우측에 보여준다.
    // → Data의 Editor를 보여주려면 실제로 Data의 Editor 객체를 생성해야 한다. 
    //    이때 생성한 Data의 Editor 객체를 저장할 변수이다.
    private Editor cashedEditor;

    // Database List의 Selected Background Texture
    // → '이 Data는 사용자가 선택한 Data다'라는 걸 보여주기 위해 선택된 Data의 배경에 적용할 Texture
    private Texture2D selectedBoxTexture; // Texture
    // Database List의 Selected Style
    // → 선택된 Data의 GUI에 적용할 Style이다.
    private GUIStyle selectedBoxStyle; // Style

    private List<string> statOwnerType = new();
    string statPath;
    private int currentSelectedStatType = 0;
    private int prevSelectedStatType = -1;
    private bool isRefresh = false;
    #endregion

    #region OpenWindow
    // Editor Tools 탭에 Skill System 항목이 추가되고, Click시 Window가 열림
    [MenuItem("Tool/Skill System")]
    private static void OpenWindow()
    {
        // GetWindow : Skill System이란 명칭을 가진 Window를 생성
        // ※ 인자 값 : 생성될 Window의 Title
        var window = GetWindow<SkillSystemWindow>("Skill System");
        window.minSize = new Vector2(800, 700);
        window.Show();

        // OpenWindow 함수만 코드를 작성하게 되면, 아무 내용도 없는 빈창이 뜨게 된다.
    }
    #endregion

    #region SetUp
    // Texture와 Style 변수를 Setup 하는 함수
    private void SetUpStyle()
    {
        // 1x1 Pixel의 Texture를 만듬
        selectedBoxTexture = new Texture2D(1, 1);

        // SetPixel을 통해 청색으로 설정한다.
        // → 1x1 Pixel이므로 0,0 좌표 픽셀은 전체 픽셀이 되며, 전체 Pixel의 Color(=청색)를 설정해준 것이 된다.
        selectedBoxTexture.SetPixel(0, 0, new Color(0.31f, 0.4f, 0.5f));

        // 위에서 설정한 Color값을 적용
        selectedBoxTexture.Apply();

        // 이 Texture는 Window에서 관리할 것이기 때문에 Unity에서 자동 관리하지말라(DontSave) Flag를 설정해줌
        // → 해당 flag가 없다면 Editor에서 Play를 누른채로 SetupStyle 함수가 실행되면
        //    texture가 Play 상태에 종속되어 Play를 중지하면 texture가 자동 Destroy 되버린다.
        // → DontSave Flag를 설정해주면 이런 문제를 예방하고 Texture의 Destroy 시점을 우리가 정해줄 수 있다.
        selectedBoxTexture.hideFlags = HideFlags.DontSave;

        selectedBoxStyle = new GUIStyle();
        // Select된 Data의 Normal 상태 Backgorund Texture를 위에서 생성한 Texture로 설정
        // → Select된 Data의 Background는 청색으로 나와서 강조된다. 
        selectedBoxStyle.normal.background = selectedBoxTexture;
    }

    // Database를 Project에서 Load 해오거나 Database가 없을 시에 자동 생성해주는 함수 
    // ※ dataTypes : IdentifiedObject를 상속받는 Type들 ex) Category, Stat 등
    private void SetUpDatabase(Type[] dataTypes)
    {
        // 아직 Database를 Load하지 않은 것이므로 Load 작업을 진행
        if (databasesByType.Count == 0)
        {
            // ※ AssetDatabase.IsValidFolder : 인자로 넣은 Folder Path가 존재하는지 확인
            // → Resources Folder에 Database Folder가 있는지 확인
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/GameResources/Resources/Database"))
            {
                // ※ 추가사항
                // → 해당 코드에서는 미리 Resources 폴더를 만들어놨기 때문에 Resources 폴더가 있는지 확인을 안 함
                // → 편의성을 더 높이고 싶다면, 지금 작업과 똑같이 Resources 폴더가 있는지, 확인하고 없다면 만들어주면 된다.

                // Database Folder를 만들어 준다.
                AssetDatabase.CreateFolder("Assets/Prefabs/GameResources/Resources", "Database");
            }

            foreach (var type in dataTypes)
            {
                // ※ AssetDatabase.LoadAssetAtPath<IODatabase>() : Project에 만들어진 Database 불러오기 
                // → Database 폴더에 있는 type.Name의 Database(Database.asset)를 불러온다. 
                // ex) type이 Category라면 CategoryDatabase라는 이름을 가진 Database를 불러온다. 
                var database = AssetDatabase.LoadAssetAtPath<IODatabase>($"Assets/Prefabs/GameResources/Resources/Database/{type.Name}Database.asset");

                // Project에 해당 Database가 존재하지 않음 → Database 생성
                if (database == null)
                {
                    // ※ CreateInstance : ScriptableObject 객체 생성
                    database = CreateInstance<IODatabase>();

                    // 생성한 Database를 Database 폴더에 (type.Name)Database라는 이름으로 저장
                    // → .asset은 ScriptableObject의 파일 확장자이다. 예를 들어, Audio Clip Asset을 만들고 싶다면
                    //    뒤에 확장자로 .audioClip으로 해야 한다.
                    AssetDatabase.CreateAsset(database, $"Assets/Prefabs/GameResources/Resources/Database/{type.Name}Database.asset");

                    // 지정한 주소에 하위 Folder를 생성
                    // → 해당 Folder는 Window에 의해 생성된 IdentifiedObject가 저장될 장소임
                    AssetDatabase.CreateFolder("Assets/Prefabs/GameResources/Resources", type.Name);
                }

                // Database Load or 생성
                // → 불러온 or 생성된 Database를 Dictionary에 보관
                databasesByType[type] = database;
                // → ScrollPosition Data 생성
                scrollPositionByType[type] = Vector2.zero;
                // → SelectObject Data 생성
                selectedObjectsByType[type] = null;
            }

            // ※ Linq.Select : dataTypes에서 Name만 가져옴
            // → 인자로 받은 Type들의 이름을 Array 형태로 가져오기
            // → databaseTypeNames는 SkillSystem의 Toolbar Tab을 구성하는데 사용될 것
            databaseTypeNames = dataTypes.Select(x => x.Name).ToArray();
            // → 인자로 받은 Type들도 나중에 활용하기 위해 받아두기 
            databaseTypes = dataTypes;
        }
    }

    private void SetUpStatOwnerType(List<string> statOwnerType)
    {
        // Stat 폴더 set
        statPath = "Assets/Prefabs/GameResources/Resources/Stat";

        var directory = new System.IO.DirectoryInfo(statPath);

        foreach (var dir in directory.GetDirectories())
        {
            string fileDirectory = dir.Name;
            statOwnerType.Add(fileDirectory);
        }
    }
    #endregion

    #region DrawDatabase
    // Database가 가진 Data 목록을 GUI로 그려주는 함수 
    private void DrawDatabase(Type dataType)
    {
        // 인자로 받은 Type으로 그려줄 Database를 가져온다. 
        var database = databasesByType[dataType];

        // Editor에 Caching되는 Preview Texture의 수를 최소 32개, 최대 32 + database의 Count까지 늘림
        // → 이 작업을 안해주면 그려야하는 IO 객체의 Icon들이 많을 경우 제대로 그려지지 않는 문제가 발생함
        // → 이 작업을 통해 Database에 Data가 아무리 많아도 모든 Texture를 Caching할 수 있게 된다.
        // → 어차피 Texture가 많이 Caching 된다고 해도 개발자 컴퓨터의 Ram을 아주 약간 더 사용한다는 점과 
        //    Texture 검색에 마찬가지로 아주 약간의 시간이 더 소요된다는 점 말고는 아무 문제가 없다. 
        AssetPreview.SetPreviewTextureCacheSize(Mathf.Max(32, 32 + database.Count));

        // Database의 Data 목록을 그려주기 시작 
        // (1) 가로 정렬 시작
        EditorGUILayout.BeginHorizontal();
        {
            // (2) 세로 정렬 시작, Style은 HelpBox, 넓이는 300f
            // → 300 넓이 크기의 세로 박스가 그려짐 
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300f));
            {
                if (dataType == typeof(Stat))
                {
                    prevSelectedStatType = currentSelectedStatType;
                    currentSelectedStatType = EditorGUILayout.Popup(currentSelectedStatType, statOwnerType.ToArray());
                    EditorGUILayout.Space(2f);
                    if (GUILayout.Button("Update Stat Folder"))
                    {
                        isRefresh = true;
                    }
                    EditorGUILayout.Space(4f);
                }

                // 지금부터 그릴 GUI는 초록색
                GUI.color = Color.green;

                // 새로운 Data를 만드는 Button을 그려줌
                // → 사용자가 해당 버튼을 Click 했을 때, True
                if (GUILayout.Button($"New {dataType.Name}"))
                {
                    // System Namespace의 Guid 구조체를 이용해서 고유 식별자를 생성 → Data의 임시 codeName으로 쓰기 위함
                    // → 고유 식별자라는건 절때로 겹칠 수 없는 어떤 값
                    // ※ 추가사항
                    // → 고유 식별자를 만드는 알고리즘은 여러가지가 있는데 여기서는 간단하게 Guid를 직접 만들지 않고
                    //    c#에서 제공하는 Guid 구조체를 이용 
                    var guid = Guid.NewGuid();

                    // dataType을 이용해서 scriptableObject를 생성 → IdentifiedObject로 형변환
                    var newData = CreateInstance(dataType) as IdentifiedObject;

                    // Reflection을 이용해 IdentifiedObject의 codeName Field를 찾아와서(1) newData의 codeName을 임시 codeName인 guid로 Set(2)
                    // ※ dataType.BaseType : dataType이 상속받고 있는 부모 Type을 가져오기 (부모 Type에 codeName 변수가 존재하기 때문)
                    //                      : 인자로 들어오는 dataType은 Category처럼 무조건 IdentifiedObject를 상속받고 있는 Class이기 때문에 
                    //                      : 이 경우에는 IdentifiedObject가 될 것이다. 
                    dataType.BaseType.GetField("codeName", BindingFlags.NonPublic | BindingFlags.Instance) // 1
                        .SetValue(newData, guid.ToString());                                               // 2

                    // 만든 newData를 Project에 생성 (newData는 현재 codeName이 guid로 설정 된 상황)
                    // → Resources 폴더에 Type 이름 폴더
                    // → 데이터의 명칭은 Type.Name의 대문자_guid (ScriptableObject)
                    // ex) 타입이 Category면 CATEGORY_Guid가 됨
                    if (dataType == typeof(Stat))
                    {
                        AssetDatabase.CreateAsset(newData,
                            $"Assets/Prefabs/GameResources/Resources/{dataType.Name}/{statOwnerType[currentSelectedStatType]}/{statOwnerType[currentSelectedStatType].ToUpper()}_{guid}.asset");
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(newData, $"Assets/Prefabs/GameResources/Resources/{dataType.Name}/{dataType.Name.ToUpper()}_{guid}.asset");
                    }
                    
                    // 생성한 Data를 Database에 추가하기 
                    // ※ 추가사항 "정의 피킹" 사용
                    // → 해당 함수 정의한 코드를 찾아서 보여줌
                    database.Add(newData);

                    // database에서 data를 추가했으니(= Serialize 변수인 datas 변수에 변화가 생김) → id변수 새로 설정함
                    // → SetDirty를 설정하여 Unity에 database의 Serialize 변수가 변했다고 알림
                    EditorUtility.SetDirty(database);

                    // Dirty flag 대상을 저장함
                    // → 모든 수정 사항들을 저장 
                    AssetDatabase.SaveAssets();

                    // selectedObject를 새로 생성한 Data로 설정하여 우측에 Editor가 그려지게 만든다. 
                    // → 현재 보고 있는 IdentifiedObject를 새로 만들어진 IdentifiedObject로 설정
                    // → 새로 만든 Data가 자동 선택되어 바로 우측 Editor에서 작업할 수 있게 편의성을 고려 
                    selectedObjectsByType[dataType] = newData;
                }

                // 지금부터 그릴 GUI는 빨간색
                GUI.color = Color.red;
                // 마지막 순번의 Data를 삭제하는 Button을 그려줌
                if (dataType != typeof(Stat) && GUILayout.Button($"Remove Last {dataType.Name}")) // ex) Remove Last Category
                {
                    // Database의 마지막 Data를 가져온다. 
                    // → Data의 ID는 Database에 추가된 순서이기 때문에 마지막 순번이 가장 최근의 Data이다. 
                    var lastData = database.Count > 0 ? database.Datas.Last() : null;
                    if (lastData)
                    {
                        // 가져온 마지막 Data 삭제
                        database.Remove(lastData);

                        // ※ AssetDatabase.DeleteAsset : Project에 저장된 Data 삭제
                        // ※ AssetDatabase.GetAssetPath : Data가 Project에 어디에 있는지 위치를 찾아오기 
                        // Data의 Asset 폴더 내 위치를 찾아와서 삭제
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(lastData));

                        // database에서 data를 제거했으니 SetDirty를 설정하여 Unity에 database에 변화가 생겼다고 알림
                        EditorUtility.SetDirty(database);
                        AssetDatabase.SaveAssets();
                    }
                }

                // 지금부터 그릴 GUI는 Cyan : 청록색
                GUI.color = Color.cyan;
                // // Data를 이름 순으로 정렬하는 Button을 그림
                if (dataType != typeof(Stat) && GUILayout.Button($"Sort By Name"))
                {
                    // 정렬 실행
                    // → Data를 CodeName을 기준으로 오름차순으로 정렬함
                    database.SortByCodeName();

                    // database의 data들의 순서가 바뀌었으니 SetDirty를 설정하여 Unity에 database에 변화가 생겼다고 알림
                    EditorUtility.SetDirty(database);
                    AssetDatabase.SaveAssets();
                }

                // 지금부터 그릴 GUI는 흰색
                GUI.color = Color.white;

                EditorGUILayout.Space(2f);
                // 위, 아래를 나누는 구분선
                CustomEditorUtility.DrawUnderLine(); 
                EditorGUILayout.Space(4f);

                // 이제부터 Database의 Data 목록을 GUI로 보여줄 것
                //                      ↓↓↓
                // 지금부터 Scroll 가능한 Box를 그림, UNITY UI 중 ScrollView와 동일함
                // 첫번째 인자는 현재 Scroll Posiiton → 처음에는 Vector.zero가 들어 있다. (기본 위치)
                // 두번째 인자는 수평 Scroll 막대를 그릴 것인가? → false
                // 세번째 인자는 항상 수직 Scroll 막대를 그릴 것인가? → true
                // 네번째 인자는 항상 수평 Scroll 막대의 Style → GUIStyle.none : 가로 스크롤을 쓰지 않겠다. 
                //                                                              : none을 넘겨주게되면 해당 막대는 아예 없애버림
                // 다섯번째 인자는 수직 Scroll 막대의 Style → GUI.skin.verticalScrollbar
                // 여섯번째 인자는 Background Style → 우리가 이미 위에서 helpBox로 세로 정렬을 하고 있기 때문에 none으로 넣어줌
                // return 값은 사용자의 조작에 의해 바뀌게 된 Scroll Posiiton → return 값을 scrollPosition Dictionary에 저장한다. 

                // ※ 추가사항
                // ScrollView의 크기는 위에서 BeginVertical 함수에 넣은 넓이 300과 동일함
                // BeginScrollView는 여러 Overloading이 있기 때문에 그냥 현재 Scroll Position만 넣어도 ScrollView가 만들어짐
                // 여기서는 수평 막대를 쓰지 않으려고 인자가 많은 함수를 씀
                scrollPositionByType[dataType] = EditorGUILayout.BeginScrollView(scrollPositionByType[dataType], false, true,
                    GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
                {
                    // Database의 Data들을 돌아줌
                    foreach (var data in database.Datas)
                    {
                        string path = AssetDatabase.GetAssetPath(data);
                        string statType = statOwnerType[currentSelectedStatType];
                        if (dataType == typeof(Stat) && !path.Contains(statType))
                        {
                            continue;
                        }

                        // Database의 목록을 그림
                        // CodeName을 그려줄 넓이를 정함, 만약 Icon이 존재한다면 Icon의 크기를 고려하며 좁은 넓이를 가짐
                        // → 아이콘이 존재하면 200, 없다면 245
                        float labelWidth = data.Icon != null ? 200f : 245f;

                        // 현재 Data가 유저가 선택한 Data면 selectedBoxStyle(=배경이 청색)을 가져옴
                        // 아니라면 아무런 설정이 없는 none Style을 가져온다. 
                        var style = selectedObjectsByType[dataType] == data ? selectedBoxStyle : GUIStyle.none;

                        // (3) 수평 정렬 시작 : style과 높이 40
                        // → 밑에 생성된 IdentifiedObject 목록을 보여준다.
                        // ex)
                        //                    ...
                        //                Sort By Name
                        // -----------------------------------------
                        //
                        // 1bf80795-4c57-4866-abf1-7fbcef46b9a1   X     ← 요거!  
                        //
                        EditorGUILayout.BeginHorizontal(style, GUILayout.Height(40f));
                        {
                            // Data에 Icon이 있다면 40x40 사이즈로 그려줌
                            if (data.Icon)
                            {
                                // Icon의 Preview Texture를 가져옴.
                                // → 한번 가져온 Texture는 Unity 내부에 Caching되며, 
                                //    Cache된 Texture 수가 위에서 설정한 TextureCacheSize에 도달하면 오래된 Texture부터 지워짐
                                // ※ AssetPreview : Preview Texture 가져오기
                                // ※ Preview Texture : Project 창을 보면 Asset마다 보여지는 Image
                                var preview = AssetPreview.GetAssetPreview(data.Icon);

                                // GUILayout.Label : icon의 Texture를 그려준다. 
                                // 높이와 넓이는 40으로 설정
                                GUILayout.Label(preview, GUILayout.Height(40f), GUILayout.Width(40f));
                            }

                            // Data의 CodeName을 그려준다. 
                            EditorGUILayout.LabelField(data.CodeName, GUILayout.Width(labelWidth), GUILayout.Height(40f));

                            // (4) 수직 정렬 시작
                            // → 이건 여러 GUI를 수직으로 그리기 위해서가 아니라 X Button을 중앙 정렬을 하기 위해서임
                            EditorGUILayout.BeginVertical();
                            {
                                // 현재 수직 정렬을 시작한 상태기 때문에 위에서 10칸을 띄우게됨
                                EditorGUILayout.Space(10f);

                                GUI.color = Color.red;

                                // data를 삭제할 수 있는 X Button을 그림
                                // → 넓이는 20, 넓이를 설정하면 높이도 자동으로 20이 된다. 
                                // → 기본 높이가 40이기 때문에 위에 10, Button 20, 아래 10해서 X Button이 중앙 정렬의 형태가 된다
                                if (GUILayout.Button("X", GUILayout.Width(20f)))
                                {
                                    database.Remove(data);

                                    // data의 Asset 폴더 내 위치를 찾아와서 삭제
                                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(data));
                                    EditorUtility.SetDirty(database);
                                    AssetDatabase.SaveAssets();
                                }
                            }
                            // (4) 수직 정렬 종료
                            EditorGUILayout.EndVertical();

                            GUI.color = Color.white;
                        }
                        // (3) 수평 정렬 종료
                        EditorGUILayout.EndHorizontal();

                        // ★★★★★ 여기까지 하면 Data의 icon, codeName, 삭제 Button이 있는 하나의 Box GUI가 만들어지게 된다. 

                        // data가 삭제되었다면 즉시 Database 목록을 그리는걸 멈추고 빠져나옴
                        if (data == null)
                            break;
                        // 아니라면 Data를 선택할 수 있도록 Click 기능을 만든다. 

                        // 마지막으로 그린 GUI의 좌표와 크기를 가져옴
                        // → 이 경우 바로 위에 그린 수평 GUI의 좌표와 사이즈임(=BeginHorizontal)
                        var lastRect = GUILayoutUtility.GetLastRect();

                        // MosueDown Event고 mosuePosition이 GUI안에 있다면(=Click) Data를 선택한 것으로 처리함
                        if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
                        {
                            // selectedObject에 현재 data를 넣어주기 
                            selectedObjectsByType[dataType] = data;
                            // 우측 Editor(IdentifiedObjectEditor)의 Scroll Position인 drawingEditorScrollPosition은 zero로 설정
                            drawingEditorScrollPosition = Vector2.zero;
                            // Event에 대한 처리를 했다고 Unity에 알림
                            Event.current.Use();
                        }
                    }
                }
                // ScrollView 종료
                EditorGUILayout.EndScrollView();
            }
            // (2) 수직 정렬 종료
            EditorGUILayout.EndVertical();

            // ★★★★★★★★★★★★★★★★★★★★
            // 여기까지 좌측 Data 목록 GUI의 작성은 끝!
            // ★★★★★★★★★★★★★★★★★★★★

            // ★★★ 우측에 선택된 Data의 Editor를 띄워주는 작업 ★★★
            // → BeginHorizontal (수평 그리기) 계속 진행중!

            // 선택된 Data가 존재한다면 해당 Data의 Editor를 우측에 그려줌
            if (selectedObjectsByType[dataType])
            {
                // ScrollView를 그림, 이번에는 Scroll Position 정보만 넘겨줘서 수직, 수평 막대 다 있는 일반적인 ScrollView를 그림
                // → 단, always 옵션이 없으므로 수직, 수평 막대는 Scroll이 가능한 상태일 때만 나타남
                drawingEditorScrollPosition = EditorGUILayout.BeginScrollView(drawingEditorScrollPosition);
                {
                    // 두칸을 띄워서 좌측의 Data 목록과 간격을 주기 
                    EditorGUILayout.Space(2f);

                    // ※ Editor.CreateCachedEditor : Data의 Editor 만들기 
                    // 1. 첫번째 인자는 Editor를 만들 Target
                    // 2. 두번째 인자는 Target의 Type → null을 넣으면 Target의 기본 Type을 사용 → dataType
                    // 3. 세번째는 내부에서 만들어진 Editor를 담을 Editor 변수
                    // → CreateCachedEditor는 내부에서 만들어야할 Editor와 cachedEditor가 같다면
                    //    Editor를 새로 만들지 않고 그냥 cachedEditor를 그대로 반환함
                    //    만약 내부에서 만들어야할 Editor와 cachedEditor가 다르다면                                                         
                    //    cachedEditor를 Destroy하고 새로 만든 Editor를 넣음
                    // ex) A라는 Data를 보고 있어서 cachedEditor는 A의 Editor였는데 B라는 Data를 Click해서 
                    //     selectedObject가 B로 바뀌면 cachedEditor는 A의 Editor이고 selectedObjec는 B이니까 서로 안맞는다. 
                    //     이 경우에 저장된 Editor를 파괴하고 B의 Editor를 만들어서 저장한다. 
                    //     반대로, Target과 Editor가 일치한다면 아무것도 하지 않는다. 
                    //     → Target이 Data A, cashedEditor이 Data A이면 아무것도 하지 않음
                    Editor.CreateCachedEditor(selectedObjectsByType[dataType], null, ref cashedEditor);

                    // Editor를 그려줌 → IdentifiedObjectEditor의 OnInspectorGUI
                    cashedEditor.OnInspectorGUI();
                }
                // ScrollView 종료
                EditorGUILayout.EndScrollView();
            }
        }
        // (1) 수평 정렬 종료
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    // 위에서 만든 함수 사용
    #region DrawGUI
    private void OnEnable() // Window가 켜졌을 때, 실행
    {
        SetUpStyle();
        // Database : Category, Stat
        SetUpDatabase(new[] { typeof(Category), typeof(Stat), typeof(Skill), typeof(Effect) });
        SetUpStatOwnerType(statOwnerType);
    }

    private void OnDisable() // Window가 꺼졌을 때, 실행
    {
        DestroyImmediate(cashedEditor);
        // selectedBoxTexture.hideFlags = HideFlags.DontSave; 해당 코드 때문에
        // 우리가 직접 Destroy 해줘야 한다. 
        DestroyImmediate(selectedBoxTexture);
    }

    private void Update()
    {
        if (isRefresh)
        {
            statOwnerType.Clear();
            SetUpStatOwnerType(statOwnerType);
            isRefresh = false;
        }
    }

    // ※ OnGUI
    // 1. Update문 처럼 매 프레임마다 호출
    // 2. 이 때 함수 내에 구성한 그래픽대로 화면에 찍어주는 그래픽 코드
    // 3. 또한 중요한 점이 Update처럼 해당 오브젝트(스크립트)가 활성화될 때만 계속 호출
    // → 따라서 오브젝트(스크립트)를 끄면 비활성화되서 GUI를 더이상 안그려서 화면에 그린게 사라짐
    private void OnGUI() // Window에 GUI를 그려주는 Editor 함수 
    {
        // GUILayout.Toolbar : 제일 상단 부분에 Toolbar를 만듬
        // Database들이 관리 중인 IdentifiedObject들의 Type Name으로 Toolbar를 그려줌
        // ※ return : The index of the selected button
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, databaseTypeNames);

        EditorGUILayout.Space(4f);
        CustomEditorUtility.DrawUnderLine();
        EditorGUILayout.Space(4f);

        // DrawDatabase 함수에 toolbarIndex에 해당하는 Type을 넣어준다. 
        // → SkillSystemWindow 강의 당시 Toolbar 목록이 Category 하나 뿐이라, Index가 무조건 첫 번째인 0으로 나오고 
        //    databaseTypes의 0번째는 Category이니 무조건 Category만 그린다. 
        // → 앞으로 IdentifiedObject를 상속받는 class들이 추가됨에 따라 SetupDatabase의 인자 배열에도 여러 Type이 추가될 것
        //    ex) typeof(Stat), typeof(Skill)
        DrawDatabase(databaseTypes[toolbarIndex]);

        if (currentSelectedStatType != prevSelectedStatType)
        {
            selectedObjectsByType[typeof(Stat)] = null;
            prevSelectedStatType = currentSelectedStatType;
        }
    }
    #endregion
}
