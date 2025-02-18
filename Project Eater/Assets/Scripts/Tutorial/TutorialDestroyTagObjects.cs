using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 월드에 존재하는 특정 적을 처치한다. 
// 2. 월드에 존재하는 특정 아이템을 획득한다. 
public class TutorialDestroyTagObjects : TutorialBase
{
    [SerializeField]
    private GameObject[]        objectList;

    public override void Enter()
    {
        // 플레이어의 이동, 공격이 가능하도록 설정
        PlayerController.Instance.enabled = true;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);

        // 파괴해야할 오브젝트들을 활성화
        for (int i = 0; i < objectList.Length; ++i)
        {
            objectList[i].SetActive(true);
        }
    }

    public override void Execute(TutorialController controller)
    {
        if (objectList.Length == 0)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        PlayerController.Instance.enabled = false;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Default);
    }
}
