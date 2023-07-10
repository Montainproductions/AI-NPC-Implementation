using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Sc_SearchingSLState : Sc_AIBaseStateHierarchical
{
    //State manager script
    private Sc_AIStatesManagerHierarchical stateManager;

    //Commen methods script.
    private Sc_HFSMCommenMethods commenMethods;

    //Last known location of the player and the position the AI wants to walk to
    private Vector3 playerLastLocation, chosenPosition;

    //Area where a walking position can be determined
    private float xPosition, xMaxPosition, xMinPosition, yPosition, yMaxPosition, yMinPosition, zPosition, zMaxPosition, zMinPosition;

    //Collection of positions that the AI wants to walk to
    private Vector3[] walkingPositions = new Vector3[3];
    //Variables used to keep count of amount of positions created and any variance the search patter will have
    private int positionsCreated, searchFormat;

    //Bool checking if new position is in range of any objects
    private bool inRange;

    //An asortment of variables used for the size of the radius check, time the AI should spend searching an area for the player and its distance to the last known location of the player
    private float radiusSpawnCheck, searchTimer, distToLastPlayerPosition;

    //Initial method that runs on the first frame it enters the state
    public override void EnterState(Vector3 playerPosition)
    {
        playerLastLocation = playerPosition;
        positionsCreated = 0;
        ChooseSearchPath();

        stateManager.StartCoroutine(commenMethods.CloseFoiliage());
    }

    public override void UpdateState()
    {

    }

    //Setting up important variables the state needs to operate
    public void SearchStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commenMethods, float radiusSpawnCheck, float searchTimer)
    {
        this.stateManager = stateManager;
        this.commenMethods = commenMethods;
        this.radiusSpawnCheck = radiusSpawnCheck;
        this.searchTimer = searchTimer;
    }

    //Sets up the trait information
    public void SetUpTrait(Trait aiTrait)
    {
        xMaxPosition = aiTrait.ReturnXMaxPosition();
        xMinPosition = aiTrait.ReturnXMinPosition();
        yMaxPosition = aiTrait.ReturnYMaxPosition();
        yMinPosition = aiTrait.ReturnYMinPosition();
        zMaxPosition = aiTrait.ReturnZMaxPosition();
        zMinPosition = aiTrait.ReturnZMinPosition();

    }

    //Determines how straight forward or not should the AI search for the player. Helps give more variance to hte way the AI is searching and reacting to the player for the player
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

    //Chooses a position and checks if it is somewhere that it can walk to or if it is colliding with anything.
    //Side Note: I am first testing that it works on a flat serface and then will add and test if the AI can also move when elevation is at play. That is why currently its only create a new position and changing 2 out of the 3 directions.
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

        commenMethods.StartMovement(walkingPositions, "Searching");
        RestartChoosenPosition();
    }

    //Will look around its current position as if searchng for the player. 
    IEnumerator LookAround()
    {
        commenMethods.LookRandomDirections(searchTimer);
        yield return new WaitForSeconds(searchTimer);
        stateManager.SwitchFLState(stateManager.nonCombatFLState);
        stateManager.SwitchSLState(stateManager.patrolState);
        yield return null;
    }

    //Once it has finished walking through all of its positions it will look around.
    public void FinishedWalking()
    {
        stateManager.StartCoroutine(LookAround());
    }


    //Will check close by foiliage and cover positions of note in case the player has hidden in them. This would make them seem like they are eliminating all places they player could be hidden
    public void GoToClosestLocation()
    {

    }

    //Restarts all of the positions in the array. More for future cases if one isnt changes.
    public void RestartChoosenPosition()
    {
        chosenPosition = Vector3.zero;
        for (int i = 0; i < 3; i++) 
        {
            walkingPositions[i] = Vector3.zero;
        }
        positionsCreated = 0;
    }

    //Checks if anything colides with the object
    public bool RadiusCheck(Vector3 walkingPos)
    {
        inRange = Physics.CheckSphere(walkingPos, radiusSpawnCheck);
        return inRange;
    }
}
