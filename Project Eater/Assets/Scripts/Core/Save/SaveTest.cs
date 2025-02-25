using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public int num;
    public string name;
    public Color color;
}


public class SaveTest : MonoBehaviour
{
    public SaveData testData = new();
    public string saveTag = "test";

    // Start is called before the first frame update
    void Start()
    {
        testData.color = Color.white;
        testData.name = "test";
        testData.num = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SaveSystem.Instance.AddSaves("testtag", "testestest");
            SaveSystem.Instance.AddSaves(saveTag, testData);

            SaveSystem.Instance.Save();
        }


        if(Input.GetKeyDown(KeyCode.F6))
        {
            SaveData temp = SaveSystem.Instance.FindSaveData<SaveData>(saveTag);

            Debug.Log("Load Test :" + temp.name);
        }
    }
}
