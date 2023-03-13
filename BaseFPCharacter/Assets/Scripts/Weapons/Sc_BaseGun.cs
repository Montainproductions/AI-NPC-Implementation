using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BaseGun : MonoBehaviour {
    [SerializeField]
    private bool isPlayerGun;

    [SerializeField]
    private GameObject player, playerBox;

    [SerializeField]
    private float dmgPerBullet, bulletSpeed;

    [SerializeField]
    private int fireRate, effectiveRange;
    [SerializeField]
    private float timeBetweenShots, timeBetweenFireRates;

    [SerializeField]
    private GameObject spawnBullet, barrolHole;
    [SerializeField]
    private AudioSource audioSC;

    [SerializeField]
    private int maxAmmo, currentMaxAmmo, maxClipAmmo, currentAmmoAmount;
    [HideInInspector]
    public bool shotRecently;

    [SerializeField]
    private bool reloaded;
    [SerializeField]
    private float reloadTimer;

    // Start is called before the first frame update
    void Start()
    {
        currentMaxAmmo = maxAmmo;
        currentMaxAmmo -= maxClipAmmo;
        currentAmmoAmount = maxClipAmmo;
        shotRecently = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerGun)
        {
            Sc_Basic_UI.Instance.SetCurrentAmmo(currentAmmoAmount, currentMaxAmmo);
        }
    }

    //A Coroutine that runs whenever the player or the AI trys shoots the current gun.
    public IEnumerator ShotFired()
    {
        if (currentAmmoAmount > 0 && !shotRecently && !reloaded)
        {
            shotRecently = true;
            for (int i = 0; i < fireRate; i++)
            {
                currentAmmoAmount--;
                GameObject newBullet = Instantiate(spawnBullet, barrolHole.transform);
                if (!isPlayerGun) {
                    newBullet.GetComponent<Sc_Bullet>().SetDamageAmount(player, isPlayerGun, dmgPerBullet);
                }
                else
                {
                    newBullet.GetComponent<Sc_Bullet>().SetDamageAmount(playerBox, isPlayerGun, dmgPerBullet);
                }
                newBullet.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletSpeed, ForceMode.Impulse);
                //Debug.Log(currentAmmoAmount);
                audioSC.Play();
                yield return new WaitForSeconds(timeBetweenShots);
                //Debug.Log("Player ammo count: " + currentAmmoAmount);
            }
            yield return new WaitForSeconds(timeBetweenFireRates);
            shotRecently = false;
        }
        else if(currentAmmoAmount > 0 && shotRecently)
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
        currentMaxAmmo -= maxClipAmmo;
        reloaded = false;
        //Debug.Log("Reloaded");
        yield return null;
    }

    public int ReturnFireRate() { return fireRate; }

    public int ReturnCurrentAmmo() { return currentAmmoAmount; }

    public int ReturnMaxClipAmmo() { return maxClipAmmo; }

    public int ReturnEffectiveRange() { return effectiveRange; }

    public float ReturnReloadTimer() { return reloadTimer; }
}
