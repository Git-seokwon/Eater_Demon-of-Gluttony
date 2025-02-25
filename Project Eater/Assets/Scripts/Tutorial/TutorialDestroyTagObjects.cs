using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 1. 월드에 존재하는 특정 적을 처치한다. 
// 2. 월드에 존재하는 특정 아이템을 획득한다. 
public class TutorialDestroyTagObjects : TutorialBase
{
    [SerializeField]
    private GameObject[]        objectList;

    public override void Enter()
    {
    }

    public override void Execute(TutorialController controller)
    {
        if (objectList.All(obj => !obj.activeSelf)) // 모든 오브젝트가 비활성화되었는지 확인
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
