using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AIStateManager : MonoBehaviour
{
    [HideInInspector]
    public Sc_AIBaseState currentState;
    [HideInInspector]
    public Sc_AttackState attackState = new Sc_AttackState();
    [HideInInspector]
    public Sc_IdleState idleState = new Sc_IdleState();
    [HideInInspector]
    public Sc_PatrolState patrolState = new Sc_PatrolState();
    [HideInInspector]
    public Sc_AggressionState aggressionState = new Sc_AggressionState();
    [HideInInspector]
    public Sc_CoverState coverState = new Sc_CoverState();
    [HideInInspector]
    public Sc_SearchState searchState = new Sc_SearchState();

    [SerializeField]
    private float speed;
    
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    
    [SerializeField]
    private Sc_AIDirector directorAI;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float visionRange, visionConeAngle, alertedTimer;
    private float distPlayer, angleToPlayer;

    [SerializeField]
    private GameObject currentStateTxt;

    [Header("Patroling")]
    [SerializeField]
    private GameObject[] patrolPoints;

    [HideInInspector]
    public int decisionValue = 0;

    [Header("Attacking/Chasing")]
    [SerializeField]
    private GameObject currentWeapon;

    [Header("Cover")]
    [SerializeField]
    private GameObject[] cover;
    [SerializeField]
    private float coverDistance;

    // Start is called before the first frame update
    void Start()
    {
        currentState = patrolState;
        patrolState.PatrolStartStateInfo(patrolPoints, navMeshAgent, visionRange, visionConeAngle, gameObject);
        attackState.AttackStartStateInfo(gameObject, player, currentWeapon, navMeshAgent, visionRange, visionConeAngle);
        aggressionState.AggressionStartStateInfo(gameObject, player, currentWeapon, cover, coverDistance, directorAI, this, navMeshAgent);
        coverState.CoverStartStateInfo(gameObject, player, currentWeapon, cover, navMeshAgent, visionRange, visionConeAngle);
        currentState.EnterState(this, speed);

        Instantiate(currentStateTxt, Sc_Basic_UI.Instance.transform.position, Quaternion.identity);
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

    public void SetDecisionValue(int value)
    {
        decisionValue = value;
    }
}
