using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_PatrolState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;

    private GameObject[] allPatrolPoints, patrolPoints = new GameObject[5];
    private int currentPos;

    private NavMeshAgent navMeshAgent;

    private Transform movePositionTransfrom;

    private float visionRange, visionConeAngle, alertedTimer, rayCastRange, distPlayer;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        Debug.Log("Patroling");
        ChooseRandomPatrolPos();
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        CanSeePlayer(state, distPlayer, angleToPlayer);

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


    public void PatrolStartStateInfo(GameObject[] patrolPoints, NavMeshAgent aiNavigationAgent, float distRange, float visionAngleRange, Sc_AIStateManager stateManager)
    {
        allPatrolPoints = patrolPoints;
        navMeshAgent = aiNavigationAgent;
        visionRange = distRange;
        visionConeAngle = visionAngleRange;
        this.stateManager = stateManager;
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

    public void CanSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5)
        {
            //directorAI.PlayerFound(state.gameObject);
            state.SwitchState(state.aggressionDesicionState);
        }
    }
}