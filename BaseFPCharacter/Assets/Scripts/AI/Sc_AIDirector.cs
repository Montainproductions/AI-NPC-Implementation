using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AIDirector : MonoBehaviour
{
    [SerializeField]
    private Sc_QuickSort quickSort;

    [SerializeField]
    private GameObject[] allCurrentEnemy, spawnLocations;
    private GameObject[] enemyAIDesicionValue;

    private GameObject enemy;

    private float spawnerWaitTimer, enemyLimit, currentEnemyCounter;

    private bool playerSeen;

    // Start is called before the first frame update
    void Start()
    {
        enemyAIDesicionValue = new GameObject[allCurrentEnemy.Length];

        playerSeen = false;
        StartCoroutine(WhatToDoTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerFound()
    {
        playerSeen = !playerSeen;
    }

    public void WantToSpawnMore(float amountToSpawn, int spawnLocation)
    {
        if (enemyLimit < currentEnemyCounter + amountToSpawn)
        {
            SpawnEnemy(enemyLimit - currentEnemyCounter, spawnLocation);
        }
        else
        {
            SpawnEnemy(amountToSpawn, spawnLocation);
        }
    }

    IEnumerator SpawnEnemy(float amountToSpawn, int spawnLocation)
    {
        for (int i = 0; i <= amountToSpawn; i++)
        {
            GameObject newEnemy = Instantiate(enemy, spawnLocations[spawnLocation].transform);
            yield return new WaitForSeconds(spawnerWaitTimer);
        }
        yield return null;
    }

    IEnumerator WhatToDo()
    {
        Debug.Log(enemyAIDesicionValue.Length);
        quickSort.Main(enemyAIDesicionValue);
        Debug.Log("Ready to decide");
        //Debug.Log(enemyAIDesicionValue.Length);
        yield return null;
    }

    public IEnumerator AIAttackAddList(GameObject enemyObj)
    {
        foreach(GameObject i in enemyAIDesicionValue) {
            Debug.Log("Enemy added");
            enemyAIDesicionValue[i] = enemyObj;
        }
        yield return null;
    }

    public IEnumerator AIAttackRemoveList(GameObject enemyObj)
    {
        for (int i = 0; i < enemyAIDesicionValue.Length; i++)
        {
            if (enemyAIDesicionValue[i] == enemyObj)
            {
                enemyAIDesicionValue[i] = null;
                break;
            }
        }
        yield return null;
    }

    IEnumerator WhatToDoTimer()
    {
        yield return new WaitForSeconds(2);
        //StartCoroutine(WhatToDo());
        StartCoroutine(WhatToDoTimer());
        yield return null;
    }
}
