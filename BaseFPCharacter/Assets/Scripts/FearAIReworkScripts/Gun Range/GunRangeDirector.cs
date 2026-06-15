using System.Collections;
using UnityEngine;

public class GunRangeDirector : MonoBehaviour
{
    [SerializeField]
    private BoxCollider[] spawnLocationsForTargets;

    [SerializeField]
    private GameObject targetPrefabs;

    private bool targetAlreadySpawned;

    [SerializeField]
    private float timeBetweenSpawns;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnTarget());
    }

    public void TargetTakenDown()
    {
        StartCoroutine(SpawnTarget());
    }

    public IEnumerator SpawnTarget()
    {
        int areaToSpawn = Random.Range(0, 4);

        Bounds bounds = spawnLocationsForTargets[areaToSpawn].bounds;

        // Generate a random position within the bounds
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            -0.5f,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        yield return new WaitForSeconds(timeBetweenSpawns);

        Instantiate(targetPrefabs, randomPosition, Quaternion.identity);

        yield return null;
    }
}
