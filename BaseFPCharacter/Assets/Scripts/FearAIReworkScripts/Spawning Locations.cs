using UnityEngine;

public class SpawningLocations : MonoBehaviour
{
    public static SpawningLocations Instance { get; private set; }
    public int area;

    public bool availableSpawningLocation;

    public void SpawnZombie(GameObject prefabToSpawn, double health)
    {
        if (!availableSpawningLocation) { return; }

        GameObject newZombie = Instantiate(prefabToSpawn, transform.position, transform.rotation);
        newZombie.GetComponent<Zombies>().SetHealth(health);
    }
}
