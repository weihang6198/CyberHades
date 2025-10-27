using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<EnemyControllerTutorial>
{
    public override void Enter(EnemyControllerTutorial owner)
    {
        owner.VisionSensor.gameObject.SetActive(false);
        EnemyManagerTutorial.instance.RemoveEnemyInRange(owner);

        owner.NavAgent.enabled = false;
        owner.CharacterController.enabled = false;
    }
}
