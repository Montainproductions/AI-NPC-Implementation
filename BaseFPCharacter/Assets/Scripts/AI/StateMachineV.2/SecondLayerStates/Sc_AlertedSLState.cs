using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AlertedSLState : Sc_AIBaseStateHierarchical
{

    /// <summary>
    /// This state is currently meant to be a transitionary state for when the player is seen by the NPC.
    /// If the NPC still has line of sight of the player after 2 seconds (Might change) then it will just transition to the Agression state else it will transition back to the Patrol state
    /// As I work on the searching state which is part of the same Alert/Search parent state I might add some features to this state if I feel like it improves the AI.
    /// </summary>
    public override void EnterState()
    {

    }

    public override void UpdateState()
    {

    }
}
