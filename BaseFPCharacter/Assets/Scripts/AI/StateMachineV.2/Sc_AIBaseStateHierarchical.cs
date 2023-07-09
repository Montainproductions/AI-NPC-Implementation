using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sc_AIBaseStateHierarchical
{
    public abstract void EnterState(Vector3 playerPosition);

    public abstract void UpdateState();
}
