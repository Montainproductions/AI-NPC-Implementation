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

    private float visionRange, visionConeAngle, attackRange, decisionTimer, gunDistance;

    private GameObject[] allGunsOnFloor;
    private GameObject pickUpWeapon;
    private Transform weaponPosition;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        //Debug.Log("Going to attack");
        isDeciding = false;

        newPosition = Vector3.zero;

        if (currentWeapon == null)
        {
            state.StartCoroutine(LookingForGun(state));
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
        else if(currentWeapon == null && newPosition != Vector3.zero)
        {
            navMeshAgent.destination = newPosition;
            state.transform.LookAt(newPosition);
            float distToWeapon = Vector3.Distance(state.transform.position, newPosition);
            if(distToWeapon <= 0.5f)
            {
                state.StartCoroutine(PickUpGun());
                state.StartCoroutine(ReDecide(state));
            }
        }
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }

    public void AttackStartStateInfo(GameObject thisObj, GameObject playerObj, GameObject currentWeaponObj, NavMeshAgent aiNavigationAgent, float visionRange, float visionConeAngle, float decisionTimer, Transform weaponPosition)
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
        this.weaponPosition = weaponPosition;
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

    IEnumerator LookingForGun(Sc_AIStateManager state)
    {
        gunDistance = 0;
        for (int i = 0; i < allGunsOnFloor.Length; i++)
        {
            float tempDist = Vector3.Distance(allGunsOnFloor[i].transform.position, state.transform.position);
            if (gunDistance > tempDist)
            {
                gunDistance = tempDist;
                pickUpWeapon = allGunsOnFloor[i];
            }
        }
        newPosition = pickUpWeapon.transform.position;
        yield return null;
    }

    IEnumerator PickUpGun()
    {
        yield return new WaitForSeconds(2.5f);
        currentWeapon = pickUpWeapon;
        currentWeapon.transform.position = weaponPosition.position;
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
