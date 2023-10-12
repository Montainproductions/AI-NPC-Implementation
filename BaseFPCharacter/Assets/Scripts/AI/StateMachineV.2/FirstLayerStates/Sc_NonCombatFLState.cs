using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class Sc_NonCombatFLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStatesManagerHierarchical stateManager;

    private Sc_AIDirector directorAI;

    private Sc_HFSMCommenMethods commenMethods;

    private GameObject player;
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
        //Debug.Log("Noncombat FL");
        stateManager.StartCoroutine(CanSeePlayer());
    }

    public override void UpdateState()
    {
        distPlayer = Vector3.Distance(player.transform.position, aitransform.position);
        angleToPlayer = Vector3.Angle(aitransform.forward, player.transform.position - aitransform.position);

        direction = player.transform.position - aitransform.position;
        playerBehindWall = Physics.Raycast(aitransform.position, direction, out hit, visionRange - 5, layerMask);
    }

    public void NonCombatSetUp(Sc_AIStatesManagerHierarchical stateManager, Sc_AIDirector directorAI, Sc_HFSMCommenMethods commenMethods, GameObject player, Transform aitransform, float visionRange, float visionConeAngle)
    {
        this.stateManager = stateManager;
        this.directorAI = directorAI;
        this.commenMethods = commenMethods;
        this.player = player;
        playerMovemenetScript = player.GetComponent<Sc_Player_Movement>();
        this.aitransform = aitransform;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
    }

    IEnumerator CanSeePlayer()
    {
        //Debug.Log("Check player");
        playerSeen = PlayerInVision(distPlayer, angleToPlayer, playerBehindWall);
        if (playerSeen)
        {
            stateManager.StartCoroutine(commenMethods.StopMovement(player.transform));
            yield return new WaitForSeconds(0.75f);
            stateManager.PlayRandomAudioOneShot(6, 8);
            directorAI.PlayerFound(stateManager.gameObject);
            stateManager.playerNoticed = true;
            
            stateManager.SwitchFLState(stateManager.alertFLState);
            stateManager.SwitchSLState(stateManager.alertedState);
            //Debug.Log("Player First Seen");
        }
        yield return new WaitForSeconds(0.25f);
        stateManager.StartCoroutine(CanSeePlayer());
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
