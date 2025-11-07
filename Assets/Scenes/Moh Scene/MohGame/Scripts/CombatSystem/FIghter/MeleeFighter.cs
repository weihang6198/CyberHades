using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using UnityEngine.Windows;



public class MeleeFighter : FighterBase
{

   
    
    [SerializeField] GameObject sword;
    [SerializeField] SlashEffect slashEffect;
    BoxCollider swordCollider;
    Vector3 AttackDir;
    bool doCombo;
    int comboCount = 0;
    public Camera mainCamera;       // assign your main camera in Inspector

    public bool isDashing = false;
    private bool canDash = true;
    public float dashWaitPercent = 0.7f;
    public float dashCooldown = 1.5f;


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
       
        targetDirection.y = 0f; // keep rotation horizontal
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        animator.CrossFade(attacks[comboCount].AnimName, 0.2f, 1);
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
        float timer = 0f;

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

            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (attackState == AttackStates.Windup)
            {
                if (normalizedTime >= attacks[comboCount].ImpactStartTime)
                {
                    attackState = AttackStates.Impact;
                    swordCollider.enabled = true;

       
                }
            }
            else if (attackState == AttackStates.Impact)
            {
                if (normalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    attackState = AttackStates.Cooldown;
                    swordCollider.enabled = false;


                    //slashEffect.GetCalculatedSlashRotation(animator,);
                    //Spawn Slash VFX
                    if (slashEffect != null)
                    {
                        Transform handTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
                        handTransform.position = animator.GetBoneTransform(HumanBodyBones.Chest).position;


                        slashEffect.SpawnEffect(handTransform);
                    }
                       
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

        attackState = AttackStates.Idle;
        comboCount = 0;
        InAction = false;
    }


    public void TryToDash()
    {
        if (canDash)
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
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
       
        yield return new WaitForSeconds(animState.length * dashWaitPercent);

        animator.applyRootMotion = false; // Enable root motion
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

}
