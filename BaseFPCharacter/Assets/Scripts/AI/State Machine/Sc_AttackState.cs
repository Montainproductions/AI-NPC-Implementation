using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AttackState : Sc_AIBaseState
{
    private GameObject player;
    private Vector3 playerPos;


    public override void EnterState(Sc_AIStateManager state, float speed) {
    }

    public override void UpdateState(Sc_AIStateManager state) {
        playerPos = player.transform.position;
        //transform.LookAt(playerPos);
    }

    public override void OnCollisionEnter(Sc_AIStateManager state) { }

    public void AttackStateInfo(GameObject playerObj)
    {
        player = playerObj;
    }
}
