using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//A set of methods that multiple states use. Helps so that there arent duplicates of methods in diffrent states when one in a central location works best.
//Mostly for movement and the redeciding timer.
public class Sc_CommonMethods : MonoBehaviour
{
    public static Sc_CommonMethods Instance { get; private set; }

    private NavMeshAgent navMeshAgent;

    private Vector3 walkingPosition;

    [SerializeField]
    private float decisionTimer;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(walkingPosition != Vector3.zero) { 
        }
    }

    public void StartMovement(Vector3 position, bool lookAtPlayer = false)
    {
        walkingPosition = position;
    }

    public void StopMovement()
    {
        walkingPosition = Vector3.zero;
    }

    //Once the timer is finished the AI will return to the agression decision state and decide if its better to go to cover or continue attacking the player.
    public IEnumerator ReDecide(Sc_AIStateManager state)
    {
        float newDecisionTimer = Random.Range(decisionTimer - 5, decisionTimer + 5);
        yield return new WaitForSeconds(newDecisionTimer);
        state.SwitchState(state.aggressionDesicionState);
        yield return null;
    }
}
