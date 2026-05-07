using UnityEngine;

[CreateAssetMenu(fileName = "GunInfo", menuName = "Scriptable Objects/GunInfo")]
public class GunInfo : ScriptableObject
{
    string gunName, ammoType;

    float amountOfClips, currentAmmoInClip, maxAmmoInClip;

    float fireRate;

}
