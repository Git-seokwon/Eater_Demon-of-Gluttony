using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Editor
using System.Reflection; // c# Reflection

// IdentifiedObject의 관리를 맡는 데이터베이스
[CreateAssetMenu(menuName = "IODatabase")]
public class IODatabase : ScriptableObject
{
    [SerializeField]
    private List<IdentifiedObject> datas = new();

    // List 변수를 외부에 공개하기 위한 Property
    public IReadOnlyList<IdentifiedObject> Datas => datas;

    // Count Property
    public int Count => datas.Count;

    // int index를 통해 Data에 접근할 수 있게 해주는 Indexer
    public IdentifiedObject this[int index] => datas[index];

    // IdentifiedObject의 ID를 Setting 해주는 함수
    #region ID SETTING
    private void SetID(IdentifiedObject target, int id)
    {
        // Reflection → Type을 이용해서 IdentifiedObject의 id 변수를 필드로 찾아옴
        // Type → typeof(Type)
        // Type.GetField : 이름으로 필드를 검색, 해당 필드에 대한 하나의 FieldInfo 개체를 반환, 찾지 못한 경우 null을 반환
        //               : GetField는 Public인 필드에 한해서만 검색하므로, Public이 아닌 필드에 대해 검색하려면
        //                 BindingFlags를 사용
        // BindingFlags.NonPublic : 한정자가 public이 아니여야함
        // BindingFlags.Instance  : static type이 아니여야함
        var field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        // 가져온 id 변수 정보로 토대로 target의 id 변수 값을 수정함
        // → 이런 식으로 private 변수라도 은닉성을 지키면서 값을 Setting할 수 있게 된다.
        field.SetValue(target, id);

        // Serialize 변수(여기서는 id 변수)를 code상에서 수정했을 경우 EditorUtility.SetDirty를 통해서 Serailize 변수가 수정되었음을
        // Unity에 알려줘야한다. 그렇지 않으면 수정한 값이 반영되지 않고 이전 값으로 돌아간다. 
        // 여기서는 값이 수정되었다고 Unity에 알려줄뿐, 실제로 값이 저장될려면 Editor Code에서 ApplyModifiedProperties 함수 혹은
        // 프로젝트 전체를 저장하는 AssetDatabase.SaveAssets 함수가 호출되어야 한다.
        // (여기서는 나중에 다른 곳에서 AssetDatabase.SaveAssets를 호출 할 것이기 따로 작성하지 않는다)
#if UNITY_EDITOR
        EditorUtility.SetDirty(target);
#endif
    }
    #endregion

    // Data List 안에 있는 IdentifiedObject의 ID를 Index 순서에 따라 Setting 해주는 함수
    // → SetID 함수와 똑같이 Reflection을 이용해서 ID 변수의 정보를 가져오고 for문을 돌면서 data의 ID 값을 Index 값으로 Setting
    private void ReOrderDatas()
    {
        var field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < datas.Count; i++)
        {
            field.SetValue(datas[i], i);
#if UNITY_EDITOR
            EditorUtility.SetDirty(datas[i]);
#endif
        }
    }

    // 데이터 추가 
    public void Add(IdentifiedObject newData)
    {
        // 데이터 추가
        datas.Add(newData);
        // ID 설정
        SetID(newData, datas.Count - 1);
    }

    // 데이터 삭제
    public void Remove(IdentifiedObject data)
    {
        //  데이터 삭제
        datas.Remove(data);
        // 재정렬
        ReOrderDatas();
    }

    // ID로 해당 데이터 가져오기 (ID는 곧 Index)
    public IdentifiedObject GetDataByID(int id) => datas[id];

    // 편의성 목적으로 ID를 찾아냄과 동시에 형변환까지 해주는 Generic 함수
    // → 형변환은 IdentifiedObject 클래스로 제한 GetDataByID로 데이터 가져오기
    public T GetDataByID<T>(int id) where T : IdentifiedObject => GetDataByID(id) as T;

    // codeName으로 해당 데이터를 가져오기 (Find 함수에 deligate 사용)
    public IdentifiedObject GetDataCodeName(string codeName) => datas.Find(item => item.CodeName == codeName);

    // 편의성 목적으로 CodeName을 찾아냄과 동시에 형변환까지 해주는 Generic 함수 
    public T GetDataByCodeName<T>(string codeName) where T : IdentifiedObject => GetDataCodeName(codeName) as T;

    // 해당 데이터가 존재하는지 탐색하기 
    public bool Contains(IdentifiedObject item) => datas.Contains(item);

    // Data를 CodeName을 기준으로 오름차순으로 정렬
    public void SortByCodeName()
    {
        datas.Sort((x, y) => x.CodeName.CompareTo(y.CodeName));

        // CodeName 기준으로 오름차순 정렬된 상태에 ID 재부여 
        ReOrderDatas();
    }
}
