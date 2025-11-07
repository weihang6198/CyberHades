using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState: State<EnemyController>
{

    [SerializeField] float attackDistance = 1.2f;
    bool isAttacking;
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {

        enemy = owner;
        //change the enemy stopping distance closer to player when atking
        enemy.NavAgent.stoppingDistance = attackDistance;
    }

    public override void Execute()
    {
        if (isAttacking) { return; }//prevent enemy from chasing player
        if(enemy.enemyType==EnemyType.Melee)
        {
            Debug.Log("chasing player");
            enemy.NavAgent.SetDestination(enemy.Target.transform.position); //melee enemy chase player
        }
        

        if (enemy.Fighter.CanAttack(enemy.Target.transform.position))
        {
            Debug.Log("enemy fighter attacking player");
            StartCoroutine(Attack(Random.Range(0, enemy.Fighter.attacks.Count + 1)));
        }


      
    }

    IEnumerator Attack(int comboCount = 1)
    {
       
        isAttacking = true;
        enemy.animator.applyRootMotion = true;
        enemy.Fighter.TryToAttack(enemy.Target);
        for (int i = 1; i < comboCount; i++)
        {
            //combo mechanic, make enemy attack more than 1 times 
            //wait the atk to go cooldown states, then do attack again
            yield return new WaitUntil(() => enemy.Fighter.attackState == AttackStates.Cooldown);
            enemy.Fighter.TryToAttack();
        }
        yield return new WaitUntil(() => enemy.Fighter.attackState == AttackStates.Idle);

        enemy.animator.applyRootMotion = false;
        isAttacking = false;

        if (enemy.enemyType == EnemyType.Melee && enemy.IsInState(EnemyStates.Attack))
            enemy.ChangeState(EnemyStates.RetreatAfterAttack);
        //if (enemy.enemyType == EnemyType.Ranged && enemy.IsInState(EnemyStates.Attack))


    }

    public override void Exit()
    {
        enemy.NavAgent.ResetPath();
    }
}
