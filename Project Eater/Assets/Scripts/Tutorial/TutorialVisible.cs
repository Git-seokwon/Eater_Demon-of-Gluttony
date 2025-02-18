using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VisibleObject
{
    public GameObject visibleObject;
    public bool visible;
}

public class TutorialVisible : TutorialBase
{
    [SerializeField]
    private VisibleObject[] objects;

    public override void Enter()
    {
        for (int i = 0; i < objects.Length; ++i)
        {
            objects[i].visibleObject.SetActive(objects[i].visible);
        }
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
    }
}
