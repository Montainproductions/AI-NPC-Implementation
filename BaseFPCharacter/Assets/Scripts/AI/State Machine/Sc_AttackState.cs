using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AttackState : Sc_AIBaseState
{
    private NavMeshAgent navMeshAgent;

    private GameObject self, player, currentWeapon, walkingPoint;
    private Sc_BaseGun gunScript;
    private Vector3 playerPos;

    private Transform newPositionToMove;

    private float visionRange, visionConeAngle, attackRange, alertedTimer;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        Debug.Log("Going to attack");

    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        CantSeePlayer(state, distPlayer, angleToPlayer);
        playerPos = player.transform.position;
        state.transform.LookAt(playerPos);
        //Debug.Log(distFromPlayer);
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }

    public void CantSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if(distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            //state.SwitchState(state.patrolState);
        }
    }

    public void AttackStartStateInfo(GameObject thisObj, GameObject playerObj, GameObject currentWeaponObj, NavMeshAgent aiNavigationAgent)
    {
        self = thisObj;
        player = playerObj;
        currentWeapon = currentWeaponObj;
        navMeshAgent = aiNavigationAgent;
        gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        attackRange = gunScript.effectiveRange;
    }

    public Vector3 CreatePosition()
    {
        Vector3 newVectorPos = new Vector3(0,0,0);
        return newVectorPos;
    }

    IEnumerator GettingCloser(Sc_AIStateManager state)
    {
        float playerDist = Vector3.Distance(playerPos, self.transform.position);
        float diffDistToAttack = playerDist - attackRange;
        if (diffDistToAttack > 0)
        {
            float aprochDistance = Random.Range(diffDistToAttack, diffDistToAttack + 2);
            Vector3 newPosition = state.transform.position + state.transform.forward * aprochDistance;
            navMeshAgent.destination = newPosition;
        }
        yield return null;
    }

    IEnumerator AttackingWithGun()
    {
        yield return new WaitForSeconds(1.25f);
        currentWeapon.GetComponent<Sc_BaseGun>().ShotFired();
        yield return new WaitForSeconds(1);
        yield return null;
    }
}
