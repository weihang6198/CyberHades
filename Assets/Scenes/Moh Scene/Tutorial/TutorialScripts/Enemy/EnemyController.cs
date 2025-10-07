using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle,CombatMovement}
public class EnemyController : MonoBehaviour
{
    [field:SerializeField]public float Fov { get; private set; } = 180f;
    public List<MeleeFighter> TargetsInRange {  get; private set; }= new List<MeleeFighter>();
    public MeleeFighter Target { get;  set; }
    public StateMachine<EnemyController> stateMachine {  get; private set; }
    Dictionary<EnemyState, State<EnemyController>> stateDict;

    public NavMeshAgent NavAgent {  get;  private set;}
    public Animator Animator { get; private set; }
    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Animator=GetComponent<Animator>();
        //initialize the state machine
        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle]=GetComponent<IdleState>(); 
        stateDict[EnemyState.CombatMovement]=GetComponent<CombatMovmentState>(); 


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
        //apply to all conditions
        Animator.SetFloat("MoveAmount", NavAgent.velocity.magnitude / NavAgent.speed);
    }
}
