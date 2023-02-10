using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

/// <summary>
/// The cover state script allows each enemy to decide what is the closest unocupide cover point and then go to there and go to the cover point. 
/// This works by going through all the cover objects (Main parent object which contains the rectangle main cover and all 4 cover points) to find the closest cover object. 
/// After finding the closest cover object the AI will check all the 4 cover points that are part of the cover object and run a method that will run a raycast from the cover point to the player and if it hits a cover position then it is considered behind cover and will consider using the cover. 
/// If at this point there isnt another AI enemy using the cover point then the AI will start going to the cover point.
/// </summary>
public class Sc_CoverState : Sc_AIBaseState
{
    private Sc_AIStateManager stateManager;

    private GameObject self, player, closestCover, currentWeapon;
    private Vector3 playerPos, coverPosition;

    private GameObject[] allCover;

    private float closestDist, visionRange, visionConeAngle, decisionTimer;

    private NavMeshAgent navMeshAgent;

    //When first entering the state the choosing cover IEnumerator and the redeciding timer.
    public override void EnterState(Sc_AIStateManager state, float speed, bool playerSeen)
    {
        closestDist = Mathf.Infinity;
        //Debug.Log("Going to cover Start");
        coverPosition = Vector3.zero;
        state.StartCoroutine(ChoosingCover());
        state.StartCoroutine(ReDecide(state));
    }

    //
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

    //Recives important variables that are needed for the entire state to work properly.
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

    //If the player leaves the AIs line of site then it will stop trying to go to cover and start to search for the player.
    public void CantSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            state.SwitchState(state.searchState);
        }
    }

    //Fine closest cover object and then determines which of the cover points children is behind the cover.
    IEnumerator ChoosingCover() {
        stateManager.SetCurrentAction("Choosing closest cover point");
        Vector3 selfPos = self.transform.position;
        for (int i = 0; i < allCover.Length; i++)
        {
            float dist = Vector3.Distance(allCover[i].transform.position, selfPos);
            if (dist <= closestDist)
            {
                closestDist = dist;
                closestCover = allCover[i];
            }
        }
        //Debug.Log(closestCover);
        //int allCoverPos = closestCover.transform.childCount;

        for (int i = 1; i <= 4; i++)
        {
            Sc_CoverPoints coverScript = closestCover.transform.GetChild(i).GetComponent<Sc_CoverPoints>();
            bool behindCover = coverScript.IsBehindCover();
            if (behindCover && !coverScript.beingUsed)
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
