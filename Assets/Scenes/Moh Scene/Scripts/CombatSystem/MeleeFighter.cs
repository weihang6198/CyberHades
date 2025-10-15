using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFighter : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    bool InAction = false;

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
        animator.CrossFade("attack01", 0.2f);
        yield return null;
        var animStat = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(animStat.length);

        InAction = false;
    }
}
