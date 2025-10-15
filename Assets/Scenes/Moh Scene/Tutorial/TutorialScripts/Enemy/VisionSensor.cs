using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    [SerializeField] EnemyController enemy;
    private void Awake()
    {
        enemy.VisionSensor = this;
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("enter  vison sensor");
        var fighter=other.GetComponent<MeleeFighterTutorial>();
        if(fighter != null )
        {
            enemy.TargetsInRange.Add(fighter);
            Debug.Log("add player to target in range");
            EnemyManager.instance.AddEnemyRange(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit  vison sensor");
        var fighter = other.GetComponent<MeleeFighterTutorial>();
        if (fighter != null)
        {
            enemy.TargetsInRange.Remove(fighter);
            EnemyManager.instance.RemoveEnemyInRange(enemy);
        }
    }
}
