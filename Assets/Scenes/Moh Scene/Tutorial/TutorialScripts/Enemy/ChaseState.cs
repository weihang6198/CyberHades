using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
        Debug.Log("Enter chase state");
    }

    public override void Execute()
    {
        Debug.Log("execute chase state");
    }

    public override void Exit()
    {
        Debug.Log("exit chase state");
    }
}
