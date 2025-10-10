using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle,CombatMovement,Attack,RetreatAfterAttack,Dead,GettingHit}
public class EnemyController : MonoBehaviour
{
    [field:SerializeField]public float Fov { get; private set; } = 180f;
    public List<MeleeFighter> TargetsInRange {  get; private set; }= new List<MeleeFighter>();
    public MeleeFighter Target { get;  set; }
    public SkinMeshHighlighter MeshHighlighter { get; private set; }

    //track how long the enemy is in this state
    public float CombatMovementTimer { get; set; } = 0f;
    public StateMachine<EnemyController> stateMachine {  get; private set; }
    Dictionary<EnemyState, State<EnemyController>> stateDict;

    public NavMeshAgent NavAgent {  get;  private set;}
    public CharacterController CharacterController {  get;  private set;}
    public Animator animator { get; private set; }
    public MeleeFighter MeleeFighter { get; private set; }


    public VisionSensor VisionSensor {  get;  set; }
    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        CharacterController = GetComponent<CharacterController>();
        animator=GetComponent<Animator>();
        MeleeFighter=GetComponent<MeleeFighter>();
        MeshHighlighter = GetComponent<SkinMeshHighlighter>();

        //initialize the state machine
        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle]=GetComponent<IdleState>(); 
        stateDict[EnemyState.CombatMovement]=GetComponent<CombatMovmentState>(); 
        stateDict[EnemyState.Attack]=GetComponent<AttackState>(); 
        stateDict[EnemyState.RetreatAfterAttack]=GetComponent<RetreatAfterAttackState>(); 
        stateDict[EnemyState.Dead]=GetComponent<DeadState>(); 
        stateDict[EnemyState.GettingHit] =GetComponent<GettingHitState>(); 


        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyState.Idle]);

       // MeleeFighter.OnGotHit += ReactToHit; //simple way 
        MeleeFighter.OnGotHit +=() => ChangeState(EnemyState.GettingHit); //advnced way 
    }

    void ReactToHit()
    {
        ChangeState(EnemyState.GettingHit);
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
        var deltaPos=animator.applyRootMotion? Vector3.zero :transform.position - prevPos;
        var velocity=deltaPos/Time.deltaTime;

        float forwardSpeed=Vector3.Dot(velocity, transform.forward);
        //apply to all conditions
        animator.SetFloat("ForwardSpeed", forwardSpeed / NavAgent.speed,0.2f,Time.deltaTime);

        float angle=Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        float strafeSpeed=Mathf.Sin(angle*Mathf.Deg2Rad);

        animator.SetFloat("StrafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);
        prevPos=transform.position;
    }

    public MeleeFighter FindTarget()
    {
        foreach (var target in TargetsInRange)
        {
            var vecToTarget = target.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, vecToTarget);

            if (angle <= Fov / 2)
            {
                return target;
            }
        }
        return null;
    }
}
