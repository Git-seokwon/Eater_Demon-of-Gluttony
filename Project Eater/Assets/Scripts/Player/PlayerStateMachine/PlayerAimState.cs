using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerAimState : PlayerState
{
    public PlayerAimState(Player player, PlayerStateMachine stateMachine, string animParameterName) : base(player, stateMachine, animParameterName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        AimInput(out player.startSkillDirection, out player.startSkillAngleDegrees, out player.playerAngleDegrees, out player.playerAimDirection);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        PlayerLook(player.playerAimDirection);
    }


    // Aim Input
    private void AimInput(out Vector3 startSkillDirection, out float startSkillAngleDegrees, out float playerAngleDegrees,
                          out AimDirection playerAimDirection)
    {
        // Aim 파라미터 설정 
        // Get mouse world position
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        // Calculate direction vector of mouse cursor from start skill shoot position 
        startSkillDirection = (mouseWorldPosition - player.startSkillShootPosition);

        // Calculate direction vector of mouse cursor from player transform position
        Vector3 playerDirection = (mouseWorldPosition - player.transform.position);

        // Get weapon to cursor angle 
        startSkillAngleDegrees = HelperUtilities.GetAngleFromVector(startSkillDirection);

        // Get player to cursor angle
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        // Set player aim direction 
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
    }

    // Player Look 
    private void PlayerLook(AimDirection aimDirection)
    {
        // Flip Player transform based on player direction
        switch (aimDirection)
        {
            case AimDirection.Right:
                player.spriteRenderer.flipX = false;
                break;

            case AimDirection.Left:
                player.spriteRenderer.flipX = true;
                break;

            default:
                break;
        }
    }
}
