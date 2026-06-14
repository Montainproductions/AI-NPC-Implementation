using UnityEngine;

[CreateAssetMenu(fileName = "GunInfo", menuName = "Scriptable Objects/GunInfo")]
public class GunInfo : ScriptableObject
{
    public string gunName, ammoType;

    public float maxAmountInClip, maxAmountOfClips;

    public float fireRate, timeBetweenShots, timeBetweenFireRates;

    public float reloadTime;
}
