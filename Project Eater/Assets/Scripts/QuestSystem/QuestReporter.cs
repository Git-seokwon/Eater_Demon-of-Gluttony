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
    public void Report()
    {
        Debug.Log($"QuestReport - Report - {gameObject.name}");
        QuestSystem.Instance.ReceiveReport(category, target, successCount);
    }
}
