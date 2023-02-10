using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The Sc_AIDirector is the main script that grabs all of the current NPCs in the map and will decide how many AI NPCS can do certain tasks. This helps control the NPCs so that there arent to many enemies attack the player at the same time 
 * 
 * 
 * 
 */
public class Sc_AIDirector : MonoBehaviour
{
    //Grabs the quick sort algorithem script
    [SerializeField]
    private Sc_QuickSort quickSort;

    //All current enemis in the map
    [SerializeField]
    private GameObject[] allCurrentEnemy, spawnLocations;
    //List of current enemy Decision values.
    //It is a list instead of an array since not all enemies will have to decide what to do when it sees the player at the same time.
    public static List<GameObject> enemyAIDesicionValue = new List<GameObject>();
    //The state manager is the specific scprit that controls each individual AI NPC
    private Sc_AIStateManager stateManager;

    //Numbers of enemies that are allowed and are currently attacking the player
    [SerializeField]
    private int maxAttacking, currentAttacking;
    private float average = 0;

    //The specific enemy game object
    private GameObject enemy;

    //Controls enemy spawning. This is for a later part of the project where I plan to have a timer start after a certain amount of enemies have detected the player so that after that point more enemies start spawning. This will force the player to be more active and try to finish the objective before too many extra enemies are called in.
    private float spawnerWaitTimer, enemyLimit, currentEnemyCounter;

    private bool playerSeen;

    // Start is called before the first frame update
    void Start()
    {
        //enemyAIDesicionValue = new GameObject[allCurrentEnemy.Length];

        playerSeen = false;
        //
        StartCoroutine(WhatToDoTimer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //If the player was found by an enemy it will aleart all other enemies in the map. I might slightly change this in the future so that its in a radius of the enemy so that it dosent feel over welming that all enemies seem to instantly know where the player is after being spotted.
    public void PlayerFound(GameObject enemyObject)
    {
        playerSeen = true;
        
        //Debug.Log("Activating Enemy");

        for (int i = 0; i < allCurrentEnemy.Length; i++) {
            if (allCurrentEnemy[i] != enemyObject)
            {
                stateManager = allCurrentEnemy[i].GetComponent<Sc_AIStateManager>();
                stateManager.SwitchState(stateManager.aggressionDesicionState);
                //stateManager = null;
            }
        }
    }

    //This will decide how many more enemies to spawn if there is less enemies then the limit
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

    //Will instantiate the required amount of enemies.
    IEnumerator SpawnEnemy(float amountToSpawn, int spawnLocation)
    {
        for (int i = 0; i <= amountToSpawn; i++)
        {
            GameObject newEnemy = Instantiate(enemy, spawnLocations[spawnLocation].transform);
            yield return new WaitForSeconds(spawnerWaitTimer);
        }
        yield return null;
    }

    //This method will grab the list of current enemies that are waiting for a desicion to either attack or run to cover and quick sort it for largest number at the beginning.
    //After that it will go through the list and then it will check if the limit of enemies attacking the player has been meet and if so will have the enemy run to cover. If the limit of attacking enemies hasnt been reached and the current value is greater then 
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
