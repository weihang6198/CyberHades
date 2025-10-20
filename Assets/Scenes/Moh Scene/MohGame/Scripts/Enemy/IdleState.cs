using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        Debug.Log("enemy enter idle  state");
     
    }

    public override void Execute()
    {
        foreach (var target in enemy.TargetsInRange)
        {
            var vecToTarget = target.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, vecToTarget);

            if (angle <= enemy.Fov / 2)
            {
                enemy.Target = target;
                Debug.Log("enemy.Target :" + enemy.Target);
                enemy.ChangeState(EnemyStates.CombatMovement);
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
            enemy.ChangeState(EnemyStates.CombatMovement);
    }

    
    public override void Exit()
    {
        Debug.Log("enemy enter exiting idle state");
    }
}
