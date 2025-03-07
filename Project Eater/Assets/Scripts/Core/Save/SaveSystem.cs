using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// ���̺� �ý���

public class Saves // ������ �� Ŭ������.
{
    public delegate void SavesChangeHandler(SaveWrap save);
    public event SavesChangeHandler OnSavesChanged;

    public List<SaveWrap> saveList = new(); //
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
        if (FindSaveData(wrap) != null)
        {
            FindSaveData(wrap).ChangeValue(wrap.value);
            return;
        }
        saveList.Add(wrap);
        OnSavesChanged?.Invoke(wrap);
    }
    public SaveWrap FindSaveData(SaveWrap wrap) => saveList.FirstOrDefault(x => x.IsEqual(wrap));
    public SaveWrap FindSaveData(string tag) => saveList.FirstOrDefault(x => x.IsEqual(tag));
}

[Serializable]
public class SaveWrap
{
    public string tag;
    public string value; // value�� ����Ǵ� ���� Json ����� string

    public SaveWrap(string tag, object value)
    {
        this.tag = tag;
        this.value = JsonUtility.ToJson(value);
        
        if (this.value == "{}")
        {
            Debug.Log(value.ToString() + "/" + value.GetType().ToString() + ": Failed To convert");
        }
    }

    public SaveWrap(string tag, string value)
    {
        this.tag = tag;
        this.value = value;
    }

    // ���忡 ������ value���� json ��Ŀ��� �����Ͽ� ��ȯ
    public T GetValue<T>()
    {
        T data = JsonUtility.FromJson<T>(value);
        return data;
    }

    public bool IsEqual(string tag) => tag == this.tag;
    public bool IsEqual(SaveWrap save) => IsEqual(save.tag);
    public bool ChangeValue(string value)
    {
        this.value = value;
        return true;
    }
    public bool ChangeValue(object value)
    {
        string temp = JsonUtility.ToJson(value);
        if (temp == "{}")
            return false;
        this.value = temp;
        return true;
    }
}

public class SaveSystem : MonoBehaviour
{
    public delegate void SaveHandler();

    public static event SaveHandler OnLoaded;
    public static event SaveHandler OnSave; 

    private static SaveSystem instance;
    private static Saves saveInstance;

    public bool IsSaving { get; private set; }

    public static SaveSystem Instance => instance;

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
            OnLoaded?.Invoke();
            return;
        }
        saveInstance = new();
    }

    // save�� ȣ���ϸ� ���� �ִ� savesInstance�� saves�� JSON�������� ��ȯ�Ǿ� �����.
    public void Save()
    {
        OnSave?.Invoke();
        string jsonData = JsonUtility.ToJson(saveInstance, true); //saveInstance�� JSON���� ��ȯ
        string path = Path.Combine(Application.dataPath, "Save.json"); // ��� ����

        // Saves�� SaveWrap�� �̹� ����Ǿ� ���� ��.

        if (jsonData == "{}") // "{}"�� ��ȯ�Ǿ� ������ JSON���� ��ȯ�� ������ ��
        {
            Debug.Log("save failed, Check saveInstance has data");
        }

        StartCoroutine(WriteToFileAsync(path, jsonData));

        Debug.Log("SaveSystem - Save - Executed");
    }

    private IEnumerator WriteToFileAsync(string path, string jsonData)
    {
        IsSaving = true;
        File.WriteAllText(path, jsonData);
        yield return new WaitForEndOfFrame();
        IsSaving = false;
    }

    // ������ ���۵Ǹ� saveInstance�� Saves�� �ҷ����� �޼���
    public bool Load()
    {
        string path = Path.Combine(Application.dataPath, "Save.json"); //Path
        Saves root;

        try
        {
            string jsonDataPath = File.ReadAllText(path);
            root = JsonUtility.FromJson<Saves>(jsonDataPath);
        }
        catch
        {
            Debug.Log("Couldn't read data from 'Save.json'");
            File.WriteAllText(path, null);
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

    public void AddSaves(SaveWrap wrap) => saveInstance.AddSaves(wrap);
    public void AddSaves(string tag, string value) => AddSaves(new SaveWrap(tag, value));
    public void AddSaves(string tag, object value) => AddSaves(new SaveWrap(tag, value));
    public T FindSaveData<T>(string tag)
    {
        SaveWrap data;
        data = saveInstance.FindSaveData(tag);
        if (data == null)
            return default;
        return data.GetValue<T>();
    }
        

    private void OnApplicationQuit()
    {
        Save();
    }
}
