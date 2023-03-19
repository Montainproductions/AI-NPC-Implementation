using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

/* The Sc_AIDirector is the main script that grabs all of the current NPCs in the map and will decide how many AI NPCS can do certain tasks. This helps control the NPCs so that there arent to many enemies attack the player at the same time 
 */
public class Sc_AIDirector : MonoBehaviour
{
    public Sc_AIDirector Instance { get; set; }

    //Grabs the quick sort algorithem script
    [SerializeField]
    private Sc_QuickSort quickSort;

    //All current enemis in the map
    [SerializeField]
    private GameObject[] allCurrentEnemy, spawnLocations;
    private Sc_AIStateManager[] allAIManagerScript;
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

    [SerializeField]
    private float audioRange;

    // Start is called before the first frame update
    void Start()
    {
        //enemyAIDesicionValue = new GameObject[allCurrentEnemy.Length];

        allAIManagerScript = new Sc_AIStateManager[allCurrentEnemy.Length];

        playerSeen = false;
        //Starts the timer coroutine so that each time it will grab the current set of AIs that have seen the Player and chosses which state they go to.
        StartCoroutine(WhatToDoTimer());
        //Debug.Log("Active? " + gameObject.activeInHierarchy);
        StartCoroutine(AIManagerScripts());
    }

    // Update is called once per frame
    void Update()
    {

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

    //If the player was found by an enemy it will aleart all other enemies in the map. I might slightly change this in the future so that its in a radius of the enemy so that it dosent feel over welming that all enemies seem to instantly know where the player is after being spotted.
    public IEnumerator PlayerFound(GameObject enemyObject)
    {
        playerSeen = true;

        for (int i = 0; i < allCurrentEnemy.Length; i++) {
            //Debug.Log(allCurrentEnemy[i]);
            //Debug.Log(enemyObject);
            if (allCurrentEnemy[i] != enemyObject && (Vector3.Distance(allCurrentEnemy[i].transform.position, enemyObject.transform.position)) < audioRange)
            {
                yield return new WaitForSeconds(1.25f);
                stateManager = allCurrentEnemy[i].GetComponent<Sc_AIStateManager>();
                if (!allAIManagerScript[i].playerNoticed)
                {
                    stateManager.playerNoticed = true;
                    stateManager.SwitchState(stateManager.aggressionDesicionState);
                }
                    //stateManager = null;
            }
        }
        yield return null;
    }

    //Adds the AI Enemy to the list and recalculates the average decision value of all AI in list
    public void AIAttackAddList(GameObject enemyObj)
    {
        enemyAIDesicionValue.Add(enemyObj);
        StartCoroutine(AverageDecisionValue());
    }

    //Remove the enemy from the list of enemis that need to decide what to do
    public void AIAttackRemoveList(GameObject enemyObj)
    {
        enemyAIDesicionValue.Remove(enemyObj);
        StartCoroutine(AverageDecisionValue());
    }

    //Grabs all of the manager scripts from the AI
    IEnumerator AIManagerScripts()
    {
        for (int i = 0; i < allCurrentEnemy.Length; i++)
        {
            allAIManagerScript[i] = allCurrentEnemy[i].GetComponent<Sc_AIStateManager>();
        }
        yield return null;
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
            
            //If too many enemy AIs are attacking the player then the current one will go to the cover state
            if (currentAttacking >= maxAttacking)
            {
                stateManager.SwitchState(stateManager.coverState);

            }
            //If the current desicion value of the enemy is greater than the average value of all enemies then the enemy will have a 75% chance of going to the attack state or a 25% chance to go to the cover state.
            //This is meant to help
            else if (stateManager.ReturnDecisionValue() >= valueLimit)
            {
                if(v >= 2.5f) //2.5f
                {
                    //Debug.Log("Attacking");
                    //stateManager.SwitchState(stateManager.attackState);
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
                if (v >= 7.5f) //7.5f
                {
                    //Debug.Log("Attacking part 2");
                    //stateManager.SwitchState(stateManager.attackState);
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

    //Calculates the average of all the decions values for each of the enemies in the enemenyAIDesicionValue list
    IEnumerator AverageDecisionValue()
    {
        for (int i = 0; i < enemyAIDesicionValue.Count; i++)
        {
            stateManager = enemyAIDesicionValue[i].GetComponent<Sc_AIStateManager>();
            average += stateManager.ReturnDecisionValue();
        }
        average = average / enemyAIDesicionValue.Count;
        yield return null;
    }

    //Timer for how often to get all the enemies waiting for a desicion to start again
    IEnumerator WhatToDoTimer()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(WhatToDo(average));
        StartCoroutine(WhatToDoTimer());
        yield return null;
    }

    //When a shot is fire
    public IEnumerator ShotFired(Vector3 positionOfShot)
    {

        for(int i = 0; i < allCurrentEnemy.Length; i++)
        {
            if (allCurrentEnemy[i] == null) { continue; }
            //yield return new WaitForSeconds(1.0f);
            //Debug.Log(allCurrentEnemy[i]);
            if (Vector3.Distance(allCurrentEnemy[i].transform.position, positionOfShot) < audioRange && (allAIManagerScript[i].currentState == allAIManagerScript[i].idleState || allAIManagerScript[i].currentState == allAIManagerScript[i].patrolState))
            {
                Debug.Log("Player Heard");
                allAIManagerScript[i].playerNoticed = true;
                allAIManagerScript[i].SwitchState(allAIManagerScript[i].searchState);
            }
        }
        yield return null;
    }
}
