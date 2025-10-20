using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateTutorial :State<EnemyControllerTutorial>
{
    EnemyControllerTutorial enemy;
    public override void Enter(EnemyControllerTutorial owner)
    {
        enemy = owner;
        Debug.Log("Enter idle state");

        enemy.animator.SetBool("CombatMode", false);
    }

    public override void Execute()
    {
        //foreach(var target in enemy.TargetsInRange)
        //{
        //    var vecToTarget=target.transform.position-transform.position;
        //    float angle= Vector3.Angle(transform.forward,vecToTarget);

        //    if(angle<=enemy.Fov/2)
        //    {
        //        enemy.Target = target;
        //        enemy.ChangeState(EnemyState.CombatMovement);
        //        break;
        //    }
        //}
        enemy.Target=enemy.FindTarget();
        if(enemy.Target!=null)
        {
            enemy.AlertNearbyEnemies(); ;
            enemy.ChangeState(EnemyState.CombatMovement);
        }
    }

    public override void Exit()
    {
        Debug.Log("exit idle state");
    }
}
