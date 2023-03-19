using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Attacking : MonoBehaviour{
    //The player input system
    private PlayerInputActions playerInputActions;
    [SerializeField]
    private bool attackForPlayer;
    [SerializeField]
    private GameObject playerBox;

    //Melee
    [SerializeField]
    [Tooltip("Is melee allowed?")]
    private bool canMeleeAttack;
    [SerializeField]
    [Tooltip("Damping for box collider to follow camera and know when an object is close by.")]
    [Range(0,100)]
    private float damping;
    [SerializeField]
    [Tooltip("Amount of damage the melee object will do.")]
    [Range(0, 100)]
    private float meleeDamage;
    [SerializeField]
    private float attackTimer;

    //Shooting
    [SerializeField]
    [Tooltip("Is shooting allowed")]
    private bool canShootAttack;
    [SerializeField]
    [Tooltip("Current Weapon Object")]
    private GameObject currentGun;
    private Sc_BaseGun baseGunScript;

    //Sets up the player input system for later
    public void Awake(){
        if (attackForPlayer)
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
            playerInputActions.Player.Attacking.performed += Attacking_performed;
            playerInputActions.Player.Reload.performed += Reload_preformed;
        }
    }

    public void Start()
    {
        baseGunScript = currentGun.GetComponent<Sc_BaseGun>();
    }

    // Update is called once per frame
    void Update(){
        if (attackForPlayer) {
            if (Input.GetMouseButton(0)) {
                StartCoroutine(Attacking());
            }
            PlayerAttackBox();
        }
    }

    public void PlayerAttackBox()
    {
        var lookPos = (Vector3.up * Sc_Player_Camera.Instance.mouseX) - playerBox.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        playerBox.transform.rotation = Quaternion.Slerp(playerBox.transform.rotation, rotation, Time.deltaTime * damping);
    }

    public IEnumerator Attacking(){
        if (canMeleeAttack)
        {
            Sc_Basic_UI.Instance.CantAttackUI();
            yield return new WaitForSeconds(attackTimer);
            Sc_Basic_UI.Instance.CanAttackUI();
            yield return null;
        }
        else if (canShootAttack && !baseGunScript.shotRecently)
        {
            //Debug.Log("Attacking");
            StartCoroutine(baseGunScript.ShotFired());
        }
        yield return null;
    }

    //If the attack button is pressed (Left mouse button currently) then flip some values to allow for damage to take place
    private void Attacking_performed(InputAction.CallbackContext context){
        //Debug.Log(context.ReadValue<float>());
        if(!context.performed && attackForPlayer) return;
        
        //StartCoroutine(Attacking());
    }

    private void Reload_preformed(InputAction.CallbackContext context)
    {
        if (!context.performed && attackForPlayer) return;

        StartCoroutine(baseGunScript.Reloading());
    }

    //Checks if the enemy is hiting the collidor and then deal damage
    public void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            if (canMeleeAttack)
            {
                collision.GetComponent<Sc_Health>().TakeDamage(meleeDamage);
            }
        }
    }

    public void OnDestroy()
    {
        if (attackForPlayer)
        {
            playerInputActions.Player.Attacking.performed -= Attacking_performed;
            playerInputActions.Player.Reload.performed -= Reload_preformed;
        }
    }
}
