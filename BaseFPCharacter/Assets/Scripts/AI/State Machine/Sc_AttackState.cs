using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AttackState : Sc_AIBaseState
{
    private NavMeshAgent navMeshAgent;

    private GameObject player, currentWeapon;
    private Vector3 playerPos;

    private float visionRange, visionConeAngle, attackRangeMin, attackRangeMax, attackRange, alertedTimer;

    public override void EnterState(Sc_AIStateManager state, float speed) {
        AttackDistance();
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer) {
        CanSeePlayer(state, distPlayer, angleToPlayer);
        playerPos = player.transform.position;
        state.transform.LookAt(playerPos);
        if(Vector3.Distance(player.transform.position, state.transform.position) <= attackRange)
        {
            state.StartCoroutine(AttackingWithGun());
        }
        else
        {
            navMeshAgent.destination = player.transform.position;
        }
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }

    public void CanSeePlayer(Sc_AIStateManager state,float distPlayer, float angleToPlayer)
    {
        if(distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            //state.SwitchState(state.patrolState);
        }
    }

    public void Shooting()
    {

    }

    public void AttackDistance()
    {
        attackRange = Random.Range(attackRangeMin, attackRangeMax);
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
        AttackDistance();
        yield return null;
    }
}
