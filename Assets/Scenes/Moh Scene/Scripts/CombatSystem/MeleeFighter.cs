using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStates {Idle,Windup,Impact,Cooldown};
public class MeleeFighter : MonoBehaviour
{
    [SerializeField] List<AttackData> attacks;
    [SerializeField] GameObject sword;

    // Start is called before the first frame update
    Animator animator;
    AttackStates attackState;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public bool InAction { get; private set; } = false;

    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());

        }
    }

    IEnumerator Attack()
    {
      

        InAction = true;
        attackState = AttackStates.Windup ;
        animator.CrossFade(attacks[0].AnimName, 0.2f, 1);
        yield return null;

        var  animState = animator.GetCurrentAnimatorStateInfo(1);
        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;
            if (attackState == AttackStates.Windup)
            {
                if (normalizedTime >= attacks[0].ImpactStartTime)
                {
                    attackState = AttackStates.Impact;
                    Debug.Log("in impact state attack");
                }
            }
            else if (attackState == AttackStates.Impact)
            {
                if (normalizedTime >= attacks[0].ImpactEndTime)
                {
                    attackState = AttackStates.Cooldown;
                    Debug.Log("in cooldown state attack");
                }
            }
            else if (attackState == AttackStates.Cooldown)
            {

            }
            yield return null;
        }
        //        // Wait until the animation has fully started
        //        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(1).IsName("attack01"));

        //// Then wait until it's done
        //yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f);
        attackState = AttackStates.Idle;

        InAction = false;
    }
}
