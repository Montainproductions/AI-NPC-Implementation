using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_PatrolState : Sc_AIBaseState
{
    private GameObject[] allPatrolPoints, patrolPoints;
    private int amountOfPoints, currentPos;

    private NavMeshAgent navMeshAgent;

    private Transform movePositionTransfrom;

    private float speed;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        this.speed = speed;
        ChooseRandomPatrolPos();
    }

    public override void UpdateState(Sc_AIStateManager state) {
        navMeshAgent.destination = movePositionTransfrom.position;

        if (Vector3.Distance(navMeshAgent.destination, movePositionTransfrom.position) < 0.5f)
        {
            Patroling();
        }
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }


    public void PatrolStateInfo(GameObject[] patrolPoints, NavMeshAgent aiNavigationAgent)
    {
        allPatrolPoints = patrolPoints;
        navMeshAgent = aiNavigationAgent;
    }

    public void ChooseRandomPatrolPos()
    {
        amountOfPoints = Random.Range(0, allPatrolPoints.Length);
        for (int i = 0; i < amountOfPoints; i++)
        {
            patrolPoints[i] = allPatrolPoints[Random.Range(0, allPatrolPoints.Length)];
        }
        currentPos = 0;
        Patroling();
    }

    public void Patroling()
    {
        if (currentPos > patrolPoints.Length)
        {
            currentPos = 0;
        }
        else
        {
            currentPos++;
        }


        movePositionTransfrom = patrolPoints[currentPos].transform;
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(0.75f);
        yield return null;
    }
}