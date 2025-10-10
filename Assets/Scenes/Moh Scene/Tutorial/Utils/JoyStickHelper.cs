using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickHelper : MonoBehaviour
{
    public  static JoyStickHelper instance {  get; private set; }
    private void Awake()
    {
        instance = this;
    }
    Dictionary<string, bool> axisStates=new Dictionary<string, bool>();

   public bool GetAxisDown(string axisName)
    {
        bool currentlyPressed =Mathf.Abs( Input.GetAxisRaw(axisName)) > 0.1f;

        if (axisStates.ContainsKey(axisName))
        {
            axisStates.Add(axisName, false);
        }
        bool previouslyPressed = axisStates[axisName];
        axisStates[axisName] = currentlyPressed;

        //newly pressed
        if (!previouslyPressed && currentlyPressed)
            return true;
       

        //already pressed
        return false;

    }
}
