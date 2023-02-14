using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AttackState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;
    public string currentAction;

    private Sc_CommonMethods commonMethodsScript;

    //Navigation
    private NavMeshAgent navMeshAgent;

    private GameObject self, player, currentWeapon;
    private Sc_BaseGun gunScript;
    private Vector3 playerPos, newPosition;

    [HideInInspector]
    public bool isMoving;

    private float visionRange, visionConeAngle, attackRange, gunDistance, timeDelay, diffDistToAttack;

    private GameObject[] allGunsOnFloor;
    private GameObject pickUpWeapon;
    private Transform weaponPosition;

    //When first entering the attack state it will strat a redecide timer so that is will go back to the aggression state
    public override void EnterState(Sc_AIStateManager state, float speed, bool playerSeen) {
        //Debug.Log("Going to attack");
        isMoving = false;

        newPosition = Vector3.zero;
        state.StartCoroutine(commonMethodsScript.ReDecide());
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
        if (currentWeapon != null)
        {
            CantSeePlayer(state, distPlayer, angleToPlayer);
            //state.transform.LookAt(playerPos);

            state.StartCoroutine(PlayerDistance(state));

            //Debug.Log(diffDistToAttack);
            if (diffDistToAttack >= 0 && !isMoving)
            {
                isMoving = true;
                state.StartCoroutine(commonMethodsScript.AttackingGettingCloser(diffDistToAttack));
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
                state.StartCoroutine(commonMethodsScript.ReDecide());
            }
        }
    }

    public void AttackStartStateInfo(GameObject thisObj, GameObject playerObj, GameObject currentWeaponObj, NavMeshAgent aiNavigationAgent, float visionRange, float visionConeAngle, Transform weaponPosition, Sc_AIStateManager stateManager, Sc_CommonMethods commonMethods)
    {
        self = thisObj;
        player = playerObj;
        currentWeapon = currentWeaponObj;
        navMeshAgent = aiNavigationAgent;
        gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        attackRange = gunScript.effectiveRange;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
        this.weaponPosition = weaponPosition;
        this.stateManager = stateManager;
        commonMethodsScript = commonMethods;
    }

    public void CantSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            state.SwitchState(state.searchState);
        }
    }

    //Will shoot to the player with random time delays so that it looks like the AI is shooting at random intervals and taking its time to aim and shoot. 
    IEnumerator AttackingWithGun(Sc_AIStateManager state)
    {
        stateManager.SetCurrentAction("Shooting player");
        //Debug.Log("Shooting");
        timeDelay = Random.Range(2, 3.25f);
        yield return new WaitForSeconds(timeDelay);
        state.StartCoroutine(gunScript.ShotFired());
        //Debug.Log("Enemy ammo count: " + gunScript.currentAmmoAmount);
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

    //Will reload the current weapon if out of ammo. There is also wait timer so that it seams like the person is taking time to realize that they are out of ammo.
    IEnumerator Reloading(Sc_AIStateManager state)
    {
        stateManager.SetCurrentAction("Reloading");
        //Debug.Log("Shooting");
        yield return new WaitForSeconds(3.25f);
        state.StartCoroutine(gunScript.Reloading());
        yield return new WaitForSeconds(2);
        yield return null;
    }

    //Once the AI can pick up a weapon they will pick it up and attach it to the AI
    IEnumerator PickUpGun()
    {
        stateManager.SetCurrentAction("Picking Up Gun");
        yield return new WaitForSeconds(2.5f);
        currentWeapon = pickUpWeapon;
        currentWeapon.transform.position = weaponPosition.position;
        yield return null;
    }

    IEnumerator PlayerDistance(Sc_AIStateManager state)
    {
        float playerDist = Vector3.Distance(playerPos, self.transform.position);
        diffDistToAttack = playerDist - attackRange;
        yield return new WaitForSeconds(1f);
        state.StartCoroutine(PlayerDistance(state));
        yield return null;
    }
}
