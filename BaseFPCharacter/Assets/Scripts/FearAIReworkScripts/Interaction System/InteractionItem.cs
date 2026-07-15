using UnityEngine;
using System;
using TMPro;
using Unity.VisualScripting;

public class InteractionItem : MonoBehaviour
{
    public BuyableObject scriptableObjectToInteract;

    [SerializeField]
    private Gun gunScript;

    private bool beingLookedAt, specificItemBeingLookedAt;
    string action = "";
    int pointCost = 0;

    public bool toBeDelete = false;

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
        if (!specificItemBeingLookedAt) { return; }

        if (scriptableObjectToInteract == null) { Debug.Log("No object Info"); return; }

        if (scriptableObjectToInteract.pointsCost >  playerPoints) { Debug.Log("Costs more then what the player has"); return; }

        specificItemBeingLookedAt = false;
        beingLookedAt = false;
        action = "";
        pointCost = 0;

        itemBeingLookedAt?.Invoke(beingLookedAt, action, pointCost);

        bool pickupable = false;

        if (gunScript == null){ pickupable = true; }

        if (scriptableObjectToInteract.action == "Buyable Door") {
            doorOpened?.Invoke(scriptableObjectToInteract.commonId);
        }

        passingInteractibleToPlayer?.Invoke(gameObject, scriptableObjectToInteract.pointsCost, pickupable);
    }

    public void IsLookedAt(int givenID)
    {
        if (givenID != -1)
        {
            beingLookedAt = true;
            if (scriptableObjectToInteract.objectID == givenID)
            {
                specificItemBeingLookedAt = true;
                action = scriptableObjectToInteract.action;
                pointCost = scriptableObjectToInteract.pointsCost;
            }
        }else
        {
                beingLookedAt = false;
                action = "";
                pointCost = 0;
        }

            itemBeingLookedAt?.Invoke(beingLookedAt, action, pointCost);
    }

    public void OnDestroy()
    {
        UnbindEvent();
    }
}
