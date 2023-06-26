using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PatrolingSLState : Sc_AIBaseStateSL
{
    private Sc_HFSMCommenMethods aiCommonMethods;

    private GameObject[] allPatrolPoints, patrolPoints;
    private int currentPos;

    private Vector3 movePositionTransfrom;

    public override void EnterState()
    {
        currentPos = 0;
        ChooseRandomPatrolPos();
    }

    public override void UpdateState()
    {

    }

    public void PatrolStartStateInfo(Sc_HFSMCommenMethods aiCommonMethods, GameObject[] allPatrolPoints)
    {
        this.aiCommonMethods = aiCommonMethods;
        this.allPatrolPoints = allPatrolPoints;
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


        movePositionTransfrom = patrolPoints[currentPos].transform.position;
        aiCommonMethods.StartMovement(movePositionTransfrom, "Patrolling", false);
    }
}
