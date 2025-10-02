using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float rotationSpeed = 2f;

    [SerializeField] float distance= 5f;
    [SerializeField] float minVerticalAngle = -45f;
    [SerializeField] float maxVerticalAngle = 45f;

    [SerializeField] Vector2 framingOffset;

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    float invertXVal;
    float invertYVal;

    float rotationX;
    float rotationY;

    private void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        invertXVal= (invertX)? -1 : 1;
        invertYVal = (invertY)? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed ;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        
        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        //where the cam look at the head
        var focusPosition=followTarget.position + new Vector3(framingOffset.x,framingOffset.y);

        transform.position = focusPosition - targetRotation  * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    public Quaternion PlaneRotation => Quaternion.Euler(0, rotationY, 0);
}
