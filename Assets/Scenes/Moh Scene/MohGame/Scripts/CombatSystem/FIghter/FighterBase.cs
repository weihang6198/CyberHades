using MagicaCloth2;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;


//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI.Table;
using Random = UnityEngine.Random;
public enum AttackStates { Idle, Windup, Impact, Cooldown };
public enum BlockStates
{
    Idle,
    BlockStart, // pressed
    Blocking,   // hold
    BlockEnd    // released
 };
public abstract class FighterBase : MonoBehaviour
{
    [SerializeField] public LayerMask layerMaskRayCastTest;
    [field:SerializeField] public float health { get;  set; } = 25f;
    [field:SerializeField] public float maxHealth { get; set; } = 25f;
        
    [SerializeField] public List<AttackData> attacks;
    protected int comboCount = 0;
    [SerializeField] float hitStopDuration = 0.05f;

    [field:SerializeField] public GameObject hitBloodVFX;
    [field:SerializeField] public GameObject thunderVFX;
    [SerializeField] public AudioClip[] hitSoundClips;

    //delegate
    public event Action<FighterBase,bool > OnGotHit;
    //public event Action  OnGotHit;
    public event Action OnHitComplete;
    
    public event Action OnDead;

    public event Action OnVictory;

    protected StarterAssetsInputs PlayerInput;
    protected GameObject character; // assign in Inspector
    protected Animator animator;
    public bool InAction { get; protected set; }
    public bool takingDamage { get; protected set; }
    public bool isDead { get; private set; }

    public AttackStates attackState;

