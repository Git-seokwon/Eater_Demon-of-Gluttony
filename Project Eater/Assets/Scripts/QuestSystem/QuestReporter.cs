using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// ��� ��ü�� �ִٸ� QuestReporter�� ������Ʈ�� ������ ��!
// trigger�� �߻��� ������ ����Ǳ� ������, ���ָ� ��.
public class QuestReporter : MonoBehaviour
{
    [SerializeField] private QCategory category;
    [SerializeField] private QTaskTarget target;
    [SerializeField] private int successCount;
    [SerializeField] private string[] colliderTags;
    public void Report()
    {
        Debug.Log($"QuestReport - Report - {gameObject.name}");
        QuestSystem.Instance.ReceiveReport(category, target, successCount);
    }
}
