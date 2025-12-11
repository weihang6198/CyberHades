using StarterAssets;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;


public class BossFighter : FighterBase
{
    //enum are public enum BossAttackType { NormalAttack, GroundLightingAttack, LaserProjectileAttack, ProjectileAttack };
    //ignore normalAttack, start 
    [SerializeField] public List<AttackData> BossAttacks;
    [SerializeField] public SpawnLaserEffectObject spawnLaserEffectObject;

    SpawnProjectiles spawnProjectiles;
    public Vector2 attackRandomTimer = new Vector2(0.5f, 1.2f);
    [SerializeField] List<Transform> TeleportPosition=new List<Transform>();
    public BossEnemyController boss;
    protected override void Awake()
    {
        base.Awake(); // runs FighterBase.Awake()
        spawnProjectiles = GetComponent<SpawnProjectiles>();
        spawnProjectiles.owner = this;
        boss=GetComponent<BossEnemyController>();
        spawnLaserEffectObject=GetComponent<SpawnLaserEffectObject>();
    }

    public override bool CanAttack(Vector3 targetPosition, float attackDistance = 1.5f)
    {
        return true; //can always attack
    }
    public override void TryToAttack(FighterBase target = null)
    {
        if (!InAction)
        {
            //Debug.Log("start couroutine atk function");
            Debug.Log("inside boss fighter TryToAttack");
            StartCoroutine(Attack(target));

        }

    }

    public override IEnumerator Attack(FighterBase target = null)
    {
        Debug.Log("<color=yellow>---- ATTACK START ----</color>");

        if (target == null)
            Debug.LogError("<color=red>[ERROR] Target is NULL in Attack() !!!</color>");
        else
            Debug.Log("<color=green>[OK] Target is NOT null</color>");

        attackState = AttackStates.Windup;
        Debug.Log($"AttackState → {attackState}");



        // Direction
        Quaternion targetRotation = CalculateTargetRotation(target);

        // Play animation
        Debug.Log($"Playing animation: {attacks[0].AnimName}");
        animator.CrossFade(attacks[0].AnimName, 0.2f, 1);
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
        Debug.Log($"Animation length: {animState.length}");

        float timer = 0f;

        while (timer <= animState.length)
        {
            InAction = true;

            // ■■■ Rotation Debug ■■■
            if (attackState != AttackStates.Cooldown)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime
                );
            }
            else
            {
                Debug.Log("<color=cyan>[Cooldown] No rotation</color>");
            }

            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            Debug.Log($"Timer: {timer:F2}, Normalized: {normalizedTime:F2}, State: {attackState}");

            // ■■■ STATE MACHINE ■■■
            if (attackState == AttackStates.Windup)
            {
                if (normalizedTime >= attacks[0].ImpactStartTime)
                {
                    attackState = AttackStates.Impact;
                    Debug.Log("<color=orange>➡ WINDUP → IMPACT</color>");
                }
            }
            else if (attackState == AttackStates.Impact)
            {
                if (normalizedTime >= attacks[0].ImpactEndTime)
                {
                    attackState = AttackStates.Cooldown;
                    Debug.Log("<color=cyan>➡ IMPACT → COOLDOWN</color>");
                }
            }
            else if (attackState == AttackStates.Cooldown)
            {
                // ADD A LOG TO CONFIRM THIS IS EXECUTING
                Debug.Log("<color=cyan>[Cooldown phase running]</color>");

                // If you want to debug cancel issues:
                // Debug.Log($"Input move: {input.move}");
            }

            yield return null;
        }

        Debug.Log("<color=magenta>Animation finished</color>");

        float waitTimer = Random.Range(attackRandomTimer.x, attackRandomTimer.y);
        Debug.Log($"Waiting extra {waitTimer:F2} seconds before Idle");
        yield return new WaitForSeconds(waitTimer);

        attackState = AttackStates.Idle;
        InAction = false;

        Debug.Log("<color=lime>➡ ATTACK COMPLETE → Idle</color>");
        Debug.Log("<color=yellow>---- ATTACK END ----</color>");
    }


    public IEnumerator GroundLightingAttack(FighterBase target)
    {
        attackState = AttackStates.Windup;
        yield return StartCoroutine(Teleport());
        Debug.Log("doing GroundLightingAttack");
        yield return new WaitForSeconds(3f);
        attackState = AttackStates.Idle;
       
    }

    public IEnumerator LaserProjectileAttack(FighterBase target)
    {
        attackState = AttackStates.Windup;
        Debug.Log("LaserProjectileAttack func");
        //teleport first then do laser projectile
        yield return StartCoroutine(Teleport());
        // Direction
        Quaternion targetRotation = CalculateTargetRotation(target);

        int index = (int)BossAttackType.LaserProjectileAttack;
        // Play animation
        // animator.CrossFade(BossAttacks[index].AnimName, 0.2f, 1);
        Debug.Log("BossAttacks[index].AnimName:" + BossAttacks[index].AnimName);
        animator.CrossFade(BossAttacks[index].AnimName, 0.2f, 1);
        
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);


        float timer = 0f;
      
        while (timer <= animState.length)
        {
            InAction = true;

            // ■■■ Rotation Debug ■■■
            if (attackState != AttackStates.Cooldown)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime
                );
            }


            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            // ■■■ STATE MACHINE ■■■
            if (attackState == AttackStates.Windup)
            {
                if (normalizedTime >= BossAttacks[index].ImpactStartTime)
                {
                    attackState = AttackStates.Impact;
                    //emit projectile
                    StartCoroutine(spawnLaserEffectObject.StartBeam());
                    Debug.Log("emit proj");
                }
            }
            else if (attackState == AttackStates.Impact)
            {
                if (normalizedTime >= BossAttacks[index].ImpactEndTime)
                {
                    attackState = AttackStates.Cooldown;
                    //end projectile
                    Debug.Log("end proj");
                }
            }
            else if (attackState == AttackStates.Cooldown)
            {
                //do nothing
            }
            yield return null;
            
        }

        yield return new WaitForSeconds(3f);
        Debug.Log("laser proj atk done");
        attackState = AttackStates.Idle;
    }

    public IEnumerator ProjectileAttack(FighterBase target)
    {
        attackState = AttackStates.Windup;
        yield return StartCoroutine(Teleport());



        Debug.Log("doing ProjectileAttack");
        yield return new WaitForSeconds(3f);
        attackState = AttackStates.Idle;
    }

    public IEnumerator Teleport()
    {
        boss.NavAgent.ResetPath(); 
        if (TeleportPosition == null || TeleportPosition.Count == 0)
        {
            Debug.LogWarning("TeleportPosition list is empty!");
           //yield return null;
        }
        animator.CrossFade("TeleportStart", 0.2f, 1);
        animator.speed = 1f;
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
        Debug.Log("animState.length for teleport start:" + animState.length);

      
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(1).length);
        //teleport
        int index = Random.Range(0, TeleportPosition.Count); // Random int
        transform.position = new Vector3(TeleportPosition[index].position.x, 0, TeleportPosition[index].position.z);

        //teleport end anim
        animator.CrossFade("TeleportEnd", 0.2f, 1);
        Debug.Log("animState.length for teleport end:" + animState.length);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(1).length);
        yield return new WaitForSeconds(1.5f);
        Debug.Log("teleport done");

    }
    public override bool ShouldEndRetreat(float distanceToTarget)
    {
        // Ranged: keep more distance
        return distanceToTarget >= 8f;
    }

    public Quaternion CalculateTargetRotation(FighterBase target)
    {
        // Direction
        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        return targetRotation;
    }

}
