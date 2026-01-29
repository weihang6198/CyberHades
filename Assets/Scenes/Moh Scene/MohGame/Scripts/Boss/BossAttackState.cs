using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BossAttackType { LaserProjectileAttack, GroundLightingAttack, ProjectileAttack,NormalAttack };
public class BossAttackState : State<EnemyController>
{
    /*
    * this state will decide how boss will choose each attack 
    * BossCombatMovementState -> idle/circling
    * transition->Sp atk 1 (ground lighting)
    *            ->Sp atk 2 (laser beam)
    *            ->normal attack
    *            ->projectile attack
    *            ->teleport
    */



    public int maxRepeat = 2;          // max consecutive repeat
    public float penaltyMultiplier = 0.1f; // reduce weight if repeated too much
  
    [System.Serializable]
    public struct AttackData
    {
        public BossAttackType type;
        public float weight;
        public int repeatCount;
    }

    public List<AttackData> attacks = new List<AttackData>()
    {
        new AttackData{ type = BossAttackType.NormalAttack, weight = 1f, repeatCount = 0 },
        new AttackData{ type = BossAttackType.GroundLightingAttack, weight = 1f, repeatCount = 0 },
        new AttackData{ type = BossAttackType.LaserProjectileAttack, weight = 1f, repeatCount = 0 },
        new AttackData{ type = BossAttackType.ProjectileAttack, weight = 1f, repeatCount = 0 },
    };
    [SerializeField] float attackDistance = 1.2f;
   // [SerializeField] public GameObject attackHintVFX;
    bool isAttacking;
    BossEnemyController enemy;

    BossAttackType bossAttackType;
    public override void Enter(EnemyController owner)
    {

        enemy = (BossEnemyController)owner;
        //change the enemy stopping distance closer to player when atking
        enemy.NavAgent.stoppingDistance = attackDistance;
        bossAttackType= GetNextAttack();
    }

    public override void Execute()
    {
        if (isAttacking) { return; }//prevent enemy from chasing player
       

        StartCoroutine(ExecuteAttack(bossAttackType));
       // StartCoroutine(Attack(Random.Range(0, enemy.Fighter.attacks.Count + 1)));
    }

   
    public override void Exit()
    {
        enemy.NavAgent.ResetPath();
    }

    public BossAttackType GetNextAttack()
    {
        // Apply penalty for repeated attacks
        List<AttackData> currentWeights = attacks.Select(a =>
        {
            var copy = a;
            if (copy.repeatCount >= maxRepeat)
                copy.weight *= penaltyMultiplier;
            return copy;
        }).ToList();

        // Weighted random
        float total = currentWeights.Sum(a => a.weight);
        float rand = Random.value * total;
        BossAttackType selected = BossAttackType.NormalAttack; // fallback
        for (int i = 0; i < currentWeights.Count; i++)
        {
            rand -= currentWeights[i].weight;
            if (rand <= 0f)
            {
                selected = currentWeights[i].type;
                break;
            }
        }

        // Update repeat counts (struct fix)
        for (int i = 0; i < attacks.Count; i++)
        {
            var a = attacks[i];
            if (a.type == selected)
                a.repeatCount++;
            else
                a.repeatCount = 0;
            attacks[i] = a;
        }

        // --- Debug log ---
        Debug.Log($"Boss selected attack: {selected} | Weights: {string.Join(", ", attacks.Select(a => $"{a.type}:{a.weight}"))} | Repeat counts: {string.Join(", ", attacks.Select(a => $"{a.type}:{a.repeatCount}"))}");

        return selected;
    }

    IEnumerator Attack(int comboCount = 1)
    {

        isAttacking = true;
        enemy.animator.applyRootMotion = true;
       // enemy.Fighter.TryToAttack(enemy.Target);
        enemy.Fighter.TryToAttack(enemy.Target);
        //StartCoroutine(enemy.Fighter.Attack(enemy.Target));
        //GameObject attackHintVFXInstance = Instantiate(attackHintVFX, enemy.animator.GetBoneTransform(HumanBodyBones.Head).position, Quaternion.identity); ;

        //ParticleSystem ps = attackHintVFXInstance.GetComponentInChildren<ParticleSystem>();
        //if (ps != null)
        //{
        //    Destroy(attackHintVFXInstance, ps.main.duration);
        //}

        //for (int i = 1; i < comboCount; i++)
        //{
        //    //combo mechanic, make enemy attack more than 1 times 
        //    //wait the atk to go cooldown states, then do attack again
        //    yield return new WaitUntil(() => enemy.Fighter.attackState == AttackStates.Cooldown);
        //    enemy.Fighter.TryToAttack();
        //}
        yield return new WaitUntil(() => enemy.Fighter.attackState == AttackStates.Idle);
        Debug.Log("attack state is idle , atk done");
        enemy.animator.applyRootMotion = false;
        isAttacking = false;

        if (enemy.enemyType == EnemyType.Melee && enemy.IsInState(EnemyStates.Attack))
        {
            enemy.ChangeState(EnemyStates.RetreatAfterAttack);
            Debug.Log("transition from attack state to retreat after attack state");
        }
        else
        {
            enemy.ChangeState(EnemyStates.CombatMovement);
            Debug.Log("transition from attack state to combat movement state");
        }


        //if (enemy.enemyType == EnemyType.Ranged && enemy.IsInState(EnemyStates.Attack))


    }
    IEnumerator ExecuteAttack(BossAttackType attackType)
    {
        Debug.Log("inside execute attack");
        isAttacking = true;
        enemy.animator.applyRootMotion = false;
        //enemy.Fighter.TryToAttack(enemy.Target);
        //StartCoroutine(enemy.Fighter.Attack(enemy.Target));
        BossFighter bossFighter =(BossFighter) enemy.Fighter;
       // attackType = BossAttackType.LaserProjectileAttack;
        attackType = BossAttackType.NormalAttack;
        switch (attackType)
        {
            case BossAttackType.NormalAttack:
                StartCoroutine(bossFighter.Attack(enemy.Target));
                Debug.Log("NormalAttack");
                break;
            case BossAttackType.ProjectileAttack:
                StartCoroutine(bossFighter.ProjectileAttack(enemy.Target));
                Debug.Log("ProjectileAttack");
                break;
            //case BossAttackType.GroundLightingAttack:
            //    StartCoroutine(bossFighter.GroundLightingAttack(enemy.Target));
            //    Debug.Log("GroundLightingAttack");
            //    break;
            case BossAttackType.LaserProjectileAttack:
                StartCoroutine(bossFighter.LaserProjectileAttack(enemy.Target));
                Debug.Log("LaserProjectileAttack");
                break;
            default:
                Debug.Log("Inside default");
                break;
        }

        yield return new WaitUntil(() => enemy.Fighter.attackState == AttackStates.Idle);
        enemy.animator.applyRootMotion = false;
        isAttacking = false;
        enemy.ChangeState(EnemyStates.RetreatAfterAttack);
    }
}
