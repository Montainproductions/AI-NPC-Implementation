using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_IdleSLState : Sc_AIBaseStateSL
{
    private Sc_AIStatesManagerHierarchical stateManager;
    private Sc_HFSMCommenMethods commonMethods;

    private float idleTimer;

    public override void EnterState()
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
        yield return new WaitForSeconds(idleTimer);
        stateManager.SwitchSLState(stateManager.patrolState);
        yield return null;
    }
}
