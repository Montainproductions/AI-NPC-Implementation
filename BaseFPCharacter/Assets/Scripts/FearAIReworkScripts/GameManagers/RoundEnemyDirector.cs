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
    private int enemiesLeftToSpawn, enemiesSpawnedIn, maxEnemiesSpawnedIn = 20;
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
        GameMode_Match.matchStart += StartMatch;
    }

    public void UnBindEvent()
    {
        AreaChecker.checkArea -= PlayersCurrentArea;
        InteractionItem.doorOpened -= NewSpawnersUnlocked;
        GameMode_Match.matchStart -= StartMatch;
    }

    public void StartMatch(GameObject[] startingWeapons)
    {
        StartCoroutine(StartMatchCoroutine());
    }

    public IEnumerator StartMatchCoroutine()
    {
        yield return new WaitForEndOfFrame();

        int commonIdNN = -1;
        int areaID = 0;

        NewSpawnersUnlocked(commonIdNN, areaID);

        StartRound();
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

        enemiesLeftToSpawn = (int)((0.0541695* MathF.Pow(round,2)) +1.75191 * round + 4.68158);
        StartCoroutine(SpawningCheck());
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
        yield return new WaitForSeconds(2f);
        if (enemiesSpawnedIn <= 0 && enemiesLeftToSpawn <=0) { 
            EndRound();
            yield break;
        }

        if (enemiesSpawnedIn >= maxEnemiesSpawnedIn || enemiesLeftToSpawn <= 0)
        {
            StartCoroutine(SpawningCheck());
            yield break;
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
            allAreaDictionary[area.areaInfo.areaID] = area;
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
            if (allAreaDictionary.TryGetValue(connectedIndex, out AreaChecker area))
            {
                areasScriptsInEffect.Add(area);
            }
        }
        foreach (var area in areasScriptsInEffect)
        {
            listOfSpawnLocations.AddRange(area.spawnPoints);
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
