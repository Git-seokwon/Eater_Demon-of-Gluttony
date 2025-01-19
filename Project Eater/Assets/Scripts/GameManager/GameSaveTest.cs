using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;

[Serializable]
public class HashWrap
{
    public List<string> targetToList;

    public HashWrap(HashSet<string> target)
    {
        targetToList = new List<string>(target);
    }

    public HashSet<string> ToHashSet()
    {
        return new HashSet<string>(targetToList);
    }
}

public class GameSaveTest : MonoBehaviour
{
    HashSet<string> target;

    public void SetSaveDataPrefs()
    {
        //gameManager = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();

        HashWrap hashWrap = new HashWrap(target);


        string temp = JsonUtility.ToJson(hashWrap);
        if (temp == "{}")
            Debug.Log("hash´Â ¾ÈµÇ³ªºÁ");
        string path = Path.Combine(Application.dataPath, "tempData.json");
        
        System.IO.File.WriteAllText(path, temp);
    }


    public void TestSave()
    {
        target = new HashSet<string>();

        target.Add("Something");
        target.Add("Test");
    }

    private void Start()
    {
        TestSave();
        SetSaveDataPrefs();
    }
}
