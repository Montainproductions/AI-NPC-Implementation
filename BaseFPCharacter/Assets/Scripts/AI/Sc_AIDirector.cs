using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AIDirector : MonoBehaviour
{
    [SerializeField]
    private Sc_QuickSort quickSort;

    [SerializeField]
    private GameObject[] allCurrentEnemy, spawnLocations;
    //private GameObject[] enemyAIDesicionValue;
    public static List<GameObject> enemyAIDesicionValue = new List<GameObject>();
    private Sc_AIStateManager stateManager;

    [SerializeField]
    private int maxAttacking, currentAttacking, diffStateCounter;
    private int lowest = 2;

    private GameObject enemy;

    private float spawnerWaitTimer, enemyLimit, currentEnemyCounter;

    private bool playerSeen;

    // Start is called before the first frame update
    void Start()
    {
        //enemyAIDesicionValue = new GameObject[allCurrentEnemy.Length];
        diffStateCounter = 0;

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

    IEnumerator WhatToDo(int valueLimit)
    {
        quickSort.Main(enemyAIDesicionValue);
        Debug.Log(enemyAIDesicionValue.Count);
        Debug.Log("Ready to decide");

        if(diffStateCounter == enemyAIDesicionValue.Count)
        {
            diffStateCounter = 0;
            enemyAIDesicionValue.Clear();
            yield return null;
        }

        if (lowest > valueLimit)
        {
            yield return null;
        }

        diffStateCounter = 0;
        for (int i = 0; i < enemyAIDesicionValue.Count; i++)
        {
            stateManager = enemyAIDesicionValue[i].GetComponent<Sc_AIStateManager>();
            if (stateManager.currentState != stateManager.aggressionState)
            {
                diffStateCounter++;
                break;
            }
            if (currentAttacking >= maxAttacking)
            {
                stateManager.SwitchState(stateManager.coverState);

            }
            else if(stateManager.decisionValue == 2)
            {
                stateManager.SwitchState(stateManager.attackState);
                currentAttacking++;
            }
            else if (stateManager.decisionValue >= valueLimit)
            {
                float v = Random.Range(1.0f, 10.0f);
                if(v >= 2.5f)
                {
                    stateManager.SwitchState(stateManager.attackState);
                    currentAttacking++;
                }
            }
        }

        //Debug.Log(enemyAIDesicionValue.Count);
        //StartCoroutine(WhatToDo(valueLimit--));
        yield return null;
    }

    public IEnumerator AIAttackAddList(GameObject enemyObj)
    {
        enemyAIDesicionValue.Add(enemyObj);
        StartCoroutine(LowestDecisionValue());
        yield return null;
    }

    public IEnumerator AIAttackRemoveList(GameObject enemyObj)
    {
        enemyAIDesicionValue.Remove(enemyObj);
        yield return null;
    }

    IEnumerator LowestDecisionValue()
    {
        for (int i = 0; i < enemyAIDesicionValue.Count; i++)
        {
            stateManager = enemyAIDesicionValue[i].GetComponent<Sc_AIStateManager>();
            if (stateManager.decisionValue < lowest)
            {
                lowest = stateManager.decisionValue;
            }
        }
        yield return null;
    }

    IEnumerator WhatToDoTimer()
    {
        yield return new WaitForSeconds(2);
        diffStateCounter = 0;
        StartCoroutine(WhatToDo(2));
        StartCoroutine(WhatToDoTimer());
        yield return null;
    }
}
