using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sc_AIBaseParentState
{
    public abstract void EnterState();

    public abstract void UpdateState(float distPlayer, float angleToPlayer, bool playerBehindWall);
}
