using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateTutorial : State<EnemyControllerTutorial>
{
    [SerializeField] float attackDistance = 1.2f;
    bool isAttacking;
    EnemyControllerTutorial enemy;
    public override void Enter(EnemyControllerTutorial owner)
    {
        
        enemy= owner;
        //change the enemy stopping distance closer to player when atking
        enemy.NavAgent.stoppingDistance = attackDistance;
    }

    public override void Execute()
    {
        if (isAttacking) { return; }//prevent enemy from chasing player
        enemy.NavAgent.SetDestination(enemy.Target.transform.position);

        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= attackDistance +0.03f)
        {
            StartCoroutine(Attack(Random.Range(0,enemy.MeleeFighter.attacks.Count+1)));
        }
    }

    //num of attack based on  @param comboCount
    IEnumerator Attack(int comboCount=1)
    {
        isAttacking = true;
        enemy.animator.applyRootMotion = true;
        enemy.MeleeFighter.TryToAttack();
        for (int i = 1; i < comboCount; i++)
        {
            //combo mechanic, make enemy attack more than 1 times 
            //wait the atk to go cooldown states, then do attack again
            yield return new WaitUntil(() => enemy.MeleeFighter.attackState == AttackStates.Cooldown);
            enemy.MeleeFighter.TryToAttack();
        }
        yield return new WaitUntil(() => enemy.MeleeFighter.attackState == AttackStates.Idle);

        enemy.animator.applyRootMotion = false;
        isAttacking = false;

        if(enemy.IsInState(EnemyState.Attack))
            enemy.ChangeState(EnemyState.RetreatAfterAttack);
    }

    public override void Exit()
    {
      enemy.NavAgent.ResetPath(); 
    }
}
