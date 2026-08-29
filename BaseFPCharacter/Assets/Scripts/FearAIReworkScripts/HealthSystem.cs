using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    //Health
    [SerializeField]
    [Tooltip("Current health that the character has at any point.")]
    [Range(0, 1000)]
    private double currentHealth;

    [SerializeField]
    [Tooltip("Max health character can have at any point in time.")]
    [Range(0, 1000)]
    private double maxHealth;

    [SerializeField]
    [Tooltip("Can the character heal healingRate HP per second?")]
    private bool healingOverTimeAllowed;
    private float lastTimeHitTimer, waitTimeBeingHit; //Timer to know when to start the self healing
    private bool recentlyHit; //If the character was recently hit most for the healing

    [SerializeField]
    [Tooltip("Rate in which the character will heal over time.")]
    [Range(0, 1000)]
    private int healingRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

        waitTimeBeingHit = 7.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Death(string bodyPartHit, string fromWho)
    {
        gameObject.GetComponent<NonPlayerCharacter>().WhatKilledThem(bodyPartHit, fromWho);
        Destroy(gameObject);
    }

    public void GetHit(float healthDamage, string bodyPartHit, string fromWho)
    {
        Debug.Log("Got Hit by: " + fromWho);
        Debug.Log(" ");
        recentlyHit = true;

        switch (bodyPartHit)
        {
            case "Head":
                currentHealth -= (healthDamage*2); 
                break;
            case "MainBody":
                currentHealth -= (healthDamage*1.4f);
                break;
            default:
                currentHealth -= healthDamage;
                break;
        }

        Debug.Log("Current Health is: " + currentHealth);
        Debug.Log(" ");
        if (currentHealth <= 0)
        {
            Death(bodyPartHit, fromWho);
            return;
        }else if (currentHealth <= (maxHealth * 0.2))
        {
            //LowHealthUI();
        }

        StartCoroutine(TimerBeforeRecentlyHitReset());
    }

    IEnumerator TimerBeforeRecentlyHitReset()
    {
        yield return new WaitForSeconds(waitTimeBeingHit);
        recentlyHit = false;

        if (healingOverTimeAllowed)
        {
            StartCoroutine(HealingOverTime());
        }
    }

    IEnumerator HealingOverTime()
    {
        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            yield break;
        }


        yield return new WaitForSeconds(0.1f);
        currentHealth += maxHealth * 0.001f;

        StartCoroutine(HealingOverTime());
    }

    public void SetHealth(double newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = newMaxHealth;
    }

    public double CurrentHealthValue()
    {
        return currentHealth;
    }

    public double MaxHealthValue()
    {
        return maxHealth;
    }

    public bool ReturnRecentlyHit()
    {
        return recentlyHit;
    }
}
