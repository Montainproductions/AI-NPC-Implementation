using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_PatrolState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;
    private Sc_Player_Movement playerMovementScript;

    private GameObject[] allPatrolPoints, patrolPoints = new GameObject[5];
    private int currentPos;

    private NavMeshAgent navMeshAgent;

    private Transform movePositionTransfrom;

    private float visionRange, visionConeAngle;

    public override void EnterState(float speed, bool playerNoticed) {
        Debug.Log("Patroling");
        ChooseRandomPatrolPos();
    }

    public override void UpdateState(float distPlayer, float angleToPlayer) {
        CanSeePlayer(distPlayer, angleToPlayer);

        //Debug.Log(self.name + " Desination: " + navMeshAgent.destination);

        if (Vector3.Distance(navMeshAgent.destination, movePositionTransfrom.position) < 0.5f)
        {
            Patroling();
        }
        else
        {
            stateManager.SetCurrentAction("Going to patrol point");
            navMeshAgent.destination = movePositionTransfrom.position;
        }
    }


    public void PatrolStartStateInfo(Sc_AIStateManager stateManager, Sc_Player_Movement playerMovementScript, GameObject[] allPatrolPoints, NavMeshAgent navMeshAgent, float distRange, float visionAngleRange)
    {
        this.stateManager = stateManager;
        this.playerMovementScript = playerMovementScript;
        this.allPatrolPoints = allPatrolPoints;
        this.navMeshAgent = navMeshAgent;
        visionRange = distRange;
        visionConeAngle = visionAngleRange;
    }

    public void ChooseRandomPatrolPos()
    {
           
        for (int i = 0; i < 5; i++)
        {
            //Debug.Log(allPatrolPoints[Random.Range(0, allPatrolPoints.Length)]);
            patrolPoints[i] = allPatrolPoints[Random.Range(0, allPatrolPoints.Length)];
        }
        currentPos = 0;
        Patroling();
    }

    public void Patroling()
    {
        if (currentPos >= patrolPoints.Length - 1)
        {
            currentPos = 0;
        }
        else
        {
            currentPos++;
        }


        movePositionTransfrom = patrolPoints[currentPos].transform;
    }

    public void CanSeePlayer(float distPlayer, float angleToPlayer)
    {
        bool playerHidden = playerMovementScript.IsHiddenReturn();
        if ((distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5) || !playerHidden)
        {
            //directorAI.PlayerFound(state.gameObject);
            stateManager.playerNoticed = true;
            stateManager.SwitchState(stateManager.aggressionDesicionState);
        }
    }
}