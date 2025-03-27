using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuperArmorState : State<PlayerEntity>
{
    public bool isTimeOver => Settings.superArmorDuration <= currentDuration;

    private float currentDuration = 0f;

    public override void Enter()
    {
        Entity.GritShield.SetActive(true);

        Entity.Collider.enabled = false;

        currentDuration = 0f;
    }

    public override void Update()
    {
        currentDuration += Time.deltaTime;
    }

    public override void Exit()
    {
        Entity.GritShield.SetActive(false);
        Entity.Collider.enabled = true;
    }
}
