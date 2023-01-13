using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    private float dmgFromBullet;

    public void SetDamageAmount(float damage){
        dmgFromBullet = damage;
    }

    public void OnTriggerEnter(Collider other){
        if(other.gameObject.GetComponent<Sc_Health>()){
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
        }

        if (other.tag != "Gun")
        {
            Destroy(gameObject);
        }
    }
}
