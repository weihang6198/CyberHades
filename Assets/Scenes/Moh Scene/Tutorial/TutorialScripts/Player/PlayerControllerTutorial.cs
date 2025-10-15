using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControllerTutorial : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float animBlendInTime = 0.1f;

    [Header("Ground check settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;
    float ySpeed;

    public float moveAmount;

    Quaternion targetRotation;

    public Vector3 InputDir {  get; private set; }

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;
    MeleeFighterTutorial meleeFighter;

    CombatControllerTutorial combatController;

    public static PlayerControllerTutorial instance { get; private set; } 
    private void Awake()
    {
        cameraController=Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController=GetComponent<CharacterController>();
        meleeFighter=GetComponent<MeleeFighterTutorial>();
        combatController=GetComponent<CombatControllerTutorial>();

        instance = this;
    }
    private void Update()
    {

        if (meleeFighter.InAction ||meleeFighter.health<=0)
        {
            targetRotation = transform.rotation;
            animator.SetFloat("ForwardSpeed",0f);
            return; //if player is attacking , return
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

         moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        var moveInput = (new Vector3(h, 0, v)).normalized;

        var moveDir= cameraController.PlaneRotation* moveInput;
        InputDir= moveDir; 

        GroundCheck();
        //Debug.Log("GroundCheck :" + isGrounded);

        if(isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }
            var velocity = moveDir * moveSpeed;
       
        //if player is in combat mode (player lock on to enemy)
        //rotate player and cam towards lock on enemy
        if (combatController.CombatMode)
        {
            //player cannot run in this mode
            velocity /= 2f;

            //Rotate and face the target enemy
            var targetVec=combatController.TargetEnemy.transform.position - transform.position;
            targetVec.y = 0;

            //only do this if player is moving
            if (moveAmount > 0)
            {
                //rotate chara relative to camera 
                targetRotation = Quaternion.LookRotation(targetVec);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
                    rotationSpeed * Time.deltaTime);

            }
            //split the velocity into its forward and sideward comp and set it intot he forwardSpeed and strafeSpeed
            float forwardSpeed = Vector3.Dot(velocity, transform.forward);
            //apply to all conditions
            animator.SetFloat("ForwardSpeed", forwardSpeed / moveSpeed, 0.2f, Time.deltaTime);

            float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
            float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);

            animator.SetFloat("StrafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);
            
        }
        else
        {
            //player is moving
            if (moveAmount > 0)
            {
                //rotate chara relative to camera 
                targetRotation = Quaternion.LookRotation(moveDir);
                //transform.position += moveDir * moveSpeed * Time.deltaTime; //old 

            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            animator.SetFloat("ForwardSpeed", moveAmount, animBlendInTime, Time.deltaTime);
      
        }
        //apply gravity if player not on ground 
        velocity.y = ySpeed;

        //move the chara relative to camera,vertical movement is also applied
        characterController.Move(velocity * Time.deltaTime);


    }

    void GroundCheck()
    {
        isGrounded=Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    public Vector3 GetIntentDirection()
    {
        return InputDir != Vector3.zero ? PlayerControllerTutorial.instance.InputDir : transform.forward;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}

