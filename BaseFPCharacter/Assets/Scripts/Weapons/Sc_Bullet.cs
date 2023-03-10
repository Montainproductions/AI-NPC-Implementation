using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    private GameObject playersBox;

    private bool playerGun;

    private float dmgFromBullet;

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(float damage){
        dmgFromBullet = damage;
    }

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(GameObject playersBox, bool playerbullets, float damage)
    {
        this.playersBox = playersBox;
        playerGun = playerbullets;
        dmgFromBullet = damage;
    }

    public void OnTriggerEnter(Collider other){
        //Debug.Log(other.gameObject);
        //Damages an enemy if it has health
        if (other.gameObject == playersBox && playerGun) { return; }
        else if (other.gameObject.GetComponent<Sc_Health>())
        {
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
        }
        if (other.tag != "Gun") //Will destroy the bullet when it hits something that isnt the gun itself
        {
            Destroy(gameObject);
        }
    }
}
