using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalAttackDirection : MonoBehaviour
{
 
    void Update()
    {
        RotatePlaneTowardMouse();
    }

    Vector3 GetMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f; // keep rotation flat
            return direction.normalized;
        }

        // fallback (if something goes wrong)
        return transform.forward;
    }

    public Quaternion GetQuaternionRotateTowardMouse()
    {

        Vector3 direction = GetMouseDirection();
        if (direction.sqrMagnitude > 0.01f)
        {
            return Quaternion.LookRotation(direction);
        }
        else
            return Quaternion.identity;
    }
    void RotatePlaneTowardMouse()
    {
        transform.rotation = GetQuaternionRotateTowardMouse();
    }
}
