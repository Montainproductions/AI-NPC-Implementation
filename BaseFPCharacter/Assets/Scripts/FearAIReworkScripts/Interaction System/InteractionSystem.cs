using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using static UnityEngine.UI.Image;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    RaycastHit raycastHit;
    LayerMask layerMask;

    [SerializeField]
    private GameObject playerCamera;

    [SerializeField]
    private float maxDistanceForInteraction;

    public static Action<int> interactingWithInteractible;
    public static Action<string, GameObject> onLook;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += InteractPerformed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layerMask = LayerMask.GetMask("Interactible");
        StartCoroutine(CheckInteractible());
    }

    IEnumerator CheckInteractible()
    {
        yield return new WaitForSeconds(0.25f);

        //Debug.Log("Searching");
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;
        if (Physics.Raycast(origin, direction, out RaycastHit mainHit, maxDistanceForInteraction, layerMask))
        {
            //Debug.DrawLine(origin, direction, Color.green);
            raycastHit = mainHit;

            InteractionItem interactibleScript = raycastHit.transform.gameObject.GetComponent<InteractionItem>();
            if (interactibleScript != null)
            {
                //Debug.Log("An item is being looked at");
                onLook?.Invoke(interactibleScript.name, playerCamera);
            }
        }
        else
        {
            //Debug.DrawRay(origin, direction * 20, Color.red);
            onLook?.Invoke("", null);
        }

            StartCoroutine(CheckInteractible());
    }

    private void InteractPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed){ return; }

        if (raycastHit.collider == null){ return; }

        interactingWithInteractible?.Invoke(PlayerInventory.Instance.GetPlayerPoints());
    }

    public void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= InteractPerformed;
    }
}
