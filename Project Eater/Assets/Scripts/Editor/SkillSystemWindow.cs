using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

public class SkillSystemWindow : EditorWindow // ������ â�� ��ӹ��� ex) Animator â
{
    // Static ���� : Window�� �ݰ� �ٽ� ������ ��, Data���� ������ ������ �� �״�� �ٽ� ���� ����
    #region Static ����
    // ���� ���� �ִ� database�� index
    private static int toolbarIndex = 0;

    // Database List�� Scroll Position (SkillSystemWindow ���� Editor Scroll)
    // �� Database�� ���� Scroll�� ��� ��ġ�� �ִ��� ��ǥ���� ������ ����
    // �� Ű ���� IdentifiedObejct�� ��ӹ޴� Data�� Type�̴�. ex) Category, Stat ��
    private static Dictionary<Type, Vector2> scrollPositionByType = new();

    // ���� �����ְ� �ִ� data�� Scroll Posiiton
    // �� ���� Editor(IdentifiedObjectEditor)�� Scroll Position
    private static Vector2 drawingEditorScrollPosition;

    //                   ��ũ�� ��ġ
    /* ------------------------------------------------------
     *                        |
     *  scrollPositionByType  |  drawingEditorScrollPosition
     *                        |
       ------------------------------------------------------ */

    // ���� ������ Data
    // �� Database �� ����ڰ� ��Ͽ��� ������ Data�� ���� �����ϴ� ���� 
    private static Dictionary<Type, IdentifiedObject> selectedObjectsByType = new();
    #endregion

    #region �Ϲ� ����
    // Type�� Database(Category, Stat, Skill ���...)
    private readonly Dictionary<Type, IODatabase> databasesByType = new();
    
    // Database Data���� Type��
    private Type[] databaseTypes;
    
    // �� Type���� string �̸�
    private string[] databaseTypeNames;
    
    // ���� �����ְ� �ִ� data�� Editor class
    // �� ��Ͽ��� ������ Data�� Editor�� ������ �����ش�.
    // �� Data�� Editor�� �����ַ��� ������ Data�� Editor ��ü�� �����ؾ� �Ѵ�. 
    //    �̶� ������ Data�� Editor ��ü�� ������ �����̴�.
    private Editor cashedEditor;

    // Database List�� Selected Background Texture
    // �� '�� Data�� ����ڰ� ������ Data��'��� �� �����ֱ� ���� ���õ� Data�� ��濡 ������ Texture
    private Texture2D selectedBoxTexture; // Texture
    // Database List�� Selected Style
    // �� ���õ� Data�� GUI�� ������ Style�̴�.
    private GUIStyle selectedBoxStyle; // Style

    private List<string> statOwnerType = new();
    string statPath;
    private int currentSelectedStatType = 0;
    private int prevSelectedStatType = -1;
    private bool isRefresh = false;
    #endregion

    #region OpenWindow
    // Editor Tools �ǿ� Skill System �׸��� �߰��ǰ�, Click�� Window�� ����
    [MenuItem("Tool/Skill System")]
    private static void OpenWindow()
    {
        // GetWindow : Skill System�̶� ��Ī�� ���� Window�� ����
        // �� ���� �� : ������ Window�� Title
        var window = GetWindow<SkillSystemWindow>("Skill System");
        window.minSize = new Vector2(800, 700);
        window.Show();

        // OpenWindow �Լ��� �ڵ带 �ۼ��ϰ� �Ǹ�, �ƹ� ���뵵 ���� ��â�� �߰� �ȴ�.
    }
    #endregion

