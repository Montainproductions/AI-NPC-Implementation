using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_SearchState : Sc_AIBaseState
{
    public override void EnterState(Sc_AIStateManager state, float speed)
    {
        Debug.Log("Searching for Player");
    }

    public override void UpdateState(Sc_AIStateManager state, float distPlayer, float angleToPlayer)
    {
    }
}
