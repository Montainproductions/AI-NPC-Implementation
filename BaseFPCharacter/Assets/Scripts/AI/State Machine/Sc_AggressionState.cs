using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The Aggression state is meant to be a transitionary state in the general state machine from idling/patrolling into combat against the player. The state mostly calculates how close the player is and if they can attack them or if there are a lot of cover positions near by. This will then be placed into the decitionVal variable and be sent to the director AI which then decide which enemy will attack the player or go to cover and change to the corisponding state.
/// </summary>
public class Sc_AggressionState : Sc_AIBaseState
{
    //The manager of the AI that controls all of the info and transitions of state for the AI.
    private Sc_AIStateManager stateManager;
    //The director AI to send inte desicion value
    private Sc_AIDirector directorAI;
    //The script for the weapons
    private Sc_BaseGun baseGunScript;

    //A bool for if the player has been noticed or not.
    private bool playerNoticed;

    //The player and the main AI game object
    private GameObject player, self;
    //All the cover positions in the map
    private GameObject[] coverPositions;

    //The nav mesh agent of the AI
    private NavMeshAgent navMeshAgent;

    //Currentl player position
    private Vector3 playerPos;

    //The optimal attack range of the current weapon and the current cover distance.
    private float attackRange, coverDistance;

    //The decision val of the AI
    private int decisionVal;

    //Method for when the AI first enters the state. Will determine if the player was already found or if they are the first enemy to notice the player. Will then start calculating the value for wether to attack or run to cover.
    public override void EnterState( bool playerSeen)
    {
        playerNoticed = playerSeen;
        stateManager.StartCoroutine(StoppingAI());
        if (!playerNoticed)
        {
            playerNoticed = true;
            directorAI.PlayerFound(stateManager.gameObject);
        }
        //Debug.Log(self.name + " Player detected");

        decisionVal = 0;
        attackRange = baseGunScript.ReturnEffectiveRange();
        WhenToAttack();
    }

    //Continuasly updates the players current position and to have the enemy look at the player position
    public override void UpdateState(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
    }

    //Reciving all the important information that the state needs to operate
    public void AggressionStartStateInfo(Sc_AIStateManager stateManager, Sc_AIDirector directorAI, GameObject self, GameObject player, GameObject currentWeapon, GameObject[] coverPos, NavMeshAgent navMeshAgent, float coverDist)
    {
        this.stateManager = stateManager;
        this.directorAI = directorAI;
        this.self = self;
        this.player = player;
        baseGunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        this.coverPositions = coverPos;
        this.navMeshAgent = navMeshAgent;
        this.coverDistance = coverDist;


    }

    /*This method determines the AIs decision value. If the AIs is close enough to the player and their weapons range is less then the distance then it will increase the value by 2.
    I use a random range of the attack range and the range -2 because that will simulate the enemy determining the range it belives it needs to be at to confidently deal damage
    to the player.It will then go through each cover positon in the map and if the distance between the AI and the cover position is less then the cover distance that has already
    been predetermined then it will decrease the decision Value by 1. It will then set the AI managers current decision value the the final one it has calculated and will also
    state to the directorAI that the current AI wishes to attack the player and to determine if there is space for it to do so.*/
    public void WhenToAttack()
    {
        float distFromPlayer = Vector3.Distance(player.transform.position, stateManager.transform.position);
        float currentAttackRange = Random.Range(attackRange, attackRange - 3);
        if (currentAttackRange > distFromPlayer)
        {
            decisionVal += 2;
        }
        foreach(GameObject i in coverPositions)
        {
            float distFromCover = Vector3.Distance(stateManager.transform.position, i.transform.position);
            if(distFromCover < coverDistance)
            {
                decisionVal--;
            }
        }
        //Debug.Log("Obj: " + self.name + " Value: " + decisionVal);

        stateManager.SetDecisionValue(decisionVal);
        
        directorAI.AIAttackAddList(self);
    }

    //This method will have the AI stop moving towards any position that it might be going to. This is princippaly so that if the AI is transitioning from the patrol state then it will actually stop moving towards its patrol point and face the player.
    IEnumerator StoppingAI()
    {
        yield return new WaitForSeconds(0.45f);
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(stateManager.transform.position);
        Debug.Log(stateManager.name);
        Debug.Log(navMeshAgent.destination);
        yield return null;
    }
}
