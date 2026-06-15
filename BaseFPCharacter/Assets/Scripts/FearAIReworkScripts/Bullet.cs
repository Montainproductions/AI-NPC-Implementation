using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour{
    private GameObject player;

    private string fromWhichTeam;

    private float dmgFromBullet;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private Rigidbody rb;

    public void Start()
    {
        StartCoroutine(BulletAlive());
        BulletRelease();
    }

    public void BulletRelease()
    {
        rb.AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.Impulse);
    }

    //Will set the damage of the bullet for when it impacts an object with health
    public void SetDamageAmount(string teamSourceOfBullet, float damage){
        fromWhichTeam = teamSourceOfBullet;
        dmgFromBullet = damage;
    }

    IEnumerator BulletAlive()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
        yield return null;
    }

    public void OnTriggerEnter(Collider other){
        CharacterGeneral charGeneral = other.gameObject.GetComponent<CharacterGeneral>();
        if (charGeneral == null) { return; }

        List<string> mainTag = charGeneral.GetCharacterData().fullTagList;
        
        if (mainTag == null) { return; }

        if (mainTag[0] == "Damagable" && mainTag[1] != fromWhichTeam) { other.gameObject.GetComponent<HealthSystem>().GetHit(dmgFromBullet); }

        Destroy(gameObject);
    }
}
