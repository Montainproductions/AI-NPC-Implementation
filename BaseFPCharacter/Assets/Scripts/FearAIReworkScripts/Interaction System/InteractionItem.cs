using UnityEngine;
using System;
using TMPro;

public class InteractionItem : MonoBehaviour
{
    public BuyableObject scriptableObjectToInteract;

    [SerializeField]
    private GameObject pickUpUI;

    [SerializeField]
    private Gun gunScript;

    private bool beingLookedAt;

    public static Action<GameObject, int, bool> passingInteractibleToPlayer;

    public static Action<int> doorOpened;

    [SerializeField]
    private ItemCanvasFollowPlayerCamera canvasRotation;

    public void Start()
    {
        BindEvent();
        beingLookedAt = false;
        pickUpUI.SetActive(beingLookedAt);
    }

    public void BindEvent()
    {
        InteractionSystem.onLook += IsLookedAt;
        InteractionSystem.interactingWithInteractible += InteractOnItem;
    }

    public void UnbindEvent()
    {
        InteractionSystem.onLook -= IsLookedAt;
        InteractionSystem.interactingWithInteractible -= InteractOnItem;
    }

    public void InteractOnItem(int playerPoints)
    {
        if (!beingLookedAt){ return; }

        if (scriptableObjectToInteract == null) { Debug.Log("No object Info"); return; }

        if (scriptableObjectToInteract.pointsCost >  playerPoints) { Debug.Log("Costs more then what the player has"); return; }

        bool pickupable = false;

        if (gunScript != null){ pickupable = false; }

        if (scriptableObjectToInteract.action == "BuyableDoor") { 
            pickupable = true;

            doorOpened?.Invoke(scriptableObjectToInteract.objectID);
        }

        passingInteractibleToPlayer?.Invoke(gameObject, scriptableObjectToInteract.pointsCost, pickupable);
    }

    public void IsLookedAt(string givenName, GameObject newCamera)
    {
        if (gameObject.name == givenName)
        {
            beingLookedAt = true;
            canvasRotation.SetCamera(newCamera);
        }
        else
        {
            beingLookedAt = false;
            canvasRotation.SetCamera(null);
        }

        ShowPickUpInfo();
    }

    public void ShowPickUpInfo()
    {
        pickUpUI.GetComponentInChildren<TextMeshProUGUI>().text = scriptableObjectToInteract.name;
        pickUpUI.SetActive(beingLookedAt);
    }

    public void OnDestroy()
    {
        UnbindEvent();
    }
}
