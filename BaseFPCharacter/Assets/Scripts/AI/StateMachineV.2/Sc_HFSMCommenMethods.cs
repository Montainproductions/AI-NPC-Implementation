using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class Sc_HFSMCommenMethods : MonoBehaviour
{
    [SerializeField]
    private Sc_AIStatesManagerHierarchical stateManager;

    private Trait aiTrait;

    private GameObject self, player;

    private NavMeshAgent navMeshAgent;

    private Sc_Player_Movement playerMovemenetScript;

    private string currentState;

    private Vector3 walkingPosition;
    private Vector3 direction;

    private Transform lookingAtTransform;

    private float visionRange, visionConeAngle;

    //Player info
    private float distPlayer, angleToPlayer;

    [SerializeField]
    private float decisionTimer;

    //To check if the player is being blocked by some objects.
    // Bit shift the index of the layer (9) to get a bit mask
    private int layerMask = 1 << 9;

    private RaycastHit hit;

    private AudioSource aiAudioSource;
    private AudioClip[] aiAudioClips;

    private int recentlyPlayedAudio;
    private float lastAudioTimer;
    private bool canPlayAudio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Calculates the distance and angle to the player. Used mostly to determine if the player is in view. Currently planning on changing so that the player is deteced by a game object collider.
        distPlayer = Vector3.Distance(transform.position, player.transform.position);
        angleToPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);

        //Determines if the player is currently behind a wall. Currently being used to check if the AI can see the player while it is patroling but will be also used to check if the AI can shoot directly at the player
        Vector3 direction = player.transform.position - transform.position;
        bool playerBehindWall = Physics.Raycast(transform.position, direction, out hit, visionRange - 5, layerMask);

        if (stateManager.currentFLState == stateManager.nonCombatFLState || stateManager.currentFLState == stateManager.alertFLState)
        {
            StartCoroutine(CanSeePlayer(distPlayer, angleToPlayer, playerBehindWall));
        }

        if (walkingPosition != Vector3.zero)
        {
            if (Vector3.Distance(self.transform.position, walkingPosition) > 1.1f)
            {
                //Debug.Log("Moving");
                stateManager.SetCurrentAction("Going to " + currentState + " point");

                direction = (walkingPosition - transform.position).normalized;
                //Creates Quaternion version of the vector3 direction
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                //Rotate Enemy over time according to speed until we are in the required rotation
                //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 0.25f);
                transform.LookAt(lookingAtTransform, Vector3.up);
                navMeshAgent.destination = walkingPosition;
            }
            else
            {
                //Debug.Log("At Cover");
                //self.transform.localScale = new Vector3(1,0.75f,1);
                walkingPosition = Vector3.zero;
                if (currentState == "Cover")
                {
                    StartCoroutine(stateManager.coverState.AtCover());
                }
                else if (currentState == "Attack")
                {
                    stateManager.attackState.isMoving = false;
                }
                else if (currentState == "Patrolling")
                {
                    StopMovement();
                    stateManager.patrolState.Patroling();
                }
            }
        }
    }

    public void CommenMethodSetUp(Sc_Player_Movement playerMovemenetScript, GameObject self, GameObject player, NavMeshAgent navMeshAgent, AudioSource audioSource, float visionRange, float visionConeAngle)
    {
        this.playerMovemenetScript = playerMovemenetScript;
        this.self = self;
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.aiAudioSource = audioSource;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
    }

    public void SetUpTrait(Trait aiTrait, AudioClip[] audioClips)
    {
        this.aiTrait = aiTrait;
        this.aiAudioClips = audioClips;
    }

    public void StartMovement(Vector3 position, string currentState, bool lookAtPlayer = false, Transform lookAt = null)
    {
        walkingPosition = position;
        this.currentState = currentState;
        //Debug.Log("Current action state: " + currentState);
        //Debug.Log(this.currentState);
        //lookingAtPlayer = lookAtPlayer;
        lookingAtTransform = lookAt;
    }

    public IEnumerator StopMovement()
    {
        yield return new WaitForSeconds(0.25f);
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(stateManager.transform.position);
        //Debug.Log(stateManager.name);
        //Debug.Log(navMeshAgent.destination);
        yield return null;
    }

    //Once the timer is finished the AI will return to the agression decision state and decide if its better to go to cover or continue attacking the player.
    public IEnumerator ReDecide()
    {
        float newDecisionTimer = Random.Range(decisionTimer - 5, decisionTimer + 5);
        yield return new WaitForSeconds(newDecisionTimer);
        stateManager.SwitchSLState(stateManager.aggressionDesicionState);
        yield return null;
    }

    public IEnumerator AttackingGettingCloser(Transform player, float diffDistToAttack)
    {
        float zDistance = Random.Range(diffDistToAttack + 1 + aiTrait.ReturnApprochingPlayer(), diffDistToAttack + 6 + aiTrait.ReturnApprochingPlayer());
        //Debug.Log(zDistance);
        //float yDistance = Random.Range(-diffDistToAttack, diffDistToAttack);
        Vector3 newPosition = stateManager.transform.position + stateManager.transform.forward * zDistance;
        StartMovement(newPosition, "Approching", true, player);
        yield return null;
    }

    public IEnumerator CanSeePlayer(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {

        int alertedTimeLeft = 0;
        bool playerSeenSecondCheck, playerSeenFirstCheck = PlayerInVision(distPlayer, angleToPlayer, playerBehindWall);

        if (playerSeenFirstCheck && stateManager.currentFLState == stateManager.nonCombatFLState)
        {
            //stateManager.StartCoroutine(stateManager.PlayAudioOneShot(6, 8));
            //directorAI.PlayerFound(state.gameObject);
            yield return new WaitForSeconds(0.75f);
            stateManager.playerNoticed = true;
            stateManager.SwitchFLState(stateManager.alertFLState);
            stateManager.SwitchSLState(stateManager.alertedState);
            Debug.Log("Player First Seen");
        }else if (stateManager.currentFLState == stateManager.alertFLState)
        {
            for (alertedTimeLeft = 2; alertedTimeLeft > 0; alertedTimeLeft--)
                if (stateManager.playerNoticed)
                {
                    stateManager.SwitchFLState(stateManager.combatFLState);
                    stateManager.SwitchSLState(stateManager.aggressionDesicionState);
                    Debug.Log("Combat Started");
                }
                yield return null;
            playerSeenSecondCheck = PlayerInVision(distPlayer, angleToPlayer, playerBehindWall);

            if (playerSeenSecondCheck)
            {
                stateManager.SwitchFLState(stateManager.combatFLState);
                stateManager.SwitchSLState(stateManager.aggressionDesicionState);
                Debug.Log("Combat Started");
            }
            else
            {
                stateManager.SwitchFLState(stateManager.nonCombatFLState);
                stateManager.SwitchSLState(stateManager.idleState);
                Debug.Log("Back to idling/patroling");
            }
        }
        yield return null;
    }

    public IEnumerator CantSeePlayer(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {
        yield return null;
    }

    public bool PlayerInVision(float distPlayer, float angleToPlayer, bool playerBehindWall)
    {
        bool playerVisible = false;
        bool playerHidden = playerMovemenetScript.ReturnIsHidden();
        if ((distPlayer <= visionRange - 15 && angleToPlayer <= visionConeAngle - 15) && !playerHidden && !playerBehindWall)
        {
            playerVisible = true;
        }
        return playerVisible;
    }

    public void PlayAudioOneShot(int audioPosition)
    {
        if (!aiAudioSource.isPlaying)
        {
            aiAudioSource.PlayOneShot(aiAudioClips[audioPosition]);
        }
    }

    public IEnumerator PlayRandomAudioOneShot(int lowerLevelIncl, int higherLevelIncl)
    {
        Debug.Log("Playing audio");
        if (!aiAudioSource.isPlaying && canPlayAudio)
        {
            int audioPosition = Random.Range(lowerLevelIncl, higherLevelIncl);

            if (recentlyPlayedAudio != audioPosition)
            {
                recentlyPlayedAudio = audioPosition;
                canPlayAudio = false;
                //Debug.Log("PlayingAudio");
                aiAudioSource.PlayOneShot(aiAudioClips[audioPosition]);
                yield return new WaitForSeconds(lastAudioTimer);
                canPlayAudio = true;
            }
        }

        yield return null;
    }
}
