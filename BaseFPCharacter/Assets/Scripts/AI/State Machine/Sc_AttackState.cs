using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AttackState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;
    private Sc_CommonMethods commonMethodsScript;
    private Sc_Player_Movement playerMovementScript;

    public string currentAction;

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
    public override void EnterState(bool playerSeen) {
        //Debug.Log("Going to attack");
        isMoving = false;

        newPosition = Vector3.zero;
        stateManager.StartCoroutine(commonMethodsScript.ReDecide());
    }

    public override void UpdateState(float distPlayer, float angleToPlayer, bool playerBehindWall) {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
        if (currentWeapon != null)
        {
            CantSeePlayer(distPlayer, angleToPlayer);
            //state.transform.LookAt(playerPos);

            stateManager.StartCoroutine(PlayerDistance());

            //Debug.Log(diffDistToAttack);
            if (diffDistToAttack >= 0 && !isMoving)
            {
                isMoving = true;
                stateManager.StartCoroutine(commonMethodsScript.AttackingGettingCloser(diffDistToAttack));
            }
            else if(gunScript.ReturnCurrentAmmo() > 0)
            {
                stateManager.StartCoroutine(AttackingWithGun());
            }
            else if (gunScript.ReturnCurrentAmmo() <= 0)
            {
                stateManager.StartCoroutine(Reloading());
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
            stateManager.transform.LookAt(newPosition);
            float distToWeapon = Vector3.Distance(stateManager.transform.position, newPosition);
            if(distToWeapon <= 0.5f)
            {
                stateManager.StartCoroutine(PickUpGun());
                stateManager.StartCoroutine(commonMethodsScript.ReDecide());
            }
        }
    }

    public void AttackStartStateInfo(Sc_AIStateManager stateManager, Sc_CommonMethods commonMethodsScript, Sc_Player_Movement playerMovementScript, GameObject self, GameObject player, GameObject currentWeapon, NavMeshAgent navMeshAgent, float visionRange, float visionConeAngle)
    {
        this.stateManager = stateManager;
        this.commonMethodsScript = commonMethodsScript;
        this.playerMovementScript = playerMovementScript;
        this.self = self;
        this.player = player;
        this.currentWeapon = currentWeapon;
        gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        attackRange = gunScript.ReturnEffectiveRange();
        this.navMeshAgent = navMeshAgent;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
    }

    public void CantSeePlayer(float distPlayer, float angleToPlayer)
    {
        bool playerHidden = playerMovementScript.ReturnIsHidden();
        if (distPlayer > visionRange || angleToPlayer > visionConeAngle || playerHidden)
        {
            stateManager.SwitchState(stateManager.searchState);
        }
    }

    //Will shoot to the player with random time delays so that it looks like the AI is shooting at random intervals and taking its time to aim and shoot. 
    IEnumerator AttackingWithGun()
    {
        stateManager.SetCurrentAction("Shooting player");
        stateManager.SetIsAttacking(true);
        stateManager.SetIsWalking(false);
        //Debug.Log("Shooting");
        timeDelay = Random.Range(5, 8.25f);
        yield return new WaitForSeconds(timeDelay);
        stateManager.StartCoroutine(gunScript.ShotFired());
        //Debug.Log("Enemy ammo count: " + gunScript.currentAmmoAmount);
        timeDelay = Random.Range(3.5f, 5.75f);
        yield return new WaitForSeconds(timeDelay);
        stateManager.SetIsAttacking(false);
        yield return null;
    }

    //If the AI currently dosent have a weapon then the AI will look and grab the closest weapon to them.
    IEnumerator LookingForGun()
    {
        stateManager.SetCurrentAction("Looking for near by gun");
        gunDistance = 0;
        for (int i = 0; i < allGunsOnFloor.Length; i++)
        {
            float tempDist = Vector3.Distance(allGunsOnFloor[i].transform.position, stateManager.transform.position);
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
    IEnumerator Reloading()
    {
        stateManager.SetCurrentAction("Reloading");
        //Debug.Log("Shooting");
        yield return new WaitForSeconds(3.25f);
        stateManager.StartCoroutine(gunScript.Reloading());
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

    IEnumerator PlayerDistance()
    {
        float playerDist = Vector3.Distance(playerPos, self.transform.position);
        diffDistToAttack = playerDist - attackRange;
        yield return new WaitForSeconds(1f);
        stateManager.StartCoroutine(PlayerDistance());
        yield return null;
    }
}
