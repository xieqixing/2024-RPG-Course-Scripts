using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = rb.gravityScale;

        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;

        player.canDash = false;
    }

    public override void Exit()
    {
        base.Exit();

        player.MakeTransparent(false);
        rb.gravityScale = defaultGravity;

        player.canDash = true;  
    }

    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 15);

        if(stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if(!skillUsed)
            {
                if(player.skill.blackhole.CanUseSkill())
                    skillUsed = true;
            }
        }

        // we exit this state after all the attacks are completed in the blackhole skill controller
    }
}
