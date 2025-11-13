using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class DeadState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
         Debug.Log("enter dead state of enemy");
        owner.VisionSensor.gameObject.SetActive(false);
        EnemyManager.instance.RemoveEnemyInRange(owner);

        owner.NavAgent.enabled = false;
        owner.CharacterController.enabled = false;
        
    }
}


public class Human //Element
{
    //element is a group of collection of elements/attributes
    string name; //attribute
    int age;//attribute
    string hobby;//attribute

    GameObject Assets; 
    GameObject house;
    GameObject Car;
}

public class Car //Element  Parent class
{
    int number;
    string color;
    int size;

    GameObject tire;// child class
    GameObject steerings;// child class
    GameObject solarPanel;// child class


}

public class Tire
{
    string color;
    int size;

}

public class steerings
{
    string color;
    int size;

}