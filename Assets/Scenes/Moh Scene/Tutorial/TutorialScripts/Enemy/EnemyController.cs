using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle,CombatMovement,Attack}
public class EnemyController : MonoBehaviour
{
    [field:SerializeField]public float Fov { get; private set; } = 180f;
    public List<MeleeFighter> TargetsInRange {  get; private set; }= new List<MeleeFighter>();
    public MeleeFighter Target { get;  set; }

    //track how long the enemy is in this state
    public float CombatMovementTimer { get; set; } = 0f;
    public StateMachine<EnemyController> stateMachine {  get; private set; }
    Dictionary<EnemyState, State<EnemyController>> stateDict;

    public NavMeshAgent NavAgent {  get;  private set;}
    public Animator animator { get; private set; }
    public MeleeFighter Fighter { get; private set; }
    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        animator=GetComponent<Animator>();
        Fighter=GetComponent<MeleeFighter>();

        //initialize the state machine
        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle]=GetComponent<IdleState>(); 
        stateDict[EnemyState.CombatMovement]=GetComponent<CombatMovmentState>(); 
        stateDict[EnemyState.Attack]=GetComponent<AttackState>(); 


        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyState.Idle]);
    }

    public void ChangeState(EnemyState state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }

    public bool IsInState(EnemyState state)
    {
        return stateMachine.CurrentState == stateDict[state];
    }
    Vector3 prevPos;
    private void Update()
    {
        stateMachine.Execute();

        //v=dx/dt
        var deltaPos=transform.position - prevPos;
        var velocity=deltaPos/Time.deltaTime;

        float forwardSpeed=Vector3.Dot(velocity, transform.forward);
        //apply to all conditions
        animator.SetFloat("ForwardSpeed", forwardSpeed / NavAgent.speed,0.2f,Time.deltaTime);

        float angle=Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        float strafeSpeed=Mathf.Sin(angle*Mathf.Deg2Rad);

        animator.SetFloat("StrafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);
        prevPos=transform.position;
    }
}
