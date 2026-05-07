using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    //Health
    [SerializeField]
    [Tooltip("Current health that the character has at any point.")]
    [Range(0, 1000)]
    private float currentHealth;

    [SerializeField]
    [Tooltip("Max health character can have at any point in time.")]
    [Range(0, 1000)]
    private float maxHealth;

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

    void Death()
    {

    }

    void GetHit(float healthDamage)
    {
        recentlyHit = true;
        currentHealth -= healthDamage;

        if (currentHealth <= 0)
        {
            Death();
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

    public float CurrentHealthValue()
    {
        return currentHealth;
    }

    public float MaxHealthValue()
    {
        return maxHealth;
    }

    public bool ReturnRecentlyHit()
    {
        return recentlyHit;
    }
}
