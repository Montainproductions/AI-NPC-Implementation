using System;
using System.Collections;
using UnityEngine;

public class RoundEnemyDirector : MonoBehaviour
{
    public static RoundEnemyDirector Instance { get; private set; }

    private int typeOfEnemy;

    private GameObject zombiePrefab;

    private double enemyMaxHP = 0, roundHPIncreaseChange;
    private int enemiesLeftToSpawn, maxEnemiesToSpawn = 6, enemiesSpawnedIn, maxEnemiesSpawnedIn = 20;
    private int[] speedOfZombies = new int[3];

    public Action newRound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        BindEvent();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedOfZombies[0] = 100;
        speedOfZombies[1] = 0;
        speedOfZombies[2] = 0;

        //StartRound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BindEvent()
    {

    }

    public void UnBindEvent()
    {

    }

    public void StartRound()
    {
        GameMode_Match.Instance.IncreaseCurrentRound();

        int round = GameMode_Match.Instance.GetCurrentRound();

        if (round <= roundHPIncreaseChange)
        {
            enemyMaxHP += 100;
        }
        else
        {
            enemyMaxHP *= 1.15;
        }

        maxEnemiesToSpawn = (int)(0.35853 * (round) - 1.70944);
    }

    public void EndRound()
    {
        newRound?.Invoke();
        StartRound();
    }

    public void EnemyKilled()
    {
        
    }

    IEnumerator SpawningCheck()
    {
        yield return new WaitForSeconds(0.2f);

        if (enemiesLeftToSpawn <=0) { 
            EndRound();
            yield return null;
        }

        if (enemiesSpawnedIn >= maxEnemiesSpawnedIn)
        {
            StartCoroutine(SpawningCheck());
            yield return null;
        }

        SpawnEnemy();
        StartCoroutine(SpawningCheck());
    }

    public void SpawnEnemy()
    {
        enemiesSpawnedIn++;
        enemiesLeftToSpawn--;
    }

    public void SetTypeOfEnemies(int typeOfEnemies)
    {
        typeOfEnemy = typeOfEnemies;
    }
}
