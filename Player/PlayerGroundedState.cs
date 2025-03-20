using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        //if(Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    stateMachine.ChangeState(player.dashState);
        //}

        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.cooldownTimer <= 0 && player.skill.blackhole.blackholeUnlocked) 
            stateMachine.ChangeState(player.blackhole);

        if(Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
            stateMachine.ChangeState(player.aimSword);

        if(Input.GetKey(KeyCode.Q) && player.skill.parry.CanUseSkill())
            stateMachine.ChangeState(player.counterAttack);

        if(Input.GetKey(KeyCode.Mouse0)) 
            stateMachine.ChangeState(player.primaryAttack);

        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }
}
