using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    private StarterAssetsInputs _input;

 
   MeleeFighter meleeFighter;

    private void Awake()
    {
        meleeFighter = GetComponent<MeleeFighter>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (_input.attack)
        {
            Debug.Log("attack inside combat controller");
            _input.attack = false;
        }
    }
}
