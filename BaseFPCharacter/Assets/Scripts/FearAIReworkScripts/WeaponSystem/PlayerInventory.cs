using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    [SerializeField]
    private int currentPoints;

    [SerializeField]
    private Transform handPosition, secondaryWeaponPosition;

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
        playerInputActions.Player.SwitchWeapons.performed += ChangeWeaponAction;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentItemsPosition = 0;

        Basic_UI.Instance.SetPoints(currentPoints);

        BindEvents();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMainHands[currentItemsPosition] == null) { return;  }

        if (currentItemsPosition == 0) {
            if (currentMainHands[1] == null) { UpdateWeaponPositon(0, -1); }
            else { UpdateWeaponPositon(0, 1); }
        }
        else{
            if (currentMainHands[0] == null) { UpdateWeaponPositon(1, -1); }
            else { UpdateWeaponPositon(1, 0); } 
        }
    }

    public void UpdateWeaponPositon(int currentItemPosition, int otherItemPosition)
    {
        currentMainHands[currentItemPosition].transform.position = handPosition.position;
        currentMainHands[currentItemPosition].transform.rotation = handPosition.rotation;

        if (otherItemPosition == -1)
        {
            return;
        }

        currentMainHands[otherItemPosition].transform.position = secondaryWeaponPosition.position;
        currentMainHands[otherItemPosition].transform.rotation = secondaryWeaponPosition.rotation;
    }

    public void BindEvents()
    {
        InteractionItem.passingInteractibleToPlayer += AquireItem;
        NonPlayerCharacter.onKilled += EliminationPoints;
    }

    public void UnbindEvent()
    {
        InteractionItem.passingInteractibleToPlayer -= AquireItem;
        NonPlayerCharacter.onKilled -= EliminationPoints;
    }

    public void AquireItem(GameObject itemBeingAquired, int pointsCost, bool pickupable)
    {
        if (itemBeingAquired == null)
        {
            Debug.Log("Weapon Inventory ERROR: weapon given is null");
        }

        currentPoints -= pointsCost;

        if (!pickupable){ return;  }
        
        if (currentMainHands[0] == null) { currentItemsPosition = 0; }
        else if (currentMainHands[1] == null) { currentItemsPosition = 1; }

        currentMainHands[currentItemsPosition] = itemBeingAquired;
        currentMainHands[currentItemsPosition].transform.position = handPosition.position;

        if (currentMainHands[currentItemsPosition].GetComponent<Gun>())
        {
            currentGunScript = currentMainHands[currentItemsPosition].GetComponent<Gun>();
            currentGunScript.IsBeingHeld(characterData.fullTagList[1], true);
        }
    }

    public void DropItem()
    {
        currentMainHands[currentItemsPosition] = null;
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
    public void ChangeWeaponAction(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        currentMainHands[currentItemsPosition].transform.position = secondaryWeaponPosition.position;

        if (currentItemsPosition == 0)
        {
            if (currentMainHands[1] == null) { return; }

            currentItemsPosition = 1;
        }
        else
        {
            if (currentMainHands[0] == null) { return; }

            currentItemsPosition = 0;
        }

        currentGunScript.NoLongerBeingHeld();

        currentMainHands[currentItemsPosition].transform.position = handPosition.position;

        if (currentMainHands[currentItemsPosition].GetComponent<Gun>())
        {
            currentGunScript = currentMainHands[currentItemsPosition].GetComponent<Gun>();
            currentGunScript.IsBeingHeld(characterData.fullTagList[1], true);
        }
    }

    public void EliminationPoints(int points)
    {
        currentPoints += points;
        Basic_UI.Instance.SetPoints(currentPoints);
    }

    public int GetPlayerPoints()
    {
        return currentPoints;
    }

    public void OnDestroy()
    {
        playerInputActions.Player.Primary.performed -= PrimaryAction;
        playerInputActions.Player.Secondary.performed -= SecondaryAction;
        playerInputActions.Player.Reload.performed -= ReloadAction;
        playerInputActions.Player.Inspecting.performed -= InspectWeapon;
        playerInputActions.Player.SwitchWeapons.performed -= ChangeWeaponAction;
        UnbindEvent();
    }
}
