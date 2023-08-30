using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class Sc_NonCombatFLState : Sc_AIBaseStateHierarchical
{
    private GameObject player;

    private Sc_AIStatesManagerHierarchical stateManager;

    private Sc_Player_Movement playerMovemenetScript;

    private Transform aitransform;

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
        
    }

    public override void UpdateState()
    {
        distPlayer = Vector3.Distance(player.transform.position, aitransform.position);
        angleToPlayer = Vector3.Angle(aitransform.forward, player.transform.position - aitransform.position);

        direction = player.transform.position - aitransform.position;
        playerBehindWall = Physics.Raycast(aitransform.position, direction, out hit, visionRange - 5, layerMask);

        CanSeePlayer();
    }

    public void NonCombatSetUp(Sc_Player_Movement playerMovemenetScript)
    {
        this.playerMovemenetScript = playerMovemenetScript;
    }

    IEnumerator CanSeePlayer()
    {
        playerSeen = PlayerInVision(distPlayer, angleToPlayer, playerBehindWall);

        //stateManager.StartCoroutine(stateManager.PlayAudioOneShot(6, 8));
        //directorAI.PlayerFound(state.gameObject);
        yield return new WaitForSeconds(0.75f);
        stateManager.playerNoticed = true;
        stateManager.SwitchFLState(stateManager.alertFLState);
        stateManager.SwitchSLState(stateManager.alertedState);
        Debug.Log("Player First Seen");
        yield return null;
    }

    public bool PlayerInVision(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {
        bool playerHidden = playerMovemenetScript.ReturnIsHidden();
        if ((distPlayer <= visionRange - 15 && angleToPlayer <= visionConeAngle - 15) && !playerHidden && !playerBehindWall)
        {
            return true;
        }
        return false;
    }
}
