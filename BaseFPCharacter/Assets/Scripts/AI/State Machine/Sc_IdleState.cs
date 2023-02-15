using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sc_IdleState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;
    private Sc_Player_Movement playerMovementScript;

    private float idleTimer, visionRange, visionConeAngle;

    Vector3 randomLookDirection;

    public override void EnterState(float speed, bool playerSeen) {
        stateManager.StartCoroutine(IdleTimed());
    }

    public override void UpdateState(float distPlayer, float angleToPlayer) {
        stateManager.StartCoroutine(CanSeePlayer(distPlayer, angleToPlayer));
    }

    public void IdleStartStateInfo(Sc_AIStateManager stateManager, Sc_Player_Movement playerMovementScript, float idleTime, float distRange, float visionAngleRange)
    {
        this.stateManager = stateManager;
        this.playerMovementScript = playerMovementScript;
        idleTimer = idleTime;
        visionRange = distRange;
        visionConeAngle = visionAngleRange;
    }

    IEnumerator IdleTimed()
    {
        yield return new WaitForSeconds(idleTimer / 3);
        randomLookDirection.x = Random.Range(0, 360);
        randomLookDirection.z = Random.Range(0, 360);
        stateManager.transform.LookAt(randomLookDirection);
        yield return new WaitForSeconds(idleTimer / 3);
        randomLookDirection.x = Random.Range(0, 360);
        randomLookDirection.z = Random.Range(0, 360);
        stateManager.transform.LookAt(randomLookDirection);
        yield return new WaitForSeconds(idleTimer / 3);
        stateManager.SwitchState(stateManager.patrolState);
        yield return null;
    }

    IEnumerator CanSeePlayer(float distPlayer, float angleToPlayer)
    {
        bool playerHidden = playerMovementScript.IsHiddenReturn();
        if ((distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5) || !playerHidden)
        {
            stateManager.SwitchState(stateManager.aggressionDesicionState);
            yield return null;
        }
        yield return null;
    }
}
