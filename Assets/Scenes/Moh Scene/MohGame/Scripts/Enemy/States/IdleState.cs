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
