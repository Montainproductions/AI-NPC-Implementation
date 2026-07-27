using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundEnemyDirector : MonoBehaviour
{
    public static RoundEnemyDirector Instance { get; private set; }

    [SerializeField]
    private AreaChecker[] allAreaInfos;
    private Dictionary<int, AreaChecker> allAreaDictionary;
    private List<AreaChecker> areasScriptsInEffect = new List<AreaChecker>();

    private AreaChecker playerAreaInfo;

    List<SpawningLocations> listOfSpawnLocations = new List<SpawningLocations>();

    private int typeOfEnemy;

    [SerializeField]
    private GameObject zombiePrefab;

    private double enemyMaxHP = 0, roundHPIncreaseChange;
    private int enemiesLeftToSpawn, maxEnemiesToSpawn = 6, enemiesSpawnedIn, maxEnemiesSpawnedIn = 20;
    private int[] speedOfZombies = new int[3];

    public Action newRound;

    public Action<int> spawnEnemy;

    [SerializeField]
    private Transform[] playerTransform;

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

        BuildAreaLookup();

        //StartRound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BindEvent()
    {
        AreaChecker.checkArea += PlayersCurrentArea;
        InteractionItem.doorOpened += NewSpawnersUnlocked;
    }

    public void UnBindEvent()
    {
        AreaChecker.checkArea -= PlayersCurrentArea;
        InteractionItem.doorOpened -= NewSpawnersUnlocked;
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
        yield return new WaitForSeconds(0.1f);

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

        int spawnPositionIndex = UnityEngine.Random.Range(0, listOfSpawnLocations.Count);
        if (typeOfEnemy == 0)
        {
            listOfSpawnLocations[spawnPositionIndex].SpawnZombie(zombiePrefab, enemyMaxHP, playerTransform);
        }
    }

    public void SetTypeOfEnemies(int typeOfEnemies)
    {
        typeOfEnemy = typeOfEnemies;
    }

    void BuildAreaLookup()
    {
        allAreaDictionary = new Dictionary<int, AreaChecker>();
        foreach (AreaChecker area in allAreaInfos)
        {
            allAreaDictionary[area.areaInfo.areaIndex] = area;
            Debug.Log(allAreaDictionary[area.areaInfo.areaIndex]);
        }
    }

    public void PlayersCurrentArea(AreaChecker currentPlayerArea)
    {
        playerAreaInfo = currentPlayerArea;
        areasScriptsInEffect.Clear();
        listOfSpawnLocations.Clear();

        areasScriptsInEffect.Add(playerAreaInfo);
        foreach (int connectedIndex in playerAreaInfo.areaInfo.connectedAreasIndex)
        {
            Debug.Log(connectedIndex);
            if (allAreaDictionary.TryGetValue(connectedIndex, out AreaChecker area))
            {
                Debug.Log(area.areaInfo.areaName);
                Debug.Log(area.AreaAvailable);
                areasScriptsInEffect.Add(area);
            }
        }
        Debug.Log(areasScriptsInEffect[0].areaInfo.areaName);
        foreach (var area in areasScriptsInEffect)
        {
            listOfSpawnLocations.AddRange(area.spawnPoints);
            Debug.Log(area.spawnPoints);
        }
    }

    public void NewSpawnersUnlocked(int commonId, int areaID)
    {
        if (allAreaDictionary.TryGetValue(areaID, out AreaChecker area))
        {
            area.UpdatedAvailableSpawningPoints();
        }
    }
}
