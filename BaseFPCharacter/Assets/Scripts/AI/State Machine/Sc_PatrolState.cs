using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class Sc_PatrolState : Sc_AIBaseState
{
    private Sc_CommonMethods aiCommonMethods;
    private Sc_AIStateManager stateManager;
    private Sc_Player_Movement playerMovementScript;

    private GameObject[] allPatrolPoints, patrolPoints = new GameObject[5];
    private int currentPos;

    private Vector3 movePositionTransfrom;

    private float visionRange, visionConeAngle;

    // Bit shift the index of the layer (7) to get a bit mask
    private int layerMask = 1 << 6;

    private RaycastHit hit;

    public override void EnterState(float speed, bool playerNoticed) {
        //Debug.Log("Patroling");
        ChooseRandomPatrolPos();
    }

    public override void UpdateState(float distPlayer, float angleToPlayer) {
        CanSeePlayer(distPlayer, angleToPlayer);

        //Debug.Log(self.name + " Desination: " + navMeshAgent.destination);

        /*if (Vector3.Distance(navMeshAgent.destination, movePositionTransfrom.position) < 0.5f)
        {
            Patroling();
        }
        else
        {
            stateManager.SetCurrentAction("Going to patrol point");
            navMeshAgent.destination = movePositionTransfrom.position;
        }*/
    }


    public void PatrolStartStateInfo(Sc_AIStateManager stateManager, Sc_CommonMethods aiCommonMethods, Sc_Player_Movement playerMovementScript, GameObject[] allPatrolPoints, float distRange, float visionAngleRange, float audioDist)
    {
        this.aiCommonMethods = aiCommonMethods;
        this.stateManager = stateManager;
        this.playerMovementScript = playerMovementScript;
        this.allPatrolPoints = allPatrolPoints;
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


        movePositionTransfrom = patrolPoints[currentPos].transform.position;
        aiCommonMethods.StartMovement(movePositionTransfrom, "Patrolling", false);
    }

    public void CanSeePlayer(float distPlayer, float angleToPlayer)
    {
        Vector3 direction = playerMovementScript.transform.position - stateManager.transform.position;
        bool seePlayer = Physics.Raycast(stateManager.transform.position, direction, out hit, visionRange - 5, layerMask);

        bool playerHidden = playerMovementScript.ReturnIsHidden();

        Debug.Log(seePlayer);
        if ((distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5) && !playerHidden && seePlayer)
        {
            //directorAI.PlayerFound(state.gameObject);
            stateManager.playerNoticed = true;
            stateManager.SwitchState(stateManager.aggressionDesicionState);
        }
    }
}