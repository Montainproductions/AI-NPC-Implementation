using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    private GameObject self;

    private Vector3 walkingPosition;
    private Vector3 direction;

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
        if (walkingPosition != Vector3.zero) {
            //Debug.Log(Vector3.Distance(self.transform.position, walkingPosition));
            if (Vector3.Distance(self.transform.position, walkingPosition) > 1.1f)
            {
                //Debug.Log("Moving");
                stateManager.SetCurrentAction("Going to " + currentState + " point");
                
                direction = (walkingPosition - transform.position).normalized;
                //Creates Quaternion version of the vector3 direction
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                //Rotate Enemy over time according to speed until we are in the required rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 0.25f);
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
