using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.dash.CreateCloneOnDashStart();
        //player.skill.clone.CreateClone(player.transform, Vector3.zero);

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        player.SetVelocity(0, 0);

        player.skill.dash.CreateCloneOnDashOver();

        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if(stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
