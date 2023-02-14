using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//A set of methods that multiple states use. Helps so that there arent duplicates of methods in diffrent states when one in a central location works best.
//Mostly for movement and the redeciding timer.
public class Sc_CommonMethods : MonoBehaviour
{
    [SerializeField]
    private Sc_AIStateManager stateManager;
    //Sc_AIStateManager state

    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private string currentState;

    [SerializeField]
    private GameObject self, player;

    private Vector3 walkingPosition;

    [SerializeField]
    private float decisionTimer;

    private bool lookingAtPlayer;

    public void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(walkingPosition != Vector3.zero) {
            if (Vector3.Distance(self.transform.position, walkingPosition) > 0.4f)
            {
                //Debug.Log("Going to Cover");
                stateManager.SetCurrentAction("Going to " + currentState + " point");
                if (!lookingAtPlayer)
                {
                    stateManager.transform.LookAt(walkingPosition);
                }
                else
                {
                    Vector3 playerPos = player.transform.position;
                    stateManager.transform.LookAt(playerPos);
                }
                navMeshAgent.destination = walkingPosition;
            }
            else
            {
                //Debug.Log("At Cover");
                //self.transform.localScale = new Vector3(1,0.75f,1);
                walkingPosition = Vector3.zero;
                if (currentState == "Cover")
                {
                    StartCoroutine(stateManager.coverState.AtCover(stateManager));
                }
                else if (currentState == "Attack")
                {
                    stateManager
                }
            }
        }
    }



    public void StartMovement(Vector3 position, string currentState, bool lookAtPlayer = false)
    {
        walkingPosition = position;
        this.currentState = currentState;
        lookingAtPlayer = lookAtPlayer;
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
        stateManager.SwitchState(stateManager.aggressionDesicionState);
        yield return null;
    }
    
    public IEnumerator AttackingGettingCloser(float diffDistToAttack)
    {
        float zDistance = Random.Range(diffDistToAttack + 1, diffDistToAttack + 6);
        //Debug.Log(zDistance);
        //float yDistance = Random.Range(-diffDistToAttack, diffDistToAttack);
        Vector3 newPosition = stateManager.transform.position + stateManager.transform.forward * zDistance;
        StartMovement(newPosition, "Approching", true);
        yield return null;
    }
}
