using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class Sc_CoverState : Sc_AIBaseState
{
    private GameObject self, player, closestCover, currentWeapon;
    private Vector3 playerPos, coverPosition;

    private GameObject[] allCover;

    private float closestDist, coverTimer, visionRange, visionConeAngle;

    private NavMeshAgent navMeshAgent;

    public override void EnterState(Sc_AIStateManager state, float speed)
    {
        closestDist = Mathf.Infinity;
        Debug.Log("Going to cover");
        state.StartCoroutine(ChoosingCover());
        state.StartCoroutine(NewDecisionToTake(state));
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        playerPos = player.transform.position;
        state.transform.LookAt(playerPos);

        if (Vector3.Distance(self.transform.position, closestCover.transform.position) > 0.4f)
        {
            navMeshAgent.destination = closestCover.transform.position;
        }
        else
        {
            //self.transform.localScale = new Vector3(1,0.75f,1);
            state.StartCoroutine(AtCover(state));
        }
    }

    public override void OnCollisionEnter(Sc_AIStateManager state)
    {

    }

    public void CoverStartStateInfo(GameObject selfObj, GameObject playerObj, GameObject currentWeaponObj, GameObject[] allCoverObjs, NavMeshAgent navMeshAgent, float visionRange, float visionConeAngle)
    {
        self = selfObj;
        player = playerObj;
        currentWeapon = currentWeaponObj;
        allCover = allCoverObjs;
        this.navMeshAgent = navMeshAgent;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
    }

    public void CantSeePlayer(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
        if (distPlayer >= visionRange && angleToPlayer >= visionConeAngle)
        {
            state.SwitchState(state.searchState);
        }
    }

    IEnumerator ChoosingCover() {
        for (int i = 0; i >= allCover.Length; i++)
        {
            if (Vector3.Distance(allCover[i].transform.position, self.transform.position) <= closestDist)
            {
                closestDist = Vector3.Distance(allCover[i].transform.position, self.transform.position);
                closestCover = allCover[i];
            }
        }
        int allCoverPos = closestCover.transform.childCount;

        for (int i = 1; i >= allCoverPos; i++)
        {
            //GameObject coverPoint = closestCover.transform.GetChild(i);
            Sc_CoverPoints coverScript = closestCover.transform.GetChild(i).GetComponent<Sc_CoverPoints>();
            if (coverScript.behindCover && !coverScript.beingUsed)
            {
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
            self.transform.localScale = new Vector3(1, 0.75f, 1);
        }
        else
        {
            self.transform.localScale = new Vector3(1, 1, 1);
            state.StartCoroutine(AttackingWithGun(state));
            self.transform.localScale = new Vector3(1, 0.75f, 1);
        }
        yield return new WaitForSeconds(coverTimer);
        yield return null;
    }

    IEnumerator AttackingWithGun(Sc_AIStateManager state)
    {
        Debug.Log("Shooting");
        //yield return new WaitForSeconds(0.30f);
        Sc_BaseGun gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        state.StartCoroutine(gunScript.ShotFired());
        yield return new WaitForSeconds(1.0f);
        yield return null;
    }

    IEnumerator NewDecisionToTake(Sc_AIStateManager state)
    {
        yield return new WaitForSeconds(45);
        state.SwitchState(state.aggressionState);
        yield return null;
    }
}
