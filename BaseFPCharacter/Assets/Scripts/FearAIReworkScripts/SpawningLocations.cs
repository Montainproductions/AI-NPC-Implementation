using UnityEngine;

public class SpawningLocations : MonoBehaviour
{
    public int area;

    public bool availableSpawningLocation = false;

    public bool recentlySpawnedZombie = false;
    public int timeBetweenSpawns = 1;

    public void SpawningLocationBecomeAvailable()
    {
        availableSpawningLocation = true;
    }

    public void SpawnZombie(GameObject prefabToSpawn, double health, Transform[] playerTransforms)
    {
        if (!availableSpawningLocation) { return; }
        Debug.Log("Spawning Enemy");
        GameObject newZombie = Instantiate(prefabToSpawn, transform.position, transform.rotation);
        newZombie.GetComponent<Zombies>().SetHealth(health);
        newZombie.GetComponent<Zombies>().SetPlayerTransforms(playerTransforms);
    }
}
