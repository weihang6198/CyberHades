using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPBar : MonoBehaviour
{
    public Material HPBarMat;
    public MeleeFighter playerMeleeFighterClass;
    public float MaxHP = 100f;
    public float CurrentHP = 100f;

    private float prevPercent;
    private float diffPercent;

    public float lerpSpeed = 2f;

    void Start()
    {
        prevPercent = CurrentHP / MaxHP;
        diffPercent = prevPercent;
    }

    void Update()
    {
        if(playerMeleeFighterClass != null)
        {
            CurrentHP = playerMeleeFighterClass.health;
            MaxHP = playerMeleeFighterClass.maxHealth;
        }

        float currentPercent = CurrentHP / MaxHP;

        HPBarMat.SetFloat("_HPPercent", currentPercent);

        diffPercent = Mathf.Lerp(diffPercent, currentPercent, Time.deltaTime * lerpSpeed);
        HPBarMat.SetFloat("_HPDifPercent", diffPercent);

        prevPercent = currentPercent;
    }
}
