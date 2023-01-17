using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AttackState : Sc_AIBaseState
{
    private NavMeshAgent navMeshAgent;

    private GameObject player, currentWeapon;
    private Vector3 playerPos;

    private float visionRange, visionConeAngle, attackRange, alertedTimer;

    private int desicionVal;

    public override void EnterState(Sc_AIStateManager state, float speed) {
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        CantSeePlayer(state, distPlayer, angleToPlayer);
        playerPos = player.transform.position;
        state.transform.LookAt(playerPos);
        float distFromPlayer = Vector3.Distance(player.transform.position, state.transform.position);
        //Debug.Log(distFromPlayer);
        WhenToAttack(state, distFromPlayer);
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }

    public void CantSeePlayer(Sc_AIStateManager state,float distPlayer, float angleToPlayer)
    {
        if(distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            //state.SwitchState(state.patrolState);
        }
    }

    public void WhenToAttack(Sc_AIStateManager state, float distFromPlayer)
    {
        float currentAttackRange = Random.Range(attackRange + 2, attackRange - 2);
        if (currentAttackRange > distFromPlayer)
        {
            desicionVal++;
            
        }


    }

    public void AttackStateInfo(GameObject playerObj, GameObject currentWeaponObj, NavMeshAgent aiNavigationAgent)
    {
        player = playerObj;
        currentWeapon = currentWeaponObj;
        navMeshAgent = aiNavigationAgent;
    }

    IEnumerator AttackingWithGun()
    {
        yield return new WaitForSeconds(1.25f);
        currentWeapon.GetComponent<Sc_BaseGun>().ShotFired();
        yield return new WaitForSeconds(1);
        yield return null;
    }
}
