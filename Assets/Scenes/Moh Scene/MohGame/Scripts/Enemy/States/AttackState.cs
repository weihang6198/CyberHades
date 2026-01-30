using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState: State<EnemyController>
{

    [SerializeField] float attackDistance = 1.2f;
    [SerializeField] public GameObject attackHintVFX;
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
       // Debug.Log("Execute Combat | isAttacking: " + isAttacking);

        if (isAttacking)
        {
            //Debug.Log("Return: already attacking");
            return;
        }

        if (enemy.enemyType == EnemyType.Melee)
        {
            if (enemy.Target)
            {
                Debug.Log("Chasing target: " + enemy.Target.name);
                enemy.NavAgent.SetDestination(enemy.Target.transform.position);
            }
            else
            {
                Debug.Log("No enemy target found");
            }
        }

        if (enemy.Target)
        {
            bool canAttackRange = enemy.Fighter.CanAttack(enemy.Target.transform.position);
            Debug.Log("CanAttack range check: " + canAttackRange);

            if (canAttackRange)
            {
                Debug.Log("enemy.canAttack: " + enemy.canAttack);

                if (enemy.canAttack)
                {
                    int attackIndex = Random.Range(0, enemy.Fighter.attacks.Count);
                    Debug.Log("Start Attack, index: " + attackIndex);

                    StartCoroutine(Attack(attackIndex));
                }
            }
        }
        else
        {
            Debug.Log("Skip attack: Target is null");
        }
    }


    IEnumerator Attack(int comboCount = 1)
    {
       
        isAttacking = true;
        enemy.animator.applyRootMotion = true;
        enemy.Fighter.TryToAttack(enemy.Target);

        GameObject attackHintVFXInstance = Instantiate(attackHintVFX, enemy.animator.GetBoneTransform(HumanBodyBones.Head).position, Quaternion.identity); ;

        ParticleSystem ps = attackHintVFXInstance.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            Destroy(attackHintVFXInstance, ps.main.duration);
        }

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
        {
            enemy.ChangeState(EnemyStates.RetreatAfterAttack);
            Debug.Log("transition from attack state to retreat after attack state");
        }
            
            
        //if (enemy.enemyType == EnemyType.Ranged && enemy.IsInState(EnemyStates.Attack))


    }

    public override void Exit()
    {
        enemy.NavAgent.ResetPath();
    }
}
