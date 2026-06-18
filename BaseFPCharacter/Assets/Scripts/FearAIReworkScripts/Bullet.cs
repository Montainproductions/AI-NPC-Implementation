using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour{
    private int maxPassThrough, currentPassThrough;

    private string gunSource;

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
    public void SetBulletInfo(string gunSource, float baseDamage){
        this.gunSource = gunSource;
        dmgFromBullet = baseDamage;
    }

    IEnumerator BulletAlive()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        yield return null;
    }

    public void OnTriggerEnter(Collider other){
        CharacterGeneral charGeneral = other.gameObject.GetComponentInParent<CharacterGeneral>();
        Debug.Log(other.name);

        if (charGeneral == null) { return; }

        List<string> mainTag = charGeneral.GetCharacterData().fullTagList;
        
        if (mainTag == null) { return; }

        if (mainTag[0] == "Damagable") {
            Debug.Log("Damagable Item Found");
            other.gameObject.GetComponentInParent<HealthSystem>().GetHit(dmgFromBullet, other.gameObject.tag, gunSource); 
        }

        if(currentPassThrough >= maxPassThrough || other.gameObject.tag == "Wall") { Destroy(gameObject); }
        else { currentPassThrough++; }
    }
}
