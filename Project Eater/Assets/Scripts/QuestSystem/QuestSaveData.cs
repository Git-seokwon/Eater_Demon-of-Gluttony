using System;

[Serializable]
public class QuestSaveData
{
    public string codeName;
    public QuestState state;
    public int taskGroupIndex;
    public int[] taskSuccessCounts;
    public bool isRewardGiven;
}