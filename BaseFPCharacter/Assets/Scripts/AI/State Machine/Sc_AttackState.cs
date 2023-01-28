using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AttackState : Sc_AIBaseState
{
    private NavMeshAgent navMeshAgent;

    private GameObject self, player, currentWeapon;
    private Sc_BaseGun gunScript;
    private Vector3 playerPos, newPosition;

    private bool isDeciding;

    private float visionRange, visionConeAngle, attackRange, decisionTimer;

    private GameObject[] allGunsOnFloor;
    private GameObject weaponPickUp;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        //Debug.Log("Going to attack");
        isDeciding = false;

        if(currentWeapon == null)
        {
            state.StartCoroutine(LookingForGun());
        }
        else
        {
            state.StartCoroutine(ReDecide(state));
        }
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        if (currentWeapon != null)
        {
            CantSeePlayer(state, distPlayer, angleToPlayer);
            playerPos = player.transform.position;
            state.transform.LookAt(playerPos);

            float playerDist = Vector3.Distance(playerPos, self.transform.position);
            float diffDistToAttack = playerDist - attackRange;
            //Debug.Log(diffDistToAttack);
            if (diffDistToAttack > 0 && !isDeciding)
            {
                isDeciding = true;
                state.StartCoroutine(GettingCloser(state, diffDistToAttack));
            }
            else
            {
                state.StartCoroutine(AttackingWithGun(state));
            }

            navMeshAgent.destination = newPosition;
            //Debug.Log(distFromPlayer);
        }
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }

    public void AttackStartStateInfo(GameObject thisObj, GameObject playerObj, GameObject currentWeaponObj, NavMeshAgent aiNavigationAgent, float visionRange, float visionConeAngle, float decisionTimer)
    {
        self = thisObj;
        player = playerObj;
        currentWeapon = currentWeaponObj;
        navMeshAgent = aiNavigationAgent;
        gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        attackRange = gunScript.effectiveRange;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
        this.decisionTimer = decisionTimer;
    }

    public void CantSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            state.SwitchState(state.searchState);
        }
    }

    IEnumerator GettingCloser(Sc_AIStateManager state, float diffDistToAttack)
    {
        float zDistance = Random.Range(diffDistToAttack + 1, diffDistToAttack + 6);
        //Debug.Log(zDistance);
        //float yDistance = Random.Range(-diffDistToAttack, diffDistToAttack);
        newPosition = state.transform.position + state.transform.forward * zDistance;
        //Debug.Log(newPosition);
        isDeciding = false;
        yield return null;
    }

    IEnumerator AttackingWithGun(Sc_AIStateManager state)
    {
        //Debug.Log("Shooting");
        //yield return new WaitForSeconds(0.30f);
        Sc_BaseGun gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        state.StartCoroutine(gunScript.ShotFired());
        yield return new WaitForSeconds(1.75f);
        yield return null;
    }

    IEnumerator LookingForGun()
    {
        for(int i = 0; i < allGunsOnFloor.Length; i++)
        {

        }
        yield return null;
    }

    IEnumerator ReDecide(Sc_AIStateManager state)
    {
        float newDecisionTimer = Random.Range(decisionTimer - 5, decisionTimer + 5);
        yield return new WaitForSeconds(newDecisionTimer);
        state.SwitchState(state.aggressionState);
        yield return null;
    }
}
