using UnityEngine;

public class ZombiesGameMode : MonoBehaviour
{

    private int zombieMaxHP, roundHPIncreaseChange;
    private int zombiesLeftToSpawn, maxZombiesToSpawn, zombiesSpawnedIn, totalZombiesSpawnedIn;
    private int[] speedOfZombies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRound()
    {


    }

    public void EndRound()
    {

    }

    public void ZombieKilled()
    {
        if (zombiesLeftToSpawn > 0)
        {
            SpawnZombie();
        }else if (zombiesLeftToSpawn <= 0 && zombiesSpawnedIn <= 0)
        {
            EndRound();
        }
    }

    public void SpawnZombie()
    {

    }
}
