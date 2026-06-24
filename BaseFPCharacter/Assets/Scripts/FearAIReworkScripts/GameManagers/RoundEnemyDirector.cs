using UnityEngine;

public class RoundEnemyDirector : MonoBehaviour
{
    private int enemyMaxHP, roundHPIncreaseChange;
    private int enemiesLeftToSpawn, maxEnemiesToSpawn, enemiesSpawnedIn;
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

    public void EnemyKilled()
    {
        if (enemiesLeftToSpawn > 0 && maxEnemiesToSpawn < enemiesSpawnedIn)
        {
            SpawnZombie();
        }
        else if (enemiesLeftToSpawn <= 0 && enemiesSpawnedIn <= 0)
        {
            EndRound();
        }
    }

    public void SpawnZombie()
    {

    }
}
