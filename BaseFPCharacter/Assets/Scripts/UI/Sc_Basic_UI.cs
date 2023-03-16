using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Sc_Basic_UI : MonoBehaviour{
    //Singleton
    public static Sc_Basic_UI Instance { get; private set; }

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
    private GameObject healthInt;
    private TextMeshProUGUI healthTextUI;

    [SerializeField]
    private GameObject currentAmmo;
    private TextMeshProUGUI ammoTextUI;

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
            healthTextUI = healthInt.GetComponent<TextMeshProUGUI>();
            ammoTextUI = currentAmmo.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            mainMenu.SetActive(true);
            mainGame.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void NewHealth(float currentHealth) {
        healthTextUI.SetText(currentHealth.ToString());
    }

    public void SetCurrentAmmo(float currentAmmo, float maxCurrentAmmo)
    {
        //Debug.Log(currentAmmo + maxCurrentAmmo);
        ammoTextUI.SetText(currentAmmo.ToString() + "/" + maxCurrentAmmo.ToString());
    }

    //Activates the green square to signify that the player can melee
    public void CanAttackUI(){
        //Debug.Log("Can attack");
        canAttack.SetActive(true);
        cantAttack.SetActive(false);
    }

    //Activates red square to signify that the player can not melee
    public void CantAttackUI(){
        //Debug.Log("Cant attack");
        canAttack.SetActive(false);
        cantAttack.SetActive(true);
    }

    private void Escape_performed(InputAction.CallbackContext context)
    {
        if (!inMenu && context.performed)
        {
            pauseActive = !pauseActive;
            //Sc_Player_Camera.Instance.UIMouse();
            pausedMenu.SetActive(pauseActive);
        }
    }
}
