using System.Collections;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;
using System.Linq;
using UnityEngine;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;



public class MeleeFighter : FighterBase
{



    [SerializeField] GameObject sword;
    [SerializeField] SlashEffect slashEffect;
    [SerializeField] MeshTrailEffect meshTrailEffect;
    [SerializeField] DashEffect dashEffect;
    [SerializeField] float dashDistance = 4f;
    [SerializeField] Vector2 dashTime = new Vector2(10f, 31f);
    [SerializeField] float attackAsssitMaxAngle = 45;
    [SerializeField] float attackAsssitMaxDistance =10;
    BoxCollider swordCollider;
    [SerializeField] BoxCollider leftHand;
     [SerializeField] BoxCollider rightHand;

    [SerializeField] private AudioClip[] whooshSoundClips;
    [SerializeField] private AudioClip dashSoundClips;


    bool showDebugSphere = false;
    Vector3 debugPos;
    Vector3 AttackDir;
    bool doCombo;
    bool isSlashSpawned = false;
    Transform trans;
    
    public Camera mainCamera;       // assign your main camera in Inspector
    

    public bool canBlock = true;
    public bool isBlocking = false;
 

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
            if(swordCollider )
            {
               
              //  Debug.Log("owner : "+ gameObject.name +"sword collider exist");
            }
        }
    }

    //public override bool CanAttack(Vector3 targetPosition,float attackDistance)
    //{
    //   return  Vector3.Distance(targetPosition, transform.position) <= attackDistance + 0.03f;


    //}

    public override bool CanAttack(Vector3 targetPosition, float attackDistance=1.9f)
    {
        attackDistance = 1.9f;
        Vector3 selfPos = transform.position;
        Vector3 targetPos = targetPosition;

        // Ignore height difference
        selfPos.y = 0f;
        targetPos.y = 0f;

        float dist = Vector3.Distance(selfPos, targetPos);

        //Debug.Log("CanAttack | dist: " + dist + " | attackDistance: " + attackDistance);

        return dist <= attackDistance;
    }
    public override void TryToAttack(FighterBase target = null)
    {
        if (!InAction && !isDashing) 
        {
            //Debug.Log("start couroutine atk function");

            StartCoroutine(Attack());
                
        }
        //else if (attackState == AttackStates.Impact || attackState == AttackStates.Cooldown)
        else if (attackState == AttackStates.Cooldown )
        {
            Debug.Log("attackState == AttackStates.Cooldown is correct doing combo");
            doCombo = true;
        }
    }

    public override IEnumerator Attack(FighterBase target = null)
    {
        Vector3 originalPos = transform.position;
        attackState = AttackStates.Windup;
        InAction = true;

        animator.applyRootMotion = true;
        animator.speed = 1f;

        // ===== Target Direction (captured once) =====
        Vector3 targetDirection = transform.forward;
        targetDirection.y = 0f;

        if (character.CompareTag("Player"))
            targetDirection = CalculatePlayerTargetRotation();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // ===== Play animation =====
        animator.CrossFade(attacks[comboCount].AnimName, 0.2f, 1);
        yield return null;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(1);

        float timer = 0f;
        float currentAnimSpeed = attacks[comboCount].WindupSpeed;
        isSlashSpawned = false;

        // ===== Main Loop =====
        while (attackState != AttackStates.Idle)
        {
            //transform.position= originalPos; //fix the player in current position
           // Debug.Log("[AttackState] " + attackState);

            // ---- Forced exits ----
            if (isDashing || InCounter)
                break;

            // ---- Rotate every frame ----
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                50f * Time.deltaTime
            );

            // ---- Timing ----
            timer += Time.deltaTime * currentAnimSpeed;
            float normalizedTime = timer / animState.length;

            // =========================
            //          WINDUP
            // =========================
            if (attackState == AttackStates.Windup)
            {
                showDebugSphere = true;
                debugPos = transform.position;
                currentAnimSpeed = attacks[comboCount].WindupSpeed;

                animator.SetFloat("AttackAnimSpeed", currentAnimSpeed);
              

                currentAnimSpeed = attacks[comboCount].ImpactSpeed;
               
            }

            // =========================
            //          IMPACT
            // =========================
            else if (attackState == AttackStates.Impact)
            {
                currentAnimSpeed = attacks[comboCount].ImpactSpeed;

                animator.SetFloat("AttackAnimSpeed", currentAnimSpeed);
                showDebugSphere = false;

                SetDamageColliderEnabled(attacks[comboCount], true);

                if (character.CompareTag("Player"))
                    cameraShake.ShakeByDuration(cameraShakeDuration, cameraShakeStrength); 

                // ---- Slash VFX ----
                if (!isSlashSpawned && slashEffect != null)
                {
                    SpawnSlashEffect();
                  
                SoundFXManager.instance.PlayRandomSoundFXClip(whooshSoundClips, transform, 0.6f, new Vector2(0.9f, 1.1f));

                }
                // attackState = AttackStates.Cooldown;
            }

            // =========================
            //         COOLDOWN
            // =========================
            else if (attackState == AttackStates.Cooldown)
            {
               
                showDebugSphere = false;

                currentAnimSpeed = attacks[comboCount].CooldownSpeed;
                SetDamageColliderEnabled(attacks[comboCount], false); 
                animator.SetFloat("AttackAnimSpeed", currentAnimSpeed);
                if (doCombo)
                {
                  //  Debug.Log("inside do combo true after attack states .cooldown");
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
                    yield break;
                }
            }
         


            yield return null;
        }

        // ===== Cleanup =====
        if(consecutiveHitsTaken>maxConsecutiveHitsAllowed) 
        {
            consecutiveHitsTaken = 0;
        }
        ResetAttackParam();
      
    }



    public void TryToDash()
    {
        if (canDash && !takingDamage)
        {

            StartCoroutine(Dash());

        }
    }
    //IEnumerator Dash()
    //{

    //    //reset 
    //    animator.applyRootMotion = true; // Enable root motion
    //    comboCount = 0;
    //    attackState = AttackStates.Idle;
    //    InAction = false;
    //    isDashing = true;
    //    canDash = false;

    //    animator.CrossFade("Dash", 0.2f);
    //    yield return null; //wait for 1 frame

    //    var animState = animator.GetCurrentAnimatorStateInfo(1);

    //    //spawn mesh trail here
    //    if (meshTrailEffect != null)
    //    {
    //        meshTrailEffect.Execute();
    //    }
    //    else
    //    {
    //        Debug.Log("meshTrailEffect class has null");
    //    }
    //    //spawn dash
    //    if (dashEffect != null)
    //    {
    //        dashEffect.Execute();
    //    }
    //    else
    //        Debug.Log("dashEffect class has null");

    //    yield return new WaitForSeconds(animState.length * dashWaitPercent);

    //    animator.applyRootMotion = false; //disable root motion
    //    isDashing = false;

    //    yield return new WaitForSeconds(dashCooldown);
    //    canDash = true;
    //}
    IEnumerator Dash()
    {
        animator.applyRootMotion = true;
        comboCount = 0;
        attackState = AttackStates.Idle;
        InAction = false;
        isDashing = true;
        canDash = false;

        AnimationClip dashClip = animator.runtimeAnimatorController.animationClips
            .First(c => c.name == "DashFront");

        float totalFrames = dashClip.length * dashClip.frameRate;
        float startNorm = dashTime.x / totalFrames;
        float endNorm = dashTime.y/ totalFrames;

        animator.Play("Dash", 1, startNorm);

        yield return null;

        if (meshTrailEffect != null) meshTrailEffect.Execute();
        if (dashEffect != null)
        {
            dashEffect.Execute(
            animator.GetBoneTransform(HumanBodyBones.Chest).position,
            new Vector3(0, 0, 0.3f)
        );
            SoundFXManager.instance.PlaySoundFXClip(dashSoundClips,transform,0.6f,new Vector2(0.7f,0.9f));
        }

        // 👉 DASH MOVEMENT OCCURS DURING THE ACTIVE FRAMES
        yield return StartCoroutine(MoveCharacter(
            null,        // no target, use forward
            dashDistance,          // dash distance
            startNorm,   // start movement at animation frame 10
            endNorm      // stop at frame 29
        )); 

        // 👉 MoveCharacter is finished here. Now we freeze at frame 32.

        // Wait until frame 32
        while (true)
        {
            var state = animator.GetCurrentAnimatorStateInfo(1);

            if (state.normalizedTime >= endNorm)
            {
                animator.speed = 0f;
                break;
            }
            yield return null;
        }

        animator.applyRootMotion = false;
        isDashing = false;
        animator.speed = 1f;

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

    public void TryToBlock()
    {
        if (canBlock && !takingDamage)
        {

            StartCoroutine(Block());

        }
    }

    IEnumerator Block()
    {

        //reset 
       
        comboCount = 0;
      
        InAction = true;
        canBlock = false;
        isBlocking = true;

        animator.SetLayerWeight(3, 1f);
        animator.CrossFade("BlockStart", 0.2f);
        yield return null; //wait for 1 frame

        var animState = animator.GetCurrentAnimatorStateInfo(4);



        yield return new WaitForSeconds(animState.length );
        animator.CrossFade("BlockLoop", 0.2f);

        yield return new WaitWhile(() => isBlocking);
        animator.CrossFade("BlockEnd", 0.15f);

        yield return null;
        animState = animator.GetCurrentAnimatorStateInfo(4);
        yield return new WaitForSeconds(animState.length);
        canBlock = true;
        isBlocking = false;
        InAction = false;
        Debug.Log("block anim done");

        //yield return new WaitForSeconds(dashCooldown);
        //canDash = true;


    }

    public void TryToEndBlock()
    {
       
            StartCoroutine(EndBlock());

        
    }


    IEnumerator EndBlock()
    {

        //reset 

        comboCount = 0;

        InAction = true;
        canBlock = false;
        isBlocking = true;

        animator.SetLayerWeight(3, 1f);
        animator.CrossFade("BlockEnd", 0.2f);
        yield return null; //wait for 1 frame

        var animState = animator.GetCurrentAnimatorStateInfo(4);



        yield return new WaitForSeconds(animState.length);
       
        canBlock = true;
        isBlocking = false;
        InAction = false;
        Debug.Log("block anim done");

        //yield return new WaitForSeconds(dashCooldown);
        //canDash = true;


    }

    private void OnDrawGizmos()
    {
        if (showDebugSphere)
        {
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(debugPos, 1f);
        }
    }


    private Vector3 CalculatePlayerTargetRotation()
    {

        Vector3 targetDirection = GetMouseDirection();
        targetDirection.y = 0f;
        //check if any enemy
        var enemy = EnemyManager.instance.GetClosestEnemyForwardDir(targetDirection, attackAsssitMaxAngle, attackAsssitMaxDistance);
        if (enemy != null)
        {
            targetDirection = enemy.transform.position - transform.position;
            targetDirection.y = 0f;
        }
        Debug.Log("targetDirection in CalculatePlayerTargetRotation:" + targetDirection);
        return targetDirection.normalized;
    }

    private void SetDamageColliderEnabled(AttackData attack,bool enabled)
    {
        //{ LeftHand,RightHand ,LeftFoot, RightFoot,Sword};
        switch (attack.HitBoxToUse)
        {
            case AttackHitbox.Sword:
                swordCollider.enabled = enabled;
               // Debug.Log($"<color=cyan>swordCollider hitbox: {enabled}</color>");
                break;
            case AttackHitbox.LeftHand:
                leftHand.enabled = enabled;
                break;
            case AttackHitbox.RightHand:
                rightHand.enabled  = enabled;
                break;
            case AttackHitbox.LeftFoot:
                break;
            case AttackHitbox.RightFoot:
                break;
           
           
        }
    }

    public void ResetAttackParam()
    {
        animator.speed = 1f;
        animator.applyRootMotion = false;
        SetDamageColliderEnabled(attacks[comboCount], false);
        attackState = AttackStates.Idle;
        comboCount = 0;
        InAction = false;
    }

    public override void SpawnSlashEffect()
    {
        Transform chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        chest.rotation = sword.transform.rotation;

        slashEffect.SpawnSlashEffect(chest);
        isSlashSpawned = true;
    }

}


