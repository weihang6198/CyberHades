using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] public FighterBase meleeFighterClass;
    [SerializeField] public Transform canvasTransform;
    [SerializeField] public float reduceSpeed = 2.0f;
    public bool isHPChanged;
    public float prevHealth;

    private float targetHP = 0; 

    private void Update()
    {
  
        canvasTransform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        if (meleeFighterClass != null)
        {
            prevHealth = meleeFighterClass.health;
            isHPChanged = prevHealth != meleeFighterClass.health ? true : false;
            UpdateHealthBar(meleeFighterClass.maxHealth, meleeFighterClass.health);
            healthBarImage.fillAmount = Mathf.MoveTowards(healthBarImage.fillAmount, targetHP, reduceSpeed * Time.deltaTime);
        }
        else
        {
           // Debug.Log("meleeFighterClass missing!");
        }
    }
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        targetHP = currentHealth / maxHealth;
    }
}
