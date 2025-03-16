using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuperArmorState : State<PlayerEntity>
{
    public bool isTimeOver => Settings.superArmorDuration <= currentDuration;

    private float currentDuration = 0f;

    public override void Enter()
    {
        Debug.Log("슈퍼 아머 진입");

        Entity.Collider.enabled = false;

        currentDuration = 0f;
    }

    public override void Update()
    {
        currentDuration += Time.deltaTime;

        Debug.Log("currentDuration : " + currentDuration);
    }

    public override void Exit()
    {
        Debug.Log("슈퍼 아머 해제");
        Entity.Collider.enabled = true;
    }
}
