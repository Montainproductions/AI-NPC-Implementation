using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_IdleSLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStatesManagerHierarchical stateManager;
    private Sc_HFSMCommenMethods commonMethods;

    private float idleTimer;

    public override void EnterState(Vector3 playerPosition)
    {
        commonMethods.StopMovement();
        stateManager.StartCoroutine(IdleTimer());
    }

    public override void UpdateState(){}

    public void IdleStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commonMethods, float idleTimer)
    {
        this.stateManager = stateManager;
        this.commonMethods = commonMethods;
        this.idleTimer = idleTimer;
    }
        
    IEnumerator IdleTimer()
    {
        stateManager.SetIsIdling(true);
        
        commonMethods.LookRandomDirections(idleTimer);

        stateManager.SwitchSLState(stateManager.patrolState);
        stateManager.SetIsIdling(false);
        yield return null;
    }
}
