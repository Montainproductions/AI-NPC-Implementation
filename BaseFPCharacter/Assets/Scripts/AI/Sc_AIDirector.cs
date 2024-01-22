using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using static UnityEditor.Experimental.GraphView.GraphView;

/* The Sc_AIDirector is the main script that grabs all of the current NPCs in the map and will decide how many AI NPCS can do certain tasks. This helps control the NPCs so that there arent to many enemies attack the player at the same time 
 */
public class Sc_AIDirector : MonoBehaviour
{
    public Sc_AIDirector Instance { get; set; }

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private TypeOfAIToSpawn sco_HFSM;
    private bool hasHFSM;

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
    private List<GameObject> allCurrentEnemy =  new List<GameObject>();
    public static List<Sc_AIStateManager> allEnemyAIManagerScript = new List<Sc_AIStateManager>();
    public static List<Sc_AIStatesManagerHierarchical> allEnemyAIManagerScriptHFSM = new List<Sc_AIStatesManagerHierarchical>();

    [SerializeField]
    private Sc_CoverandPatrolPoints[] coverPatrolPoints;

    //All possible spawn locations for new AI
    [SerializeField]
    private GameObject[] spawnLocations, patrolPoints, coverPoints;

    //The two types of AI (FSM, HFSM)
    [SerializeField]
    private GameObject[] aiTypes;

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
        hasHFSM = sco_HFSM.isHFSM;
        StartCoroutine(AIManagerScripts());
    }

    //Increases the recently played audio counter
    public bool PlayAudio()
    {
        if(currentSoundsPlaying < maxSoundsPlaying)
        {
            currentSoundsPlaying++;
            return true;
        }
        return false;
    }

    IEnumerator PlayAudioLaterTimer(Sc_AIStateManager stateManager, int audioPosition, int arrayOfSoundsPosition)
    {
        yield return new WaitForSeconds(1.5f);
        if (stateManager != null)
        {
            stateManager.PlayAudioOneShot(audioPosition);
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
        if (hasHFSM)
        {
            allEnemyAIManagerScriptHFSM.Remove(enemyThatDied.GetComponent<Sc_AIStatesManagerHierarchical>());
        }
        else
        {
            allEnemyAIManagerScript.Remove(enemyThatDied.GetComponent<Sc_AIStateManager>());
        }
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
        foreach (GameObject i in spawnLocations)
        {
            if (hasHFSM)
            {
                GameObject newAI = Instantiate(aiTypes[1], new Vector3(i.transform.position.x, 0.5f, i.transform.position.z), Quaternion.identity);
                
                allCurrentEnemy.Add(newAI);
                allEnemyAIManagerScriptHFSM.Add(newAI.GetComponent<Sc_AIStatesManagerHierarchical>());
                Sc_AIStatesManagerHierarchical stateManagerHFSM = allEnemyAIManagerScriptHFSM.Last();
                stateManagerHFSM.SetUpInfoDirector(this, player, i);
                float randomValue = Random.Range(0.0f, 1.0f);
                if (randomValue >= 0.75f)
                {
                    //Debug.Log(Aggressive.ReturnAgressionValue());
                    stateManagerHFSM.SetUpTraits(Aggressive, agressiveAudioClips1);
                }
                else if (0.75f > randomValue && randomValue >= 0.5f)
                {
                    //Debug.Log(Bold.ReturnAgressionValue());
                    stateManagerHFSM.SetUpTraits(Bold, agressiveAudioClips1);
                }
                else if (0.5f > randomValue && randomValue >= 0.25f)
                {
                    //Debug.Log(Cautious.ReturnAgressionValue());
                    stateManagerHFSM.SetUpTraits(Cautious, agressiveAudioClips1);
                }
                else
                {
                    //Debug.Log(Scared.ReturnAgressionValue());
                    stateManagerHFSM.SetUpTraits(Scared, agressiveAudioClips1);
                }
            }
            else
            {
                GameObject newAI = Instantiate(aiTypes[0], i.transform);
                allCurrentEnemy.Add(newAI);
                allEnemyAIManagerScript.Add(newAI.GetComponent<Sc_AIStateManager>());
                Sc_AIStateManager stateManager = allEnemyAIManagerScript.Last();
                stateManager.SetUpInfoDirector(this, player, i);
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
                if (!allEnemyAIManagerScriptHFSM[i].playerNoticed)
                {
                    allEnemyAIManagerScriptHFSM[i].playerNoticed = true;

                    allEnemyAIManagerScriptHFSM[i].SwitchFLState(allEnemyAIManagerScriptHFSM[i].combatFLState);
                    allEnemyAIManagerScriptHFSM[i].SwitchSLState(allEnemyAIManagerScriptHFSM[i].aggressionDesicionState);
                }
                //stateManager = null;
            }
        }
        yield return null;
    }

    public IEnumerator PlayerFoundHFSM(GameObject enemyObject)
    {
        for (int i = 0; i < allCurrentEnemy.Count; i++)
        {
            //Debug.Log(allCurrentEnemy[i]);
            //Debug.Log(enemyObject);
            if (allCurrentEnemy[i] != enemyObject && !allEnemyAIManagerScript[i].playerNoticed && (Vector3.Distance(allCurrentEnemy[i].transform.position, enemyObject.transform.position)) < audioRange)
            {
                yield return new WaitForSeconds(1.25f);
                allEnemyAIManagerScript[i].playerNoticed = true;

                allEnemyAIManagerScript[i].SwitchState(allEnemyAIManagerScript[i].aggressionDesicionState);
                //stateManager = null;
            }
        }
        yield return null;
    }

    //This method will grab the list of current enemies that are waiting for a desicion to either attack or run to cover and quick sort it for largest number at the beginning.
    //After that it will go through the list and then it will check if the limit of enemies attacking the player has been meet and if so will have the enemy run to cover. If the limit of attacking enemies hasnt been reached and the current value is greater then 
    IEnumerator WhatToDo(float valueLimit)
    {
        quickSort.Main(enemyAIToDecide, hasHFSM);
        //Debug.Log(enemyAIDesicionValue.Count);
        //Debug.Log("Ready to decide");

        for (int i = 0; i < enemyAIToDecide.Count; i++)
        {

            if (hasHFSM)
            {
                Sc_AIStatesManagerHierarchical aiScript = enemyAIToDecide[i].GetComponent<Sc_AIStatesManagerHierarchical>();

                //If too many enemy AIs are attacking the player then the current one will go to the cover state
                if (currentAttacking >= maxAttacking)
                {
                    aiScript.SwitchFLState(aiScript.combatFLState);
                    aiScript.SwitchSLState(aiScript.coverState);
                }
                //If the current desicion value of the enemy is greater than the average value of all enemies then the enemy will have a 75% chance of going to the attack state or a 25% chance to go to the cover state.
                //This is meant to help
                else if (aiScript.ReturnDecisionValue() >= valueLimit)
                {
                    float v = Random.Range(1.0f, 10.0f);
                    if (v >= 2.5f) //2.5f
                    {
                        //Debug.Log("Attacking");
                        //stateManager.SwitchState(stateManager.attackState);
                        aiScript.SwitchFLState(aiScript.combatFLState);
                        aiScript.SwitchSLState(aiScript.attackState);
                        currentAttacking++;
                    }
                    else
                    {
                        //Debug.Log("Lower");
                        aiScript.SwitchFLState(aiScript.combatFLState);
                        aiScript.SwitchSLState(aiScript.coverState);
                    }
                }
                else
                {
                    float v = Random.Range(1.0f, 10.0f);
                    if (v >= 7.5f) //7.5f
                    {
                        //Debug.Log("Attacking part 2");
                        //stateManager.SwitchState(stateManager.attackState);
                        aiScript.SwitchFLState(aiScript.combatFLState);
                        aiScript.SwitchSLState(aiScript.attackState);
                        currentAttacking++;
                    }
                    else
                    {
                        //Debug.Log("Lower Part 2");
                        aiScript.SwitchFLState(aiScript.combatFLState);
                        aiScript.SwitchSLState(aiScript.coverState);
                    }
                }
            }
            else
            {
                Sc_AIStateManager aiScript = enemyAIToDecide[i].GetComponent<Sc_AIStateManager>();

                //If too many enemy AIs are attacking the player then the current one will go to the cover state
                if (currentAttacking >= maxAttacking)
                {
                    aiScript.SwitchState(aiScript.coverState);
                }
                //If the current desicion value of the enemy is greater than the average value of all enemies then the enemy will have a 75% chance of going to the attack state or a 25% chance to go to the cover state.
                //This is meant to help
                else if (aiScript.ReturnDecisionValue() >= valueLimit)
                {
                    float v = Random.Range(1.0f, 10.0f);
                    if (v >= 2.5f) //2.5f
                    {
                        //Debug.Log("Attacking");
                        //stateManager.SwitchState(stateManager.attackState);
                        aiScript.SwitchState(aiScript.attackState);
                        currentAttacking++;
                    }
                    else
                    {
                        //Debug.Log("Lower");
                        aiScript.SwitchState(aiScript.coverState);
                    }
                }
                else
                {
                    float v = Random.Range(1.0f, 10.0f);
                    if (v >= 7.5f) //7.5f
                    {
                        //Debug.Log("Attacking part 2");
                        //stateManager.SwitchState(stateManager.attackState);
                        aiScript.SwitchState(aiScript.attackState);
                        currentAttacking++;
                    }
                    else
                    {
                        //Debug.Log("Lower Part 2");
                        aiScript.SwitchState(aiScript.coverState);
                    }
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
            if (hasHFSM)
            {
                Sc_AIStatesManagerHierarchical aiScript = enemyAIToDecide[i].GetComponent<Sc_AIStatesManagerHierarchical>();
                average += aiScript.ReturnDecisionValue();
            }
            else
            {
                Sc_AIStateManager aiScript = enemyAIToDecide[i].GetComponent<Sc_AIStateManager>();
                average += aiScript.ReturnDecisionValue();
            }
        }
        average = average / enemyAIToDecide.Count;
        yield return null;
    }

    //Timer for how often to get all the enemies waiting for a desicion to start again
    IEnumerator WhatToDoTimer()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(WhatToDo(average));
        StartCoroutine(WhatToDoTimer());
        yield return null;
    }

    //When a shot is fired all enemies near by hear
    public IEnumerator ShotFired(Vector3 positionOfShot)
    {

        for(int i = 0; i < allCurrentEnemy.Count; i++)
        {
            if (allCurrentEnemy[i] == null) { continue; }
            //yield return new WaitForSeconds(1.0f);
            //Debug.Log(allCurrentEnemy[i]);
            if (hasHFSM)
            {
                if (Vector3.Distance(allCurrentEnemy[i].transform.position, positionOfShot) < audioRange && (allEnemyAIManagerScriptHFSM[i].currentFLState == allEnemyAIManagerScriptHFSM[i].nonCombatFLState || allEnemyAIManagerScriptHFSM[i].currentFLState == allEnemyAIManagerScriptHFSM[i].alertFLState))
                {
                    allEnemyAIManagerScriptHFSM[i].PlayRandomAudioOneShot(6, 8);
                    //Debug.Log("Player Heard");
                    allEnemyAIManagerScriptHFSM[i].playerNoticed = true;
                    allEnemyAIManagerScriptHFSM[i].SwitchFLState(allEnemyAIManagerScriptHFSM[i].combatFLState);
                    allEnemyAIManagerScriptHFSM[i].SwitchSLState(allEnemyAIManagerScriptHFSM[i].aggressionDesicionState);
                }
            }
            else
            {
                if (Vector3.Distance(allCurrentEnemy[i].transform.position, positionOfShot) < audioRange && (allEnemyAIManagerScript[i].currentState == allEnemyAIManagerScript[i].idleState || allEnemyAIManagerScript[i].currentState == allEnemyAIManagerScript[i].patrolState))
                {
                    StartCoroutine(allEnemyAIManagerScript[i].PlayAudioOneShot(6, 8));
                    //Debug.Log("Player Heard");
                    allEnemyAIManagerScript[i].playerNoticed = true;
                    allEnemyAIManagerScript[i].SwitchState(allEnemyAIManagerScript[i].aggressionDesicionState);
                }
            }
        }
        yield return null;
    }
}

//The class trait which helps give more personality and variance to the individual AIs. It is used to slightly change the outcome of certain desicions allowing the AI to seem like its doing its own desicions instead of the same one
public class Trait
{
    private string traitName;
    private float healthChange, agressionValue, approchPlayerChange;

    private float xMaxPosition, xMinPosition, yMaxPosition, yMinPosition, zMaxPosition, zMinPosition;

    public Trait(){}

    public Trait(string traitName, float healthChange, float agressionValueChange, float approchPlayerChange, float xMaxPosition, float xMinPosition, float yMaxPosition, float yMinPosition, float zMaxPosition, float zMinPosition)
    {
        this.traitName = traitName;
        this.healthChange = healthChange;
        this.agressionValue = agressionValueChange;
        this.approchPlayerChange = approchPlayerChange;
        this.xMaxPosition = xMaxPosition;
        this.xMinPosition = xMinPosition;
        this.yMaxPosition = yMaxPosition;
        this.yMinPosition = yMinPosition;
        this.zMaxPosition = zMaxPosition;
        this.zMinPosition = zMinPosition;
    }

    public void SetUpName(string traitName)
    {
        this.traitName = traitName;
    }

    public void SetUpHealthChange(float healthChange)
    {
        this.healthChange = healthChange;
    }

    public void SetUpAgressionValue(float agressionValue)
    {
        this.agressionValue = agressionValue;
    }

    public void SetUpApprochingPlayer(float approchPlayerChange)
    {
        this.approchPlayerChange = approchPlayerChange;
    }

    public void SetUpXMaxPosition(float xMaxPosition)
    {
        this.xMaxPosition = xMaxPosition;
    }

    public void SetUpXMinPosition(float xMinPosition)
    {
        this.xMinPosition = xMinPosition;
    }

    public void SetUpYMaxPosition(float yMaxPosition)
    {
        this.yMaxPosition = yMaxPosition;
    }

    public void SetUpYMinPosition(float yMinPosition)
    {
        this.yMinPosition = yMinPosition;
    }

    public void SetUpZMaxPosition(float zMaxPosition)
    {
        this.zMaxPosition = zMaxPosition;
    }

    public void SetUpZMinPosition(float zMinPosition)
    {
        this.zMinPosition = zMinPosition;
    }

    public void SetUpSearchArea(float xMaxPosition, float xMinPosition, float yMaxPosition, float yMinPosition, float zMaxPosition, float zMinPosition)
    {
        this.xMaxPosition = xMaxPosition;
        this.xMinPosition = xMinPosition;
        this.yMaxPosition = yMaxPosition;
        this.yMinPosition = yMinPosition;
        this.zMaxPosition = zMaxPosition;
        this.zMinPosition = zMinPosition;
    }

    public string ReturnName()
    {
        return traitName;
    }

    public float ReturnHealthChange()
    {
        return healthChange;
    }

    public float ReturnAgressionValue()
    {
        return agressionValue;
    }

    public float ReturnApprochingPlayer()
    {
        return approchPlayerChange;
    }

    public float ReturnXMaxPosition()
    {
        return xMaxPosition;
    }

    public float ReturnXMinPosition()
    {
        return xMinPosition;
    }

    public float ReturnYMaxPosition()
    {
        return yMaxPosition;
    }

    public float ReturnYMinPosition()
    {
        return yMinPosition;
    }

    public float ReturnZMaxPosition()
    {
        return zMaxPosition;
    }

    public float ReturnZMinPosition()
    {
        return zMinPosition;
    }

    public void ReturnSearchArea(out float xMaxPosition, out float xMinPosition, out float yMaxPosition, out float yMinPosition, out float zMaxPosition, out float zMinPosition)
    {
        xMaxPosition = this.xMaxPosition;
        xMinPosition = this.xMinPosition;
        yMaxPosition = this.yMaxPosition;
        yMinPosition = this.yMinPosition;
        zMaxPosition = this.zMaxPosition;
        zMinPosition = this.zMinPosition;
    }
}
