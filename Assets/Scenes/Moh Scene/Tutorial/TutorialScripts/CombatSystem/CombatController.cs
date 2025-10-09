using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public EnemyController targetEnemy;
    MeleeFighter meleeFighter;
    Animator animator;
    CameraController cam;
    private void Awake()
    {
        meleeFighter = GetComponent<MeleeFighter>();
        animator= GetComponent<Animator>();
        cam=Camera.main.GetComponent<CameraController>();
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
        var vecFromCam=transform.position - cam.transform.position;
        vecFromCam.y = 0f;
        return vecFromCam.normalized;

    }
}
