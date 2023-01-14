using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    private float dmgFromBullet;

    //Will set the damage of the bullet for when it impacts an enemy
    public void SetDamageAmount(float damage){
        dmgFromBullet = damage;
    }

    public void OnTriggerEnter(Collider other){
        //Damages an enemy if it has health
        if(other.gameObject.GetComponent<Sc_Health>()){
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
        }

        //Will destroy the bullet when it hits something that isnt the gun itself
        if (other.tag != "Gun")
        {
            Destroy(gameObject);
        }
    }
}
