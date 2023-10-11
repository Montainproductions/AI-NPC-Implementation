using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PatrolingSLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStatesManagerHierarchical stateManager;

    //Common methods script
    private Sc_HFSMCommenMethods commenMethods;

    //Arrays containg all of the patrol points it can use and the current ones the AI is walking to.
    private GameObject[] allPatrolPoints, patrolPoints = new GameObject[5];
    private int currentPos;

    //current position it will walk to
    private Vector3 movePositionTransfrom;

    //When first starting the state it will run through this
    public override void EnterState(Vector3 playerPosition)
    {
        Debug.Log("Patrolling SL");
        currentPos = 0;
        ChooseRandomPatrolPos();
    }

    public override void UpdateState()
    {

    }

    //Sets up all important information given by the AI manager
    public void PatrolStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commenMethods, GameObject[] allPatrolPoints)
    {
        this.stateManager = stateManager;
        this.commenMethods = commenMethods;
        this.allPatrolPoints = allPatrolPoints;
    }

    //Chooses a random set of patrol points and places them in a seperate array
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

    //Has the AI patrol through all of the points it has selected. Might change since I have stared to work on a method that allows for entire arrays to be sent.
    public void Patroling()
    {
        stateManager.SetIsWalking(true);
        if (currentPos >= patrolPoints.Length - 1)
        {
            currentPos = 0;
        }
        else
        {
            currentPos++;
        }


        movePositionTransfrom = patrolPoints[currentPos].transform.position;
        commenMethods.StartMovement(movePositionTransfrom, "Patrolling");
    }
}
