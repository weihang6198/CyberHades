using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMovementState : State<EnemyController>
{
    [SerializeField] float distanceToStop = 3f;
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.NavAgent.stoppingDistance = distanceToStop;

    }

    public override void Execute()
    {
        enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        enemy.animator.SetFloat("Speed",enemy.NavAgent.velocity.magnitude);
        enemy.animator.SetFloat("MotionSpeed", 1);
    }

    public override void Exit()
    {
        Debug.Log("enemy enter exiting combatmovement state");
    }
}
