using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Basic_UI : MonoBehaviour{
    //Singleton
    public static Basic_UI Instance { get; private set; }

    //The player input system
    private PlayerInputActions playerInputActions;

    [SerializeField]
    private bool inMenu;
    [SerializeField]
    private GameObject mainMenu, mainGame, pausedMenu;
    private bool pauseActive;

    [SerializeField]
    [Tooltip("Wether the player can do a melee attack or not.")]
    private GameObject canAttack, cantAttack;

    [SerializeField]
    private TextMeshProUGUI pointsTextUI;

    [SerializeField]
    private TextMeshProUGUI currentAmmoInClipUIText;

    [SerializeField]
    private GameObject holdFText;

    private string textOfInteractible = "";

    public void Awake(){
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Escape.performed += Escape_performed;
    }

    // Start is called before the first frame update
    void Start(){
        pauseActive = false;
        pausedMenu.SetActive(pauseActive);
        if (!inMenu)
        {
            mainMenu.SetActive(false);
            mainGame.SetActive(true);
            CanAttackUI();
            ShowPickUpInfo(false,"",0);

            BindEvents();
        }
        else
        {
            mainMenu.SetActive(true);
            mainGame.SetActive(false);
        }
    }

    public void BindEvents()
    {
        Gun.ammoUsed += AmmoUsed;
        InteractionItem.itemBeingLookedAt += ShowPickUpInfo;
    }

    public void UnbindEvent()
    {
        Gun.ammoUsed -= AmmoUsed;
        InteractionItem.itemBeingLookedAt -= ShowPickUpInfo;
    }

    public void AmmoUsed(int currentAmmoInClip, int currentAmountOfClips)
    {
        if (!inMenu) {
            SetCurrentAmmo(currentAmmoInClip, currentAmountOfClips);
        }
    }

    public void SetPoints(int points) {
        pointsTextUI.SetText(points.ToString());
    }

    public void SetCurrentAmmo(float currentAmmoInsideClip, float amountOfClipsLeft)
    {
        currentAmmoInClipUIText.SetText(currentAmmoInsideClip.ToString() + "/" + amountOfClipsLeft.ToString());
    }

    //Activates the green square to signify that the player can melee
    public void CanAttackUI(){
        canAttack.SetActive(true);
        cantAttack.SetActive(false);
    }

    //Activates red square to signify that the player can not melee
    public void CantAttackUI(){
        canAttack.SetActive(false);
        cantAttack.SetActive(true);
    }

    public void ShowPickUpInfo(bool beingLookedAt, string action, int pointCost)
    {

        if (beingLookedAt)
        {

            if (action == "Buyable Door") { textOfInteractible = "Press F to open door. [" + pointCost + "]"; }
            else if (action == "Pickupable") { textOfInteractible = "Press F to pick up."; }
            else if (action == "MysteryBox") { textOfInteractible = "Press F to role box roullete. [" + pointCost + "]"; }
        }

        holdFText.GetComponentInChildren<TextMeshProUGUI>().text = textOfInteractible;
        holdFText.SetActive(beingLookedAt);
    }

    private void Escape_performed(InputAction.CallbackContext context)
    {
        if (!inMenu && context.performed)
        {
            pauseActive = !pauseActive;

            pausedMenu.SetActive(pauseActive);
        }
    }
}
