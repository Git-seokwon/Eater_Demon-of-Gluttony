using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 플레이어가 특정 아이템을 획득
// 2. 플레이어가 특정 위치까지 이동 
public class TutorialTrigger : TutorialBase
{
    [SerializeField]
    private Transform triggerObject;

    public bool isTrigger { set; get; } = false;

    public override void Enter()
    {
        // 플레이어 이동 가능
        PlayerController.Instance.enabled = true;
        // Trigger 오브젝트 활성화
        triggerObject.gameObject.SetActive(true);
    }

    public override void Execute(TutorialController controller)
    {
        // 거리 기준
        if ((triggerObject.position - PlayerController.Instance.transform.position).sqrMagnitude < 0.1f)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // 플레이어 이동 불가능
        PlayerController.Instance.enabled = false;
        // Trigger 오브젝트 비활성화
        triggerObject.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.Equals(triggerObject))
        {
            isTrigger = true;

            collision.gameObject.SetActive(false);
        }
    }
}
