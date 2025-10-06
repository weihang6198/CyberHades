using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttackState
{
    Idle,Windup,Impact,Cooldown
}

public class MeleeFighter : MonoBehaviour
{
    [SerializeField] List<AttackData> attacks;
    [SerializeField] GameObject sword;

    BoxCollider swordCollider;
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    Animator animator;

    //variables
    public AttackState attackState;
    bool doCombo;
    int comboCount = 0;
    public bool InAction { get;private set; } = false;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

  

    private void Start()
    {
        if(sword!=null)
        {
            swordCollider = sword.GetComponent<BoxCollider>();
            leftHandCollider    = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            rightHandCollider   = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            leftFootCollider    = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            rightFootCollider   = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();


            DisableAllHitBox();

        }
    }

    public void TryToAttack()
    {
        //if not atking, perform atk
        if (!InAction) 
        {
            StartCoroutine(Attack());
        }
        else if(attackState == AttackState.Impact ||attackState==AttackState.Cooldown)
        {
            doCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        attackState = AttackState.Windup;



        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        yield return null;//wait for a single frame

        //1 represent override layer
        var animState=animator.GetNextAnimatorStateInfo(1);
        float timer = 0f;

        while(timer<=animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;
            //prepare to attack
            if (attackState == AttackState.Windup)
            {
                //if anim time> impact start time, enable sword collider
                if(normalizedTime > attacks[comboCount].ImpactStartTime)
                {
                    attackState = AttackState.Impact;
                    //enable sword collider
                    EnableHitBox(attacks[comboCount]);
                }
            }
            //disable sword collision after impact time
            else if(attackState == AttackState.Impact)
            {
                //if anim time> impact end time, disable sword collider
                if (normalizedTime > attacks[comboCount].ImpactEndTime)
                {
                    attackState = AttackState.Cooldown;
                    //disable sword collider
                    DisableAllHitBox();
                }   
            }
            //handle combo
            else if(attackState == AttackState.Cooldown)
            {
               
                if(doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;
                    StartCoroutine(Attack());
                    yield break;
                    /*
                     * since the atk still continues, this perevent from executing 
                     * the code below for  
                         attackState = AttackState.Idle;
                         InAction = false;
                     */
                   

                }
            }
                yield return null;
            //yield return new WaitForSeconds(animState.length);// old way
        }

        //reset all attacks
        attackState = AttackState.Idle;
        comboCount = 0;
        InAction = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="HitBox"&& !InAction) //check if has the hitbox tag and not in other action
        {
            Debug.Log("charac was hit");
            StartCoroutine(PlayHitReaction());
        }
    }

    IEnumerator PlayHitReaction()
    {
        InAction = true;
        animator.CrossFade("SwordImpact", 0.2f);
        yield return null;//wait for a single frame

        //1 represent override layer
        var animState = animator.GetNextAnimatorStateInfo(1);

        float animeEndPercentage = 0.6f;

        yield return new WaitForSeconds(animState.length* animeEndPercentage);

        InAction = false;
    }

    void EnableHitBox(AttackData attack)
    {
        switch(attack.HitBoxToUse)
        {
            case AttackHitbox.LeftHand:
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.LeftFoot:
                leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RightFoot:
                rightFootCollider.enabled = true;
                break;
            default:
                swordCollider.enabled = true;
                break;
        }
    }
    void DisableAllHitBox()
    {
        swordCollider.enabled = false;
        leftHandCollider.enabled = false;
        rightHandCollider.enabled = false;
        leftFootCollider.enabled = false;
        rightFootCollider.enabled = false;

    }
}
