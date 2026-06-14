using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private GunInfo gunInfo;

    private float currentAmountOfAmmoInClip, currentAmountOfClips;

    private bool canReload, shotRecently;

    private bool beingHeld;
    private string heldByWhichTeam;

    [SerializeField]
    private GameObject prefabBullet, gunExit;

    [SerializeField]
    private AudioSource audioSC;

    public void Start()
    {
        beingHeld = false;
        shotRecently = false;


        currentAmountOfAmmoInClip = gunInfo.maxAmountInClip;
        currentAmountOfClips = gunInfo.maxAmountOfClips;
    }

    public void PrimaryAction()
    {
        if (!beingHeld){ return; }

        if (canReload && currentAmountOfAmmoInClip <= 0)
        {
            Reload();
            return;
        }

        StartCoroutine(FiringWeapon());
    }

    public IEnumerator FiringWeapon()
    {
        if (!shotRecently)
        {
            shotRecently = true;
            for (int i = 0; i < gunInfo.fireRate; i++)
            {
                if (currentAmountOfAmmoInClip <= 0){ yield return null; }

                currentAmountOfAmmoInClip--;
                GameObject newBullet = Instantiate(prefabBullet, gunExit.transform);

                newBullet.GetComponent<Sc_Bullet>().SetDamageAmount(heldByWhichTeam, 20);

                audioSC.Play();

                yield return new WaitForSeconds(gunInfo.timeBetweenShots);
            }
            yield return new WaitForSeconds(gunInfo.timeBetweenFireRates);
            shotRecently = false;
        }

        yield return null;
    }

    public void SecondaryAction()
    {

    }

    public IEnumerator Reload()
    {
        canReload = false;
        float reloadTimer = 0;
        float timeDifference = 0.2f;
        while (reloadTimer < gunInfo.reloadTime)
        {
            yield return new WaitForSeconds(timeDifference);
            reloadTimer += timeDifference;
        }

        currentAmountOfClips--;
        currentAmountOfAmmoInClip = gunInfo.maxAmountInClip;
    }

    public void IsBeingHeld(string whichTeam = "")
    {
        heldByWhichTeam = whichTeam;

        beingHeld = !beingHeld;
    }
}
