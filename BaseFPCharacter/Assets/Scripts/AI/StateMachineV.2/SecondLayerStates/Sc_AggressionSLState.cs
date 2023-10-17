using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AggressionSLState : Sc_AIBaseStateHierarchical
{
    //The manager of the AI that controls all of the info and transitions of state for the AI.
    private Sc_AIStatesManagerHierarchical stateManager;
    
    //The director AI to send inte desicion value
    private Sc_AIDirector directorAI;

    //Script containing a varaity of commen methods
    public Sc_HFSMCommenMethods commenMethods;

    //Current AI trait
    private Trait aiTrait;

    //The player and the main AI game object
    private GameObject self, player;

    //The script for the weapons
    private Sc_BaseGun baseGunScript;

    //All the cover positions in the map
    private GameObject[] coverPositions;

    //Currentl player position
    private Vector3 playerPos;

    //The optimal attack range of the current weapon and the current cover distance.
    private float attackRange, coverDistance;

    //The decision val of the AI
    private float decisionVal;

    //First frame that runs when the state is called for the first time
    public override void EnterState(Vector3 playerPosition)
    {
        Debug.Log("Desicion SL");
        decisionVal = 0;
        attackRange = baseGunScript.ReturnEffectiveRange();
        WhenToAttack();
    }

    //Runs every frame the state is active
    public override void UpdateState()
    {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
    }

    //Sets variables for important variable needed in the state
    public void AggressionStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commenMethods, Sc_AIDirector directorAI, GameObject self, GameObject player, GameObject currentWeapon, GameObject[] coverPos, Trait aiTrait, float coverDist)
    {
        this.stateManager = stateManager;
        this.commenMethods = commenMethods;
        this.directorAI = directorAI;
        this.self = self;
        this.player = player;
        baseGunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        this.aiTrait = aiTrait;
        this.coverPositions = coverPos;
        this.coverDistance = coverDist;
    }

    //Sets up the AI trait
    public void SetUpTrait(Trait newAITrait)
    {
        this.aiTrait = newAITrait;
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

        //Debug.Log(aiTrait.ReturnAgressionValue());

        if (currentAttackRange > distFromPlayer)
        {
            decisionVal += 2;
        }
        foreach (GameObject i in coverPositions)
        {
            float distFromCover = Vector3.Distance(stateManager.transform.position, i.transform.position);
            if (distFromCover < coverDistance)
            {
                decisionVal--;
            }
        }

        decisionVal += aiTrait.ReturnAgressionValue();

        Debug.Log("Enemy name: " + self.name + " Value: " + decisionVal);
        stateManager.StartCoroutine(commenMethods.StopMovement());
        stateManager.SetDecisionValue(decisionVal);

        directorAI.AIAttackAddList(self);
        decisionVal = 0;
        stateManager.StartCoroutine(AITakingTooLong());
    }

    //Forces the AI to change to the cover state if it takes to long to choose which state to change to.
    IEnumerator AITakingTooLong()
    {
        yield return new WaitForSeconds(6.0f);
        if (stateManager.currentSLState == stateManager.aggressionDesicionState)
        {
            stateManager.SwitchSLState(stateManager.coverState);
        }
        yield return null;
    }
}
