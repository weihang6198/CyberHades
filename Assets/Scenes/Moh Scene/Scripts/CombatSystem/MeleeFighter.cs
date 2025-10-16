using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStates {Idle,Windup,Impact,Cooldown};
public class MeleeFighter : MonoBehaviour
{
    [SerializeField] List<AttackData> attacks;
    [SerializeField] GameObject sword;

    BoxCollider swordCollider;

    Animator animator;

    public AttackStates attackState;
    bool doCombo;
    int comboCount = 0;
    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        if (!InAction)
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

       // Debug.Log("inside atk function");
        InAction = true;
        attackState = AttackStates.Windup;
        animator.CrossFade(attacks[comboCount].AnimName, 0.2f, 1);
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
        float timer = 0f;
        while (timer <= animState.length)
        {
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
                if(doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
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
}
