using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    private GameObject player;

    private bool playerGun;

    private float dmgFromBullet;

    public void Start()
    {
        StartCoroutine(BulletAlive());
    }

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(bool playerGun, float damage){
        this.playerGun = playerGun;
        dmgFromBullet = damage;
    }

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(GameObject player, bool playerGun, float damage)
    {
        this.player = player;
        //Debug.Log(player);
        this.playerGun = playerGun;
        dmgFromBullet = damage;
    }

    IEnumerator BulletAlive()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
        yield return null;
    }

    public void OnTriggerEnter(Collider other){
        Debug.Log(other.gameObject);
        //Damages an enemy if it has health

        if (other.gameObject.tag == "Enemy" && playerGun) {
            Debug.Log("Enemy Hit");
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player" && !playerGun)
        {
            player.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "Walls" || other.gameObject.tag == "Cover") {
            Destroy(gameObject);
        }
    }
}
