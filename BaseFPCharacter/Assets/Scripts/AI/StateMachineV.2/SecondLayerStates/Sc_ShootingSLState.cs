using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_ShootingSLState : Sc_AIBaseStateHierarchical
{
    //State machince manager
    private Sc_AIStatesManagerHierarchical stateManager;

    //Script containing commen methods.
    private Sc_HFSMCommenMethods commenMethods;

    //Current string with the current action the AI is doing
    public string currentAction;

    //Game object for the specific AI NPC and the player
    private GameObject self, player;

    //Vecter3 position of the player
    private Vector3 playerPos;

    //Script of the gun that the AI is using
    private Sc_BaseGun gunScript;

    //If the player is moving or not
    [HideInInspector]
    public bool isMoving;

    //Floats used to determine the range that the AI can attack from, amount of time it needs to delay actions and the difference in distance from the player and the effective range of the AI.
    private float attackRange, timeDelay, diffDistToAttack;

    //First frame when the state is initially called
    public override void EnterState(Vector3 playerPosition)
    {
        stateManager.StartCoroutine(commenMethods.StopMovement());
        isMoving = false;

        stateManager.StartCoroutine(AttackOrMove());
        stateManager.StartCoroutine(commenMethods.ReDecide());

        stateManager.StartCoroutine(PlayerAttackDistance());
    }

    //Updates each frame the state script is active
    public override void UpdateState()
    {
        playerPos = player.transform.position;
        stateManager.transform.LookAt(playerPos);
    }

    //Sets up the important variable information that is important to the script
    public void AttackStartStateInfo(Sc_AIStatesManagerHierarchical stateManager, Sc_HFSMCommenMethods commenMethods, GameObject self, GameObject player, GameObject currentWeapon)
    {
        this.stateManager = stateManager;
        this.commenMethods = commenMethods;
        this.self = self;
        this.player = player;
        gunScript = currentWeapon.GetComponent<Sc_BaseGun>();
        attackRange = gunScript.ReturnEffectiveRange();
    }

    //Desiceds if it the AI needs to move towards the player or shoot at them
    IEnumerator AttackOrMove()
    {
        if (diffDistToAttack >= 0)
        {
            //isMoving = true;
            stateManager.StartCoroutine(commenMethods.AttackingGettingCloser(player.transform, diffDistToAttack));
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

    //Shoots at the player. Random range is used to create a better variance between shooting from the AI
    IEnumerator AttackingWithGun()
    {
        stateManager.SetCurrentAction("Shooting player");
        stateManager.SetIsAttacking(true);
        stateManager.SetIsWalking(false);
        //Debug.Log("Shooting");
        timeDelay = Random.Range(1.5f, 2.5f);
        yield return new WaitForSeconds(timeDelay);
        stateManager.StartCoroutine(commenMethods.PlayRandomAudioOneShot(15, 17));
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
        stateManager.StartCoroutine(commenMethods.PlayRandomAudioOneShot(9, 11));
        yield return new WaitForSeconds(4.25f);

        stateManager.StartCoroutine(gunScript.Reloading());
        yield return new WaitForSeconds(2);
        yield return null;
    }

    //Determines the distance that the AI is from the player and has them get closer.
    IEnumerator PlayerAttackDistance()
    {
        float playerDist = Vector3.Distance(playerPos, self.transform.position);
        diffDistToAttack = playerDist - attackRange;
        yield return new WaitForSeconds(0.1f);
        stateManager.StartCoroutine(PlayerAttackDistance());
        yield return null;
    }
}
