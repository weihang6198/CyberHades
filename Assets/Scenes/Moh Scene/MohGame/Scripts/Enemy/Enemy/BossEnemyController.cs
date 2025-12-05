using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BossEnemyController : EnemyController
{
   // [SerializeField] public BossFighter bossFighter { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        EnemyManager.instance.RegisterEnemy();

        NavAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Fighter = GetComponent<BossFighter>();
        if (Fighter != null) Debug.Log("boss fighter exist");
        CharacterController = GetComponent<CharacterController>();
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();

        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.Idle].RegisterName("boss idle");

        stateDict[EnemyStates.CombatMovement] = GetComponent<BossCombatMovementState>();
        stateDict[EnemyStates.CombatMovement].RegisterName("boss combat movement state");

        stateDict[EnemyStates.Attack] = GetComponent<BossAttackState>();
        stateDict[EnemyStates.Attack].RegisterName("boss attack state");

        stateDict[EnemyStates.RetreatAfterAttack] = GetComponent<RetreatAfterAttackState>();
        stateDict[EnemyStates.RetreatAfterAttack].RegisterName("boss RetreatAfterAttack state");

        stateDict[EnemyStates.Dead] = GetComponent<DeadState>();
        stateDict[EnemyStates.Dead].RegisterName("boss dead state");

        stateDict[EnemyStates.GettingHit] = GetComponent<GettingHitState>();
        stateDict[EnemyStates.GettingHit].RegisterName("boss getting hit state");

        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);

        // Fighter.OnGotHit += ReactToHit;
        Fighter.OnGotHit += (FighterBase attacker) =>
        {

            if (Fighter.health > 0)
            {

                Debug.Log("enemy getting hit");
                ChangeState(EnemyStates.GettingHit); //advnced way 
            }

            else
            {
                Debug.Log("enemy boss is  dead");
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
