using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_SearchState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;
    private Sc_Player_Movement playerMovementScript;

    private GameObject self, player, chosenSearchPath;
    private GameObject[] searchPathOptions;

    private NavMeshAgent navMeshAgent;

    private float visionRange, visionConeAngle;

    private Vector3 playerLastLocation;
    public override void EnterState(float speed, bool playerSeen)
    {
        //Debug.Log("Searching for Player");
        playerLastLocation = player.transform.position;
        stateManager.StartCoroutine(ChooseSearchPath());
    }

    public override void UpdateState(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {

    }

    public void SearchStartStateInfo(Sc_AIStateManager stateManager, Sc_Player_Movement playerMovementScript, GameObject self, GameObject player, GameObject[] searchPathOptions, NavMeshAgent navMeshAgent, float visionRange, float visionConeAngle)
    {
        this.stateManager = stateManager;
        this.playerMovementScript = playerMovementScript;
        this.self = self;
        this.player = player;
        this.searchPathOptions = searchPathOptions;
        this.navMeshAgent = navMeshAgent;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
    }

    public void CanSeePlayer(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {
        bool playerHidden = playerMovementScript.ReturnIsHidden();
        if ((distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5) && !playerHidden && !playerBehindWall)
        {
            //directorAI.PlayerFound(state.gameObject);
            stateManager.playerNoticed = true;
            stateManager.SwitchState(stateManager.aggressionDesicionState);
        }
    }

    IEnumerator ChooseSearchPath()
    {
        yield return null;
    }
}
