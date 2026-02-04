using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarFill : MonoBehaviour
{
    public Material HPBarMat;
    public FighterBase FighterBaseClass;
    float MaxHP = 100f;
    float CurrentHP = 100f;
    float PreviousHP = 100f;

    private float currentPercent;
    private float diffPercent;
    private float target;

    public float reduceSpeed = 2f;
    public float backgroundreduceSpeed = 0.2f;

    public bool isHPChanged = false;

    void Start()
    {
        target = CurrentHP / MaxHP;
    }

    void Update()
    {
        PreviousHP = CurrentHP;
        if (FighterBaseClass != null)
        {
            CurrentHP = FighterBaseClass.health;
            MaxHP = FighterBaseClass.maxHealth;
        }

        target = CurrentHP / MaxHP;
        currentPercent = Mathf.MoveTowards(currentPercent, target, Time.deltaTime * reduceSpeed);

        HPBarMat.SetFloat("_HPPercent", currentPercent);

        diffPercent = Mathf.MoveTowards(diffPercent, target, Time.deltaTime * backgroundreduceSpeed);
        HPBarMat.SetFloat("_HPDifPercent", diffPercent);

        isHPChanged = PreviousHP != CurrentHP ? true : false;
    }
}
