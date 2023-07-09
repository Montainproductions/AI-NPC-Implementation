using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Sc_SearchingSLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStateManager stateManager;
    private Sc_HFSMCommenMethods commonMethods;
    private Sc_Player_Movement playerMovementScript;

    private GameObject self, player, chosenSearchPath;
    private GameObject[] searchPathOptions;

    private NavMeshAgent navMeshAgent;

    private Vector3 playerLastLocation, chosenPosition;

    //Area where a walking position can be determined
    private float xPosition, xMaxPosition, xMinPosition, yPosition, yMaxPosition, yMinPosition, zPosition, zMaxPosition, zMinPosition;

    private bool inRange;

    private float radiusSpawnCheck, distToPlayerPosition, distToLastPlayerPosition;

    public override void EnterState(Vector3 playerPosition)
    {
        playerLastLocation = playerPosition;

    }

    public override void UpdateState()
    {

    }

    public void SearchStartStateInfo(Sc_AIStateManager stateManager, Sc_HFSMCommenMethods commonMethods, Sc_Player_Movement playerMovementScript, GameObject self, GameObject player, GameObject[] searchPathOptions, NavMeshAgent navMeshAgent, float radiusSpawnCheck)
    {
        this.stateManager = stateManager;
        this.commonMethods = commonMethods;
        this.playerMovementScript = playerMovementScript;
        this.self = self;
        this.player = player;
        this.searchPathOptions = searchPathOptions;
        this.navMeshAgent = navMeshAgent;
        this.radiusSpawnCheck = radiusSpawnCheck;
    }

    public void SetUpTrait(Trait aiTrait)
    {
        xMaxPosition = aiTrait.ReturnXMaxPosition();
        xMinPosition = aiTrait.ReturnXMinPosition();
        yMaxPosition = aiTrait.ReturnYMaxPosition();
        yMinPosition = aiTrait.ReturnYMinPosition();
        zMaxPosition = aiTrait.ReturnZMaxPosition();
        zMinPosition = aiTrait.ReturnZMinPosition();

    }

    public void ChoosePosition()
    {
        while (RadiusCheck(chosenPosition) || chosenPosition == Vector3.zero)
        {
            xPosition = Random.Range(xMinPosition, xMaxPosition);
            yPosition = Random.Range(yMinPosition, yMaxPosition);
            zPosition = Random.Range(zMinPosition, zMaxPosition);

            chosenPosition = new Vector3(xPosition, yPosition, zPosition);
        }

        distToLastPlayerPosition = Vector3.Distance(playerLastLocation,chosenPosition);

        commonMethods.StartMovement(chosenPosition, "Searching");

        if (distToPlayerPosition <= 2)
        {
            LookAround();
            //GoToClosestHidingLocation();
        }

        RestartChoosenPosition();
    }

    public void LookAround()
    {
        commonMethods.LookRandomDirections(12);
    }
    
    public void GoToClosestHidingLocation()
    {

    }

    public bool RadiusCheck(Vector3 walkingPos)
    {
        inRange = Physics.CheckSphere(walkingPos, radiusSpawnCheck);
        return inRange;
    }

    public void RestartChoosenPosition()
    {
        chosenPosition = Vector3.zero;
    }
}
