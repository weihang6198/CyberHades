using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using UnityEngine.Windows;

public enum AttackStates {Idle,Windup,Impact,Cooldown};

public class MeleeFighter : MonoBehaviour
{

    [SerializeField] List<AttackData> attacks;
    [SerializeField] GameObject sword;

    BoxCollider swordCollider;

    Animator animator;
    Vector3 AttackDir;
    public AttackStates attackState;
    bool doCombo;
    int comboCount = 0;

    private StarterAssetsInputs input;
   

    public Camera mainCamera;       // assign your main camera in Inspector

    public bool isDashing = false;
    private bool canDash = true;
    public float dashWaitPercent = 0.7f;
    public float dashCooldown = 1.5f;

    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        if (sword != null)
        {
            swordCollider=sword.GetComponent<BoxCollider>();
            swordCollider.enabled = false;
        }
    }
    public bool InAction { get; private set; } = false;

    public void TryToAttack()
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

    IEnumerator Attack()
    {
        
        attackState = AttackStates.Windup;

        // Capture the mouse direction once at attack start
        Vector3 targetDirection = GetMouseDirection();
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
                //chara can move after cooldown state
                if (input.move != Vector2.zero)
                {
                    attackState = AttackStates.Idle;
                    comboCount = 0;
                    InAction = false;
                    //cancel the current animation and go back to locomotion
                    
                    yield break;
                }

            }

            yield return null;
        }

        attackState = AttackStates.Idle;
        comboCount = 0;
        InAction = false;
    }

    IEnumerator PlayHitReaction()
    {
        InAction = true;
        animator.CrossFade("SwordImpact", 0.2f);
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
        yield return new WaitForSeconds(animState.length);

        InAction= false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="HitBox" && !InAction)
        {
            Debug.Log("enemy character was hit");
            StartCoroutine( PlayHitReaction());
        }
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
        Vector3 direction= ray.direction;
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            direction = hitInfo.point - transform.position;
            direction.y = 0f; // keep rotation flat
        }
        return direction;
    }
    public void RotateTowardMouse()
    {
   
        Vector3 direction= GetMouseDirection();
        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
    }

    public void TimerCooldown(float value)
    {

    }
}
