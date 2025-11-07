using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingHitStateTutorial : State<EnemyControllerTutorial>
{
    [SerializeField] float stunTime = 0.5f;
    EnemyControllerTutorial enemy;
    public override void Enter(EnemyControllerTutorial owner)
    {
        StopAllCoroutines(); 
        enemy = owner;
        enemy.MeleeFighter.OnHitComplete += () => StartCoroutine(GotToCombatMovement());
        
    }

    IEnumerator GotToCombatMovement()
    {
        yield return new WaitForSeconds(stunTime);

        if (!enemy.IsInState(EnemyState.Dead))
        {
            enemy.ChangeState(EnemyState.CombatMovement);
        }
       
    }
}
