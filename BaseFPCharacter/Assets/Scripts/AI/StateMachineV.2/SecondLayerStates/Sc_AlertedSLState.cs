using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AlertedSLState : Sc_AIBaseStateSL
{
    //This state is meant to be a transitionary state for when the player is seen by the NPC. If the NPC still has line of sight of the player after 2 seconds (Might change) then it will just transition to the Agression state else it will transition back to the Patrol state
    public override void EnterState()
    {

    }

    public override void UpdateState()
    {

    }
}
