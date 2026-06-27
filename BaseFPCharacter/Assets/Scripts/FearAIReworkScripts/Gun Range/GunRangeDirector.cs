using System;
using System.Collections;
using UnityEngine;

public class GunRangeDirector : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnLocationsForTargets;

    [SerializeField]
    private GameObject[] lightObjectsForWarning;

    public static Action<int, float> setLightsActive;

    [SerializeField]
    private GameObject targetPrefabs;

    [SerializeField]
    private float timeBetweenSpawns;

    [SerializeField]
    private int maxAmountOfTargets;

    private bool gunRangeActive;

    private int locationToSpawn, currentAmountOfTargets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunRangeActive = false;
    }

    public void BindEvent()
    {
        NonPlayerCharacter.onKilled += TargetTakenDown;
    }

    public void UnbindEvent()
    {
        NonPlayerCharacter.onKilled -= TargetTakenDown;
    }

    public void StartGunRange()
    {
        gunRangeActive = true;
        StartCoroutine(GunRangeLaneActive());
    }

    public void TargetTakenDown(int points)
    {
        StartCoroutine(GunRangeLaneActive());
    }

    public IEnumerator GunRangeLaneActive()
    {
        if (!gunRangeActive){ yield return null; }

        StartCoroutine(SpawnTarget());

        yield return null;
    }

    public void EndGunRange()
    {
        gunRangeActive = false;
    }

    public IEnumerator SpawnTarget()
    {
        UnbindEvent();

        locationToSpawn = UnityEngine.Random.Range(0, 3);

        setLightsActive?.Invoke(locationToSpawn, timeBetweenSpawns);

        yield return new WaitForSeconds(timeBetweenSpawns);

        Instantiate(targetPrefabs, spawnLocationsForTargets[locationToSpawn].position, Quaternion.identity);
        
        BindEvent();

        yield return null;
    }

    public void OnTriggerEnter(Collider collision)
    {
        StartGunRange();
    }

    public void OnTriggerExit(Collider collision)
    {
        EndGunRange();
    }

    private void OnDestroy()
    {
        UnbindEvent();
    }
}
