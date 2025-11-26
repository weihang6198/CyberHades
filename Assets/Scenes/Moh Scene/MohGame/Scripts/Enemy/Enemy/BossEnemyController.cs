using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BossEnemyController : EnemyController
{
    [SerializeField] public BossFighter bossFighter { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        EnemyManager.instance.RegisterEnemy();

        NavAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        bossFighter =GetComponent<BossFighter>();
        if (bossFighter != null) Debug.Log("boss fighter exist");
        CharacterController = GetComponent<CharacterController>();
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();

        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<BossCombatMovementState>();
        stateDict[EnemyStates.Attack] = GetComponent<BossAttackState>();
        stateDict[EnemyStates.RetreatAfterAttack] = GetComponent<RetreatAfterAttackState>();
        stateDict[EnemyStates.Dead] = GetComponent<DeadState>();
        stateDict[EnemyStates.GettingHit] = GetComponent<GettingHitState>();

        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);

        // Fighter.OnGotHit += ReactToHit;
        bossFighter.OnGotHit += (FighterBase attacker) =>
        {

            if (Fighter.health > 0)
            {

                Debug.Log("enemy getting hit");
                ChangeState(EnemyStates.GettingHit); //advnced way 
            }

            else
            {
                Debug.Log("enemy dead");
                ChangeState(EnemyStates.Dead);
            }

        };
        RegisterMaterialsFromRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update(); 
    }
}
