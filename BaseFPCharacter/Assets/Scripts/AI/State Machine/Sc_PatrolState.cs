using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_PatrolState : Sc_AIBaseState
{
    private GameObject self;
    private GameObject[] allPatrolPoints, patrolPoints = new GameObject[5];
    private int currentPos;

    private NavMeshAgent navMeshAgent;

    private Transform movePositionTransfrom;

    private float visionRange, visionConeAngle, alertedTimer, rayCastRange, distPlayer;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        ChooseRandomPatrolPos();
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        state.StartCoroutine(CanSeePlayer(state, distPlayer, angleToPlayer));

        navMeshAgent.destination = movePositionTransfrom.position;
        //Debug.Log(self.name + " Desination: " + navMeshAgent.destination);

        if (Vector3.Distance(navMeshAgent.destination, movePositionTransfrom.position) < 0.5f)
        {
            Patroling();
        }
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }


    public void PatrolStartStateInfo(GameObject[] patrolPoints, NavMeshAgent aiNavigationAgent, float distRange, float visionAngleRange, GameObject selfObj)
    {
        allPatrolPoints = patrolPoints;
        navMeshAgent = aiNavigationAgent;
        this.visionRange = distRange;
        this.visionConeAngle = visionAngleRange;
        self = selfObj;
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

    IEnumerator CanSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5)
        {
            state.SwitchState(state.aggressionState);
            yield return null;
        }
        yield return null;
    }
}