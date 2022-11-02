using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Health : MonoBehaviour{

    //Health
    [SerializeField]
    [Tooltip("Current health that the character has at any point.")]
    [Range(0,1000)]
    private float currentHealth;
    [SerializeField]
    [Tooltip("Max health character can have at any point in time.")]
    [Range(0, 1000)]
    private float maxHealth;
    private float lastTimeHitTimer;
    private bool recentlyHit;
    [SerializeField]
    [Tooltip("Can the character heal X HP per second?")]
    [Range(0, 1000)]
    private bool healingAllowed;
    [SerializeField]
    [Tooltip("Rate in which the character will heal over time.")]
    [Range(0, 1000)]
    private int healingRate;

    // Start is called before the first frame update
    void Start(){
        currentHealth = maxHealth;
        lastTimeHitTimer = 0;
        recentlyHit = false;
    }

    // Update is called once per frame
    void Update(){
        if(currentHealth <= 0){} //If no more Health then tell the game manager to go to end game

        if (healingAllowed){
            Healing();
        }
    }

    public void Healing(){
        if(currentHealth >= maxHealth){
            currentHealth = maxHealth;
            return;
        }

        if(recentlyHit){lastTimeHitTimer += Time.deltaTime;}
        else if(recentlyHit && lastTimeHitTimer > 5.0f){
            lastTimeHitTimer = 0;
            recentlyHit = false;
        }else{
            lastTimeHitTimer = 0;
            currentHealth += healingRate * Time.deltaTime;
        }
    }

    public void TakeDamage(float damage){currentHealth -= damage;}
}
