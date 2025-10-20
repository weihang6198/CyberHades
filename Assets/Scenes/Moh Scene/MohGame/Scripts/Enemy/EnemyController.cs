using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { Idle, CombatMovement, Attack, RetreatAfterAttack, Dead, GettingHit }

public class EnemyController     : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public StateMachine<EnemyController> stateMachine { get; private set; }
    public List<MeleeFighter> TargetsInRange { get; private set; } = new List<MeleeFighter>();

    public MeleeFighter Target { get; set; }

    Dictionary<EnemyStates, State<EnemyController>> stateDict;

    public NavMeshAgent NavAgent { get; private set; }

    public Animator animator { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
      
        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<CombatMovementState>();
        //stateDict[EnemyState.CombatMovement] = GetComponent<CombatMovmentState>();
        //stateDict[EnemyState.Attack] = GetComponent<AttackState>();
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

    private void Update()
    {
        stateMachine.Execute();
    }
}
