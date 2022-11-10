using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BaseGun : MonoBehaviour{
    [SerializeField]
    private bool canShoot;
    [SerializeField]
    private bool hasShot;

    [SerializeField]
    private GameObject spawnBullets;
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
    private float currentReloadTimer;

    // Start is called before the first frame update
    void Start(){
        maxAmmo -= maxClipAmmo;
        currentAmmoAmount = maxClipAmmo;
    }

    // Update is called once per frame
    void Update(){
        if(!canShoot) return;

        if(reloaded && maxAmmo >= maxClipAmmo){
            Reloaded();
        }else if(maxAmmo < maxClipAmmo){
            //Play audio clip
            Debug.Log("Not enough ammo");
        }

        if (!hasShot && currentAmmoAmount > 0) return;
        Shotfired();
    }

    public void Shotfired(){
        currentAmmoAmount--;
        Instantiate(spawnBullets, mainHole.transform);
    }

    public void Reloaded(){
        //Play reloading audio clip
        currentReloadTimer += Time.deltaTime;
        if(currentReloadTimer >= reloadTimer) {
            currentAmmoAmount = maxClipAmmo;
            maxAmmo -= maxClipAmmo;
        }
    }
}
