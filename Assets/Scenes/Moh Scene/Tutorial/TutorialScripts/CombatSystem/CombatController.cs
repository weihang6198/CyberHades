using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    MeleeFighter meleeFighter;
    Animator animator;

    private void Awake()
    {
        meleeFighter = GetComponent<MeleeFighter>();
        animator= GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            var enemy=EnemyManager.instance.GetAttackingEnemy();
            if ((enemy!=null && enemy.MeleeFighter.IsCounterable && !meleeFighter.InAction))
            {
                StartCoroutine(meleeFighter.PerformCounterAttack(enemy));
            }
            else
            {

            }
            meleeFighter.TryToAttack(); 
        }
    }

    //apply root motion manually
    //apply root motion of rot and pos separately
    private void OnAnimatorMove()
    {
        //apply the position of root motion
        transform.position += animator.deltaPosition;

        //apply the rotation of root motion
        transform.rotation *= animator.deltaRotation;
    }
}
