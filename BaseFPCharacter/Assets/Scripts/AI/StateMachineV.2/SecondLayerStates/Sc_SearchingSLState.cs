using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Sc_SearchingSLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStatesManagerHierarchical stateManager;
    private Sc_HFSMCommenMethods commonMethods;
    private Sc_Player_Movement playerMovementScript;

    private GameObject self, player, chosenSearchPath;
    private GameObject[] searchPathOptions;

    private NavMeshAgent navMeshAgent;

    private Vector3 playerLastLocation, chosenPosition;

    //Area where a walking position can be determined
    private float xPosition, xMaxPosition, xMinPosition, yPosition, yMaxPosition, yMinPosition, zPosition, zMaxPosition, zMinPosition;

    private Vector3[] walkingPositions = new Vector3[3];
    private int positionsCreated, searchFormat;

    private bool inRange;

    private float radiusSpawnCheck, searchTimer, distToLastPlayerPosition;

    public override void EnterState(Vector3 playerPosition)
    {
        playerLastLocation = playerPosition;
        positionsCreated = 0;
        ChooseSearchPath();

        stateManager.StartCoroutine(commonMethods.CloseFoiliage());
    }

    public override void UpdateState()
    {

    }

    public void SearchStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commonMethods, Sc_Player_Movement playerMovementScript, GameObject self, GameObject player, GameObject[] searchPathOptions, NavMeshAgent navMeshAgent, float radiusSpawnCheck, float searchTimer)
    {
        this.stateManager = stateManager;
        this.commonMethods = commonMethods;
        this.playerMovementScript = playerMovementScript;
        this.self = self;
        this.player = player;
        this.searchPathOptions = searchPathOptions;
        this.navMeshAgent = navMeshAgent;
        this.radiusSpawnCheck = radiusSpawnCheck;
        this.searchTimer = searchTimer;
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

    public void ChooseSearchPath()
    {
        //zigzag, straight, middle, to closest hiding spot

        searchFormat = Random.Range(0,3);

        if (searchFormat == 0)
        {
            ChoosePosition(xMaxPosition + 2, xMinPosition - 2, zMaxPosition - 2, zMinPosition + 2);
        }
        else if(searchFormat == 1)
        {
            ChoosePosition(xMaxPosition - 2, xMinPosition + 2, zMaxPosition + 2, zMinPosition - 2);
        }
        else if(searchFormat == 2)
        {
            ChoosePosition(xMaxPosition, xMinPosition, zMaxPosition, zMinPosition);
        }
        else if (searchFormat == 3)
        {

        }
    }

    public void ChoosePosition(float xMaxPositionUpd, float xMinPositionUpd, float zMaxPositionUpd, float zMinPositionUpd)
    {

        while (positionsCreated < walkingPositions.Length && chosenPosition == Vector3.zero)
        {
            xPosition = Random.Range(xMinPositionUpd, xMaxPositionUpd);
            yPosition = Random.Range(yMinPosition, yMaxPosition);
            zPosition = Random.Range(zMinPositionUpd, zMaxPositionUpd);

            chosenPosition = new Vector3(xPosition, yPosition, zPosition);

            if (RadiusCheck(chosenPosition))
            {
                distToLastPlayerPosition = Vector3.Distance(playerLastLocation, chosenPosition);
                walkingPositions[positionsCreated] = chosenPosition;
                positionsCreated++;
            }
        }

        commonMethods.StartMovement(walkingPositions, "Searching");
        RestartChoosenPosition();
    }

    IEnumerator LookAround()
    {
        commonMethods.LookRandomDirections(searchTimer);
        yield return new WaitForSeconds(searchTimer);
        stateManager.SwitchFLState(stateManager.nonCombatFLState);
        stateManager.SwitchSLState(stateManager.patrolState);
        yield return null;
    }
    
    public void GoToClosestHidingLocation()
    {

    }

    public void FinishedWalking()
    {
        stateManager.StartCoroutine(LookAround());
    }

    public void RestartChoosenPosition()
    {
        chosenPosition = Vector3.zero;
        for (int i = 0; i < 3; i++) 
        {
            walkingPositions[i] = Vector3.zero;
        }
        positionsCreated = 0;
    }

    public bool RadiusCheck(Vector3 walkingPos)
    {
        inRange = Physics.CheckSphere(walkingPos, radiusSpawnCheck);
        return inRange;
    }
}
