using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AttackState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;
    public string currentAction;

    private NavMeshAgent navMeshAgent;

    private GameObject self, player, currentWeapon;
    private Sc_BaseGun gunScript;
    private Vector3 playerPos, newPosition;

    private bool isMoving;

    private float visionRange, visionConeAngle, attackRange, decisionTimer, gunDistance, timeDelay;

    private GameObject[] allGunsOnFloor;
    private GameObject pickUpWeapon;
    private Transform weaponPosition;

    public override void EnterState(Sc_AIStateManager state, float speed, bool playerSeen) {
        //Debug.Log("Going to attack");
        isMoving = false;

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
        playerPos = player.transform.position;
        if (currentWeapon != null)
        {
            CantSeePlayer(state, distPlayer, angleToPlayer);
            state.transform.LookAt(playerPos);

            float playerDist = Vector3.Distance(playerPos, self.transform.position);
            float diffDistToAttack = playerDist - attackRange;
            //Debug.Log(diffDistToAttack);
            if (diffDistToAttack >= 0 && !isMoving)
            {
                isMoving = true;
                state.StartCoroutine(GettingCloser(state, diffDistToAttack));
            }
            else if(gunScript.currentAmmoAmount > 0)
            {
                state.StartCoroutine(AttackingWithGun(state));
            }
            else if (gunScript.currentAmmoAmount <= 0)
            {
                state.StartCoroutine(Reloading(state));
            }

            if (newPosition != Vector3.zero)
            {
                navMeshAgent.destination = newPosition;
            }
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

    public void AttackStartStateInfo(GameObject thisObj, GameObject playerObj, GameObject currentWeaponObj, NavMeshAgent aiNavigationAgent, float visionRange, float visionConeAngle, float decisionTimer, Transform weaponPosition, Sc_AIStateManager stateManager)
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
        this.stateManager = stateManager;
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
        stateManager.SetCurrentAction("Choosing closer position to player");
        float zDistance = Random.Range(diffDistToAttack + 1, diffDistToAttack + 6);
        //Debug.Log(zDistance);
        //float yDistance = Random.Range(-diffDistToAttack, diffDistToAttack);
        newPosition = state.transform.position + state.transform.forward * zDistance;
        //Debug.Log(newPosition);
        isMoving = false;
        yield return null;
    }

    //Will shoot to the player with random time delays so that it looks like the AI is shooting at random intervals and taking its time to aim and shoot. 
    IEnumerator AttackingWithGun(Sc_AIStateManager state)
    {
        stateManager.SetCurrentAction("Shooting player");
        //Debug.Log("Shooting");
        timeDelay = Random.Range(2, 3.25f);
        yield return new WaitForSeconds(timeDelay);
        state.StartCoroutine(gunScript.ShotFired());
        timeDelay = Random.Range(1.5f, 2.75f);
        yield return new WaitForSeconds(timeDelay);
        yield return null;
    }

    //If the AI currently dosent have a weapon then the AI will look and grab the closest weapon to them.
    IEnumerator LookingForGun(Sc_AIStateManager state)
    {
        stateManager.SetCurrentAction("Looking for near by gun");
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
        yield return new WaitForSeconds(1.75f);
    }

    IEnumerator Reloading(Sc_AIStateManager state)
    {
        stateManager.SetCurrentAction("Reloading");
        //Debug.Log("Shooting");
        yield return new WaitForSeconds(2.25f);
        state.StartCoroutine(gunScript.Reloading());
        yield return new WaitForSeconds(2.75f);
        yield return null;
    }

    IEnumerator PickUpGun()
    {
        stateManager.SetCurrentAction("Picking Up Gun");
        yield return new WaitForSeconds(2.5f);
        currentWeapon = pickUpWeapon;
        currentWeapon.transform.position = weaponPosition.position;
        yield return null;
    }

    IEnumerator ReDecide(Sc_AIStateManager state)
    {
        float newDecisionTimer = Random.Range(decisionTimer - 5, decisionTimer + 5);
        yield return new WaitForSeconds(newDecisionTimer);
        state.SwitchState(state.aggressionDesicionState);
        yield return null;
    }
}
