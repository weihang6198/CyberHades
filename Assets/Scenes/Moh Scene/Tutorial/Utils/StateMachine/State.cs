using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T> : MonoBehaviour
{
    public string stateName { get; private set; }

    public void  RegisterName(string stateName)
    {
        this.stateName= stateName;
    }
    public virtual void Enter(T owner) { }

    public virtual void Execute() { }
    public virtual void Exit() { }


}
