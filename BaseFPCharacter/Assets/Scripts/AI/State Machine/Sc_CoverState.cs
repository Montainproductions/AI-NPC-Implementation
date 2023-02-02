using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class Sc_CoverState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;

    private GameObject self, player, closestCover, currentWeapon;
    private Vector3 playerPos, coverPosition;

    private GameObject[] allCover;

    private float closestDist, visionRange, visionConeAngle, decisionTimer;

    private NavMeshAgent navMeshAgent;

    public override void EnterState(Sc_AIStateManager state, float speed, bool playerSeen)
    {
        closestDist = Mathf.Infinity;
        //Debug.Log("Going to cover Start");
        coverPosition = Vector3.zero;
        state.StartCoroutine(ChoosingCover());
        state.StartCoroutine(ReDecide(state));
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        playerPos = player.transform.position;
        state.transform.LookAt(playerPos);
        if (coverPosition != Vector3.zero)
        {
            if (Vector3.Distance(self.transform.position, coverPosition) > 0.6f)
            {
                //Debug.Log("Going to Cover");
                stateManager.SetCurrentAction("Going to cover point");
                navMeshAgent.destination = coverPosition;
            }
            else
            {
                //Debug.Log("At Cover");
                //self.transform.localScale = new Vector3(1,0.75f,1);
                coverPosition = Vector3.zero;
                state.StartCoroutine(AtCover(state));
            }
        }
    }

    public void CoverStartStateInfo(GameObject selfObj, GameObject playerObj, GameObject currentWeaponObj, GameObject[] allCoverObjs, NavMeshAgent navMeshAgent, float visionRange, float visionConeAngle, float decisionTimer, Sc_AIStateManager stateManager)
    {
        self = selfObj;
        player = playerObj;
        currentWeapon = currentWeaponObj;
        allCover = allCoverObjs;
        this.navMeshAgent = navMeshAgent;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
        this.decisionTimer = decisionTimer;
        this.stateManager = stateManager;
    }

    public void CantSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            state.SwitchState(state.searchState);
        }
    }

    IEnumerator ChoosingCover() {
        stateManager.SetCurrentAction("Choosing closest cover point");
        for (int i = 0; i < allCover.Length; i++)
        {
            if (Vector3.Distance(allCover[i].transform.position, self.transform.position) <= closestDist)
            {
                closestDist = Vector3.Distance(allCover[i].transform.position, self.transform.position);
                closestCover = allCover[i];
            }
        }
        //Debug.Log(closestCover);
        //int allCoverPos = closestCover.transform.childCount;

        for (int i = 1; i <= 4; i++)
        {
            Sc_CoverPoints coverScript = closestCover.transform.GetChild(i).GetComponent<Sc_CoverPoints>();
            if (coverScript.behindCover && !coverScript.beingUsed)
            {
                //Debug.Log(closestCover.transform.GetChild(i));
                coverPosition = closestCover.transform.GetChild(i).transform.position;
                coverScript.beingUsed = true;
            }
        }

        //Debug.Log(closeCover.Length);
        yield return null;
    }

    IEnumerator AtCover(Sc_AIStateManager state)
    {
        //self.transform.localScale = new Vector3(1, 0.75f, 1);
        float attackOrCover = Random.Range(1.0f, 10.0f);
        if (attackOrCover >= 5.0f)
        {
            stateManager.SetCurrentAction("Taking cover");
            self.transform.localScale = new Vector3(1, 0.75f, 1);
        }
        else
        {
            stateManager.SetCurrentAction("Shooting from cover");
            self.transform.localScale = new Vector3(1, 1, 1);
            state.StartCoroutine(AttackingWithGun(state));
            self.transform.localScale = new Vector3(1, 0.75f, 1);
        }
        yield return new WaitForSeconds(2.5f);
        state.StartCoroutine(AtCover(state));
        yield return null;
    }

    IEnumerator AttackingWithGun(Sc_AIStateManager state)
    {
        //Debug.Log("Shooting");
        //yield return new WaitForSeconds(0.30f);
        Sc_BaseGun gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        state.StartCoroutine(gunScript.ShotFired());
        yield return new WaitForSeconds(1.0f);
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
