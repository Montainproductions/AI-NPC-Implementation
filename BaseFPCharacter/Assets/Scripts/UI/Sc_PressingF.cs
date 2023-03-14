using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Sc_PressingF : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private bool inArea, taskDone;

    public void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Action.performed += Action_performed;
    }

    // Start is called before the first frame update
    void Start()
    {
        inArea = false;
        taskDone = false;
    }

    public bool ReturnTaskDone()
    {
        return taskDone;
    }

    public void OnTriggerEnter(Collider other)
    {
        inArea = true;
    }

    public void OnTriggerExit(Collider other)
    {
        inArea = false;
    }

    private void Action_performed(InputAction.CallbackContext context)
    {
        if(inArea && context.performed)
        {
            taskDone = true;
        }
    }
}
