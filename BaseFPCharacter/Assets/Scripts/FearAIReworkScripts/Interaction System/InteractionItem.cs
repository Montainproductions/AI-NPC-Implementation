using UnityEngine;
using System;
using TMPro;
using Unity.VisualScripting;

public class InteractionItem : MonoBehaviour
{
    public BuyableObject scriptableObjectToInteract;

    [SerializeField]
    private Gun gunScript;

    private bool beingLookedAt;
    string action = "";
    int pointCost = 0;

    public static Action<GameObject, int, bool> passingInteractibleToPlayer;

    public static Action<int> doorOpened;

    public static Action<bool, string, int> itemBeingLookedAt;

    public void Start()
    {
        BindEvent();

        beingLookedAt = false;
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
        if (!beingLookedAt) { return; }

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

    public void IsLookedAt(int givenID)
    {
        beingLookedAt = false;
        action = "";
        pointCost = 0;

        Debug.Log("Given Id: " + givenID);
        Debug.Log("Scriptable Object ID: " + scriptableObjectToInteract.objectID);

        if (scriptableObjectToInteract.objectID == givenID)
        {
            beingLookedAt = true;
            action = scriptableObjectToInteract.action;
            pointCost = scriptableObjectToInteract.pointsCost;
        }

        itemBeingLookedAt?.Invoke(beingLookedAt, action, pointCost);
    }

    public void OnDestroy()
    {
        UnbindEvent();
    }
}
