using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// 대상 객체가 있다면 QuestReporter을 컴포넌트로 가져야 함!
// trigger가 발생할 때마다 실행되긴 하지만, 없애면 됨.
public class QuestReporter : MonoBehaviour
{
    [SerializeField] private QCategory category;
    [SerializeField] private QTaskTarget target;
    [SerializeField] private int successCount;
    [SerializeField] private string[] colliderTags;

    private void OnTriggerEnter(Collider other)
    {
        ReportIfPassCondition(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReportIfPassCondition(collision);
    }

    public void Report()
    {
        Debug.Log($"QuestReport - Report - {gameObject.name}");
        QuestSystem.Instance.ReceiveReport(category, target, successCount);







    }

    private void ReportIfPassCondition(Component other)
    {
        if (colliderTags.Any(x => other.CompareTag(x)))
            Report();
    }

    private void OnDestroy()
    {
        Report();
    }
}
