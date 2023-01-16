using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AIStateManager : MonoBehaviour
{
    private Sc_AIBaseState currentState;
    [HideInInspector]
    public Sc_AttackState attackState = new Sc_AttackState();
    [HideInInspector]
    public Sc_IdleState idleState = new Sc_IdleState();
    [HideInInspector]
    public Sc_PatrolState patrolState = new Sc_PatrolState();

    [SerializeField]
    private float speed;
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private float visionRange, visionConeAngle, alertedTimer;
    private float distPlayer, angleToPlayer;

    [Header("Patroling")]
    [SerializeField]
    private GameObject[] patrolPoints;

    [Header("Attacking/Chasing")]
    [SerializeField]
    private GameObject player, currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        currentState = patrolState;
        patrolState.PatrolStartStateInfo(patrolPoints, navMeshAgent, visionRange, visionConeAngle);
        attackState.AttackStateInfo(player, currentWeapon, navMeshAgent);

        currentState.EnterState(this, speed);
    }

    // Update is called once per frame
    void Update()
    {
        distPlayer = Vector3.Distance(transform.position, player.transform.position);
        angleToPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        //Debug.Log(currentState);
        currentState.UpdateState(this, distPlayer, angleToPlayer);
    }

    public void SwitchState(Sc_AIBaseState state)
    {
        currentState = state;
        currentState.EnterState(this, speed);
    }
}
