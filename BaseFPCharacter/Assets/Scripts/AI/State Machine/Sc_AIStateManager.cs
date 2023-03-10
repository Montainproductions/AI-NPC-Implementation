using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Sc_AIStateManager : MonoBehaviour
{
    public string behaviour;
    public float agressionValueChange, approchPlayerChange;
    public float accuracy;
    public AudioClip[] audioclips = new AudioClip[33];

    [SerializeField]
    private Sc_CommonMethods commonMethods;

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
    private float visionRange, visionConeAngle, alertedTimer, decisionTimer, idleTimer;
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

    [SerializeField]
    private bool canSeeEnemy;

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

    [Header("Searching")]
    [SerializeField]
    private GameObject[] searchFormats;

    // Start is called before the first frame update
    void Start()
    {
        currentState = patrolState;
        patrolState.PatrolStartStateInfo(this, player.GetComponent<Sc_Player_Movement>(), patrolPoints, navMeshAgent, visionRange, visionConeAngle);
        attackState.AttackStartStateInfo(this, commonMethods, player.GetComponent<Sc_Player_Movement>(), gameObject, player, currentWeapon, navMeshAgent, visionRange, visionConeAngle, weaponPosition);
        aggressionDesicionState.AggressionStartStateInfo(this, directorAI, gameObject, player, currentWeapon, cover, navMeshAgent, coverDistance);
        coverState.CoverStartStateInfo(this, commonMethods, player.GetComponent<Sc_Player_Movement>(), gameObject, player, currentWeapon, cover, visionRange, visionConeAngle);
        searchState.SearchStartStateInfo(this, player.GetComponent<Sc_Player_Movement>(), gameObject, player, searchFormats, navMeshAgent, visionRange, visionConeAngle);
        idleState.IdleStartStateInfo(this, player.GetComponent<Sc_Player_Movement>(), idleTimer, visionRange, visionConeAngle);
        currentState.EnterState(speed, playerNoticed);

        stateTextObj = Instantiate(stateTxtPrefab, Sc_Basic_UI.Instance.transform);
        stateText = stateTextObj.GetComponent<TextMeshProUGUI>();

        //Setting up agression trait
        if (behaviour == "Agression")
        {
            agressionValueChange = 3;
            approchPlayerChange = 2;
        }
        else if (behaviour == "Bold") //Setting up bold trait
        {
            agressionValueChange = 1.5f;
            approchPlayerChange = 0.5f;
        }
        else if (behaviour == "Cautious") //Setting up cautious trait
        {
            agressionValueChange = -1.5f;
            approchPlayerChange = -0.5f;
        }
        else if (behaviour == "Scared") //Setting up scared trait
        {
            agressionValueChange = -3;
            approchPlayerChange = -2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Calculates the distance and angle to the player. Used mostly to determine if the player is in view. Currently planning on changing so that the player is deteced by a game object collider.
        distPlayer = Vector3.Distance(transform.position, player.transform.position);
        angleToPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        //Debug.Log(currentState);
        currentState.UpdateState(distPlayer, angleToPlayer);

        //Sets the text on top of the AI to show the current state and action that the AI is doing. Helps to show what they are "Thinking"
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
        currentState.EnterState(speed, playerNoticed);
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
