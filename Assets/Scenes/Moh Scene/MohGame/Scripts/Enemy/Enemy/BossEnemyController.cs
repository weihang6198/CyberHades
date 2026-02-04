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

        stateDict[EnemyStates.GettingHit] = GetComponent<GettingHitState>();
        stateDict[EnemyStates.GettingHit].RegisterName("boss getting hit state");

        stateDict[EnemyStates.Dead] = GetComponent<DeadState>();
        stateDict[EnemyStates.Dead].RegisterName("boss dead state");

        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);

        // Fighter.OnGotHit += ReactToHit;
        Fighter.OnGotHit += (FighterBase attacker, bool filler /*doesnt do anything for now */) =>
        {

            if (Fighter.health > 0)
            {

                Debug.Log("enemy boss getting hit");
                Fighter.PlayVFXEffect(Fighter.hitBloodVFX, transform.localPosition += new Vector3(0, Random.Range(0.3f, 1.0f), 0));
                GetHitEffect();
                Target = attacker;
                if (Fighter.canIgnoreHitStun)
                {
                    Debug.Log("Fighter.consecutiveHitsTaken is setting to 0");
                    //continue with whatever doing
                    Fighter.consecutiveHitsTaken = 0;
                }
                else
                {
                    Debug.Log("enemy boss getting hit going gettingHitState");
                    ChangeState(EnemyStates.GettingHit); //advnced way 
                }

            }

            else
            {
                Debug.Log("enemy boss is  dead");
                ChangeState(EnemyStates.Dead);
            }

        };
        RegisterMaterialsFromRenderer();

        OnSummonComplete = true; //enemy boss 
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Fighter.rayCast();
    }
}
