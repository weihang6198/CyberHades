using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visionSensor : MonoBehaviour
{
    [SerializeField] EnemyController owner;
    private void Awake()
    {
        if(owner == null)
        {
            Debug.Log("vision sensor owner is null");
        }
        else
        {
            owner.VisionSensor = this;
        }
          
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("enter  vison sensor");
        var fighter = other.GetComponent<FighterBase>();
        if (fighter != null)
        {
            owner.TargetsInRange.Add(fighter);
            Debug.Log("add player to target in range");
            EnemyManager.instance.AddEnemyRange(owner);
        }else
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        ////Debug.Log("exit  vison sensor");
        //var fighter = other.GetComponent<FighterBase>();
        //var bossFighter = other.GetComponent<BossFighter>();

        //if (fighter != null)  //doesnt remove vision for boss fighter
        //{
        //    if (bossFighter != null) return;
        //    owner.TargetsInRange.Remove(fighter);
        //    EnemyManager.instance.RemoveEnemyInRange(owner);
        //}
    }
}
