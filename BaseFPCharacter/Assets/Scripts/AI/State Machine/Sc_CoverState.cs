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
    private Sc_Player_Movement playerMovementScript;
    private Sc_CommonMethods commonMethodsScript;

    private GameObject self, player, closestCover, currentWeapon;
    private Vector3 playerPos, coverPosition;

    private GameObject[] allCover;

    private float closestDist, visionRange, visionConeAngle;

    //When first entering the state the choosing cover IEnumerator and the redeciding timer.
    public override void EnterState(bool playerSeen)
    {
        closestDist = Mathf.Infinity;
        //Debug.Log("Going to cover Start");
        coverPosition = Vector3.zero;
        stateManager.StartCoroutine(ChoosingCover());
        stateManager.StartCoroutine(commonMethodsScript.ReDecide());
    }

    //Will check if the AI is near by the cover point. If its not close then contine having the AI go to the cover point, else it will stop the AI and start the atcover Corutine. 
    public override void UpdateState(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
        CantSeePlayer(distPlayer, angleToPlayer);
    }

    //Recives important variables that are needed for the entire state to work properly.
    public void CoverStartStateInfo(Sc_AIStateManager stateManager, Sc_CommonMethods commonMethodsScript, Sc_Player_Movement playerMovementScript, GameObject self, GameObject player, GameObject currentWeapon, GameObject[] allCover, float visionRange, float visionConeAngle)
    {
        this.stateManager = stateManager;
        this.commonMethodsScript = commonMethodsScript;
        this.playerMovementScript = playerMovementScript;
        this.self = self;
        this.player = player;
        this.currentWeapon = currentWeapon;
        this.allCover = allCover;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
    }

    //If the player leaves the AIs line of site then it will stop trying to go to cover and start to search for the player.
    public void CantSeePlayer(float distPlayer, float angleToPlayer)
    {
        bool playerHidden = playerMovementScript.ReturnIsHidden();
        if (distPlayer > visionRange || angleToPlayer > visionConeAngle || playerHidden)
        {
            stateManager.StartCoroutine(stateManager.PlayAudioOneShot(3, 5));
            stateManager.playerNoticed = false;
            stateManager.SwitchState(stateManager.searchState);
        }
    }

    //Find the closest cover object and then determines which of the cover points children is behind the cover.
    IEnumerator ChoosingCover() {
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
        else if(attackOrCover < 2.5f && attackOrCover >= 1.0f)
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
