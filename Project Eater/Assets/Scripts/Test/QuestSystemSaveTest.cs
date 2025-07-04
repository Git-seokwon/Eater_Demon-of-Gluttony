using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystemSaveTest : MonoBehaviour
{
    [SerializeField]
    private Quest quest;
    [SerializeField]
    private QCategory category;
    [SerializeField]
    private QTaskTarget target;
    [SerializeField]
    private QTaskTarget eliteTarget;

    void Start()
    {
        var questSystem = QuestSystem.Instance;

        if (questSystem.ActiveAchievements.Count == 0)
        {
            var newQuest = questSystem.Register(quest);
            Debug.Log($"Register, {questSystem.ActiveQuests.Count}");
        }
        else
        {
            questSystem.onAchievementCompleted += (quest) =>
            {
                Debug.Log("Complete");
            };
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("SaveTest - Space Key Down");
            QuestSystem.Instance.ReceiveReport(category, target, 300);
            QuestSystem.Instance.ReceiveReport(category, eliteTarget, 300);
        }
    }
}
