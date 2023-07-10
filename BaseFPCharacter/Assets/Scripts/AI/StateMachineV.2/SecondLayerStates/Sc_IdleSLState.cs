using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_IdleSLState : Sc_AIBaseStateHierarchical
{
    //State manager script
    private Sc_AIStatesManagerHierarchical stateManager;

    //Commen methods script containg various commen methods
    private Sc_HFSMCommenMethods commenMethods;

    //An idle timer used to determine how long it will be idling
    private float idleTimer;

    //First frame that the state runs
    public override void EnterState(Vector3 playerPosition)
    {
        commenMethods.StopMovement();
        stateManager.StartCoroutine(IdleTimer());
    }

    public override void UpdateState(){}

    //Sets up all important information that the state needs to operate
    public void IdleStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commenMethods, float idleTimer)
    {
        this.stateManager = stateManager;
        this.commenMethods = commenMethods;
        this.idleTimer = idleTimer;
    }
    
    //Will Idle the AI for a set amount of time.
    IEnumerator IdleTimer()
    {
        stateManager.SetIsIdling(true);

        commenMethods.LookRandomDirections(idleTimer);
        yield return new WaitForSeconds(idleTimer);

        stateManager.SwitchSLState(stateManager.patrolState);
        stateManager.SetIsIdling(false);
        yield return null;
    }
}
