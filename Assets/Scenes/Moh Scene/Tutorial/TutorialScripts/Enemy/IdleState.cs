using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState :State<EnemyController>
{
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        Debug.Log("Enter idle state");
    }

    public override void Execute()
    {
        Debug.Log("execute idle state");

        if(Input.GetKey(KeyCode.T))
        {
            enemy.ChangeState(EnemyState.Chase);   
        }
    }

    public override void Exit()
    {
        Debug.Log("exit idle state");
    }
}
