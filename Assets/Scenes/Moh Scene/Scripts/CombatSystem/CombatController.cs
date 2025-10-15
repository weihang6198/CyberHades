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
            _input.attack = false;
            Debug.Log("attacking");
            meleeFighter.TryToAttack();
           
        }
    }
}
