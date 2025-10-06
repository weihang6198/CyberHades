using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { Idle,Chase}
public class EnemyController : MonoBehaviour
{
    public StateMachine<EnemyController> stateMachine {  get; private set; }
    Dictionary<EnemyState, State<EnemyController>> stateDict;

    private void Start()
    {
        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle]=GetComponent<IdleState>(); 
        stateDict[EnemyState.Chase]=GetComponent<ChaseState>(); 


        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyState.Idle]);
    }

    public void ChangeState(EnemyState state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }
    private void Update()
    {
        stateMachine.Execute();
    }
}
