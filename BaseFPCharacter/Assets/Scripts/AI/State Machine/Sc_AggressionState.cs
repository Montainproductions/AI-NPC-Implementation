using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AggressionState : Sc_AIBaseState
{
    private GameObject player, currentWeapon, self;
    private GameObject[] coverPositions;

    private NavMeshAgent navMeshAgent;

    private Sc_BaseGun baseGunScript;
    private Sc_AIDirector directorAI;
    private Sc_AIStateManager manager;

    private float visionRange, visionConeAngle, attackRange, alertedTimer, coverDistance;

    private int decisionVal;

    public override void EnterState(Sc_AIStateManager state, float speed)
    {
        Debug.Log("Detected");
        decisionVal = 0;
        attackRange = baseGunScript.effectiveRange;
        WhenToAttack(state);
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {

    }

    public override void OnCollisionEnter(Sc_AIStateManager state)
    {

    }

    public void AggressionStartStateInfo(GameObject playerObj, GameObject currentWeapon, GameObject[] coverPos, GameObject currentEnemy, float coverDist, Sc_AIDirector directorAI, Sc_AIStateManager aiManager)
    {
        player = playerObj;
        baseGunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        coverPositions = coverPos;
        self = currentEnemy;
        coverDistance = coverDist;
        this.directorAI = directorAI;
        manager = aiManager;
    }

    public void WhenToAttack(Sc_AIStateManager state)
    {
        float distFromPlayer = Vector3.Distance(player.transform.position, state.transform.position);
        float currentAttackRange = Random.Range(attackRange + 2, attackRange - 2);
        if (currentAttackRange > distFromPlayer)
        {
            decisionVal += 2;
        }
        foreach(GameObject i in coverPositions)
        {
            float distFromCover = Vector3.Distance(self.transform.position, i.transform.position);
            if(distFromCover < coverDistance)
            {
                decisionVal--;
            }
        }
        Debug.Log(decisionVal);

        manager.SetDecisionValue(decisionVal);
        directorAI.AIAttackAddList(self);
    }
}
