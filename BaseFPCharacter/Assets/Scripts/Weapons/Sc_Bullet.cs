using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour{
    private GameObject player;

    private string fromWho;

    private float dmgFromBullet;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private Rigidbody rb;

    public void Start()
    {
        StartCoroutine(BulletAlive());
        rb.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.Impulse);
    }

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(string sourceOfBullet, float damage){
        fromWho = sourceOfBullet;
        dmgFromBullet = damage;
    }

    IEnumerator BulletAlive()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
        yield return null;
    }

    public void OnTriggerEnter(Collider other){
        //Debug.Log(other.gameObject);
        //Damages an enemy if it has health

        if (other.gameObject.tag == "Enemy") {
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Sc_Health>().TakeDamage(dmgFromBullet);
            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "Walls" || other.gameObject.tag == "Cover") {
            Destroy(gameObject);
        }
    }
}
