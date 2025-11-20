using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;
using static UnityEngine.UI.GridLayoutGroup;

public enum EnemyStates { Idle, CombatMovement, Attack, RetreatAfterAttack, Dead, GettingHit }

public enum EnemyType { Melee, Ranged, Boss }
public class EnemyController     : MonoBehaviour
{
    [field: SerializeField] public bool canAttack = true;
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public StateMachine<EnemyController> stateMachine { get; private set; }
    public List<FighterBase> TargetsInRange { get; private set; } = new List<FighterBase>();

    public FighterBase Target { get; set; }
    public FighterBase Fighter { get; set; }

    Dictionary<EnemyStates, State<EnemyController>> stateDict;

    public NavMeshAgent NavAgent { get; private set; }

    public Animator animator { get; private set; }

    [SerializeField]  public  EnemyType enemyType;

    public float CombatMovementTimer { get; set; } = 0f;

    public visionSensor VisionSensor { get; set; }

    [SerializeField] public Renderer[] rends;
     List<Material> materials = new List<Material>();
    [SerializeField] public AnimationCurve HitFresnelCurve;

    
    public CharacterController CharacterController { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        
        NavAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Fighter= GetComponent<FighterBase>();
        CharacterController = GetComponent<CharacterController>();
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
      
        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<CombatMovementState>();
        stateDict[EnemyStates.Attack] = GetComponent<AttackState>();
        stateDict[EnemyStates.RetreatAfterAttack] = GetComponent<RetreatAfterAttackState>();
        stateDict[EnemyStates.Dead] = GetComponent<DeadState>();
        stateDict[EnemyStates.GettingHit] = GetComponent<GettingHitState>();

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
                Debug.Log("enemy dead");
                ChangeState(EnemyStates.Dead);
            }

        };
        RegisterMaterialsFromRenderer();

    }
    public void ChangeState(EnemyStates state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }

    Vector3 prevPos;
    private void Update()
    {
      
        //stateMachine.Execute();


        //animator.SetFloat("Speed", NavAgent.velocity.magnitude);
        //animator.SetFloat("MotionSpeed", 1);


        //var deltaPos = animator.applyRootMotion ? Vector3.zero : transform.position - prevPos;
        //var velocity = deltaPos / Time.deltaTime;

        ////float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        //////apply to all conditions
        //////animator.SetFloat("Speed", forwardSpeed / NavAgent.speed, 0.2f, Time.deltaTime);


        //float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        //float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);

        //animator.SetFloat("StrafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

        //prevPos = transform.position;

        ///////////////////
        stateMachine.Execute();

        //v=dx/dt
        var deltaPos = animator.applyRootMotion ? Vector3.zero : transform.position - prevPos;
        var velocity = deltaPos / Time.deltaTime;

        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        float normalizedSpeed = forwardSpeed / NavAgent.speed;
        if (float.IsNaN(normalizedSpeed) || float.IsInfinity(normalizedSpeed))
            normalizedSpeed = 0f;

        //apply to all conditions
        animator.SetFloat("Speed", normalizedSpeed, 0.2f, Time.deltaTime);

        animator.SetFloat("MotionSpeed", 1);

        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        if (float.IsNaN(angle) || float.IsInfinity(angle))
            angle = 0f;

        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        if (float.IsNaN(strafeSpeed) || float.IsInfinity(strafeSpeed))
            strafeSpeed = 0f;

        animator.SetFloat("StrafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

      //  Debug.Log("StrafeSpeed is " + strafeSpeed);

        if (Target?.health <= 0)
        {
            TargetsInRange.Remove(Target);
            EnemyManager.instance.RemoveEnemyInRange(this);

        }
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

    void ReactToHit()
    {
        ChangeState(EnemyStates.GettingHit);
    }

    public FighterBase FindTarget()
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

    void RegisterMaterialsFromRenderer()
    {
        foreach (Renderer r in rends)
        {
            foreach (var m in r.materials)
            {
                materials.Add(m);
            }
        }
    }

    public void GetHitEffect()
    {
        foreach (var mat in materials)
        {
            mat.SetColor("_FresnelColor", new Color(4, 0, 0));
            StartCoroutine(FadeFilterByColor(mat, new Color(4, 0, 0), new Color(0, 0, 0), 0.3f));
        }
    }

    private IEnumerator FadeFilterByColor(Material mat, Color from, Color to, float duration)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float curveT = HitFresnelCurve.Evaluate(t);

            mat.SetColor("_FresnelColor", Color.Lerp(from, to, curveT));
            yield return null;
        }

        mat.SetColor("_FresnelColor", to);
    }

    public void DeadEffect()
    {
        foreach (var mat in materials)
        {
            mat.SetFloat("_VerticalClipOffset", 0);
            StartCoroutine(FadeFilterByfloat(mat, 0,1,2f));
        }
    }

    private IEnumerator FadeFilterByfloat(Material mat, float from, float to, float duration)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float curveT = HitFresnelCurve.Evaluate(t);

            mat.SetFloat("_VerticalClipOffset", Mathf.Lerp(from, to, curveT));
            mat.SetFloat("_AlphaClipThresholdOffset", Mathf.Lerp(to, from, curveT));

            yield return null;
        }

        mat.SetFloat("_VerticalClipOffset", from);
        mat.SetFloat("_AlphaClipThresholdOffset", to);

        Destroy(gameObject);

    }

    private void OnEnable()
    {
        EnemyManager.instance.RegisterEnemy();
    }
    private void OnDisable()
    {
        EnemyManager.instance.UnregisterEnemy();
    }
}
