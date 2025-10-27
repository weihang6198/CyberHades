using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public enum EnemyStates { Idle, CombatMovement, Attack, RetreatAfterAttack, Dead, GettingHit }

public class EnemyController     : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public StateMachine<EnemyController> stateMachine { get; private set; }
    public List<MeleeFighter> TargetsInRange { get; private set; } = new List<MeleeFighter>();

    public MeleeFighter Target { get; set; }
    public MeleeFighter Fighter { get; set; }

    Dictionary<EnemyStates, State<EnemyController>> stateDict;

    public NavMeshAgent NavAgent { get; private set; }

    public Animator animator { get; private set; }

    public float CombatMovementTimer { get; set; } = 0f;
    // Start is called before the first frame update
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Fighter= GetComponent<MeleeFighter>();
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
      
        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<CombatMovementState>();
        stateDict[EnemyStates.Attack] = GetComponent<AttackState>();
        //stateDict[EnemyState.RetreatAfterAttack] = GetComponent<RetreatAfterAttackState>();
        //stateDict[EnemyState.Dead] = GetComponent<DeadState>();
        //stateDict[EnemyState.GettingHit] = GetComponent<GettingHitState>();

        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);
       
    }
    public void ChangeState(EnemyStates state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }

    Vector3 prevPos;
    private void Update()
    {
        stateMachine.Execute();

        animator.SetFloat("Speed", NavAgent.velocity.magnitude);
        animator.SetFloat("MotionSpeed", 1);


        var deltaPos = animator.applyRootMotion ? Vector3.zero : transform.position - prevPos;
        var velocity = deltaPos / Time.deltaTime;

        //float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        ////apply to all conditions
        ////animator.SetFloat("Speed", forwardSpeed / NavAgent.speed, 0.2f, Time.deltaTime);


        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);

        animator.SetFloat("StrafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

        prevPos = transform.position;
    }

    public bool IsInState(EnemyStates state)
    {
        return stateMachine.CurrentState == stateDict[state];
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {

        return;
    }
}
