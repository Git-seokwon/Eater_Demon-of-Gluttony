using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyState : State<Skill>
{
    public override void Enter()
    {
        Debug.Log("ReadyState ����");

        if (Layer == 0)
        {
            if (Entity.IsActivated)
                Entity.Deactivate();

            Entity.ResetProperties();
        }
    }
}
