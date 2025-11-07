using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Windows;

public enum AttackStates { Idle, Windup, Impact, Cooldown };
public abstract class FighterBase : MonoBehaviour
{
    [field:SerializeField] public float health { get; private set; } = 25f;

    [SerializeField] public List<AttackData> attacks;

    //delegate
    //public event Action<FighterBase> OnGotHit;
    public event Action  OnGotHit;
    public event Action OnHitComplete;

    private StarterAssetsInputs input;
    protected GameObject character; // assign in Inspector
    protected Animator animator;
    public bool InAction { get; protected set; }

    public AttackStates attackState;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        if (character == null)
            character = gameObject;
        input = GetComponent<StarterAssetsInputs>();
    }

    public abstract bool CanAttack(Vector3 targetPosition, float attackDistance=1.5f);

    public abstract void TryToAttack(FighterBase target = null);

    public abstract IEnumerator Attack(FighterBase target = null);

    IEnumerator PlayHitReaction(FighterBase attacker)
    {
        InAction = true;
        var dispalcementVector = attacker.transform.position - transform.position;
        dispalcementVector.y = 0;
        transform.rotation = Quaternion.LookRotation(dispalcementVector);
        // OnGotHit(attacker);
        OnGotHit?.Invoke();
        animator.CrossFade("SwordImpact", 0.2f);
        yield return null;

        var animState = animator.GetCurrentAnimatorStateInfo(1);
        yield return new WaitForSeconds(animState.length);

        OnHitComplete?.Invoke(); 
        InAction = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HitBox" && !InAction)
        {
            var attacker = other.GetComponentInParent<FighterBase>();
            
            TakeDamage(5f);
            OnGotHit?.Invoke();
            StartCoroutine(PlayHitReaction(attacker));
            //if (health > 0)
            //    //StartCoroutine(PlayHitReaction(other.GetComponentInParent<MeleeFighter>().transform));
            //    StartCoroutine(PlayHitReaction(attacker));
            //else
            //    PlayDeathAnimation(attacker);
           
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
    }

    void PlayDeathAnimation(FighterBase fighter)
    {
        Debug.Log("plying death anim");
        animator.CrossFade("Death", 0.2f);
    }

    
}
