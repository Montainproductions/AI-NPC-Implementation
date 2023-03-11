using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    private GameObject player;

    private bool playerGun;

    private float dmgFromBullet;

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(bool playerGun, float damage){
        this.playerGun = playerGun;
        dmgFromBullet = damage;
    }

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(GameObject player, bool playerGun, float damage)
    {
        this.player = player;
        this.playerGun = playerGun;
        dmgFromBullet = damage;
    }

    public void OnTriggerEnter(Collider other){
        //Debug.Log(other.gameObject);
        //Damages an enemy if it has health
        if (other.gameObject.tag == "Enemy" && playerGun) {
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
        }
        else if (other.gameObject.tag == "Player" && !playerGun)
        {
            player.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
        }
        if (other.tag == "Player" && !playerGun) //Will destroy the bullet when it hits something that isnt the gun itself
        {
            Destroy(gameObject);
        }else if(other.tag != "Player" && playerGun)
        {
            Destroy(gameObject);
        }
    }
}
