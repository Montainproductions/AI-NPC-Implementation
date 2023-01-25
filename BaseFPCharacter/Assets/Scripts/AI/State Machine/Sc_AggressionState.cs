using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AggressionState : Sc_AIBaseState
{
    private GameObject player, self;
    private GameObject[] coverPositions;

    private NavMeshAgent navMeshAgent;

    private Sc_BaseGun baseGunScript;
    private Sc_AIDirector directorAI;
    private Sc_AIStateManager manager;

    private Vector3 playerPos;

    private float attackRange, coverDistance;

    private int decisionVal;

    public override void EnterState(Sc_AIStateManager state, float speed)
    {
        state.StartCoroutine(StoppingAI());
        //Debug.Log(self.name + " Player detected");

        decisionVal = 0;
        attackRange = baseGunScript.effectiveRange;
        WhenToAttack(state);
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        playerPos = player.transform.position;
        state.transform.LookAt(playerPos);
    }

    public override void OnCollisionEnter(Sc_AIStateManager state)
    {

    }

    public void AggressionStartStateInfo(GameObject currentEnemy, GameObject playerObj, GameObject currentWeapon, GameObject[] coverPos, float coverDist, Sc_AIDirector directorAI, Sc_AIStateManager aiManager, NavMeshAgent navMesh)
    {
        self = currentEnemy;
        player = playerObj;
        baseGunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        coverPositions = coverPos;
        coverDistance = coverDist;
        this.directorAI = directorAI;
        manager = aiManager;
        navMeshAgent = navMesh;

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
        //Debug.Log("Obj: " + self.name + " Value: " + decisionVal);

        manager.SetDecisionValue(decisionVal);
        state.StartCoroutine(directorAI.AIAttackAddList(self));
    }

    IEnumerator StoppingAI()
    {
        yield return new WaitForSeconds(0.45f);
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(self.transform.position);
        //Debug.Log(navMeshAgent.destination);
        yield return null;
    }
}
