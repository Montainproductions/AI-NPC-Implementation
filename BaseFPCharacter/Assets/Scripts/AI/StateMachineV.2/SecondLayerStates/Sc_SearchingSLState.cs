using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_SearchingSLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStateManager stateManager;
    private Sc_Player_Movement playerMovementScript;

    private GameObject self, player, chosenSearchPath;
    private GameObject[] searchPathOptions;

    private NavMeshAgent navMeshAgent;

    private float visionRange, visionConeAngle;

    private Vector3 playerLastLocation;
    public override void EnterState()
    {
        
    }

    public override void UpdateState()
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
    }

    IEnumerator ChooseSearchPath()
    {
        yield return null;
    }
}
