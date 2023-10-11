using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AlertFLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStatesManagerHierarchical stateManager;

    private Sc_AIDirector directorAI;

    private GameObject player;
    private Sc_Player_Movement playerMovemenetScript;

    private Transform aitransform;

    private float checkTimer;

    private float distPlayer, angleToPlayer;

    //AI vision
    private float visionRange, visionConeAngle;

    private bool playerSeen;

    //Check if Behindwall
    private bool playerBehindWall;
    private RaycastHit hit;
    private Vector3 direction;
    //To check if the player is being blocked by some objects.
    // Bit shift the index of the layer (9) to get a bit mask
    private int layerMask = 1 << 9;

    public override void EnterState(Vector3 playerPosition)
    {
        /*if(stateManager.currentSLState == stateManager.alertedState)
        {
            checkTimer = 1.25f;
        }
        else if(stateManager.currentSLState == stateManager.searchState)
        {
            checkTimer = 6;
        }*/

        Debug.Log("Alerted FL");
        stateManager.StartCoroutine(CanSeePlayer());
    }

    public override void UpdateState()
    {
        distPlayer = Vector3.Distance(player.transform.position, aitransform.position);
        angleToPlayer = Vector3.Angle(aitransform.forward, player.transform.position - aitransform.position);

        direction = player.transform.position - aitransform.position;
        playerBehindWall = Physics.Raycast(aitransform.position, direction, out hit, visionRange - 5, layerMask);
    }

    public void AlertSetUp(Sc_AIStatesManagerHierarchical stateManager, Sc_AIDirector directorAI, GameObject player, Transform aitransform)
    {
        this.stateManager = stateManager;
        this.directorAI = directorAI;
        this.player = player;
        playerMovemenetScript = player.GetComponent<Sc_Player_Movement>();
        this.aitransform = aitransform;
    }

    IEnumerator CanSeePlayer()
    {
        //yield return new WaitForSeconds(0.25f);

        playerSeen = PlayerInVision(distPlayer, angleToPlayer, playerBehindWall);
        Debug.Log(playerSeen);
        if (playerSeen)
        {
            yield return new WaitForSeconds(0.75f);
            //stateManager.PlayRandomAudioOneShot(6, 8);
            directorAI.PlayerFound(stateManager.gameObject);
            stateManager.playerNoticed = true;
            stateManager.SwitchFLState(stateManager.combatFLState);
            stateManager.SwitchSLState(stateManager.aggressionDesicionState);
            Debug.Log("Player First Seen");
        }
        else
        {
            yield return new WaitForSeconds(0.75f);
            stateManager.PlayRandomAudioOneShot(3, 5);
            stateManager.SwitchFLState(stateManager.nonCombatFLState);
            stateManager.SwitchSLState(stateManager.patrolState);
        }
        yield return null;
    }

    public bool PlayerInVision(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {
        bool playerHidden = playerMovemenetScript.ReturnIsHidden();
        Debug.Log("In vision cone: " + (distPlayer <= visionRange - 15 && angleToPlayer <= visionConeAngle - 15));
        Debug.Log("Player Hidden: " + playerHidden);
        Debug.Log("Player Behind Wall: " + playerBehindWall);
        if ((distPlayer <= visionRange - 15 && angleToPlayer <= visionConeAngle - 15) && !playerHidden && !playerBehindWall)
        {
            return true;
        }
        return false;
    }
}
