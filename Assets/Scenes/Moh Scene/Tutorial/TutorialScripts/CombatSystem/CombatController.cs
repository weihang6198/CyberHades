using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    MeleeFighter meleeFighter;

    private void Awake()
    {
        meleeFighter = GetComponent<MeleeFighter>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            meleeFighter.TryToAttack(); 
        }
    }
}
