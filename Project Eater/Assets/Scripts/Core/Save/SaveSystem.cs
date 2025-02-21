using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// ���̺� �ý���

public class Saves // ������ �� Ŭ������.
{
    public delegate void SavesChangeHandler(SaveWrap save);
    public event SavesChangeHandler OnSavesChanged;

    private List<SaveWrap> saveList = new(); //
    public IReadOnlyList<SaveWrap> SaveList => saveList;
    public bool IsNull
    {
        private set { }
        get
        {
            if (saveList.Count == 0) return false;
            return true;
        }
    }
    public void AddSaves(SaveWrap wrap)
    {
        saveList.Add(wrap);
        OnSavesChanged?.Invoke(wrap);
    }

    public void ChangeSaveData(SaveWrap save)
    {
        saveList.FirstOrDefault(x => x.IsEqual(save)).ChangeValue(save);
        OnSavesChanged?.Invoke(save);
    }

    public void ChangeSaveData(List<string> tags, object value)
    {
         saveList.FirstOrDefault(x => x.IsEqual(tags)).ChangeValue(value);
    }
}

[Serializable]
public class SaveWrap
{
    private List<string> tags;
    private string value; // value�� ����Ǵ� ���� Json ����� string

    public IReadOnlyList<string> Tags => tags;

    public SaveWrap(List<string> tags, object value)
    {
        this.tags = tags;
        if (CheckAbleToJson(value))
            this.value = JsonUtility.ToJson(value);
        else
            this.value = "";
    }

    private bool CheckAbleToJson(object obj)
    {
        string test = JsonUtility.ToJson(obj);
        if (test == "{}")
        {
            Debug.Log(obj.ToString() + ": CheckAbleToJson : Failed To convert");
            return false;
        }
        return true;
    }
    
    // ���忡 ������ value���� json ��Ŀ��� �����Ͽ� ��ȯ
    public T GetValue<T>()
    {
        T temp = JsonUtility.FromJson<T>(value);
        return temp;
    }

    public bool IsEqual(IReadOnlyList<string> tags)
    {
        foreach(var s in tags)
        {
            if(!this.tags.Contains(s))
                return false;
        }
        return true;
    }
    public bool IsEqual(SaveWrap save) => IsEqual(save.Tags);


    public void ChangeValue(string value)
    {
        this.value = JsonUtility.ToJson(value);
    }
    public void ChangeValue(SaveWrap save) => ChangeValue(JsonUtility.FromJson<string>(save.value));

    public void ChangeValue(object value)
    {
        string temp = JsonUtility.ToJson(value);
        if (temp == "{}")
            return;
        this.value = temp;
    }

}

public class SaveSystem : MonoBehaviour
{
    public delegate void SaveChangeHandler(SaveWrap save);
    public delegate void SaveHandler(Saves saves);

    //public event SaveChangeHandler onSaveChanged;
    //public event SaveHandler onSave;

    private bool isSavesChanged = false;
    private static SaveSystem instance;
    private static Saves saveInstance;

    public static SaveSystem Instance => instance;
    public static Saves SaveInstance => saveInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
            Destroy(gameObject); // �ߺ��� SaveSystem ����
    }

    private void Init()
    {
        if (Load())
        {
            isSavesChanged = true;
            return;
        }
        saveInstance = new();
    }

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(saveInstance, true);
        string path = Path.Combine(Application.dataPath, "Save.json");

        // Saves�� SaveWrap�� �̹� ����Ǿ� ���� ��.

        if (jsonData == "{}")
            Debug.Log("save failed");

        File.WriteAllText(path, jsonData);
        Debug.Log("SaveSystem - Save - Executed");
    }

    public bool Load()
    {
        string path = Path.Combine(Application.dataPath, "Save.json"); //Path
        Saves root;

        try
        {
            string jsonData = File.ReadAllText(path);
            root = JsonUtility.FromJson<Saves>(jsonData);
        }
        catch
        {
            Debug.Log("couldn't read data from Save.json");
            root = null;
            return false;
        }

        if(root == null)
        {
            Debug.Log("Success to read File, but cannot convert");
            return false;
        }

        saveInstance = root;
        Debug.Log("SaveSystem - Load - Executed");

        return true;
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
