using MagicaCloth2;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Windows;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;
using System.IO;
using System.IO.Enumeration;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;



public class MeleeFighter : FighterBase
{



    [SerializeField] GameObject sword;
    [SerializeField] SlashEffect slashEffect;
    [SerializeField] MeshTrailEffect meshTrailEffect;
    [SerializeField] DashEffect dashEffect;
    BoxCollider swordCollider;
    Vector3 AttackDir;
    bool doCombo;
    bool isSlashSpawned = false;
    Transform trans;
    
    public Camera mainCamera;       // assign your main camera in Inspector

 

   // public bool IsCounterable => attackState == AttackStates.Windup && comboCount == 0;
 
    protected override void Awake()
    {
        base.Awake(); // runs FighterBase.Awake() 
    }


    private void Start()
    {
       
        if (sword != null)
        {
            swordCollider=sword.GetComponent<BoxCollider>();
            swordCollider.enabled = false;
        }
    }

    public override bool CanAttack(Vector3 targetPosition,float attackDistance)
    {
       return  Vector3.Distance(targetPosition, transform.position) <= attackDistance + 0.03f;
      
        
    }
    public override void TryToAttack(FighterBase target = null)
    {
        if (!InAction && !isDashing) 
        {
            //Debug.Log("start couroutine atk function");

            StartCoroutine(Attack());

        }
        else if (attackState == AttackStates.Impact || attackState == AttackStates.Cooldown)
        {
            doCombo = true;
        }
    }
   
    public override  IEnumerator Attack(FighterBase target = null)
    {
        
        attackState = AttackStates.Windup;

        
        //default, for enemy
        Vector3 targetDirection = transform.forward;

        //only for player
        if (character.tag=="Player")
        {
            // Capture the mouse direction once at attack start
            targetDirection = GetMouseDirection();
        }
        animator.applyRootMotion=true;
        targetDirection.y = 0f; // keep rotation horizontal
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        animator.CrossFade(attacks[comboCount].AnimName, 0.2f, 1);
        yield return null;


        var animState = animator.GetCurrentAnimatorStateInfo(1);
        float timer = 0f;
        isSlashSpawned = false;
        float currentPhaseAnimSpeed = attacks[comboCount].WindupSpeed;
        while (timer <= animState.length)
        {
            if (isDashing)
                yield break; // exit coroutine immediately if dash started
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

            //modify the delta time based on current animation speed
            timer += Time.deltaTime* currentPhaseAnimSpeed;
            float normalizedTime = timer / animState.length;

            //first phase windup
            if (attackState == AttackStates.Windup)
            {
                animator.speed = currentPhaseAnimSpeed; // slow animationcurrentPhaseSpeed

                if (InCounter) break; //exit if player counter enemy attack
                if (normalizedTime >= attacks[comboCount].ImpactStartTime)
                {
                    currentPhaseAnimSpeed = attacks[comboCount].ImpactSpeed;
                    animator.speed = currentPhaseAnimSpeed; // slow animationcurrentPhaseSpeed
                    attackState = AttackStates.Impact;
                    swordCollider.enabled = true;
                }
            }
            //second phase impact 
            else if (attackState == AttackStates.Impact)
            {
                if (InCounter) break;

                if (normalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    currentPhaseAnimSpeed = attacks[comboCount].CooldownSpeed;
                    animator.speed = currentPhaseAnimSpeed; // slow animationcurrentPhaseSpeed
                    attackState = AttackStates.Cooldown;

                    swordCollider.enabled = false;


                }
                Transform handTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                handTransform.position = animator.GetBoneTransform(HumanBodyBones.Chest).position;
                handTransform.rotation = handTransform.rotation * animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation;
                trans = handTransform;
                
                if (normalizedTime >= attacks[comboCount].SlashSpawnFrame && !isSlashSpawned)
                {
                    //slashEffect.GetCalculatedSlashRotation(animator,);
                    //Spawn Slash VFX
                    if (slashEffect != null)
                    {

                        slashEffect.SpawnEffect(handTransform);
                    }

                    isSlashSpawned = true;
                }

            }
            else if (attackState == AttackStates.Cooldown)
            {
                if (doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
                    yield break;
                }
                
                //if ((character.tag == "Player"))
                //{
                //    Debug.Log("inside stop all courtine for play tag only");
                //    //only for player
                //    if (PlayerInput.move != Vector2.zero)
                //    {
                //        attackState = AttackStates.Idle;
                //        comboCount = 0;
                //        InAction = false;
                //        //cancel the current animation and go back to locomotion
                //       // StopAllCoroutines();
                //        yield break;
                //    }
                //}
             

            }

            yield return null;
        }

        animator.speed = 1;
       attackState = AttackStates.Idle;
        comboCount = 0;
        InAction = false;
        animator.applyRootMotion = false;
    }


