using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CoverSLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStateManager stateManager;
    private Sc_CommonMethods commonMethodsScript;

    private GameObject self, player, closestCover, currentWeapon;
    private Vector3 playerPos, coverPosition;

    private GameObject[] allCover;

    private float closestDist;

    public override void EnterState()
    {
        closestDist = Mathf.Infinity;
        //Debug.Log("Going to cover Start");
        coverPosition = Vector3.zero;
        stateManager.StartCoroutine(ChoosingCover());
        stateManager.StartCoroutine(commonMethodsScript.ReDecide());
    }

    public override void UpdateState()
    {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
    }

    //Recives important variables that are needed for the entire state to work properly.
    public void CoverStartStateInfo(Sc_AIStateManager stateManager, Sc_CommonMethods commonMethodsScript, GameObject self, GameObject player, GameObject currentWeapon, GameObject[] allCover, float visionRange, float visionConeAngle)
    {
        this.stateManager = stateManager;
        this.commonMethodsScript = commonMethodsScript;
        this.self = self;
        this.player = player;
        this.currentWeapon = currentWeapon;
        this.allCover = allCover;
    }

    //Find the closest cover object and then determines which of the cover points children is behind the cover.
    IEnumerator ChoosingCover()
    {
        stateManager.SetCurrentAction("Choosing closest cover point");
        Vector3 selfPos = self.transform.position;
        //Find closest cover object
        for (int i = 0; i < allCover.Length; i++)
        {
            float dist = Vector3.Distance(allCover[i].transform.position, selfPos);
            if (dist <= closestDist && dist > 0.5f)
            {
                closestDist = dist;
                closestCover = allCover[i];
            }
        }
        //Debug.Log(closestCover.transform.position);
        //int allCoverPos = closestCover.transform.childCount;

        //Choosing a cover point that is behind the cover when comparing to the player
        for (int i = 0; i < 4; i++)
        {
            //Debug.Log("ChoosingCover");
            Sc_CoverPoints coverScript = closestCover.transform.GetChild(i).GetComponent<Sc_CoverPoints>();
            bool behindCover = coverScript.IsBehindCover();
            if (behindCover && !coverScript.beingUsed)
            {
                //Debug.Log(closestCover.transform.GetChild(i));
                coverPosition = closestCover.transform.GetChild(i).transform.position;
                coverScript.beingUsed = true;
                //Debug.Log("Cover position: " + coverPosition);
                yield return new WaitForSeconds(1.0f);
                stateManager.StartCoroutine(stateManager.PlayAudioOneShot(24, 26));
                commonMethodsScript.StartMovement(coverPosition, "Cover", true);
                break;
            }
        }
        //Debug.Log(closeCover.Length);
        yield return null;
    }

    //Once the AI is behind cover
    public IEnumerator AtCover()
    {
        //self.transform.localScale = new Vector3(1, 0.75f, 1);
        float attackOrCover = Random.Range(1.0f, 10.0f);
        if (attackOrCover >= 2.5f)
        {
            stateManager.SetCurrentAction("Taking cover");
            self.transform.localScale = new Vector3(1, 0.75f, 1);
        }
        else if (attackOrCover < 2.5f && attackOrCover >= 1.0f)
        {
            stateManager.SetCurrentAction("Shooting from cover");
            self.transform.localScale = new Vector3(1, 1, 1);
            stateManager.StartCoroutine(AttackingWithGun());
            self.transform.localScale = new Vector3(1, 0.75f, 1);
        }
        else
        {
            ChoosingCover();
            //coverScript.beingUsed = false;
        }

        yield return new WaitForSeconds(2.5f);
        stateManager.StartCoroutine(AtCover());
        yield return null;
    }

    IEnumerator AttackingWithGun()
    {
        //Debug.Log("Shooting");
        stateManager.SetIsAttacking(true);
        yield return new WaitForSeconds(2.75f);
        stateManager.StartCoroutine(stateManager.PlayAudioOneShot(18, 20));
        Sc_BaseGun gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        stateManager.StartCoroutine(gunScript.ShotFired());
        yield return new WaitForSeconds(1.25f);
        stateManager.SetIsAttacking(false);
        yield return null;
    }
}
