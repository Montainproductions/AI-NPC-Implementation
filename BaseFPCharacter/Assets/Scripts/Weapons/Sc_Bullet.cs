using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    private GameObject player;

    private float dmgFromBullet;

    //Will set the damage of the bullet for when it impacts an enemy
    public void SetDamageAmount(GameObject player, float damage){
        this.player = player;
        dmgFromBullet = damage;
    }

    public void OnTriggerEnter(Collider other){
        Debug.Log(other.gameObject);
        //Damages an enemy if it has health
        if (other.gameObject.GetComponent<Sc_Health>())
        {
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
        }

        /*if (other.gameObject.layer == 6){
            /*if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
            }

            player.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
        }*/

        //Will destroy the bullet when it hits something that isnt the gun itself
        if (other.tag != "Gun")
        {
            Destroy(gameObject);
        }
    }
}
