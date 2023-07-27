using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sc_AIDirectorHFSM : MonoBehaviour
{
    public Sc_AIDirector Instance { get; set; }

    //Sets up the main current four traits
    [HideInInspector]
    public Trait Aggressive = new Trait("Aggressive", 12, 3, 2, 4, -4, 1, 1, 2, -2);
    [HideInInspector]
    public Trait Bold = new Trait("Bold", 6, 1.5f, 0.5f, 4, -4, 1, 1, 2, -2);
    [HideInInspector]
    public Trait Cautious = new Trait("Cautious", -6, -1.5f, -0.5f, 4, -4, 1, 1, 2, -2);
    [HideInInspector]
    public Trait Scared = new Trait("Scared", -12, -3, -2, 4, -4, 1, 1, 2, -2);

    //Grabs the quick sort algorithem script
    [SerializeField]
    private Sc_QuickSort quickSort;

    //All current enemis in the map
    public static List<GameObject> enemyAIToDecide = new List<GameObject>();
    [SerializeField]
    private List<GameObject> allCurrentEnemy = new List<GameObject>();
    public static List<Sc_AIStatesManagerHierarchical> allEnemyAIManagerScript = new List<Sc_AIStatesManagerHierarchical>();

    //All possible spawn locations for new AI
    [SerializeField]
    private GameObject[] spawnLocations;

    //Numbers of enemies that are allowed and are currently attacking the player
    [SerializeField]
    private int maxAttacking, currentAttacking;
    private float average = 0;

    //Various clips of audio to be played
    [SerializeField]
    private AudioClip[] agressiveAudioClips1, boldAudioClips1;

    //Limits the amount of audio that all the AIs on the map can play to not overwhelm the player
    [SerializeField]
    private int maxSoundsPlaying, currentSoundsPlaying;

    private int[] arrayOfSoundsToPlay;

    /*
    //The specific enemy game object
    private GameObject enemy;

    //Controls enemy spawning. This is for a later part of the project where I plan to have a timer start after a certain amount of enemies have detected the player so that after that point more enemies start spawning. This will force the player to be more active and try to finish the objective before too many extra enemies are called in.
    private float spawnerWaitTimer, enemyLimit, currentEnemyCounter;
    */

    //Checks if the noise being made is in audio range
    [SerializeField]
    private float audioRange;

    // Start is called before the first frame update
    void Start()
    {
        currentSoundsPlaying = 0;
        arrayOfSoundsToPlay = new int[4];

        for (int i = 0; i < arrayOfSoundsToPlay.Length; i++)
        {
            arrayOfSoundsToPlay[i] = -1;
        }

        //Starts the timer coroutine so that each time it will grab the current set of AIs that have seen the Player and chosses which state they go to.
        StartCoroutine(WhatToDoTimer());
        //Debug.Log("Active? " + gameObject.activeInHierarchy);
        StartCoroutine(AIManagerScripts());
    }

    /*
    //Bool determining if the limit of audios being played has reached its limit
    public bool PlayAudio(int audioPosition, Sc_AIStateManager statemanager)
    {
        if (currentSoundsPlaying < maxSoundsPlaying)
        {
            currentSoundsPlaying++;
            return true;
        }
        return false;
    }*/

    //Increases the recently played audio counter
    public bool PlayAudio()
    {
        if (currentSoundsPlaying < maxSoundsPlaying)
        {
            currentSoundsPlaying++;
            return true;
        }
        return false;
    }

    IEnumerator PlayAudioLaterTimer(Sc_HFSMCommenMethods commenMethods, int audioPosition, int arrayOfSoundsPosition)
    {
        yield return new WaitForSeconds(1.5f);
        if (commenMethods != null)
        {
            commenMethods.PlayAudioOneShot(audioPosition);
        }
        arrayOfSoundsToPlay[arrayOfSoundsPosition] = -1;
        yield return null;
    }

    //Reduces the recently played audio counter
    public void NotPlayingAudio()
    {
        if (currentSoundsPlaying > 0)
        {
            currentSoundsPlaying--;
        }
    }

    //Removes the game object from any lists that could contain it. Mostly done to help reduce null point exceptions when runing through the whole list and one is empty
    public void EnemyDied(GameObject enemyThatDied)
    {
        enemyAIToDecide.Remove(enemyThatDied);
        allCurrentEnemy.Remove(enemyThatDied);
        allEnemyAIManagerScript.Remove(enemyThatDied.GetComponent<Sc_AIStatesManagerHierarchical>());
    }

    /*
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
    }*/

    //Adds the AI Enemy to the list and recalculates the average decision value of all AI in list
    public void AIAttackAddList(GameObject enemyObj)
    {
        enemyAIToDecide.Add(enemyObj);
        StartCoroutine(AverageDecisionValue());
    }

    //Remove the enemy from the list of enemis that need to decide what to do
    public void AIAttackRemoveList(GameObject enemyObj)
    {
        enemyAIToDecide.Remove(enemyObj);
        StartCoroutine(AverageDecisionValue());
    }

    //Grabs all of the manager scripts from the AI
    IEnumerator AIManagerScripts()
    {
        foreach (GameObject i in allCurrentEnemy)
        {
            allEnemyAIManagerScript.Add(i.GetComponent<Sc_AIStatesManagerHierarchical>());
            Sc_AIStatesManagerHierarchical stateManager = allEnemyAIManagerScript.Last();
            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue >= 0.75f)
            {
                //Debug.Log(Aggressive.ReturnAgressionValue());
                stateManager.SetUpTraits(Aggressive, agressiveAudioClips1);
            }
            else if (0.75f > randomValue && randomValue >= 0.5f)
            {
                //Debug.Log(Bold.ReturnAgressionValue());
                stateManager.SetUpTraits(Bold, agressiveAudioClips1);
            }
            else if (0.5f > randomValue && randomValue >= 0.25f)
            {
                //Debug.Log(Cautious.ReturnAgressionValue());
                stateManager.SetUpTraits(Cautious, agressiveAudioClips1);
            }
            else
            {
                //Debug.Log(Scared.ReturnAgressionValue());
                stateManager.SetUpTraits(Scared, agressiveAudioClips1);
            }
        }
        yield return null;
    }

    //If the player was found by an enemy it will aleart all other enemies in the map. I might slightly change this in the future so that its in a radius of the enemy so that it dosent feel over welming that all enemies seem to instantly know where the player is after being spotted.
    public IEnumerator PlayerFound(GameObject enemyObject)
    {
        for (int i = 0; i < allCurrentEnemy.Count; i++)
        {
            //Debug.Log(allCurrentEnemy[i]);
            //Debug.Log(enemyObject);
            if (allCurrentEnemy[i] != enemyObject && (Vector3.Distance(allCurrentEnemy[i].transform.position, enemyObject.transform.position)) < audioRange)
            {
                yield return new WaitForSeconds(1.25f);
                if (!allEnemyAIManagerScript[i].playerNoticed)
                {
                    allEnemyAIManagerScript[i].playerNoticed = true;
                    allEnemyAIManagerScript[i].SwitchFLState(allEnemyAIManagerScript[i].alertFLState);
                    allEnemyAIManagerScript[i].SwitchSLState(allEnemyAIManagerScript[i].aggressionDesicionState);
                }
                //stateManager = null;
            }
        }
        yield return null;
    }

    //This method will grab the list of current enemies that are waiting for a desicion to either attack or run to cover and quick sort it for largest number at the beginning.
    //After that it will go through the list and then it will check if the limit of enemies attacking the player has been meet and if so will have the enemy run to cover. If the limit of attacking enemies hasnt been reached and the current value is greater then 
    IEnumerator WhatToDo(float valueLimit)
    {
        quickSort.Main(enemyAIToDecide);
        //Debug.Log(enemyAIDesicionValue.Count);
        //Debug.Log("Ready to decide");

        for (int i = 0; i < enemyAIToDecide.Count; i++)
        {
            float v = Random.Range(1.0f, 10.0f);

            Sc_AIStatesManagerHierarchical aiScript = enemyAIToDecide[i].GetComponent<Sc_AIStatesManagerHierarchical>();

            aiScript.SwitchFLState(aiScript.combatFLState);

            //If too many enemy AIs are attacking the player then the current one will go to the cover state
            if (currentAttacking >= maxAttacking)
            {
                aiScript.SwitchSLState(aiScript.coverState);
            }
            //If the current desicion value of the enemy is greater than the average value of all enemies then the enemy will have a 75% chance of going to the attack state or a 25% chance to go to the cover state.
            //This is meant to help
            else if (aiScript.ReturnDecisionValue() >= valueLimit)
            {
                if (v >= 2.5f) //2.5f
                {
                    //Debug.Log("Attacking");
                    //stateManager.SwitchState(stateManager.attackState);
                    aiScript.SwitchSLState(aiScript.attackState);
                    currentAttacking++;
                }
                else
                {
                    //Debug.Log("Lower");
                    aiScript.SwitchSLState(aiScript.coverState);
                }
            }
            else
            {
                if (v >= 7.5f) //7.5f
                {
                    //Debug.Log("Attacking part 2");
                    //stateManager.SwitchState(stateManager.attackState);
                    aiScript.SwitchSLState(aiScript.attackState);
                    currentAttacking++;
                }
                else
                {
                    //Debug.Log("Lower Part 2");
                    aiScript.SwitchSLState(aiScript.coverState);
                }
            }
        }

        enemyAIToDecide.Clear();
        //Debug.Log("All enemies decided");
        yield return null;
    }

    //Calculates the average of all the decions values for each of the enemies in the enemenyAIDesicionValue list
    IEnumerator AverageDecisionValue()
    {
        for (int i = 0; i < enemyAIToDecide.Count; i++)
        {
            Sc_AIStateManager aiScript = enemyAIToDecide[i].GetComponent<Sc_AIStateManager>();
            average += aiScript.ReturnDecisionValue();
        }
        average = average / enemyAIToDecide.Count;
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

    //When a shot is fired all enemies near by hear
    public IEnumerator ShotFired(Vector3 positionOfShot)
    {

        for (int i = 0; i < allCurrentEnemy.Count; i++)
        {
            if (allCurrentEnemy[i] == null) { continue; }
            //yield return new WaitForSeconds(1.0f);
            //Debug.Log(allCurrentEnemy[i]);
            Sc_AIStatesManagerHierarchical enemyObjScipt = allEnemyAIManagerScript[i];
            if (Vector3.Distance(allCurrentEnemy[i].transform.position, positionOfShot) < audioRange && enemyObjScipt.currentFLState == enemyObjScipt.nonCombatFLState)
            {
                enemyObjScipt.PlayRandomAudioOneShot(6, 8);
                //Debug.Log("Player Heard");
                enemyObjScipt.playerNoticed = true;
                enemyObjScipt.SwitchFLState(enemyObjScipt.combatFLState);
                enemyObjScipt.SwitchSLState(enemyObjScipt.aggressionDesicionState);
            }
        }
        yield return null;
    }
}
