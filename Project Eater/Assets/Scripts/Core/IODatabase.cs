using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Editor
using System.Reflection; // c# Reflection

// IdentifiedObject�� ������ �ô� �����ͺ��̽�
[CreateAssetMenu(menuName = "IODatabase")]
public class IODatabase : ScriptableObject
{
    [SerializeField]
    private List<IdentifiedObject> datas = new();

    // List ������ �ܺο� �����ϱ� ���� Property
    public IReadOnlyList<IdentifiedObject> Datas => datas;

    // Count Property
    public int Count => datas.Count;

    // int index�� ���� Data�� ������ �� �ְ� ���ִ� Indexer
    public IdentifiedObject this[int index] => datas[index];

    // IdentifiedObject�� ID�� Setting ���ִ� �Լ�
    #region ID SETTING
    private void SetID(IdentifiedObject target, int id)
    {
        // Reflection �� Type�� �̿��ؼ� IdentifiedObject�� id ������ �ʵ�� ã�ƿ�
        // Type �� typeof(Type)
        // Type.GetField : �̸����� �ʵ带 �˻�, �ش� �ʵ忡 ���� �ϳ��� FieldInfo ��ü�� ��ȯ, ã�� ���� ��� null�� ��ȯ
        //               : GetField�� Public�� �ʵ忡 ���ؼ��� �˻��ϹǷ�, Public�� �ƴ� �ʵ忡 ���� �˻��Ϸ���
        //                 BindingFlags�� ���
        // BindingFlags.NonPublic : �����ڰ� public�� �ƴϿ�����
        // BindingFlags.Instance  : static type�� �ƴϿ�����
        var field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        // ������ id ���� ������ ���� target�� id ���� ���� ������
        // �� �̷� ������ private ������ ���м��� ��Ű�鼭 ���� Setting�� �� �ְ� �ȴ�.
        field.SetValue(target, id);

        // Serialize ����(���⼭�� id ����)�� code�󿡼� �������� ��� EditorUtility.SetDirty�� ���ؼ� Serailize ������ �����Ǿ�����
        // Unity�� �˷�����Ѵ�. �׷��� ������ ������ ���� �ݿ����� �ʰ� ���� ������ ���ư���. 
        // ���⼭�� ���� �����Ǿ��ٰ� Unity�� �˷��ٻ�, ������ ���� ����ɷ��� Editor Code���� ApplyModifiedProperties �Լ� Ȥ��
        // ������Ʈ ��ü�� �����ϴ� AssetDatabase.SaveAssets �Լ��� ȣ��Ǿ�� �Ѵ�.
        // (���⼭�� ���߿� �ٸ� ������ AssetDatabase.SaveAssets�� ȣ�� �� ���̱� ���� �ۼ����� �ʴ´�)
#if UNITY_EDITOR
        EditorUtility.SetDirty(target);
#endif
    }
    #endregion

    // Data List �ȿ� �ִ� IdentifiedObject�� ID�� Index ������ ���� Setting ���ִ� �Լ�
    // �� SetID �Լ��� �Ȱ��� Reflection�� �̿��ؼ� ID ������ ������ �������� for���� ���鼭 data�� ID ���� Index ������ Setting
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

    // ������ �߰� 
    public void Add(IdentifiedObject newData)
    {
        // ������ �߰�
        datas.Add(newData);
        // ID ����
        SetID(newData, datas.Count - 1);
    }

    // ������ ����
    public void Remove(IdentifiedObject data)
    {
        //  ������ ����
        datas.Remove(data);
        // ������
        ReOrderDatas();
    }

    // ID�� �ش� ������ �������� (ID�� �� Index)
    public IdentifiedObject GetDataByID(int id) => datas[id];

    // ���Ǽ� �������� ID�� ã�Ƴ��� ���ÿ� ����ȯ���� ���ִ� Generic �Լ�
    // �� ����ȯ�� IdentifiedObject Ŭ������ ���� GetDataByID�� ������ ��������
    public T GetDataByID<T>(int id) where T : IdentifiedObject => GetDataByID(id) as T;

    // codeName���� �ش� �����͸� �������� (Find �Լ��� deligate ���)
    public IdentifiedObject GetDataCodeName(string codeName) => datas.Find(item => item.CodeName == codeName);

    // ���Ǽ� �������� CodeName�� ã�Ƴ��� ���ÿ� ����ȯ���� ���ִ� Generic �Լ� 
    public T GetDataByCodeName<T>(string codeName) where T : IdentifiedObject => GetDataCodeName(codeName) as T;

    // �ش� �����Ͱ� �����ϴ��� Ž���ϱ� 
    public bool Contains(IdentifiedObject item) => datas.Contains(item);

    // Data�� CodeName�� �������� ������������ ����
    public void SortByCodeName()
    {
        datas.Sort((x, y) => x.CodeName.CompareTo(y.CodeName));

        // CodeName �������� �������� ���ĵ� ���¿� ID ��ο� 
        ReOrderDatas();
    }
}
