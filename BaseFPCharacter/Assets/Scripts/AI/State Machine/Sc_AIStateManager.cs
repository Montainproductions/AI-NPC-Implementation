using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class Sc_AIStateManager : MonoBehaviour
{
    //Audioclips currently not being used: 0-2, 12-14, 21-23
    //Setting up the traits of the AI.
    private Trait aiTrait;
    private AudioClip[] aiAudioClips;
    private AudioClip audioToplay, recentlyPlayedAudio;
    [SerializeField]
    private AudioSource aiAudioSource;
    private float lastAudioTimer;
    private bool canPlayAudio;

    //All the current state the AI can be in
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

    //A script that contains a set of common methods that multiple states can call on
    [SerializeField]
    private Sc_CommonMethods commonMethods;
    
    //The navigation agent of the AI
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    
    //Director AI that controls all of the AI
    [SerializeField]
    private Sc_AIDirector directorAI;

    //The player game object and weather they have been spotted
    [SerializeField]
    private GameObject player;
    [HideInInspector]
    public bool playerNoticed;

    //Set of timers and ranges that determine how the AI percives the player and for how long they will be searching.
    [SerializeField]
    private float visionRange, visionConeAngle, audioRange, alertedTimer, decisionTimer, idleTimer;
    private float distPlayer, angleToPlayer;

    //The value that the AI determines if they should go and attack the player or go to cover
    private float decisionValue = 0;

    //Variables that are important for the patrol state
    [Header("Patroling")]
    //All the patrol points the AI can walk to and from
    [SerializeField]
    private GameObject[] patrolPoints;

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

    //All variables related to the AI searching for the player when they lose line of sight
    [Header("Searching")]
    //How long ago was the player seen
    [SerializeField]
    private float lastSeenTimer;
    //Group of positions that the AI can take to search for the player.
    [SerializeField]
    private GameObject[] searchFormats;

    //Animation information
    private bool isAttacking, isIdling, isWalking;

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
        lastAudioTimer = 4.0f;
        canPlayAudio = true;

        //Sets starting state to patroling
        currentState = patrolState;

        //Sends crucial information of varabiles and scripts that each state needs before starting as it helps with the states to operate correctly.
        patrolState.PatrolStartStateInfo(this, commonMethods, player.GetComponent<Sc_Player_Movement>(), patrolPoints, visionRange, visionConeAngle, audioRange);
        attackState.AttackStartStateInfo(this, commonMethods, player.GetComponent<Sc_Player_Movement>(), gameObject, player, currentWeapon, navMeshAgent, visionRange, visionConeAngle);
        aggressionDesicionState.AggressionStartStateInfo(this, directorAI, gameObject, player, currentWeapon, cover, navMeshAgent, coverDistance);
        coverState.CoverStartStateInfo(this, commonMethods, player.GetComponent<Sc_Player_Movement>(), gameObject, player, currentWeapon, cover, visionRange, visionConeAngle);
        searchState.SearchStartStateInfo(this, player.GetComponent<Sc_Player_Movement>(), gameObject, player, searchFormats, navMeshAgent, visionRange, visionConeAngle);
        idleState.IdleStartStateInfo(this, player.GetComponent<Sc_Player_Movement>(), idleTimer, visionRange, visionConeAngle, audioRange);
        
        currentState.EnterState(playerNoticed);

        //Sets animation to walking
        SetIsIdling(false);
        SetIsAttacking(false);
        SetIsWalking(true);

        //Sets UI text for AI to active or not
        actionUIText.SetActive(showActions);
    }

    // Update is called once per frame
    void Update()
    {
        //Calculates the distance and angle to the player. Used mostly to determine if the player is in view. Currently planning on changing so that the player is deteced by a game object collider.
        distPlayer = Vector3.Distance(transform.position, player.transform.position);
        angleToPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        //Debug.Log(currentState);

        //Determines if the player is currently behind a wall. Currently being used to check if the AI can see the player while it is patroling but will be also used to check if the AI can shoot directly at the player
        Vector3 direction = player.transform.position - transform.position;
        bool playerBehindWall = Physics.Raycast(transform.position, direction, out hit, visionRange - 5, layerMask);

        currentState.UpdateState(distPlayer, angleToPlayer, playerBehindWall);

        //If the UI text should be updated
        if (showActions)
        {
            //aiTrait.ReturnAgressionValue();
            //Sets the text on top of the AI to show the current state and action that the AI is doing. Helps to show what they are "Thinking"
            stateText.SetText(currentState.ToString() + " " + currentAction);
        }
    }

    //Switches to the new state and starts the new enter state method for that new state
    public void SwitchState(Sc_AIBaseState state)
    {
        currentState = state;
        currentState.EnterState(playerNoticed);
    }

    public void SetUpTraits(Trait newAITrait, AudioClip[] audioClips)
    {
        this.aiTrait = newAITrait;
        this.aiAudioClips = audioClips;

        //Debug.Log(newAITrait.ReturnName());
        //Debug.Log(aiTrait.ReturnName());
        commonMethods.SetUpTrait(aiTrait);
        aggressionDesicionState.SetUpTrait(aiTrait);
    }

    public void PlayAudioOneShot(int audioPosition)
    {
        if (!aiAudioSource.isPlaying)
        {
            aiAudioSource.PlayOneShot(aiAudioClips[audioPosition]);
        }
    }

    public IEnumerator PlayAudioOneShot(int lowerLevelIncl, int higherLevelIncl)
    {
        Debug.Log("Playing audio");
        if (!aiAudioSource.isPlaying && canPlayAudio)
        {
            int audioPosition = Random.Range(lowerLevelIncl, higherLevelIncl);
            
            if (directorAI.PlayAudio(audioPosition, this))
            {
                canPlayAudio = false;
                //Debug.Log("PlayingAudio");
                aiAudioSource.PlayOneShot(aiAudioClips[audioPosition]);
                directorAI.NotPlayingAudio();
                yield return new WaitForSeconds(lastAudioTimer);
                canPlayAudio = true;
            }
        }

        yield return null;
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
