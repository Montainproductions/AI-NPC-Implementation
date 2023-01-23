using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sc_IdleState : Sc_AIBaseState
{
    private float idleTimer, visionRange, visionConeAngle;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        state.StartCoroutine(IdleTimed(state));
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        state.StartCoroutine(CanSeePlayer(state, distPlayer, angleToPlayer));
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }

    public void IdleStartStateInfo(float idleTime, float distRange, float visionAngleRange)
    {
        idleTimer = idleTime;
        visionRange = distRange;
        visionConeAngle = visionAngleRange;
    }

    IEnumerator IdleTimed(Sc_AIStateManager state)
    {
        yield return new WaitForSeconds(idleTimer);
        state.SwitchState(state.patrolState);
        yield return null;
    }

    IEnumerator CanSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer <= visionRange - 5 && angleToPlayer <= visionConeAngle - 5)
        {
            state.SwitchState(state.aggressionState);
            yield return null;
        }
        yield return null;
    }
}