    #region SetUp
    // Texture�� Style ������ Setup �ϴ� �Լ�
    private void SetUpStyle()
    {
        // 1x1 Pixel�� Texture�� ����
        selectedBoxTexture = new Texture2D(1, 1);

        // SetPixel�� ���� û������ �����Ѵ�.
        // �� 1x1 Pixel�̹Ƿ� 0,0 ��ǥ �ȼ��� ��ü �ȼ��� �Ǹ�, ��ü Pixel�� Color(=û��)�� �������� ���� �ȴ�.
        selectedBoxTexture.SetPixel(0, 0, new Color(0.31f, 0.4f, 0.5f));

        // ������ ������ Color���� ����
        selectedBoxTexture.Apply();

        // �� Texture�� Window���� ������ ���̱� ������ Unity���� �ڵ� ������������(DontSave) Flag�� ��������
        // �� �ش� flag�� ���ٸ� Editor���� Play�� ����ä�� SetupStyle �Լ��� ����Ǹ�
        //    texture�� Play ���¿� ���ӵǾ� Play�� �����ϸ� texture�� �ڵ� Destroy �ǹ�����.
        // �� DontSave Flag�� �������ָ� �̷� ������ �����ϰ� Texture�� Destroy ������ �츮�� ������ �� �ִ�.
        selectedBoxTexture.hideFlags = HideFlags.DontSave;

        selectedBoxStyle = new GUIStyle();
        // Select�� Data�� Normal ���� Backgorund Texture�� ������ ������ Texture�� ����
        // �� Select�� Data�� Background�� û������ ���ͼ� �����ȴ�. 
        selectedBoxStyle.normal.background = selectedBoxTexture;
    }

    // Database�� Project���� Load �ؿ��ų� Database�� ���� �ÿ� �ڵ� �������ִ� �Լ� 
    // �� dataTypes : IdentifiedObject�� ��ӹ޴� Type�� ex) Category, Stat ��
    private void SetUpDatabase(Type[] dataTypes)
    {
        // ���� Database�� Load���� ���� ���̹Ƿ� Load �۾��� ����
        if (databasesByType.Count == 0)
        {
            // �� AssetDatabase.IsValidFolder : ���ڷ� ���� Folder Path�� �����ϴ��� Ȯ��
            // �� Resources Folder�� Database Folder�� �ִ��� Ȯ��
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/GameResources/Resources/Database"))
            {
                // �� �߰�����
                // �� �ش� �ڵ忡���� �̸� Resources ������ �������� ������ Resources ������ �ִ��� Ȯ���� �� ��
                // �� ���Ǽ��� �� ���̰� �ʹٸ�, ���� �۾��� �Ȱ��� Resources ������ �ִ���, Ȯ���ϰ� ���ٸ� ������ָ� �ȴ�.

                // Database Folder�� ����� �ش�.
                AssetDatabase.CreateFolder("Assets/Prefabs/GameResources/Resources", "Database");
            }

            foreach (var type in dataTypes)
            {
                // �� AssetDatabase.LoadAssetAtPath<IODatabase>() : Project�� ������� Database �ҷ����� 
                // �� Database ������ �ִ� type.Name�� Database(Database.asset)�� �ҷ��´�. 
                // ex) type�� Category��� CategoryDatabase��� �̸��� ���� Database�� �ҷ��´�. 
                var database = AssetDatabase.LoadAssetAtPath<IODatabase>($"Assets/Prefabs/GameResources/Resources/Database/{type.Name}Database.asset");

                // Project�� �ش� Database�� �������� ���� �� Database ����
                if (database == null)
                {
                    // �� CreateInstance : ScriptableObject ��ü ����
                    database = CreateInstance<IODatabase>();

                    // ������ Database�� Database ������ (type.Name)Database��� �̸����� ����
                    // �� .asset�� ScriptableObject�� ���� Ȯ�����̴�. ���� ���, Audio Clip Asset�� ����� �ʹٸ�
                    //    �ڿ� Ȯ���ڷ� .audioClip���� �ؾ� �Ѵ�.
                    AssetDatabase.CreateAsset(database, $"Assets/Prefabs/GameResources/Resources/Database/{type.Name}Database.asset");

                    // ������ �ּҿ� ���� Folder�� ����
                    // �� �ش� Folder�� Window�� ���� ������ IdentifiedObject�� ����� �����
                    AssetDatabase.CreateFolder("Assets/Prefabs/GameResources/Resources", type.Name);
                }

                // Database Load or ����
                // �� �ҷ��� or ������ Database�� Dictionary�� ����
                databasesByType[type] = database;
                // �� ScrollPosition Data ����
                scrollPositionByType[type] = Vector2.zero;
                // �� SelectObject Data ����
                selectedObjectsByType[type] = null;
            }

            // �� Linq.Select : dataTypes���� Name�� ������
            // �� ���ڷ� ���� Type���� �̸��� Array ���·� ��������
            // �� databaseTypeNames�� SkillSystem�� Toolbar Tab�� �����ϴµ� ���� ��
            databaseTypeNames = dataTypes.Select(x => x.Name).ToArray();
            // �� ���ڷ� ���� Type�鵵 ���߿� Ȱ���ϱ� ���� �޾Ƶα� 
            databaseTypes = dataTypes;
        }
    }

