using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestSaveWrap
{
    public string key;
    public List<QuestSaveData> value;
    public QuestSaveWrap(string key, List<QuestSaveData> value)
    {
        this.key = key;
        this.value = value;
    }
}
