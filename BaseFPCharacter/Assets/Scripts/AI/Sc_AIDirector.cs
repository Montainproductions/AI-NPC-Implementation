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
    private int maxAttacking, currentAttacking;
    private float average = 0;

    private GameObject enemy;

    private float spawnerWaitTimer, enemyLimit, currentEnemyCounter;

    private bool playerSeen;

    // Start is called before the first frame update
    void Start()
    {
        //enemyAIDesicionValue = new GameObject[allCurrentEnemy.Length];

        playerSeen = false;
        StartCoroutine(WhatToDoTimer());

        allEnemies();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayerFound(GameObject enemyObject)
    {
        playerSeen = true;
        
        Debug.Log("Activating Enemy");
        allEnemies();

        for (int i = 0; i < allCurrentEnemy.Length; i++) {
            if (allCurrentEnemy[i] != enemyObject)
            {
                stateManager = allCurrentEnemy[i].GetComponent<Sc_AIStateManager>();
                stateManager.SwitchState(stateManager.aggressionDesicionState);
                //stateManager = null;
            }
        }
    }

    public void allEnemies()
    {
        Debug.Log(allCurrentEnemy.Length);
        for (int i = 0; i < allCurrentEnemy.Length; i++)
        {
            //Debug.Log(i);
            Debug.Log(allCurrentEnemy[i]);
        }
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

    IEnumerator WhatToDo(float valueLimit)
    {
        quickSort.Main(enemyAIDesicionValue);
        //Debug.Log(enemyAIDesicionValue.Count);
        //Debug.Log("Ready to decide");

        for (int i = 0; i < enemyAIDesicionValue.Count; i++)
        {
            stateManager = enemyAIDesicionValue[i].GetComponent<Sc_AIStateManager>();
            float v = Random.Range(1.0f, 10.0f);
            
            //Debug.Log(v);
            
            if (currentAttacking >= maxAttacking)
            {
                stateManager.SwitchState(stateManager.coverState);

            }
            else if (stateManager.decisionValue >= valueLimit)
            {
                if(v >= 2.5f)
                {
                    //Debug.Log("Attacking");
                    stateManager.SwitchState(stateManager.attackState);
                    currentAttacking++;
                }
                else
                {
                    //Debug.Log("Lower");
                    stateManager.SwitchState(stateManager.coverState);
                }
            }
            else
            {
                if (v >= 6f)
                {
                    //Debug.Log("Attacking part 2");
                    stateManager.SwitchState(stateManager.attackState);
                    currentAttacking++;
                }
                else
                {
                    //Debug.Log("Lower Part 2");
                    stateManager.SwitchState(stateManager.coverState);
                }
            }
        }

        enemyAIDesicionValue.Clear();
        //Debug.Log("All enemies decided");
        yield return null;
    }

    public IEnumerator AIAttackAddList(GameObject enemyObj)
    {
        enemyAIDesicionValue.Add(enemyObj);
        StartCoroutine(AverageDecisionValue());
        yield return null;
    }

    public IEnumerator AIAttackRemoveList(GameObject enemyObj)
    {
        enemyAIDesicionValue.Remove(enemyObj);
        yield return null;
    }

    IEnumerator AverageDecisionValue()
    {
        for (int i = 0; i < enemyAIDesicionValue.Count; i++)
        {
            stateManager = enemyAIDesicionValue[i].GetComponent<Sc_AIStateManager>();
            average += stateManager.decisionValue;
        }
        average = average / enemyAIDesicionValue.Count;
        yield return null;
    }

    IEnumerator WhatToDoTimer()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(WhatToDo(average));
        StartCoroutine(WhatToDoTimer());
        yield return null;
    }
}
