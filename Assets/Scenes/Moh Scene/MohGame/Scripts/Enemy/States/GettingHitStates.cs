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
        MeleeFighter fighter = enemy.Fighter as MeleeFighter;
        
        if (fighter != null)
        {
            // safe to use fighter
            fighter.ResetAttackParam();
        }
       // enemy.GetHitEffect();

        if (enemy.Fighter.consecutiveHitsTaken > enemy.Fighter.maxConsecutiveHitsAllowed)
        {
            enemy.ChangeState(EnemyStates.Attack);
            Debug.Log("change to attack state from getting hit state");
        }
        else
        {
            enemy.Fighter.OnHitComplete += () => StartCoroutine(GotToCombatMovement());
        }
          

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
