using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
         Debug.Log("enter dead state of enemy");
        owner.VisionSensor.gameObject.SetActive(false);
        EnemyManager.instance.RemoveEnemyInRange(owner);

        owner.NavAgent.enabled = false;
        owner.CharacterController.enabled = false;
        owner.Fighter.health = 0;
        owner.DeadEffect();

    }
}
