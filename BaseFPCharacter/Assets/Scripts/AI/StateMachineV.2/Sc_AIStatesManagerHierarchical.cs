using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Sc_AIStatesManagerHierarchical : MonoBehaviour
{
    private Trait aiTrait;

    [SerializeField]
    private GameObject self;

    //All first layer states. First layer reperesents the bigger states that may contain multiple smaller states
    [HideInInspector]
    public Sc_AIBaseStateHierarchical currentFLState;
    [HideInInspector]
    public Sc_NonCombatFLState nonCombatFLState = new Sc_NonCombatFLState();
    [HideInInspector]
    public Sc_AlertFLState alertFLState = new Sc_AlertFLState();
    [HideInInspector]
    public Sc_CombatFLState combatFLState = new Sc_CombatFLState();

    //All the current states in the second layer of the HFSM which the AI can be in
    [HideInInspector]
    public Sc_AIBaseStateHierarchical currentSLState;
    [HideInInspector]
    public Sc_PatrolingSLState patrolState = new Sc_PatrolingSLState();
    [HideInInspector]
    public Sc_IdleSLState idleState = new Sc_IdleSLState();
    [HideInInspector]
    public Sc_AlertedSLState alertedState = new Sc_AlertedSLState();
    [HideInInspector]
    public Sc_SearchingSLState searchState = new Sc_SearchingSLState();
    [HideInInspector]
    public Sc_AggressionSLState aggressionDesicionState = new Sc_AggressionSLState();
    [HideInInspector]
    public Sc_ShootingSLState attackState = new Sc_ShootingSLState();
    [HideInInspector]
    public Sc_CoverSLState coverState = new Sc_CoverSLState();

    //A script that contains a set of common methods that multiple states can call on
    [SerializeField]
    private Sc_HFSMCommenMethods commonMethods;

    //The navigation agent of the AI
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    //Director AI that controls all of the AI
    [SerializeField]
    private Sc_AIDirector directorAI;

    //The player game object and weather they have been spotted
    [SerializeField]
    private GameObject player;
    private Vector3 playerPosition;
    [HideInInspector]
    public bool playerNoticed;

    [SerializeField]
    private float visionRange, visionConeAngle, audioRange, alertedTimer, decisionTimer;

    //The value that the AI determines if they should go and attack the player or go to cover
    private float decisionValue = 0;

    //Variables that are important for the patrol state
    [Header("Patroling")]
    //All the patrol points the AI can walk to and from
    [SerializeField]
    private GameObject[] patrolPoints;

    [Header("Idle")]
    [SerializeField]
    private float idleTimer;

    //All variables related to the attack state the player
    [Header("Attacking/Chasing")]
    //Current weapon gameobject
    [SerializeField]
    private GameObject currentWeapon;

    //Variables important to the cover state
    [Header("Cover")]
    //All cover positions that the player can use
    [SerializeField]
    private GameObject[] cover;
    //How far the AI is willing to run to cover
    [SerializeField]
    private float coverDistance;

    //Animation information
    private bool isAttacking, isReloading, isIdling, isWalking;

    //To check if the player is being blocked by some objects.
    // Bit shift the index of the layer (9) to get a bit mask
    private int layerMask = 1 << 9;

    private RaycastHit hit;

    //UI variables that appears on top of each AI that helps explain what their current actions are. Helps with debuging each individual AI better
    [Header("UI State Text")]
    [SerializeField]
    private bool showActions;
    [SerializeField]
    private GameObject actionUIText;
    [SerializeField]
    private TextMeshProUGUI stateText;
    public string currentAction;

    // Start is called before the first frame update
    void Start()
    {
        //Set first layer state to Non-Combate
        currentFLState = nonCombatFLState;
        //Sets starting state to patroling
        currentSLState = patrolState;

        patrolState.PatrolStartStateInfo(commonMethods, patrolPoints);
        idleState.IdleStartStateInfo(this, commonMethods, idleTimer);
        aggressionDesicionState.AggressionStartStateInfo(this, directorAI, gameObject, player, currentWeapon, cover, coverDistance);
        attackState.AttackStartStateInfo(this, commonMethods, self, player, currentWeapon);
        coverState.CoverStartStateInfo(this, commonMethods, self, player, currentWeapon, cover);

        playerPosition = player.transform.position;

        currentFLState.EnterState(playerPosition);
        currentSLState.EnterState(playerPosition);
    }

    // Update is called once per frame
    void Update()
    {
        currentSLState.UpdateState();
    }

    public void SwitchFLState(Sc_AIBaseStateHierarchical state)
    {
        playerPosition = player.transform.position;
        
        currentFLState = state;
        currentFLState.EnterState(playerPosition);
    }

    public void SwitchSLState(Sc_AIBaseStateHierarchical state)
    {
        playerPosition = player.transform.position;
        
        currentSLState = state;
        currentSLState.EnterState(playerPosition);
    }

    public void SetUpTraits(Trait newAITrait, AudioClip[] audioClips)
    {
        this.aiTrait = newAITrait;
        //this.aiAudioClips = audioClips;

        commonMethods.SetUpTrait(aiTrait, audioClips);
        searchState.SetUpTrait(aiTrait);
        aggressionDesicionState.SetUpTrait(aiTrait);
    }

    public void RecentlyHit()
    {
        if (currentFLState != combatFLState)
        {
            SwitchFLState(combatFLState);
            SwitchSLState(aggressionDesicionState);
        }
    }

    //Sets the AIs decision value
    public void SetDecisionValue(float value)
    {
        decisionValue = value;
    }

    //returns the AIs decision value
    public float ReturnDecisionValue()
    {
        return decisionValue;
    }

    //Current action which is used for some UI so that the user can better determine what each individual AI is doing.
    public void SetCurrentAction(string action)
    {
        currentAction = action;
    }

    //Sets if the AI is attacking the player
    public void SetIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }

    //Returns if the AI is attacking
    public bool ReturnIsAttacking()
    {
        return isAttacking;
    }

    public void SetIsReloading(bool isReloading)
    {
        this.isReloading = isReloading;
    }

    public bool ReturnIsReloading()
    {
        return isReloading;
    }

    //Setsif the AI is idling
    public void SetIsIdling(bool isIdling)
    {
        this.isIdling = isIdling;
    }

    //Returns if the AI is idling
    public bool ReturnIsIdling()
    {
        return isIdling;
    }

    //Sets if the AI is walking
    public void SetIsWalking(bool isWalking)
    {
        this.isWalking = isWalking;
    }

    //Returns if the AI is walking
    public bool ReturnIsWalking()
    {
        return isWalking;
    }
}
