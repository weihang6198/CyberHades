using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatStates { Idle,Chase,Circling}
public class CombatMovmentState : State<EnemyController>
{
    [SerializeField] float distanceToStand = 3f;
    [SerializeField] float adjustDistanceThreshold = 3f;
    AICombatStates state;
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
       enemy=owner;
        enemy.NavAgent.stoppingDistance = distanceToStand;
    }

    public override void Execute()
    {
        if(Vector3.Distance(enemy.Target.transform.position,enemy.transform.position)> distanceToStand+ adjustDistanceThreshold)
            StartChase(); 

        if(state==AICombatStates.Idle)
        {

        }
        else if (state == AICombatStates.Chase)
        {
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStand +0.03f)
            {
                StartIdle();
                return;
            }
            enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        }
        else if (state == AICombatStates.Circling)
        {

        }

      //apply to all conditions
        enemy.Animator.SetFloat("MoveAmount", enemy.NavAgent.velocity.magnitude/enemy.NavAgent.speed);
    }

    void StartChase()
    {
        state = AICombatStates.Chase;
        enemy.Animator.SetBool("CombatMode", false);
    }
    void StartIdle()
    {
        state = AICombatStates.Idle;
        enemy.Animator.SetBool("CombatMode", true);
    }
    public override void Exit()
    {
       
    }
}
