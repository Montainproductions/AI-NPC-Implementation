using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sc_AIBaseState
{

    public abstract void EnterState(Sc_AIStateManager state, float speed);

    public abstract void UpdateState(Sc_AIStateManager state);

    public abstract void OnCollisionEnter(Sc_AIStateManager state);
}