    public void TryToDash()
    {
        if (canDash && !takingDamage)
        {

            StartCoroutine(Dash());

        }
    }
    IEnumerator Dash()
    {

        //reset 
        animator.applyRootMotion = true; // Enable root motion
        comboCount = 0;
        attackState = AttackStates.Idle;
        InAction = false;
        isDashing = true;
        canDash = false;

        animator.CrossFade("Dash", 0.2f);
        yield return null; //wait for 1 frame

        var animState = animator.GetCurrentAnimatorStateInfo(1);

        //spawn mesh trail here
        if (meshTrailEffect != null)
        {
            meshTrailEffect.Execute();
        }
        else
        {
            Debug.Log("meshTrailEffect class has null");
        }
        //spawn dash
        if (dashEffect != null)
        {
            dashEffect.Execute();
        }
        else
            Debug.Log("dashEffect class has null");

        yield return new WaitForSeconds(animState.length * dashWaitPercent);

        animator.applyRootMotion = false; //disable root motion
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    Vector3 GetMouseDirection()
    {
        Ray ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f; // keep rotation flat
            return direction.normalized;
        }

        // fallback (if something goes wrong)
        return transform.forward;
    }
    public void RotateTowardMouse()
    {
   
        Vector3 direction= GetMouseDirection();
        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
    }

    public override bool ShouldEndRetreat(float distanceToTarget)
    {
        // Melee: re-engage quickly
        return distanceToTarget >= 3f;
    }

    public IEnumerator PerformCounterAttack(EnemyController opponent)
    {
        //setup
        InAction = true;
        InCounter = true;
        opponent.Fighter.InCounter = true;
        opponent.ChangeState(EnemyStates.Dead);

        //make sure both player and enemy face each other while performing counter atk
        var displacementVector = opponent.transform.position - transform.position;
        displacementVector.y = 0f;
        transform.rotation = Quaternion.LookRotation(displacementVector);
        opponent.transform.rotation = Quaternion.LookRotation(-displacementVector);

        //mamnually set the pos of the player while counter
        var targetPosition = opponent.transform.position - displacementVector.normalized * 1.2f;

        animator.SetLayerWeight(2, 1f);
        opponent.animator.SetLayerWeight(2, 1f);
        //play animations
        animator.CrossFade("CounterAttack2", 0.2f,2);
        opponent.animator.CrossFade("CounterAttackVictim2", 0.2f,2);
        yield return null;//wait for a single frame

        //1 represent override layer
        var animState = animator.GetNextAnimatorStateInfo(2);
     
        float timer = 0f;

        //make the player move to target position while performing counter attack
        
        while (timer <= animState.length)
        {
           // if (isTakingHit) break;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5 * Time.deltaTime);
            yield return null;
            timer += Time.deltaTime;
        }
   

        // yield return new WaitForSeconds(animState.length * animeEndPercentage);

        InCounter = false;
        opponent.Fighter.InCounter = false;
        InAction = false;
    }


}
