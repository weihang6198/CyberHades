using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatStatesTutorial { Idle,Chase,Circling}
public class CombatMovmentTutorialState : State<EnemyControllerTutorial>
{
    [SerializeField] float circlingSpeed = 20f;
    [SerializeField] float distanceToStand = 3f;
    [SerializeField] float adjustDistanceThreshold = 3f;
    [SerializeField] Vector2 IdleTimeRange = new Vector2(1, 2);
    [SerializeField] Vector2 CirclingTimeRange = new Vector2(5, 7);

    float timer = 0f;
    int circlingDir = 1;
    AICombatStatesTutorial state;
    EnemyControllerTutorial enemy;
    public override void Enter(EnemyControllerTutorial owner)
    {
       enemy=owner;
        enemy.NavAgent.stoppingDistance = distanceToStand;
        enemy.CombatMovementTimer = 0f;

        enemy.animator.SetBool("CombatMode", true);
    }

    public override void Execute()
    {
        ////search for target when entering this state if target is empty
        //if (enemy.Target == null)
        //{
        //    //find target as soon as enter combat movement state
        //    enemy.Target = enemy.FindTarget();

        //    //if no target, go back idle state
        //    if(enemy.Target==null)
        //    {
        //        enemy.ChangeState(EnemyState.Idle);
        //        return;
        //    }
        //}
        if(enemy.Target.health<=0)
        {
            enemy.Target = null;
            enemy.ChangeState (EnemyState.Idle);

            return;
        }
        if(Vector3.Distance(enemy.Target.transform.position,enemy.transform.position)> distanceToStand+ adjustDistanceThreshold)
            StartChase(); 

        if(state==AICombatStatesTutorial.Idle)
        {
            if(timer<=0)
            {
                if (Random.Range(0, 2) == 0)//either return 0 or 1, no decimal will be return
                {
                    StartIdle();
                }
                else
                {
                    StartCircling();
                }
            }
        }
        else if (state == AICombatStatesTutorial.Chase)
        {
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStand +0.03f)
            {
                StartIdle();
                return;
            }
            enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        }
        else if (state == AICombatStatesTutorial.Circling)
        {
            if(timer<=0)
            {
                StartIdle();
                return;
            }
           // transform.RotateAround(enemy.Target.transform.position, Vector3.up, circlingSpeed * circlingDir*Time.deltaTime);

            var vectorToTarget = enemy.transform.position - enemy.Target.transform.position;
            var rotatePos=Quaternion.Euler(0, circlingSpeed * circlingDir * Time.deltaTime, 0) * vectorToTarget;

            enemy.NavAgent.Move(rotatePos - vectorToTarget);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatePos);
        }

        if(timer>0)
        {
            timer-=Time.deltaTime;
        }
        enemy.CombatMovementTimer += Time.deltaTime;
    }

    void StartChase()
    {
        state = AICombatStatesTutorial.Chase;
       
        //enemy.animator.SetBool("Circling", false);
    }
    void StartIdle()
    {
        state = AICombatStatesTutorial.Idle;
        timer = Random.Range(IdleTimeRange.x, IdleTimeRange.y);

       
        //enemy.animator.SetBool("Circling", false);
    }

    void StartCircling()
    {
        state = AICombatStatesTutorial.Circling;
        enemy.NavAgent.ResetPath();     
        timer = Random.Range(CirclingTimeRange.x, CirclingTimeRange.y);
        if (Random.Range(0, 2) == 0)//either return 0 or 1, no decimal will be return
        {
            circlingDir = 1;
        }
        else
        {
            circlingDir = -1;
        }

        //enemy.animator.SetBool("Circling", true);
        //enemy.animator.SetFloat("CirclingDir", circlingDir);

    }
    public override void Exit()
    {
        enemy.CombatMovementTimer = 0f;
    }
}
