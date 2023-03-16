using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sc_AIBaseState
{

    public abstract void EnterState(float speed, bool playerSeen);

    public abstract void UpdateState(float distPlayer, float angleToPlayer, bool playerBehindWall);
}