/*
 * 
 * 
 *   public override  IEnumerator Attack(FighterBase target = null)
    {
        
        attackState = AttackStates.Windup;
        animator.applyRootMotion = true;

        //default, for enemy
        Vector3 targetDirection = transform.forward;
        targetDirection.y = 0f; // keep rotation horizontal

        //only for player
        if (character.tag=="Player")
        {
            // Capture the mouse direction once at attack start
            //targetDirection = GetMouseDirection();
            targetDirection = CalculatePlayerTargetRotation();
        }
        
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        animator.CrossFade(attacks[comboCount].AnimName, 0.2f, 1);
        yield return null;


        var animState = animator.GetCurrentAnimatorStateInfo(1);
        float timer = 0f;
        isSlashSpawned = false;
        float currentPhaseAnimSpeed = attacks[comboCount].WindupSpeed;
        //while (timer <= animState.length)
        while (attackState != AttackStates.Idle)
        {
            Debug.Log("[current attack state ]is:" + attackState);
            if (isDashing)
                yield break; // exit coroutine immediately if dash started
            InAction = true;

            //rotate the attacker towards the target
            if (attackState != AttackStates.Idle)
            {
                // Smoothly rotate toward the target direction every frame
                transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                50f * Time.deltaTime // adjust rotation speed
            );

            }

            //modify the delta time based on current animation speed
            timer += Time.deltaTime* currentPhaseAnimSpeed;
            float normalizedTime = timer / animState.length;

            //first phase windup
            if (attackState == AttackStates.Windup)
            {
               // animator.speed = currentPhaseAnimSpeed; // slow animationcurrentPhaseSpeed

                if (InCounter) break; //exit if player counter enemy attack
               // if (normalizedTime >= attacks[comboCount].ImpactStartTime) //enter attack
                {
                    showDebugSphere = true;
                    debugPos = transform.position;     // capture position at this moment
                    currentPhaseAnimSpeed = attacks[comboCount].ImpactSpeed;
                    // animator.speed = currentPhaseAnimSpeed; // slow animationcurrentPhaseSpeed
                    animator.SetFloat("AttackAnimSpeed", 0.5f);
                    //attackState = AttackStates.Impact;
                    SetDamageColliderEnabled(attacks[comboCount],true);
                   // swordCollider.enabled = true;
                }
            }
            //second phase impact 
            else if (attackState == AttackStates.Impact)
            {
                if (InCounter) break;

               // if (normalizedTime >= attacks[comboCount].ImpactEndTime) //exit attack
                {
                    showDebugSphere = false;
                    currentPhaseAnimSpeed = attacks[comboCount].CooldownSpeed;
                    //animator.speed = currentPhaseAnimSpeed; // slow animationcurrentPhaseSpeed
                    //attackState = AttackStates.Cooldown;
                    SetDamageColliderEnabled(attacks[comboCount], false);
                    //swordCollider.enabled = false;

                    if ((character.tag == "Player")) cameraShake.ShakeByDuration(0.2f, 0.3f);

                }
               // cameraShake.ShakeByDuration(1f, 3f);
                Transform SpawnTransform = animator.GetBoneTransform(HumanBodyBones.Chest);
                SpawnTransform.rotation = sword.transform.rotation;
                
                //if (normalizedTime >= attacks[comboCount].SlashSpawnFrame && !isSlashSpawned)
                {
                    //slashEffect.GetCalculatedSlashRotation(animator,);
                    //Spawn Slash VFX
                    if (slashEffect != null)
                    {

                        slashEffect.SpawnEffect(SpawnTransform);
                    }

                    isSlashSpawned = true;

                    //Camera shake
                 
                }

            }
            else if (attackState == AttackStates.Cooldown)
            {
                showDebugSphere = false;
                //if (doCombo)
                //{
                //    Debug.Log("inside do combo true after attack states .cooldown");
                //    doCombo = false;
                //    comboCount = (comboCount + 1) % attacks.Count;

                //    StartCoroutine(Attack());
                //    yield break;
                //}
                
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

*/