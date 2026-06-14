using UnityEngine;
using System;

public class InteractionItem : MonoBehaviour
{
    [SerializeField]
    private GameObject pickUpUI;

    [SerializeField]
    private Gun gunScript;

    private bool beingLookedAt;

    public static Action<GameObject> passingInteractibleToPlayer;

    [SerializeField]
    private ItemCanvasFollowPlayerCamera canvasRotation;

    public void Start()
    {
        BindToInteractions();
        beingLookedAt = false;
        pickUpUI.SetActive(beingLookedAt);
    }

    public void BindToInteractions()
    {
        InteractionSystem.onLook += IsLookedAt;
        InteractionSystem.interactingWithInteractible += InteractOnItem;
    }

    public void UnbindToInteraction()
    {
        InteractionSystem.onLook -= IsLookedAt;
        InteractionSystem.interactingWithInteractible -= InteractOnItem;
    }

    public void InteractOnItem()
    {
        if (!beingLookedAt){ return; }

        if (gunScript != null){ passingInteractibleToPlayer?.Invoke(gameObject); }
    }

    public void IsLookedAt(string givenName, GameObject newCamera)
    {
        if (gameObject.name == givenName)
        {
            //Debug.Log("Item being looked at");
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
        pickUpUI.SetActive(beingLookedAt);
    }
}
