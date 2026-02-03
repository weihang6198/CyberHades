using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardFIghter : FighterBase
{
    public override void SpawnSlashEffect()
    {
        
    }
    protected override void Awake()
    {
        base.Awake(); // runs FighterBase.Awake()

    }
    public override bool CanAttack(Vector3 targetPosition, float attackDistance)
    {
        return true;


    }
    public override void TryToAttack(FighterBase target = null)
    {
        if (!InAction)
        {
            //Debug.Log("start couroutine atk function");

            StartCoroutine(Attack(target));

        }

    }

    public override IEnumerator Attack(FighterBase target = null)
    {
        yield return null;

    }
}
