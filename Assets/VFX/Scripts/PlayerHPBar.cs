using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPBar : MonoBehaviour
{
    public Material HPBarMat;
    public MeleeFighter playerMeleeFighterClass;
    float MaxHP = 100f;
    float CurrentHP = 100f;

    private float currentPercent;
    private float diffPercent;
    private float target;

    public float reduceSpeed = 2f;
    public float backgroundreduceSpeed = 0.2f;

    void Start()
    {
        target = CurrentHP / MaxHP;
    }

    void Update()
    {
        if(playerMeleeFighterClass != null)
        {
            CurrentHP = playerMeleeFighterClass.health;
            MaxHP = playerMeleeFighterClass.maxHealth;
        }

        target = CurrentHP / MaxHP;
        currentPercent = Mathf.MoveTowards(currentPercent, target, Time.deltaTime * reduceSpeed);

        HPBarMat.SetFloat("_HPPercent", currentPercent);

        diffPercent = Mathf.MoveTowards(diffPercent, target, Time.deltaTime * backgroundreduceSpeed);
        HPBarMat.SetFloat("_HPDifPercent", diffPercent);
    }
}
