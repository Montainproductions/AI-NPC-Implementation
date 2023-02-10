using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Sc_AggressionState aggressionDesicionState = new Sc_AggressionState();
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
    [HideInInspector]
    public bool playerNoticed;

    [SerializeField]
    private float visionRange, visionConeAngle, alertedTimer, decisionTimer;
    private float distPlayer, angleToPlayer;

    [Header("UI State Text")]
    [SerializeField]
    private GameObject stateTxtPrefab;
    private GameObject stateTextObj;
    private TextMeshProUGUI stateText;
    public string currentAction;

    [Header("Patroling")]
    [SerializeField]
    private GameObject[] patrolPoints;

    [HideInInspector]
    public int decisionValue = 0;

    [Header("Attacking/Chasing")]
    [SerializeField]
    private GameObject currentWeapon;
    [SerializeField]
    private Transform weaponPosition;

    [Header("Cover")]
    [SerializeField]
    private GameObject[] cover;
    [SerializeField]
    private float coverDistance;

    // Start is called before the first frame update
    void Start()
    {
        currentState = patrolState;
        patrolState.PatrolStartStateInfo(patrolPoints, navMeshAgent, visionRange, visionConeAngle, this);
        attackState.AttackStartStateInfo(gameObject, player, currentWeapon, navMeshAgent, visionRange, visionConeAngle, decisionTimer, weaponPosition, this);
        aggressionDesicionState.AggressionStartStateInfo(gameObject, player, currentWeapon, cover, coverDistance, directorAI, this, navMeshAgent);
        coverState.CoverStartStateInfo(gameObject, player, currentWeapon, cover, navMeshAgent, visionRange, visionConeAngle, decisionTimer, this);
        currentState.EnterState(this, speed, playerNoticed);

        stateTextObj = Instantiate(stateTxtPrefab, Sc_Basic_UI.Instance.transform);
        stateText = stateTextObj.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calculates the distance and angle to the player. Used mostly to determine if the player is in view. Currently planning on changing so that the player is deteced by a game object collider.
        distPlayer = Vector3.Distance(transform.position, player.transform.position);
        angleToPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        //Debug.Log(currentState);
        currentState.UpdateState(this, distPlayer, angleToPlayer);

        stateTextObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 3);
        stateText.SetText(currentState.ToString() + " " + currentAction);
    }

    public void OnDestroy()
    {
        Destroy(stateTextObj);
    }

    //Switches to the new state and starts the new enter state method for that new state
    public void SwitchState(Sc_AIBaseState state)
    {
        currentState = state;
        currentState.EnterState(this, speed, playerNoticed);
    }

    public void SetDecisionValue(int value)
    {
        decisionValue = value;
    }

    //Current action which is used for some UI so that the user can better determine what each individual AI is doing.
    public void SetCurrentAction(string action)
    {
        currentAction = action;
    }
}
