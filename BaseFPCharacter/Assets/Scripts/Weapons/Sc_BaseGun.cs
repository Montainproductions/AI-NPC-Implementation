using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BaseGun : MonoBehaviour{
    [SerializeField]
    private float dmgPerBullet;

    [SerializeField]
    private int bulletQuant;

    [SerializeField]
    private GameObject spawnBullet;
    [SerializeField]
    private GameObject mainHole;

    [SerializeField]
    private int maxAmmo;
    [SerializeField]
    private int maxClipAmmo;
    private int currentAmmoAmount;

    [SerializeField]
    private bool reloaded;
    [SerializeField]
    private float reloadTimer;

    // Start is called before the first frame update
    void Start()
    {
        maxAmmo -= maxClipAmmo;
        currentAmmoAmount = maxClipAmmo;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator ShotFired()
    {
        if (currentAmmoAmount > 0)
        {
            for (int i = 0; i < bulletQuant; i++)
            {
                currentAmmoAmount--;
                GameObject newBullet = Instantiate(spawnBullet, mainHole.transform);
                newBullet.GetComponent<Sc_Bullet>().SetDamageAmount(dmgPerBullet);
                newBullet.GetComponent<Rigidbody>().AddForce(0, 0, 1, ForceMode.Impulse);
            }
        }
        else
        {
            //Play audio clip
            Debug.Log("Not enough ammo");
        }
        yield return null;
    }



    public IEnumerator Reloading()
    {
        yield return new WaitForSeconds(reloadTimer);
        currentAmmoAmount = maxClipAmmo;
        maxAmmo -= maxClipAmmo;
        yield return null;
    }
}