    private void SetUpStatOwnerType(List<string> statOwnerType)
    {
        // Stat ���� set
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
    // Database�� ���� Data ����� GUI�� �׷��ִ� �Լ� 
    private void DrawDatabase(Type dataType)
    {
        // ���ڷ� ���� Type���� �׷��� Database�� �����´�. 
        var database = databasesByType[dataType];

        // Editor�� Caching�Ǵ� Preview Texture�� ���� �ּ� 32��, �ִ� 32 + database�� Count���� �ø�
        // �� �� �۾��� �����ָ� �׷����ϴ� IO ��ü�� Icon���� ���� ��� ����� �׷����� �ʴ� ������ �߻���
        // �� �� �۾��� ���� Database�� Data�� �ƹ��� ���Ƶ� ��� Texture�� Caching�� �� �ְ� �ȴ�.
        // �� ������ Texture�� ���� Caching �ȴٰ� �ص� ������ ��ǻ���� Ram�� ���� �ణ �� ����Ѵٴ� ���� 
        //    Texture �˻��� ���������� ���� �ణ�� �ð��� �� �ҿ�ȴٴ� �� ����� �ƹ� ������ ����. 
        AssetPreview.SetPreviewTextureCacheSize(Mathf.Max(32, 32 + database.Count));

        // Database�� Data ����� �׷��ֱ� ���� 
        // (1) ���� ���� ����
        EditorGUILayout.BeginHorizontal();
        {
            // (2) ���� ���� ����, Style�� HelpBox, ���̴� 300f
            // �� 300 ���� ũ���� ���� �ڽ��� �׷��� 
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

                // ���ݺ��� �׸� GUI�� �ʷϻ�
                GUI.color = Color.green;

                // ���ο� Data�� ����� Button�� �׷���
                // �� ����ڰ� �ش� ��ư�� Click ���� ��, True
                if (GUILayout.Button($"New {dataType.Name}"))
                {
                    // System Namespace�� Guid ����ü�� �̿��ؼ� ���� �ĺ��ڸ� ���� �� Data�� �ӽ� codeName���� ���� ����
                    // �� ���� �ĺ��ڶ�°� ������ ��ĥ �� ���� � ��
                    // �� �߰�����
                    // �� ���� �ĺ��ڸ� ����� �˰����� ���������� �ִµ� ���⼭�� �����ϰ� Guid�� ���� ������ �ʰ�
                    //    c#���� �����ϴ� Guid ����ü�� �̿� 
                    var guid = Guid.NewGuid();

                    // dataType�� �̿��ؼ� scriptableObject�� ���� �� IdentifiedObject�� ����ȯ
                    var newData = CreateInstance(dataType) as IdentifiedObject;

                    // Reflection�� �̿��� IdentifiedObject�� codeName Field�� ã�ƿͼ�(1) newData�� codeName�� �ӽ� codeName�� guid�� Set(2)
                    // �� dataType.BaseType : dataType�� ��ӹް� �ִ� �θ� Type�� �������� (�θ� Type�� codeName ������ �����ϱ� ����)
                    //                      : ���ڷ� ������ dataType�� Categoryó�� ������ IdentifiedObject�� ��ӹް� �ִ� Class�̱� ������ 
                    //                      : �� ��쿡�� IdentifiedObject�� �� ���̴�. 
                    dataType.BaseType.GetField("codeName", BindingFlags.NonPublic | BindingFlags.Instance) // 1
                        .SetValue(newData, guid.ToString());                                               // 2

                    // ���� newData�� Project�� ���� (newData�� ���� codeName�� guid�� ���� �� ��Ȳ)
                    // �� Resources ������ Type �̸� ����
                    // �� �������� ��Ī�� Type.Name�� �빮��_guid (ScriptableObject)
                    // ex) Ÿ���� Category�� CATEGORY_Guid�� ��
                    if (dataType == typeof(Stat))
                    {
                        AssetDatabase.CreateAsset(newData,
                            $"Assets/Prefabs/GameResources/Resources/{dataType.Name}/{statOwnerType[currentSelectedStatType]}/{statOwnerType[currentSelectedStatType].ToUpper()}_{guid}.asset");
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(newData, $"Assets/Prefabs/GameResources/Resources/{dataType.Name}/{dataType.Name.ToUpper()}_{guid}.asset");
                    }
                    
                    // ������ Data�� Database�� �߰��ϱ� 
                    // �� �߰����� "���� ��ŷ" ���
                    // �� �ش� �Լ� ������ �ڵ带 ã�Ƽ� ������
                    database.Add(newData);

                    // database���� data�� �߰�������(= Serialize ������ datas ������ ��ȭ�� ����) �� id���� ���� ������
                    // �� SetDirty�� �����Ͽ� Unity�� database�� Serialize ������ ���ߴٰ� �˸�
                    EditorUtility.SetDirty(database);

                    // Dirty flag ����� ������
                    // �� ��� ���� ���׵��� ���� 
                    AssetDatabase.SaveAssets();

                    // selectedObject�� ���� ������ Data�� �����Ͽ� ������ Editor�� �׷����� �����. 
                    // �� ���� ���� �ִ� IdentifiedObject�� ���� ������� IdentifiedObject�� ����
                    // �� ���� ���� Data�� �ڵ� ���õǾ� �ٷ� ���� Editor���� �۾��� �� �ְ� ���Ǽ��� ��� 
                    selectedObjectsByType[dataType] = newData;
                }

                // ���ݺ��� �׸� GUI�� ������
                GUI.color = Color.red;
                // ������ ������ Data�� �����ϴ� Button�� �׷���
                if (dataType != typeof(Stat) && GUILayout.Button($"Remove Last {dataType.Name}")) // ex) Remove Last Category
                {
                    // Database�� ������ Data�� �����´�. 
                    // �� Data�� ID�� Database�� �߰��� �����̱� ������ ������ ������ ���� �ֱ��� Data�̴�. 
                    var lastData = database.Count > 0 ? database.Datas.Last() : null;
                    if (lastData)
                    {
                        // ������ ������ Data ����
                        database.Remove(lastData);

                        // �� AssetDatabase.DeleteAsset : Project�� ����� Data ����
                        // �� AssetDatabase.GetAssetPath : Data�� Project�� ��� �ִ��� ��ġ�� ã�ƿ��� 
                        // Data�� Asset ���� �� ��ġ�� ã�ƿͼ� ����
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(lastData));

                        // database���� data�� ���������� SetDirty�� �����Ͽ� Unity�� database�� ��ȭ�� ����ٰ� �˸�
                        EditorUtility.SetDirty(database);
                        AssetDatabase.SaveAssets();
                    }
                }

                // ���ݺ��� �׸� GUI�� Cyan : û�ϻ�
                GUI.color = Color.cyan;
                // // Data�� �̸� ������ �����ϴ� Button�� �׸�
                if (dataType != typeof(Stat) && GUILayout.Button($"Sort By Name"))
                {
                    // ���� ����
                    // �� Data�� CodeName�� �������� ������������ ������
                    database.SortByCodeName();

                    // database�� data���� ������ �ٲ������ SetDirty�� �����Ͽ� Unity�� database�� ��ȭ�� ����ٰ� �˸�
                    EditorUtility.SetDirty(database);
                    AssetDatabase.SaveAssets();
                }

                // ���ݺ��� �׸� GUI�� ���
                GUI.color = Color.white;

                EditorGUILayout.Space(2f);
                // ��, �Ʒ��� ������ ���м�
                CustomEditorUtility.DrawUnderLine(); 
                EditorGUILayout.Space(4f);

                // �������� Database�� Data ����� GUI�� ������ ��
                //                      ����
                // ���ݺ��� Scroll ������ Box�� �׸�, UNITY UI �� ScrollView�� ������
                // ù��° ���ڴ� ���� Scroll Posiiton �� ó������ Vector.zero�� ��� �ִ�. (�⺻ ��ġ)
                // �ι�° ���ڴ� ���� Scroll ���븦 �׸� ���ΰ�? �� false
                // ����° ���ڴ� �׻� ���� Scroll ���븦 �׸� ���ΰ�? �� true
                // �׹�° ���ڴ� �׻� ���� Scroll ������ Style �� GUIStyle.none : ���� ��ũ���� ���� �ʰڴ�. 
                //                                                              : none�� �Ѱ��ְԵǸ� �ش� ����� �ƿ� ���ֹ���
                // �ټ���° ���ڴ� ���� Scroll ������ Style �� GUI.skin.verticalScrollbar
                // ������° ���ڴ� Background Style �� �츮�� �̹� ������ helpBox�� ���� ������ �ϰ� �ֱ� ������ none���� �־���
                // return ���� ������� ���ۿ� ���� �ٲ�� �� Scroll Posiiton �� return ���� scrollPosition Dictionary�� �����Ѵ�. 

                // �� �߰�����
                // ScrollView�� ũ��� ������ BeginVertical �Լ��� ���� ���� 300�� ������
                // BeginScrollView�� ���� Overloading�� �ֱ� ������ �׳� ���� Scroll Position�� �־ ScrollView�� �������
                // ���⼭�� ���� ���븦 ���� �������� ���ڰ� ���� �Լ��� ��
                scrollPositionByType[dataType] = EditorGUILayout.BeginScrollView(scrollPositionByType[dataType], false, true,
                    GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
                {
                    // Database�� Data���� ������
                    foreach (var data in database.Datas)
                    {
                        string path = AssetDatabase.GetAssetPath(data);
                        string statType = statOwnerType[currentSelectedStatType];
                        if (dataType == typeof(Stat) && !path.Contains(statType))
                        {
                            continue;
                        }

                        // Database�� ����� �׸�
                        // CodeName�� �׷��� ���̸� ����, ���� Icon�� �����Ѵٸ� Icon�� ũ�⸦ ����ϸ� ���� ���̸� ����
                        // �� �������� �����ϸ� 200, ���ٸ� 245
                        float labelWidth = data.Icon != null ? 200f : 245f;

                        // ���� Data�� ������ ������ Data�� selectedBoxStyle(=����� û��)�� ������
                        // �ƴ϶�� �ƹ��� ������ ���� none Style�� �����´�. 
                        var style = selectedObjectsByType[dataType] == data ? selectedBoxStyle : GUIStyle.none;

                        // (3) ���� ���� ���� : style�� ���� 40
                        // �� �ؿ� ������ IdentifiedObject ����� �����ش�.
                        // ex)
                        //                    ...
                        //                Sort By Name
                        // -----------------------------------------
                        //
                        // 1bf80795-4c57-4866-abf1-7fbcef46b9a1   X     �� ���!  
                        //
                        EditorGUILayout.BeginHorizontal(style, GUILayout.Height(40f));
                        {
                            // Data�� Icon�� �ִٸ� 40x40 ������� �׷���
                            if (data.Icon)
                            {
                                // Icon�� Preview Texture�� ������.
                                // �� �ѹ� ������ Texture�� Unity ���ο� Caching�Ǹ�, 
                                //    Cache�� Texture ���� ������ ������ TextureCacheSize�� �����ϸ� ������ Texture���� ������
                                // �� AssetPreview : Preview Texture ��������
                                // �� Preview Texture : Project â�� ���� Asset���� �������� Image
                                var preview = AssetPreview.GetAssetPreview(data.Icon);

                                // GUILayout.Label : icon�� Texture�� �׷��ش�. 
                                // ���̿� ���̴� 40���� ����
                                GUILayout.Label(preview, GUILayout.Height(40f), GUILayout.Width(40f));
                            }

                            // Data�� CodeName�� �׷��ش�. 
                            EditorGUILayout.LabelField(data.CodeName, GUILayout.Width(labelWidth), GUILayout.Height(40f));

                            // (4) ���� ���� ����
                            // �� �̰� ���� GUI�� �������� �׸��� ���ؼ��� �ƴ϶� X Button�� �߾� ������ �ϱ� ���ؼ���
                            EditorGUILayout.BeginVertical();
                            {
                                // ���� ���� ������ ������ ���±� ������ ������ 10ĭ�� ���Ե�
                                EditorGUILayout.Space(10f);

                                GUI.color = Color.red;

                                // data�� ������ �� �ִ� X Button�� �׸�
                                // �� ���̴� 20, ���̸� �����ϸ� ���̵� �ڵ����� 20�� �ȴ�. 
                                // �� �⺻ ���̰� 40�̱� ������ ���� 10, Button 20, �Ʒ� 10�ؼ� X Button�� �߾� ������ ���°� �ȴ�
                                if (GUILayout.Button("X", GUILayout.Width(20f)))
                                {
                                    database.Remove(data);

                                    // data�� Asset ���� �� ��ġ�� ã�ƿͼ� ����
                                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(data));
                                    EditorUtility.SetDirty(database);
                                    AssetDatabase.SaveAssets();
                                }
                            }
                            // (4) ���� ���� ����
                            EditorGUILayout.EndVertical();

                            GUI.color = Color.white;
                        }
                        // (3) ���� ���� ����
                        EditorGUILayout.EndHorizontal();

                        // �ڡڡڡڡ� ������� �ϸ� Data�� icon, codeName, ���� Button�� �ִ� �ϳ��� Box GUI�� ��������� �ȴ�. 

                        // data�� �����Ǿ��ٸ� ��� Database ����� �׸��°� ���߰� ��������
                        if (data == null)
                            break;
                        // �ƴ϶�� Data�� ������ �� �ֵ��� Click ����� �����. 

                        // ���������� �׸� GUI�� ��ǥ�� ũ�⸦ ������
                        // �� �� ��� �ٷ� ���� �׸� ���� GUI�� ��ǥ�� ��������(=BeginHorizontal)
                        var lastRect = GUILayoutUtility.GetLastRect();

                        // MosueDown Event�� mosuePosition�� GUI�ȿ� �ִٸ�(=Click) Data�� ������ ������ ó����
                        if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
                        {
                            // selectedObject�� ���� data�� �־��ֱ� 
                            selectedObjectsByType[dataType] = data;
                            // ���� Editor(IdentifiedObjectEditor)�� Scroll Position�� drawingEditorScrollPosition�� zero�� ����
                            drawingEditorScrollPosition = Vector2.zero;
                            // Event�� ���� ó���� �ߴٰ� Unity�� �˸�
                            Event.current.Use();
                        }
                    }
                }
                // ScrollView ����
                EditorGUILayout.EndScrollView();
            }
            // (2) ���� ���� ����
            EditorGUILayout.EndVertical();

            // �ڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡ�
            // ������� ���� Data ��� GUI�� �ۼ��� ��!
            // �ڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡ�

            // �ڡڡ� ������ ���õ� Data�� Editor�� ����ִ� �۾� �ڡڡ�
            // �� BeginHorizontal (���� �׸���) ��� ������!

            // ���õ� Data�� �����Ѵٸ� �ش� Data�� Editor�� ������ �׷���
            if (selectedObjectsByType[dataType])
            {
                // ScrollView�� �׸�, �̹����� Scroll Position ������ �Ѱ��༭ ����, ���� ���� �� �ִ� �Ϲ����� ScrollView�� �׸�
                // �� ��, always �ɼ��� �����Ƿ� ����, ���� ����� Scroll�� ������ ������ ���� ��Ÿ��
                drawingEditorScrollPosition = EditorGUILayout.BeginScrollView(drawingEditorScrollPosition);
                {
                    // ��ĭ�� ����� ������ Data ��ϰ� ������ �ֱ� 
                    EditorGUILayout.Space(2f);

                    // �� Editor.CreateCachedEditor : Data�� Editor ����� 
                    // 1. ù��° ���ڴ� Editor�� ���� Target
                    // 2. �ι�° ���ڴ� Target�� Type �� null�� ������ Target�� �⺻ Type�� ��� �� dataType
                    // 3. ����°�� ���ο��� ������� Editor�� ���� Editor ����
                    // �� CreateCachedEditor�� ���ο��� �������� Editor�� cachedEditor�� ���ٸ�
                    //    Editor�� ���� ������ �ʰ� �׳� cachedEditor�� �״�� ��ȯ��
                    //    ���� ���ο��� �������� Editor�� cachedEditor�� �ٸ��ٸ�                                                         
                    //    cachedEditor�� Destroy�ϰ� ���� ���� Editor�� ����
                    // ex) A��� Data�� ���� �־ cachedEditor�� A�� Editor���µ� B��� Data�� Click�ؼ� 
                    //     selectedObject�� B�� �ٲ�� cachedEditor�� A�� Editor�̰� selectedObjec�� B�̴ϱ� ���� �ȸ´´�. 
                    //     �� ��쿡 ����� Editor�� �ı��ϰ� B�� Editor�� ���� �����Ѵ�. 
                    //     �ݴ��, Target�� Editor�� ��ġ�Ѵٸ� �ƹ��͵� ���� �ʴ´�. 
                    //     �� Target�� Data A, cashedEditor�� Data A�̸� �ƹ��͵� ���� ����
                    Editor.CreateCachedEditor(selectedObjectsByType[dataType], null, ref cashedEditor);

                    // Editor�� �׷��� �� IdentifiedObjectEditor�� OnInspectorGUI
                    cashedEditor.OnInspectorGUI();
                }
                // ScrollView ����
                EditorGUILayout.EndScrollView();
            }
        }
        // (1) ���� ���� ����
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    // ������ ���� �Լ� ���
    #region DrawGUI
    private void OnEnable() // Window�� ������ ��, ����
    {
        SetUpStyle();
        // Database : Category, Stat
        SetUpDatabase(new[] { typeof(Category), typeof(Stat), typeof(Skill), typeof(Effect) });
        SetUpStatOwnerType(statOwnerType);
    }

