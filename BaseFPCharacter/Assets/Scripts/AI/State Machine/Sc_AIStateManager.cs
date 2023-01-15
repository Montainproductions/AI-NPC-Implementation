using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AIStateManager : MonoBehaviour
{
    private Sc_AIBaseState currentState;
    private Sc_AttackState attackState = new Sc_AttackState();
    private Sc_IdleState idleState = new Sc_IdleState();
    private Sc_PatrolState patrolState = new Sc_PatrolState();

    [SerializeField]
    private float speed;
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [Header("Patroling")]
    [SerializeField]
    private GameObject[] patrolPoints;

    // Start is called before the first frame update
    void Start()
    {
        currentState = patrolState;
        patrolState.PatrolStateInfo(patrolPoints, navMeshAgent);

        currentState.EnterState(this, speed);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    void SwitchState(Sc_AIBaseState state)
    {
        currentState = state;
        currentState.EnterState(this, speed);
    }
}
