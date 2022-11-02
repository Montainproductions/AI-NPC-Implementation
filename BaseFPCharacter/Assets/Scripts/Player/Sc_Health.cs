using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Health : MonoBehaviour{

    //Health
    public float currentHealth, maxHealth;
    private float lastTimeHitTimer;
    private bool recentlyHit;

    // Start is called before the first frame update
    void Start(){
        currentHealth = maxHealth;
        lastTimeHitTimer = 0;
        recentlyHit = false;
    }

    // Update is called once per frame
    void Update(){
        if(currentHealth <= 0){} //If no more Health then tell the game manager to go to end game

        Healing();
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
            currentHealth += 1 * Time.deltaTime;
        }
    }

    public void TakeDamage(float damage){currentHealth -= damage;}
}
