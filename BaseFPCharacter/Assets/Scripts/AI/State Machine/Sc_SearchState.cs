using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_SearchState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;

    private GameObject self, player;
    private NavMeshAgent navMeshAgent;

    private Vector3 playerLastLocation;

    public override void EnterState(float speed, bool playerSeen)
    {
        Debug.Log("Searching for Player");
        playerLastLocation = player.transform.position;
    }

    public override void UpdateState(float distPlayer, float angleToPlayer)
    {

    }

    public void SearchStartStateInfo(GameObject self, GameObject player, Sc_AIStateManager stateManager, NavMeshAgent navMeshAgent)
    {
        this.self = self;
        this.player = player;
        this.stateManager = stateManager;
        this.navMeshAgent = navMeshAgent;
    }
}
