using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMovementState : State<EnemyController>
{

    public override void Enter(EnemyController owner)
    {
        Debug.Log("enemy enter combatmovement state");
    }

    public override void Execute()
    {
        Debug.Log("enemy enter executing combatmovement state");
    }

    public override void Exit()
    {
        Debug.Log("enemy enter exiting combatmovement state");
    }
}
