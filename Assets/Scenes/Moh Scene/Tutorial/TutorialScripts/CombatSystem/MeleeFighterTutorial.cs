using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
public enum AttackStatesTutorial
{
    Idle,Windup,Impact,Cooldown
}

public class MeleeFighterTutorial : MonoBehaviour
{
    [field: SerializeField] public float health { get; private set; } = 25f;

    [SerializeField]public  List<AttackData> attacks;
    [SerializeField]public  List<AttackData> longRangeAttacks;
    [SerializeField] float longRangeAttackThreshold = 3f;

    
    [SerializeField] GameObject sword;

    [SerializeField] float rotationSpeed=500f;

    public bool isTakingHit {  get; private set; }

    public event Action<MeleeFighterTutorial> OnGotHit; 
    public event Action OnHitComplete; 

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

    public void TryToAttack(MeleeFighterTutorial target=null)
    {
        //if not atking, perform atk
        if (!InAction) 
        {
            StartCoroutine(Attack(target));
        }
        else if(attackState == AttackStates.Impact ||attackState==AttackStates.Cooldown)
        {
            doCombo = true;
        }
    }

    
    IEnumerator Attack(MeleeFighterTutorial target=null)
    {
        InAction = true;
        
        attackState = AttackStates.Windup;

        var attack = attacks[comboCount];

        var attackDir = transform.forward;
        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.zero;

        //long range attack
        if (target != null)
        {
            var vecToTarget = target.transform.position - transform.position;
            vecToTarget.y = 0;
            attackDir = vecToTarget.normalized;
            float distance = vecToTarget.magnitude -  attack.DistanceFromTarget;

            if (distance>longRangeAttackThreshold && longRangeAttacks.Count>0)
            {
                attack = longRangeAttacks[0];
            }

            if(attack.MoveToTarget)
            {
                if (distance < attack.MaxMoveDistance)
                {
                    targetPos = target.transform.position - attackDir * attack.DistanceFromTarget;
                }
                else
                {
                    targetPos = startPos + attackDir * attack.MaxMoveDistance;
                }
            }
          
          
        }

        animator.CrossFade(attack.AnimName, 0.2f);
        yield return null;//wait for a single frame

        //1 represent override layer
        var animState=animator.GetNextAnimatorStateInfo(1);
        float timer = 0f;

        while(timer<=animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            //move the attacker towards the target while performing attack
            if (target != null && attack.MoveToTarget)
            {
                float percentageTime = (normalizedTime - attack.MoveStartTime) / (attack.MoveEndTime - attack.MoveStartTime);
              transform.position=  Vector3.Lerp(startPos , targetPos, percentageTime);
            }

            //rotate to the attacking dir
            if(attackDir!=null)
            {
               transform.rotation= Quaternion.RotateTowards(transform.rotation, 
                   Quaternion.LookRotation(attackDir),rotationSpeed*Time.deltaTime);
            }
            //prepare to attack
            //can be counter at this state
            if (attackState == AttackStates.Windup)
            {
                if (InCounter) break;
                //if anim time> impact start time, enable sword collider
                if(normalizedTime > attack.ImpactStartTime)
                {
                    attackState = AttackStates.Impact;
                    //enable sword collider
                    EnableHitBox(attack);
                }
            }
            //disable sword collision after impact time
            else if(attackState == AttackStates.Impact)
            {
                //if anim time> impact end time, disable sword collider
                if (normalizedTime > attack.ImpactEndTime)
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
        if(other.tag=="HitBox"&& !isTakingHit &&!InCounter) //check if has the hitbox tag and not in other action
        {
            var attacker = other.GetComponentInParent<MeleeFighterTutorial>();
            Debug.Log("charac was hit");
            TakeDamage(5f);
            OnGotHit?.Invoke(attacker);
            if (health > 0)
                //StartCoroutine(PlayHitReaction(other.GetComponentInParent<MeleeFighter>().transform));
                StartCoroutine(PlayHitReaction(attacker));
            else
                PlayDeathAnimation(attacker);

        }
    }

    void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
    }
    //the function that play hit reaction
    IEnumerator PlayHitReaction(MeleeFighterTutorial attacker)
    {
        InAction = true;

        //make character  face the attacker when ebing attacked
        var dispalcementVector = attacker.transform.position - transform.position;
        dispalcementVector.y = 0;
        transform.rotation = Quaternion.LookRotation(dispalcementVector);

        //this is delegate func
        //all attached func will be called
        //this is for enemy
        //OnGotHit?.Invoke();

        animator.CrossFade("SwordImpact", 0.2f);
        yield return null;//wait for a single frame

        //1 represent override layer
        var animState = animator.GetNextAnimatorStateInfo(1);

        float animeEndPercentage = 0.6f;

        yield return new WaitForSeconds(animState.length* animeEndPercentage);

        OnHitComplete?.Invoke(); 
        InAction = false;
        isTakingHit = false;
    }

    void PlayDeathAnimation(MeleeFighterTutorial fighter)
    {
        Debug.Log("plying death anim");
        animator.CrossFade("Death", 0.2f);
    }

    //the function that plays counter attack
    public IEnumerator PerformCounterAttack(EnemyControllerTutorial opponent)
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
            if (isTakingHit) break;
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
