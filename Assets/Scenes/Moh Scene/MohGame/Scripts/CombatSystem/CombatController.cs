using StarterAssets;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    private StarterAssetsInputs _input;
  

    MeleeFighter meleeFighter;
    Animator animator;
  
    private void Awake()
    {
        meleeFighter = GetComponent<MeleeFighter>();
        _input = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
       
        if (_input.attack)
        {
            _input.attack = false;
            var enemy = EnemyManager.instance.GetAttackingEnemy();
            //for parry 
            if ((enemy != null && enemy.Fighter.IsCounterable && (!meleeFighter.InAction || enemy.Fighter.isDashing)))
            {
                StartCoroutine(meleeFighter.PerformCounterAttack(enemy));
            }
            else
            {
                meleeFighter.TryToAttack();
            }
                

        }
        //if (Input.GetButtonDown("Attack") && !meleeFighter.isTakingHit)
        //{
        //    var enemy = EnemyManagerTutorial.instance.GetAttackingEnemy();
        //    if ((enemy != null && enemy.MeleeFighter.IsCounterable && !meleeFighter.InAction))
        //    {
        //        //test only
        //        StartCoroutine(meleeFighter.PerformCounterAttack(enemy));
        //        // meleeFighter.TryToAttack(PlayerControllerTutorial.instance.InputDir);
        //    }
        //    else
        //    {
        //        //rotate towards closest enemy and attack based on player input dir
        //        var enemyToAttack = EnemyManagerTutorial.instance.GetClosestEnemyToDir(PlayerControllerTutorial.instance.GetIntentDirection());

        //        if (enemyToAttack != null)
        //            meleeFighter.TryToAttack(enemyToAttack?.MeleeFighter);
        //        else
        //            meleeFighter.TryToAttack(null);

        //        CombatMode = true;

        //    }

        //}
        //player can animation cancel atk with dashing
        if (_input.dash)
        {
            _input.dash = false;

            meleeFighter.TryToDash();
        }
        if (_input.blockStart)
        {
            _input.blockStart = false;

            meleeFighter.isBlocking = !meleeFighter.isBlocking;
            Debug.Log("block pressed");
            meleeFighter.TryToBlock();
        }

        if (_input.blockEnd )
        {
            _input.blockEnd = false;
            Debug.Log("block released");
            meleeFighter.isBlocking = false;
        }
        //meleeFighter.RotateTowardMouse();
        // RotateTowardMouse();
    }

    private void OnAnimatorMove()
    {
        if (!meleeFighter.InCounter  || meleeFighter.isDashing)
        {
            //apply the position of root motion
            transform.position += animator.deltaPosition;
        }



        //apply the rotation of root motion
        transform.rotation *= animator.deltaRotation;
    }
}
