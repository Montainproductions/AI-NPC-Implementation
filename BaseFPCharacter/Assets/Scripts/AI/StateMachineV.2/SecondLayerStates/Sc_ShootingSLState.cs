using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_ShootingSLState : Sc_AIBaseStateHierarchical
{
    private Sc_AIStatesManagerHierarchical stateManager;
    private Sc_HFSMCommenMethods commonMethods;

    public string currentAction;

    private GameObject self, player;
    private Sc_BaseGun gunScript;
    private Vector3 playerPos;

    [HideInInspector]
    public bool isMoving;

    private float attackRange, timeDelay, diffDistToAttack;

    public override void EnterState()
    {
        stateManager.StartCoroutine(commonMethods.StopMovement());
        isMoving = false;

        stateManager.StartCoroutine(AttackOrMove());
        stateManager.StartCoroutine(commonMethods.ReDecide());

        stateManager.StartCoroutine(PlayerAttackDistance());
    }

    public override void UpdateState()
    {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
    }

    public void AttackStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commonMethodsScript, GameObject self, GameObject player, GameObject currentWeapon)
    {
        this.stateManager = stateManager;
        this.commonMethods = commonMethodsScript;
        this.self = self;
        this.player = player;
        gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        attackRange = gunScript.ReturnEffectiveRange();
    }

    IEnumerator AttackOrMove()
    {
        if (diffDistToAttack >= 0)
        {
            //isMoving = true;
            stateManager.StartCoroutine(commonMethods.AttackingGettingCloser(player.transform, diffDistToAttack));
        }
        else if (gunScript.ReturnCurrentAmmo() > 0 && diffDistToAttack < 0)
        {
            stateManager.StartCoroutine(AttackingWithGun());
        }
        else if (gunScript.ReturnCurrentAmmo() <= 0)
        {
            stateManager.StartCoroutine(Reloading());
        }
        yield return new WaitForSeconds(0.2f);
        stateManager.StartCoroutine(AttackOrMove());
        yield return null;
    }

    IEnumerator AttackingWithGun()
    {
        stateManager.SetCurrentAction("Shooting player");
        stateManager.SetIsAttacking(true);
        stateManager.SetIsWalking(false);
        //Debug.Log("Shooting");
        timeDelay = Random.Range(1.5f, 2.5f);
        yield return new WaitForSeconds(timeDelay);
        stateManager.StartCoroutine(commonMethods.PlayRandomAudioOneShot(15, 17));
        stateManager.StartCoroutine(gunScript.ShotFired());
        //Debug.Log("Enemy ammo count: " + gunScript.currentAmmoAmount);
        timeDelay = Random.Range(1.25f, 1.85f);
        yield return new WaitForSeconds(timeDelay);
        stateManager.SetIsAttacking(false);
        yield return null;
    }

    //Will reload the current weapon if out of ammo. There is also wait timer so that it seams like the person is taking time to realize that they are out of ammo.
    IEnumerator Reloading()
    {
        stateManager.SetCurrentAction("Reloading");
        //Debug.Log("Shooting");
        stateManager.StartCoroutine(commonMethods.PlayRandomAudioOneShot(9, 11));
        yield return new WaitForSeconds(4.25f);

        stateManager.StartCoroutine(gunScript.Reloading());
        yield return new WaitForSeconds(2);
        yield return null;
    }

    IEnumerator PlayerAttackDistance()
    {
        float playerDist = Vector3.Distance(playerPos, self.transform.position);
        diffDistToAttack = playerDist - attackRange;
        yield return new WaitForSeconds(0.1f);
        stateManager.StartCoroutine(PlayerAttackDistance());
        yield return null;
    }
}
