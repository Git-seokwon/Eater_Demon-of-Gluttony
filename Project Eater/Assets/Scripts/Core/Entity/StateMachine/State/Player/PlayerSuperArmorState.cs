using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuperArmorState : State<PlayerEntity>
{
    public bool isTimeOver => Settings.superArmorDuration >= currentDuration;

    private float currentDuration = 0f;

    public override void Enter()
    {
        Entity.Collider.enabled = false;
    }

    public override void Update()
    {
        currentDuration += Time.deltaTime;
    }

    public override void Exit()
    {
        Entity.Collider.enabled = true;
        currentDuration = 0f;
    }
}
