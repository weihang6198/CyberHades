using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
public enum AttackStates
{
    Idle,Windup,Impact,Cooldown
}

public class MeleeFighter : MonoBehaviour
{
    [SerializeField]public  List<AttackData> attacks;
   
    [SerializeField] GameObject sword;

    BoxCollider swordCollider;
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    Animator animator;

    //variables
    public AttackStates attackState {  get; private set; }
    bool doCombo;
    int comboCount = 0;
    public bool InAction { get;private set; } = false;
    public bool InCounter { get; set; } = false;

   

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
        else if(attackState == AttackStates.Impact ||attackState==AttackStates.Cooldown)
        {
            doCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        attackState = AttackStates.Windup;



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
            //can be counter at this state
            if (attackState == AttackStates.Windup)
            {
                if (InCounter) break;
                //if anim time> impact start time, enable sword collider
                if(normalizedTime > attacks[comboCount].ImpactStartTime)
                {
                    attackState = AttackStates.Impact;
                    //enable sword collider
                    EnableHitBox(attacks[comboCount]);
                }
            }
            //disable sword collision after impact time
            else if(attackState == AttackStates.Impact)
            {
                //if anim time> impact end time, disable sword collider
                if (normalizedTime > attacks[comboCount].ImpactEndTime)
                {
                    attackState = AttackStates.Cooldown;
                    //disable sword collider
                    DisableAllHitBox();
                }   
            }
            //handle combo
            else if(attackState == AttackStates.Cooldown)
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
        attackState = AttackStates.Idle;
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

    //the function that play hit reaction
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

    //the function that plays counter attack
    public IEnumerator PerformCounterAttack(EnemyController opponent)
    {
        //setup
        InAction = true;
        InCounter = true;
        opponent.MeleeFighter.InCounter = true;
        opponent.ChangeState(EnemyState.Dead);

        //make sure both player and enemy face each other while performing counter atk
        var displacementVector= opponent.transform.position - transform.position;
        displacementVector.y = 0f;
        transform.rotation = Quaternion.LookRotation(displacementVector);
        opponent.transform.rotation = Quaternion.LookRotation(-displacementVector);

        //mamnually set the pos of the player while counter
        var targetPosition=opponent.transform.position - displacementVector.normalized * 1.2f;


        //play animations
        animator.CrossFade("CounterAttack", 0.2f);
        opponent.animator.CrossFade("CounterAttackVictim", 0.2f);
        yield return null;//wait for a single frame

        //1 represent override layer
        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;

        //make the player move to target position while performing counter attack
        //
        while(timer<=animState.length)
        {
            transform.position=Vector3.MoveTowards(transform.position, targetPosition, 5 * Time.deltaTime);
            yield return null;
            timer += Time.deltaTime;
        }
       // float animeEndPercentage = 0.6f;

       // yield return new WaitForSeconds(animState.length * animeEndPercentage);

        InCounter = false;
        opponent.MeleeFighter.InCounter = false;
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
        if(swordCollider != null)       
            swordCollider.enabled = false;
        if(leftHandCollider!=null)       
            leftHandCollider.enabled = false;
        if(rightHandCollider != null)    
            rightHandCollider.enabled = false;
        if(leftFootCollider != null)     
            leftFootCollider.enabled = false;
        if(rightFootCollider != null)   
            rightFootCollider.enabled = false;

    }

    public bool IsCounterable => attackState == AttackStates.Windup && comboCount == 0;
}
