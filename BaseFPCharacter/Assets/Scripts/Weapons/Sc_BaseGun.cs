using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BaseGun : MonoBehaviour {
    [SerializeField]
    private float dmgPerBullet, bulletSpeed;


    public int fireRate, effectiveRange;

    [SerializeField]
    private GameObject spawnBullet, barrolHole;
    [SerializeField]
    private AudioSource audioSC;

    [SerializeField]
    private int maxAmmo, maxClipAmmo;
    [HideInInspector]
    public int currentAmmoAmount;
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

    //A Coroutine that runs whenever the player or the AI trys shoots the current gun.
    public IEnumerator ShotFired()
    {
        if (currentAmmoAmount > 0 && !shotRecently)
        {
            shotRecently = true;
            for (int i = 0; i < fireRate; i++)
            {
                currentAmmoAmount--;
                GameObject newBullet = Instantiate(spawnBullet, barrolHole.transform);
                newBullet.GetComponent<Sc_Bullet>().SetDamageAmount(dmgPerBullet);
                newBullet.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.Impulse);
                //Debug.Log(currentAmmoAmount);
                audioSC.Play();
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(1);
            shotRecently= false;
        }
        else if(shotRecently)
        {
            //Play audio clip
            Debug.Log("Recently Shot");
        }
        else
        {
            Debug.Log("No more ammo");
        }
        yield return null;
    }


    //Will reload gun
    public IEnumerator Reloading()
    {
        if (reloaded || maxAmmo <= 0) yield return null;
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
