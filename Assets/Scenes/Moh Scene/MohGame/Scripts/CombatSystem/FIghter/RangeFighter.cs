using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;


public class RangeFighter : FighterBase
{
    
    SpawnProjectiles spawnProjectiles;
   public Vector2 attackRandomTimer = new Vector2(0.5f, 1.2f);
    protected override void Awake()
    {
        base.Awake(); // runs FighterBase.Awake()
        spawnProjectiles = GetComponent<SpawnProjectiles>();
        spawnProjectiles.owner = this;
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
            Debug.Log("inside range fighter TryToAttack");
            StartCoroutine(Attack(target));

        }
        else
        {
            Debug.Log("inside range fighter TryToAttack !action is false");
        }
      
    }
    public override IEnumerator Attack(FighterBase target = null)
    {
        Debug.Log("<color=cyan>[Attack] Start</color>");

        Vector3 originalPos = transform.position;
        attackState = AttackStates.Windup;
        InAction = true;

        animator.applyRootMotion = true;
        animator.speed = 1f;

        Vector3 targetDirection = target.transform.position - transform.position;
       targetDirection.y = 0f;
       targetDirection.Normalize();
       Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        animator.CrossFade(attacks[comboCount].AnimName, 0.2f, 1);
        yield return null;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(1);

      
        float currentAnimSpeed = attacks[comboCount].WindupSpeed;
        bool doOnce = false;

        AttackStates lastState = attackState; // 👈 track state change

        while (attackState != AttackStates.Idle)
        {
            if (attackState != AttackStates.Idle)
            {
                // Smoothly rotate toward the target direction every frame
                transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                50f * Time.deltaTime // adjust rotation speed
            );

                // ---- Log state change only once ----
                if (attackState != lastState)
                {
                    Debug.Log($"<color=yellow>[Attack] State → {attackState}</color>");
                    lastState = attackState;
                }

                if (attackState != AttackStates.Cooldown)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        10f * Time.deltaTime
                    );
                }



                // =========================
                //          WINDUP
                // =========================
                if (attackState == AttackStates.Windup)
                {
                    currentAnimSpeed = attacks[comboCount].WindupSpeed;
                    animator.SetFloat("AttackAnimSpeed", currentAnimSpeed);
                }

                // =========================
                //          IMPACT
                // =========================
                else if (attackState == AttackStates.Impact)
                {
                    if (!doOnce)
                    {
                        doOnce = true;

                        currentAnimSpeed = attacks[comboCount].ImpactSpeed;
                        animator.SetFloat("AttackAnimSpeed", currentAnimSpeed);

                        Debug.Log("<color=red>[Attack] Impact → Spawn VFX</color>");
                        spawnProjectiles.SpawnVFX(targetDirection);
                    }
                }

                // =========================
                //         COOLDOWN
                // =========================
                else if (attackState == AttackStates.Cooldown)
                {
                    currentAnimSpeed = attacks[comboCount].CooldownSpeed;
                    animator.SetFloat("AttackAnimSpeed", currentAnimSpeed);
                }

                yield return null;
            }

            InAction = false;
            Debug.Log("<color=green>[Attack] End → Back to Idle</color>");
        }
    }

    //public override IEnumerator Attack(FighterBase target = null)
    //{
    //    Debug.Log("<color=cyan>[Attack] Enter RangeFighter Attack</color>");
    //    attackState = AttackStates.Windup;

    //    Vector3 targetDirection = target.transform.position - transform.position;
    //    targetDirection.y = 0f;
    //    targetDirection.Normalize();
    //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

    //    animator.CrossFade("attack01", 0.2f, 1);
    //    animator.speed = 1f;
    //    yield return null;

    //    while (attackState != AttackStates.Idle)
    //    {
    //        InAction = true;

    //        if (attackState != AttackStates.Cooldown)
    //        {
    //            transform.rotation = Quaternion.Slerp(
    //                transform.rotation,
    //                targetRotation,
    //                10f * Time.deltaTime
    //            );
    //        }

    //        if (attackState == AttackStates.Windup)
    //        {
    //            Debug.Log("<color=yellow>[Attack] Windup</color>");
    //        }
    //        else if (attackState == AttackStates.Impact)
    //        {
    //            Debug.Log("<color=red>[Attack] Impact → Spawn VFX</color>");
    //            spawnProjectiles.SpawnVFX(targetDirection);
    //        }
    //        else if (attackState == AttackStates.Cooldown)
    //        {
    //            Debug.Log("<color=green>[Attack] Cooldown</color>");
    //        }

    //        yield return null;
    //    }

    //    Debug.Log("<color=orange>[Attack] Finished → Waiting Random Time</color>");
    //    float waitTimer = Random.Range(attackRandomTimer.x, attackRandomTimer.y);
    //    yield return new WaitForSeconds(waitTimer);

    //    InAction = false;
    //    Debug.Log("<color=cyan>[Attack] Back to Idle</color>");
    //}

    public override bool ShouldEndRetreat(float distanceToTarget)
    {
        // Ranged: keep more distance
        return distanceToTarget >= 8f;
    }

    public override void SpawnSlashEffect()
    {
    
    }

}
