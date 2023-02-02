using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sc_IdleState : Sc_AIBaseState
{
    private float idleTimer, visionRange, visionConeAngle;

    Vector3 randomLookDirection;

    public override void EnterState(Sc_AIStateManager state, float speed, bool playerSeen) {
        state.StartCoroutine(IdleTimed(state));
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        state.StartCoroutine(CanSeePlayer(state, distPlayer, angleToPlayer));
    }

    public void IdleStartStateInfo(float idleTime, float distRange, float visionAngleRange)
    {
        idleTimer = idleTime;
        visionRange = distRange;
        visionConeAngle = visionAngleRange;
    }

    IEnumerator IdleTimed(Sc_AIStateManager state)
    {
        yield return new WaitForSeconds(idleTimer/3);
        randomLookDirection.x = Random.Range(0, 360);
        randomLookDirection.z = Random.Range(0, 360);
        state.transform.LookAt(randomLookDirection);
        yield return new WaitForSeconds(idleTimer / 3);
        randomLookDirection.x = Random.Range(0, 360);
        randomLookDirection.z = Random.Range(0, 360);
        state.transform.LookAt(randomLookDirection);
        yield return new WaitForSeconds(idleTimer / 3);
        state.SwitchState(state.patrolState);
        yield return null;
    }

    IEnumerator CanSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5)
        {
            state.SwitchState(state.aggressionDesicionState);
            yield return null;
        }
        yield return null;
    }
}
