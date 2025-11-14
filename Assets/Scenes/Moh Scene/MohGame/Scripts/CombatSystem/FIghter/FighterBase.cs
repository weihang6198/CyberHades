using MagicaCloth2;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Windows;

public enum AttackStates { Idle, Windup, Impact, Cooldown };
public abstract class FighterBase : MonoBehaviour
{

    [field:SerializeField] public float health { get; private set; } = 25f;
     public float maxHealth { get; private set; } = 25f;

    [SerializeField] public List<AttackData> attacks;
    protected int comboCount = 0;
    [SerializeField] float hitStopTime = 0.05f;

    //delegate
    public event Action<FighterBase> OnGotHit;
    //public event Action  OnGotHit;
    public event Action OnHitComplete;

    protected StarterAssetsInputs PlayerInput;
    protected GameObject character; // assign in Inspector
    protected Animator animator;
    public bool InAction { get; protected set; }
    public bool takingDamage { get; protected set; }

    public AttackStates attackState;

    //for melee Fighter
    public bool IsCounterable => attackState == AttackStates.Windup || attackState == AttackStates.Impact;
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
        InAction = true;
        takingDamage = true;

        var displacementVector = attacker.transform.position - transform.position;
        displacementVector.y = 0;
        transform.rotation = Quaternion.LookRotation(displacementVector);

        OnGotHit?.Invoke(attacker);

        // Play hit reaction on override layer 1
        animator.CrossFadeInFixedTime("SwordImpact", 0.05f, 1, 0f);
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);

        FighterBase target = attacker;
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
        }

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
        if (!other.CompareTag("HitBox"))
            return;

        FighterBase attacker = other.GetComponentInParent<FighterBase>();
        if (attacker == null)
        {
            // Maybe it’s a projectile instead of melee
            Projectile proj = other.GetComponentInParent<Projectile>();
            if (proj != null)
                attacker = proj.owner;
            Debug.Log("projectile owner");
        }

        if (attacker == null)
        {
            Debug.Log("attack is null");
            return;
        }
            
        
      

        TakeDamage(5f);
        OnGotHit?.Invoke(attacker);

        if (health > 0)
            StartCoroutine(PlayHitReaction(attacker));
        else
            PlayDeathAnimation(attacker);
    }

    public virtual bool ShouldEndRetreat(float distanceToTarget)
    {
        // Default behavior (for melee)
        return distanceToTarget >= 3f;
    }

    void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
    }

    void PlayDeathAnimation(FighterBase fighter)
    {
        Debug.Log("plying death anim");
        animator.CrossFade("Death", 0.2f);
    }

    protected IEnumerator DoHitstop(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }
}
