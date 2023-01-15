using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PatrolState : Sc_AIBaseState
{
    private GameObject[] allPatrolPoints, patrolPoints;
    private int amountOfPoints, pointsPerEnemy;

    private float speed;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        this.speed = speed;
        ChooseRandomPos();
    }

    public override void UpdateState(Sc_AIStateManager state) { }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }


    public void PatrolStateInfo(GameObject[] patrolPoints, int pPE)
    {
        allPatrolPoints = patrolPoints;
        pointsPerEnemy = pPE;
    }

    public void ChooseRandomPos()
    {
        amountOfPoints = Random.Range(0, allPatrolPoints.Length);
        for (int i = 0; i < amountOfPoints; i++)
        {
            patrolPoints[i] = allPatrolPoints[Random.Range(0, allPatrolPoints.Length)];
        }
    }
}