using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTutorial : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;

    Quaternion targetRotation;

    CameraController cameraController;
    private void Awake()
    {
        cameraController=Camera.main.GetComponent<CameraController>();
    }
    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Abs(h) + Mathf.Abs(v) ;

        var moveInput = (new Vector3(h, 0, v)).normalized;

        var moveDir= cameraController.PlaneRotation* moveInput;
        
        //player is moving
        if(moveAmount>0)
        {
            //move the chara relative to camera
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            //rotate chara relative to camera 
            targetRotation = Quaternion.LookRotation(moveDir);
        }

       transform.rotation=Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
    }
}
