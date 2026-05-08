using System.Collections;
using UnityEngine;

class Gun : BaseInteraction
{
    public GunInfo gunInfo;

    public float currentAmountOfAmmoInClip, currentAmountOfClips;

    public bool canReload;

    public void Start()
    {
        canReload = true;
    }

    public void Update()
    {

        if (canReload && currentAmountOfAmmoInClip == 0)
        {
            Reload();
        }
    }

    public override void InteractItem()
    {
     
    }

    public override void ShowPickUpInfo()
    {

    }

    public void PrimaryAction()
    {
        if (canReload && currentAmountOfAmmoInClip <= 0)
        {
            Reload();
            return;
        }

        if (!canReload){ canReload = true; }
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
}
