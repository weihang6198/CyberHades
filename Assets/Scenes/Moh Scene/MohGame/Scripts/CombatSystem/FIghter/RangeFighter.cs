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

    }

    public override bool CanAttack(Vector3 targetPosition, float attackDistance = 1.5f)
    {
        return true; //can always attack
    }
     public override void TryToAttack(FighterBase target = null)
    {
        if (!InAction)
        {
          
            StartCoroutine(Attack(target));

        }
      
    }

    public override IEnumerator Attack(FighterBase target = null)
    {
       
        attackState = AttackStates.Windup;



        //get direction to player from current enemy

        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0f; // keep horizontal
        targetDirection.Normalize(); // ✅ normalize after
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        animator.CrossFade(attacks[0].AnimName, 0.2f, 1);
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
        float timer = 0f;

        while (timer <= animState.length)
        {
           
            InAction = true;
            if (attackState != AttackStates.Cooldown)
            {
                // Smoothly rotate toward the target direction every frame
                transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime // adjust rotation speed
            );

            }

            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (attackState == AttackStates.Windup)
            {
                if (normalizedTime >= attacks[0].ImpactStartTime)
                {
                    attackState = AttackStates.Impact;
                    //swordCollider.enabled = true;


                }
            }
            else if (attackState == AttackStates.Impact)
            {
                if (normalizedTime >= attacks[0].ImpactEndTime)
                {
                    attackState = AttackStates.Cooldown;
                    //swordCollider.enabled = false;


                    //slashEffect.GetCalculatedSlashRotation(animator,);
                    //Spawn projectile VFX
                    spawnProjectiles.SpawnVFX(targetDirection,this);

                }
            }
            else if (attackState == AttackStates.Cooldown)
            {
               
                //has bug
                //cannot force cancel animation directly
                ////chara can move after cooldown state
                //if (input.move != Vector2.zero)
                //{
                //    attackState = AttackStates.Idle;
                //    comboCount = 0;
                //    InAction = false;
                //    //cancel the current animation and go back to locomotion

                //    yield break;
                //}

            }

            yield return null;
        }
        float waitTimer = Random.Range(attackRandomTimer.x, attackRandomTimer.y);
        yield return new WaitForSeconds(waitTimer);
        attackState = AttackStates.Idle;
      
        InAction = false;

    }

    public override bool ShouldEndRetreat(float distanceToTarget)
    {
        // Ranged: keep more distance
        return distanceToTarget >= 8f;
    }

}
