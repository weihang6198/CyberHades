using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visionSensor : MonoBehaviour
{
    [SerializeField] EnemyController owner;
    private void Awake()
    {
        owner.VisionSensor = this;
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("enter  vison sensor");
        var fighter = other.GetComponent<MeleeFighter>();
        if (fighter != null)
        {
            owner.TargetsInRange.Add(fighter);
            Debug.Log("add player to target in range");
            EnemyManager.instance.AddEnemyRange(owner);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit  vison sensor");
        var fighter = other.GetComponent<MeleeFighter>();
        if (fighter != null)
        {
            owner.TargetsInRange.Remove(fighter);
            EnemyManager.instance.RemoveEnemyInRange(owner);
        }
    }
}
