using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFighter : MonoBehaviour
{
    Animator animator;
    public bool InAction { get;private set; } = false;
    public void Awake()
    {
        animator = GetComponent<Animator>();
    }
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
        animator.CrossFade("Slash", 0.2f);
        yield return null;//wait for a single frame

        //1 represent override layer
        var animState=animator.GetNextAnimatorStateInfo(1);
        yield return new WaitForSeconds(animState.length);

        InAction = false;
    }
}
