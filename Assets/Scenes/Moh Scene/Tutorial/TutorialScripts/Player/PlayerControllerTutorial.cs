using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;
    MeleeFighter meleeFighter;
    private void Awake()
    {
        cameraController=Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController=GetComponent<CharacterController>();
        meleeFighter=GetComponent<MeleeFighter>();
    }
    private void Update()
    {

        if (meleeFighter.InAction)
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
        //apply gravity if player not on ground 
        velocity.y = ySpeed;

        //move the chara relative to camera,vertical movement is also applied
        characterController.Move(velocity * Time.deltaTime);
        //player is moving
        if (moveAmount>0)
        {
           
           
            //transform.position += moveDir * moveSpeed * Time.deltaTime; //old 

            //rotate chara relative to camera 
            targetRotation = Quaternion.LookRotation(moveDir);
        }

       transform.rotation=Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

       animator.SetFloat("ForwardSpeed", moveAmount, animBlendInTime, Time.deltaTime);
       // animator.SetFloat("MoveAmount", moveAmount);
    }

    void GroundCheck()
    {
        isGrounded=Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
