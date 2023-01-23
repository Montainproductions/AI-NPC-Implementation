using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CoverState : Sc_AIBaseState
{
    private GameObject[] allCover;

    public override void EnterState(Sc_AIStateManager state, float speed)
    {
        Debug.Log("Going to cover");
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {

    }

    public override void OnCollisionEnter(Sc_AIStateManager state)
    {

    }

    public void AttackStartStateInfo()
    {

    }
}
