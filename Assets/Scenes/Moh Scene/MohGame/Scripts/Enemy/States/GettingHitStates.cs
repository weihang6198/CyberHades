using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingHitState : State<EnemyController>
{
    [SerializeField] float stunTime = 1.0f;
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        StopAllCoroutines();
        enemy = owner;
        enemy.Fighter.OnHitComplete += () => StartCoroutine(GotToCombatMovement());

        enemy.GetHitEffect();
    }

    IEnumerator GotToCombatMovement()
    {
        yield return new WaitForSeconds(stunTime);

        if (!enemy.IsInState(EnemyStates.Dead))
        {
            if (!enemy.activateEnemy)
            {
                enemy.ChangeState(EnemyStates.Idle);

            }
            else
            {
                enemy.ChangeState(EnemyStates.CombatMovement);
            }
             
        }

    }
}