    public int consecutiveHitsTaken = 0; // Number of consecutive hits taken from the player
    public int maxConsecutiveHitsAllowed = 2; // Enemy can still attack after this many consecutive hits
    public bool canIgnoreHitStun = false;
    //for melee Fighter
    public bool IsCounterable => attackState == AttackStates.Windup || attackState == AttackStates.Impact;
    public bool canCounter = false; //debug usage 
    public bool isDashing = false;
    protected bool canDash = true;
    public float dashWaitPercent = 0.7f;
    public float dashCooldown = 1.5f;
    public bool InCounter { get; set; } = false;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        if (character == null)
            character = gameObject;
        PlayerInput = GetComponent<StarterAssetsInputs>();
        maxHealth = health;
    }

    public abstract bool CanAttack(Vector3 targetPosition, float attackDistance=1.5f);

    public abstract void TryToAttack(FighterBase target = null);

    public abstract IEnumerator Attack(FighterBase target = null);

    IEnumerator PlayHitReaction(FighterBase attacker)
    {
        
        FighterBase target = attacker;
        
        if (consecutiveHitsTaken > maxConsecutiveHitsAllowed || canIgnoreHitStun )
        {
            Debug.Log("inside consecutiveHitsTaken > maxConsecutiveHitsAllowed) is true");
            //if enemy consecutiveHitsTaken > maxConsecutiveHitsAllowed, enemy will not be stunned and change to attack state 
            //while enemy is being attacked by player
            OnGotHit?.Invoke(attacker, true);
            yield break;
        }
        OnGotHit?.Invoke(attacker, false);

        InAction = true;
        takingDamage = true;
       
        var displacementVector = attacker.transform.position - transform.position;
        displacementVector.y = 0;
        transform.rotation = Quaternion.LookRotation(displacementVector);

        

            // Play hit reaction on override layer 1
        animator.CrossFadeInFixedTime("SwordImpact", 0.05f, 1, 0f);

        yield return StartCoroutine(HitStopCoroutine(hitStopDuration));

        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);

       
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos;

        // enemy knockback
        if (target != null)
        {
            var vecToTarget = target.transform.position - transform.position;
            vecToTarget.y = 0;
            Vector3 attackDir = -vecToTarget.normalized; // knock *away* from attacker
            float knockbackDist = attacker.attacks[attacker.comboCount].KnockBackDistance;
            targetPos = startPos + attackDir * knockbackDist;

            PlayVFXEffect(hitBloodVFX, transform.localPosition += new Vector3(0, Random.Range(0.3f, 1.0f), 0));
            SoundFXManager.instance.PlayRandomSoundFXClip(hitSoundClips, transform, 0.7f, new Vector2(0.9f, 1.1f));


        }

        //decide when the sword impact anim finish
        float timer = 0f;
        float animEndPercentage = 0.35f;
        float animTime = animState.length * animEndPercentage;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            if(timer<= animTime)
            {
                float t = Mathf.Clamp01(timer / animTime);
                transform.position = Vector3.Lerp(startPos, targetPos, t);
            }
           
            yield return null;
        }

        OnHitComplete?.Invoke();
        takingDamage = false;
        InAction = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[OnTriggerEnter] Hit by collider: {other.name}, tag: {other.tag}");

        if (!other.CompareTag("HitBox"))
        {
            Debug.Log("[OnTriggerEnter] Ignored (not HitBox)");
            return;
        }

        Debug.Log("[OnTriggerEnter] HitBox detected");

        FighterBase attacker = other.GetComponentInParent<FighterBase>();

        //for projectile
        if (attacker != null)
        {
            Debug.Log($"[Attacker] FighterBase found: {attacker.name}");
        }
        else
        {
            Debug.Log("[Attacker] No FighterBase found, checking projectile / laser");

            // Try projectile
            Projectile proj = other.GetComponent<Projectile>();
            if (proj != null)
            {
                attacker = proj.owner;
                Debug.Log($"[Attacker] Projectile detected, owner: {attacker}");
            }
            else
            {
                Debug.Log("[Attacker] No FighterBase found → checking boss laser");

                SpawnLaserEffectObject laser =
                    other.GetComponentInParent<SpawnLaserEffectObject>();

                
                if (laser != null)
                {
                    Debug.Log("[Attacker] Boss laser component FOUND");
                    attacker = laser.owner;

                    Debug.Log($"[Attacker] Boss laser detected, owner: {attacker}");
                }
                else
                {
                    Debug.LogWarning(
                        $"[Attacker] UNKNOWN attacker source\n" +
                        $"Collider: {other.name}\n" +
                        $"Root: {other.transform.root.name}"
                    );
                }
            }

        }

        Debug.Log($"[Damage] Taking damage from: {attacker}");

        TakeDamage(5f);
        consecutiveHitsTaken++;
       // OnGotHit?.Invoke(attacker);



        if (health > 0)
        {
          
            {
               
                Debug.Log("[State] Health > 0 → Play hit reaction");
                StartCoroutine(PlayHitReaction(attacker));
            }
               
        }
        else
        {
            Debug.Log("[State] Health <= 0 → Play death animation");
            PlayDeathAnimation(attacker);
        }
    }


    public virtual bool ShouldEndRetreat(float distanceToTarget)
    {
        // Default behavior (for melee)
        return distanceToTarget >= 3f;
    }

    void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
        if (health <= 0f)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }

    void PlayDeathAnimation(FighterBase fighter)
    {
        OnGotHit?.Invoke(fighter, true);
        Debug.Log("plying death anim");
        animator.CrossFade("Death", 0.2f);
    }

    protected IEnumerator HitStopCoroutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    protected IEnumerator MoveCharacter(Transform target, float maxMoveDistance,
                          float moveStartTime = 0f, float moveEndTime = 1f)
    {
        // Ensure valid range
        moveStartTime = Mathf.Clamp01(moveStartTime);
        moveEndTime = Mathf.Clamp01(moveEndTime);

        // Read starting data
        Vector3 startPos = transform.position;
        Vector3 dir;

      
        // Fallback to forward direction
        dir = transform.forward;
        

        // Calculate final position
        Vector3 endPos = startPos + dir * maxMoveDistance;

        float timer = 0f;

        // You can later adjust this to animator state length
        float duration = 0.2f; // dash is usually instant, short

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / duration;

            // Only move during your movement window
            if (normalizedTime > moveStartTime && normalizedTime < moveEndTime)
            {
                float t = (normalizedTime - moveStartTime) / (moveEndTime - moveStartTime);
                t = Mathf.Clamp01(t);

                transform.position = Vector3.Lerp(startPos, endPos, t);
            }

            // rotate toward movement direction
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    720 * Time.deltaTime // customize as needed
                );
            }

            yield return null;
        }
    }

    
    public void OnDeathAnimationFinished()
    {
        Debug.Log("trigger OnDeathAnimationFinished ");
        OnDead?.Invoke();
        //OnVictory?.Invoke();
    }

    public void PlayVFXEffect(GameObject VFX,Vector3 position)
    {
        if (VFX != null)
        {
            // transform.localPosition += new Vector3(0, Random.Range(0.3f, 1.0f), 0)
            GameObject hitLightVFXInstance = Instantiate(VFX,position, Quaternion.identity); ;

            ParticleSystem ps = hitLightVFXInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                Destroy(hitLightVFXInstance, ps.main.duration);
            }
        }
    }

    public void rayCast()
    {
        return;
        Vector3 origin = transform.position;
        origin.y += 1.2f;
        // Vector3 forward = new Vector3(SpawnTransform.forward.x, 0f, SpawnTransform.forward.z).normalized;
        Vector3 forward = transform.forward;

        //if (Physics.Raycast(origin, forward, out RaycastHit hitInfo, 100f, layerMaskRayCastTest))
        if (Physics.Raycast(origin, forward, out RaycastHit hitInfo, 100f, layerMaskRayCastTest))
        {
            Debug.DrawRay(origin, forward * 100f, Color.red, 3f);
        }
        else
        {
            Debug.DrawRay(origin, forward * 100f, Color.blue, 3f);
        }
    }

    public abstract void SpawnSlashEffect();
    void ChangeAttackState(string attackStates)
    {
        Debug.Log($"<color=cyan>[AttackState]</color> Event received: <b>{attackStates}</b>");

        switch (attackStates)
        {
            case "Idle":
                Debug.Log("<color=green>[AttackState]</color> → <b>Idle</b>");

                attackState = AttackStates.Idle;
                break;

            case "Windup":
                Debug.Log("<color=yellow>[AttackState]</color> → <b>Windup</b>");

                //animator.speed = 0.1f;
                attackState = AttackStates.Windup;
                break;

            case "Impact":
                Debug.Log("<color=orange>[AttackState]</color> → <b>Impact</b>");

                attackState = AttackStates.Impact;
                break;

            case "Cooldown":
                Debug.Log("<color=blue>[AttackState]</color> → <b>Cooldown</b>");

                attackState = AttackStates.Cooldown;
                break;
            case "SlashEffect":
                Debug.Log("<color=blue>inside slash effect ChangeAttackState<b>Cooldown</b>");
                SpawnSlashEffect();
                break;
            default:
                Debug.LogWarning($"<color=red>[AttackState]</color> Unknown state: <b>{attackStates}</b>");
                break;
        }
    }

}
