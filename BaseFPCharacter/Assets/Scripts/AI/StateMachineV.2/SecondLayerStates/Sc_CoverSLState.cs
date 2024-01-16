using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CoverSLState : Sc_AIBaseStateHierarchical
{
    //State manager script
    private Sc_AIStatesManagerHierarchical stateManager;

    //Commen methods script containing various commen methods
    private Sc_HFSMCommenMethods commenMethods;

    //The AI gameobject, the player gameobject and the chossen closest cover gameobject
    private GameObject self, player, closestCover;

    //The vector3 position of the player and the closest cover point
    private Vector3 playerPos, coverPosition;

    //Array containing all the possible cover points
    private GameObject[] allCover;

    //Script that controls the AIs gun
    private Sc_BaseGun gunScript;

    //Distance to the closest cover point
    private float closestDist;


    //Enter state thats run when initially entering the cover state script
    public override void EnterState(Vector3 playerPosition)
    {
        closestDist = Mathf.Infinity;
        //Debug.Log("Going to cover Start");
        coverPosition = Vector3.zero;
        stateManager.StartCoroutine(ChoosingCover());
        stateManager.StartCoroutine(commenMethods.ReDecide());
    }

    //Updates each frame the state script is active
    public override void UpdateState()
    {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
    }

    //Recives important variables that are needed for the entire state to work properly.
    public void CoverStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commonMethodsScript, GameObject self, GameObject player, GameObject currentWeapon)
    {
        this.stateManager = stateManager;
        this.commenMethods = commonMethodsScript;
        this.self = self;
        this.player = player;
        gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
    }

    public void ReciveAllCoverPoints(GameObject[] allCover) 
    {
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
                stateManager.StartCoroutine(commenMethods.PlayRandomAudioOneShot(24, 26));
                commenMethods.StartMovement(coverPosition, "Cover", player.transform);
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

    //Will stand up and shoot with their weapon towards the player.
    IEnumerator AttackingWithGun()
    {
        //Debug.Log("Shooting");
        stateManager.SetIsAttacking(true);
        yield return new WaitForSeconds(2.75f);
        stateManager.StartCoroutine(commenMethods.PlayRandomAudioOneShot(18, 20));
        stateManager.StartCoroutine(gunScript.ShotFired());
        yield return new WaitForSeconds(1.25f);
        stateManager.SetIsAttacking(false);
        yield return null;
    }
}
