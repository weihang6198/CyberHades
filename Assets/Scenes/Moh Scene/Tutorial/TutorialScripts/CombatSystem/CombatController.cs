using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerTutorial : MonoBehaviour
{
    EnemyControllerTutorial targetEnemy;
    public EnemyControllerTutorial TargetEnemy
    {
        get => targetEnemy;

        set
        {
            targetEnemy = value;

            if (targetEnemy == null)
                combatMode = false;
        }
    }

    //combat mode means player lock on into enemy
    bool combatMode;
    public bool CombatMode
    {
        get => combatMode;
        set
        {
            combatMode = value;

            if (TargetEnemy == null)
                combatMode = false;

            Debug.Log("animator set bool to "+combatMode);
            animator.SetBool("CombatMode", combatMode);
        }
    }

    MeleeFighterTutorial meleeFighter;
    Animator animator;
    CameraController cam;
    private void Awake()
    {
        meleeFighter = GetComponent<MeleeFighterTutorial>();
        animator= GetComponent<Animator>();
        cam=Camera.main.GetComponent<CameraController>();
    }

    private void Start()
    {

        meleeFighter.OnGotHit += (MeleeFighterTutorial attacker) =>
        {
            //when player got hit by enemy, change the targetEnemy to enemy that is currently atking player
            if (combatMode && attacker != targetEnemy.MeleeFighter)
                targetEnemy = attacker.GetComponent<EnemyControllerTutorial>();

        };
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack") && !meleeFighter.isTakingHit)
        {
            var enemy=EnemyManager.instance.GetAttackingEnemy();
            if ((enemy!=null && enemy.MeleeFighter.IsCounterable && !meleeFighter.InAction))
            {
                //test only
               StartCoroutine(meleeFighter.PerformCounterAttack(enemy));
               // meleeFighter.TryToAttack(PlayerControllerTutorial.instance.InputDir);
            }
            else
            {
                //rotate towards closest enemy and attack based on player input dir
               var enemyToAttack= EnemyManager.instance.GetClosestEnemyToDir(PlayerControllerTutorial.instance.GetIntentDirection());

                if (enemyToAttack != null)
                    meleeFighter.TryToAttack(enemyToAttack?.MeleeFighter);
                else 
                    meleeFighter.TryToAttack(null);

                CombatMode = true;

            }

        }
            
        //if(Input.GetButtonDown("LockOn") || JoyStickHelper.instance.GetAxisDown("LockOnTrigger"))
        if(Input.GetButtonDown("LockOn") )
        {
            Debug.Log("lock on button pressed");
            CombatMode = !CombatMode;
        }
    }

    //apply root motion manually
    //apply root motion of rot and pos separately
    private void OnAnimatorMove()
    {
        if(!meleeFighter.InCounter)
        {
            //apply the position of root motion
            transform.position += animator.deltaPosition;
        }
       
       

        //apply the rotation of root motion
        transform.rotation *= animator.deltaRotation;
    }

    public Vector3 GetTargetingDir()
    {
        if(!combatMode)
        {
            var vecFromCam = transform.position - cam.transform.position;
            vecFromCam.y = 0f;
            return vecFromCam.normalized;
        }
        else
        {
            return transform.forward;
        }
      

    }
}
