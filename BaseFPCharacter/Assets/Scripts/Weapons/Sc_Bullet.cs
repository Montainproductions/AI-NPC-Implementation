using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    [SerializeField]
    private string[] targets;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float speed;

    private float dmgFromBullet;

    private void Update(){
        transform.position += transform.position * speed * Time.deltaTime;
    }

    public void SetDamageAmount(float damage){
        dmgFromBullet = damage;
    }

    public void OnTriggerEnter(Collider other){
        if(other.tag == "Enemy"){
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
            Destroy(gameObject);
        }
    }
}