    private void OnDisable() // Window�� ������ ��, ����
    {
        DestroyImmediate(cashedEditor);
        // selectedBoxTexture.hideFlags = HideFlags.DontSave; �ش� �ڵ� ������
        // �츮�� ���� Destroy ����� �Ѵ�. 
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

    // �� OnGUI
    // 1. Update�� ó�� �� �����Ӹ��� ȣ��
    // 2. �� �� �Լ� ���� ������ �׷��ȴ�� ȭ�鿡 ����ִ� �׷��� �ڵ�
    // 3. ���� �߿��� ���� Updateó�� �ش� ������Ʈ(��ũ��Ʈ)�� Ȱ��ȭ�� ���� ��� ȣ��
    // �� ���� ������Ʈ(��ũ��Ʈ)�� ���� ��Ȱ��ȭ�Ǽ� GUI�� ���̻� �ȱ׷��� ȭ�鿡 �׸��� �����
    private void OnGUI() // Window�� GUI�� �׷��ִ� Editor �Լ� 
    {
        // GUILayout.Toolbar : ���� ��� �κп� Toolbar�� ����
        // Database���� ���� ���� IdentifiedObject���� Type Name���� Toolbar�� �׷���
        // �� return : The index of the selected button
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, databaseTypeNames);

        EditorGUILayout.Space(4f);
        CustomEditorUtility.DrawUnderLine();
        EditorGUILayout.Space(4f);

        // DrawDatabase �Լ��� toolbarIndex�� �ش��ϴ� Type�� �־��ش�. 
        // �� SkillSystemWindow ���� ��� Toolbar ����� Category �ϳ� ���̶�, Index�� ������ ù ��°�� 0���� ������ 
        //    databaseTypes�� 0��°�� Category�̴� ������ Category�� �׸���. 
        // �� ������ IdentifiedObject�� ��ӹ޴� class���� �߰��ʿ� ���� SetupDatabase�� ���� �迭���� ���� Type�� �߰��� ��
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
