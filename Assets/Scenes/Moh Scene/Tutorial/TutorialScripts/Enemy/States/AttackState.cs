using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    [SerializeField] float attackDistance = 1f;
    bool isAttacking;
    EnemyController enemy;
    public override void Enter(EnemyController owner)
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
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        enemy.animator.applyRootMotion = true;
        enemy.Fighter.TryToAttack();
        yield return new WaitUntil(() => enemy.Fighter.attackState==AttackStates.Idle);

        enemy.animator.applyRootMotion = false;
        isAttacking = false;
    }
}
