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
        //foreach (var target in enemy.TargetsInRange)
        //{
        //    var vecToTarget = target.transform.position - transform.position;
        //    float angle = Vector3.Angle(transform.forward, vecToTarget);

        //    if (angle <= enemy.Fov / 2)
        //    {
        //        enemy.Target = target;
        //        Debug.Log("enemy.Target :" + enemy.Target);

        //        if (enemy.enemyType == EnemyType.Ranged)
        //        {
        //            Debug.Log(" range enemy spotted");
        //            enemy.ChangeState(EnemyStates.Attack);
        //        }  
        //        else if(enemy.enemyType==EnemyType.Melee)
        //        {
        //            Debug.Log(" going combat movement state");
        //            enemy.ChangeState(EnemyStates.CombatMovement);
        //        }
                  
              
        //        break;
        //    }
        //}

        enemy.Target = enemy.FindTarget();
        if (enemy.Target != null)
        {
            if (enemy.enemyType == EnemyType.Ranged)
            {
                Debug.Log(" range enemy spotted");
                enemy.ChangeState(EnemyStates.Attack);
            }
            else if (enemy.enemyType == EnemyType.Melee)
            {
                Debug.Log(" going combat movement state");
                enemy.ChangeState(EnemyStates.CombatMovement);
            }
  
        }
    }

    
    public override void Exit()
    {
        Debug.Log("enemy enter exiting idle state");
    }
}
