using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    [SerializeField]
    private GameObject handPosition;

    private GameObject[] currentMainHands = new GameObject[2];
    private int currentItemsPosition;

    private Gun currentGunScript;

    private int primaryUtilityAmount, secondaryUtilityAmount;

    public static Action<GunInfo> reloadEvent;

    [SerializeField]
    private CharacterData characterData;

    public void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Primary.performed += PrimaryAction;
        playerInputActions.Player.Secondary.performed += SecondaryAction;
        playerInputActions.Player.Reload.performed += ReloadAction;
        playerInputActions.Player.Inspecting.performed += InspectWeapon;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentItemsPosition = 0;

        BindEvents();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMainHands[currentItemsPosition] != null)
        {
            currentMainHands[currentItemsPosition].transform.position = handPosition.transform.position;
            currentMainHands[currentItemsPosition].transform.rotation = handPosition.transform.rotation;
        }
    }

    public void BindEvents()
    {
        InteractionItem.passingInteractibleToPlayer += AquireItem;
    }

    public void UnbindEvent()
    {
        InteractionItem.passingInteractibleToPlayer -= AquireItem;
    }

    public void AquireItem(GameObject itemBeingAquired)
    {
        if (itemBeingAquired == null)
        {
            Debug.Log("Weapon Inventory ERROR: weapon given is null");
        }

        if(currentMainHands[0] == null) { currentItemsPosition = 0; }
        else if (currentMainHands[1] == null) { currentItemsPosition = 1; }

        currentMainHands[currentItemsPosition] = itemBeingAquired;
        currentMainHands[currentItemsPosition].transform.position = handPosition.transform.position;

        if (currentMainHands[currentItemsPosition].GetComponent<Gun>())
        {
            currentGunScript = currentMainHands[currentItemsPosition].GetComponent<Gun>();
            currentGunScript.IsBeingHeld(characterData.fullTagList[0], true);
        }
    }

    public void DropItem()
    {
        currentMainHands[currentItemsPosition] = null;
    }

    public void ChangeHandItem()
    {
        currentItemsPosition++;
        if (currentItemsPosition >= 1)
        {
            currentItemsPosition = 0;
        }
    }

    public void SetItemActive(int newWeaponPosition)
    {

    }

    public void PrimaryAction(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (currentMainHands[currentItemsPosition] == null) { return; }
        if (currentGunScript == null) { return; }
        
        currentGunScript.PrimaryAction();
    }

    public void SecondaryAction(InputAction.CallbackContext context)
    {

    }

    public void ReloadAction(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (currentMainHands[currentItemsPosition] == null) { return; }
        if (currentGunScript == null){ return; }
        
        reloadEvent?.Invoke(currentGunScript.GetGunInfo());
    }

    public void InspectWeapon(InputAction.CallbackContext context)
    {

    }

    public void OnDestroy()
    {
        playerInputActions.Player.Primary.performed -= PrimaryAction;
        playerInputActions.Player.Secondary.performed -= SecondaryAction;
        playerInputActions.Player.Reload.performed -= ReloadAction;
        playerInputActions.Player.Inspecting.performed -= InspectWeapon;
        UnbindEvent();
    }
}
