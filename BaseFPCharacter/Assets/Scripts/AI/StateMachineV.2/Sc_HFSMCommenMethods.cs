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

    [SerializeField]
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

        if (stateManager.currentFLState == stateManager.nonCombatParentState || stateManager.currentFLState == stateManager.alertParentState)
        {
            StartCoroutine(CanSeePlayer(distPlayer, angleToPlayer, playerBehindWall));
        }
    }

    public void CommenMethodSetUp(GameObject self, GameObject player, NavMeshAgent navMeshAgent, Sc_Player_Movement playerMovemenetScript, float visionRange, float visionConeAngle)
    {
        this.self = self;
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.playerMovemenetScript = playerMovemenetScript;
        this.visionRange = visionRange;
        this.visionConeAngle = visionConeAngle;
    }

    public void SetUpTrait(Trait aiTrait)
    {
        this.aiTrait = aiTrait;
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

    public void StopMovement()
    {
        walkingPosition = Vector3.zero;
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

        bool playerHidden = playerMovemenetScript.ReturnIsHidden();


        if ((distPlayer <= visionRange - 15 && angleToPlayer <= visionConeAngle - 15) && !playerHidden && !playerBehindWall)
        {
            //stateManager.StartCoroutine(stateManager.PlayAudioOneShot(6, 8));
            //directorAI.PlayerFound(state.gameObject);
            stateManager.playerNoticed = true;
            stateManager.SwitchFLState(stateManager.alertParentState);
            stateManager.SwitchSLState(stateManager.aggressionDesicionState);
        }
        yield return null;
    }
}
