using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle,CombatMovement,Attack,RetreatAfterAttack,Dead,GettingHit}
public class EnemyControllerTutorial : MonoBehaviour
{
    [field:SerializeField]public float Fov { get; private set; } = 180f;
    [field: SerializeField] public float AlertRange { get; private set; } = 20f;
    public List<MeleeFighterTutorial> TargetsInRange {  get; private set; }= new List<MeleeFighterTutorial>();
    public MeleeFighterTutorial Target { get;  set; }
    public SkinMeshHighlighter MeshHighlighter { get; private set; }

    //track how long the enemy is in this state
    public float CombatMovementTimer { get; set; } = 0f;
    public StateMachine<EnemyControllerTutorial> stateMachine {  get; private set; }
    Dictionary<EnemyState, State<EnemyControllerTutorial>> stateDict;

    public NavMeshAgent NavAgent {  get;  private set;}
    public CharacterController CharacterController {  get;  private set;}
    public Animator animator { get; private set; }
    public MeleeFighterTutorial MeleeFighter { get; private set; }


    public VisionSensorTutorial VisionSensor {  get;  set; }
    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        CharacterController = GetComponent<CharacterController>();
        animator=GetComponent<Animator>();
        MeleeFighter=GetComponent<MeleeFighterTutorial>();
        MeshHighlighter = GetComponent<SkinMeshHighlighter>();

        //initialize the state machine
        stateDict = new Dictionary<EnemyState, State<EnemyControllerTutorial>>();
        stateDict[EnemyState.Idle]=GetComponent<IdleStateTutorial>(); 
        stateDict[EnemyState.CombatMovement]=GetComponent<CombatMovmentTutorialState>(); 
        stateDict[EnemyState.Attack]=GetComponent<AttackStateTutorial>(); 
        stateDict[EnemyState.RetreatAfterAttack]=GetComponent<RetreatAfterAttackState>(); 
        stateDict[EnemyState.Dead]=GetComponent<DeadState>(); 
        stateDict[EnemyState.GettingHit] =GetComponent<GettingHitState>(); 


        stateMachine = new StateMachine<EnemyControllerTutorial>(this);
        stateMachine.ChangeState(stateDict[EnemyState.Idle]);

        // MeleeFighter.OnGotHit += ReactToHit; //simple way 
        // MeleeFighter.OnGotHit +=() => ChangeState(EnemyState.GettingHit); //advnced way 
        MeleeFighter.OnGotHit += (MeleeFighterTutorial attacker) =>
        {

            if (MeleeFighter.health > 0)
            {
                if(Target==null)
                {
                    Target = attacker;
                    AlertNearbyEnemies(); ;
                }
                Debug.Log("enemy getting hit");
                ChangeState(EnemyState.GettingHit); //advnced way 
            }
               
            else
            {
                Debug.Log("enemy dead");
                ChangeState(EnemyState.Dead);
            }
                
        };
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

        if(Target?.health<=0)
        {
            TargetsInRange.Remove(Target);
            EnemyManagerTutorial.instance.RemoveEnemyInRange(this);
        }
        prevPos=transform.position;
    }

    public MeleeFighterTutorial FindTarget()
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

    public void AlertNearbyEnemies()
    {
        return;
        var colliders=Physics.OverlapBox(transform.position, new Vector3(AlertRange/2f, 1f, AlertRange/2f),
            Quaternion.identity,EnemyManagerTutorial.instance.enemyLayer);

        foreach (var collider in colliders)
        {
            if (collider.gameObject == gameObject) continue;
            var nearbyEnemy=collider.GetComponent<EnemyControllerTutorial>();
            if(nearbyEnemy != null &&nearbyEnemy.Target==null)
            {
                nearbyEnemy.Target = Target; 
                nearbyEnemy.ChangeState(EnemyState.CombatMovement);
            }

        }
    }
}
