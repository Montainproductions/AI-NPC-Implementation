using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BaseGun : MonoBehaviour {
    [SerializeField]
    private float dmgPerBullet, bulletSpeed;

    [SerializeField]
    private int bulletQuant;

    [SerializeField]
    private GameObject spawnBullet, mainHole;
    [SerializeField]
    private AudioSource audioSC;

    [SerializeField]
    private int maxAmmo, maxClipAmmo;
    private int currentAmmoAmount;
    private bool shotRecently;

    [SerializeField]
    private bool reloaded;
    [SerializeField]
    private float reloadTimer;

    // Start is called before the first frame update
    void Start()
    {
        maxAmmo -= maxClipAmmo;
        currentAmmoAmount = maxClipAmmo;
        shotRecently = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    //A Coroutine that runs whenever the player shoots the current gun.
    public IEnumerator ShotFired()
    {
        if (currentAmmoAmount > 0 && !shotRecently)
        {
            for (int i = 0; i < bulletQuant; i++)
            {
                currentAmmoAmount--;
                GameObject newBullet = Instantiate(spawnBullet, mainHole.transform);
                newBullet.GetComponent<Sc_Bullet>().SetDamageAmount(dmgPerBullet);
                newBullet.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.Impulse);
                Debug.Log(currentAmmoAmount);
                audioSC.Play();
                yield return new WaitForSeconds(0.15f);
            }
            shotRecently = true;
            yield return new WaitForSeconds(1);
        }
        else
        {
            //Play audio clip
            Debug.Log("Not enough ammo");
        }
        yield return null;
    }


    //Will reload gun
    public IEnumerator Reloading()
    {
        if (reloaded) yield return null;
        //Debug.Log("Reloading");
        reloaded = true;
        yield return new WaitForSeconds(reloadTimer);
        currentAmmoAmount = maxClipAmmo;
        maxAmmo -= maxClipAmmo;
        reloaded = false;
        //Debug.Log("Reloaded");
        yield return null;
    }
}
