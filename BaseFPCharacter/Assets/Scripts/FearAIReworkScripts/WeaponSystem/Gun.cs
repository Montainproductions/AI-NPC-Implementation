using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private GunInfo gunInfo;

    private int currentAmmoInClip, currentAmountOfClips;

    private bool canReload, shotRecently;

    private bool beingHeld;
    private string heldByWhichTeam;

    [SerializeField]
    private GameObject prefabBullet;

    [SerializeField]
    private Transform gunExit;

    [SerializeField]
    private AudioSource audioSC;

    public static Action<int, int> ammoUsed;

    public void Start()
    {
        beingHeld = false;
        shotRecently = false;

        currentAmmoInClip = gunInfo.maxAmountInClip;
        currentAmountOfClips = gunInfo.maxAmountOfClips;
    }

    public void BindEvents()
    {
        PlayerInventory.reloadEvent += ReloadAction;
    }

    public void UnbindEvent()
    {
        PlayerInventory.reloadEvent -= ReloadAction;
    }

    public void PrimaryAction()
    {
        if (!beingHeld){ return; }
        
        if (currentAmmoInClip <= 0)
        {
            Debug.Log("No bullets left");
            return;
        }

        StartCoroutine(FiringWeapon());
    }

    public IEnumerator FiringWeapon()
    {
        if (shotRecently) { yield return null;}

        shotRecently = true;
        for (int i = 0; i < gunInfo.fireRate; i++)
        {
            if (currentAmmoInClip <= 0){ break; }

            currentAmmoInClip--;

            //Basic_UI.Instance.AmmoUsed(currentAmmoInClip, currentAmountOfClips);
            ammoUsed?.Invoke(currentAmmoInClip, currentAmountOfClips);

            GameObject newBullet = Instantiate(prefabBullet, gunExit.position, gunExit.rotation);

            newBullet.GetComponent<Bullet>().SetBulletInfo(heldByWhichTeam, 20);
            audioSC.Play();

            yield return new WaitForSeconds(gunInfo.timeBetweenShots);
        }
        yield return new WaitForSeconds(gunInfo.timeBetweenFireRates);
        shotRecently = false;

        yield return null;
    }

    public void SecondaryAction()
    {

    }

    public void ReloadAction(GunInfo infoGun)
    {
        if ((!gunInfo.isAllowedToReload && currentAmmoInClip > 0) || currentAmountOfClips <= 0) { return; }

        StartCoroutine(Reload());
    }

    public IEnumerator Reload()
    {
        canReload = false;
        float reloadTimer = 0;
        float timeDifference = 0.1f;
        while (reloadTimer < gunInfo.reloadTime)
        {
            yield return new WaitForSeconds(timeDifference);
            reloadTimer += timeDifference;
        }

        currentAmountOfClips--;
        currentAmmoInClip = gunInfo.maxAmountInClip;

        ammoUsed?.Invoke(currentAmmoInClip, currentAmountOfClips);
    }

    public void IsBeingHeld(string whichTeam, bool isPlayer)
    {
        heldByWhichTeam = whichTeam;

        beingHeld = true;

        if (isPlayer && beingHeld)
        {
            ammoUsed?.Invoke(currentAmmoInClip, currentAmountOfClips);
        }

        BindEvents();
    }

    public void NoLongerBeingHeld()
    {
        beingHeld = false;
        UnbindEvent();
    }

    public GunInfo GetGunInfo() {  return gunInfo; }
}
