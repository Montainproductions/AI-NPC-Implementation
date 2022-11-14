using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Attacking : MonoBehaviour{
    //The player input system
    private PlayerInputActions playerInputActions;

    //Melee
    [SerializeField]
    [Tooltip("Is melee attacking allowed?")]
    private bool canMeleeAttack;
    [SerializeField]
    [Tooltip("Damping for box collider to follow camera and know when an object is close by.")]
    [Range(0,100)]
    private float damping;
    [SerializeField]
    [Tooltip("Amount of damage the melee object will do.")]
    [Range(0, 100)]
    private float meleeDamage;
    private float lastAttackTimer; //A timer since the player hit the attack button
    private bool attacking; //If the player is melee attacking

    //Shooting section go to base Gun scripts and any related gun scripts

    //Sets up the player input system for later
    public void Awake(){
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Attacking.performed += Attacking_performed;
    }

    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update(){
        //Moves the box collider that checks the area where the player would be able to damage any enemies. Its not the smothes but it works for what I need it to do
        var lookPos = (Vector3.up * Sc_Player_Camera.Instance.mouseX) - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

        if (canMeleeAttack){
            if(attacking){Sc_Basic_UI.Instance.CantAttackUI();}//UI element to show if the player can or cannot attack so a debug log isnt necesary anymore
            else{Sc_Basic_UI.Instance.CanAttackUI();}
            if(lastAttackTimer >= 0.0f){lastAttackTimer -= Time.deltaTime;} //Reduce the time of the last time attack so the player can attack again
            if(lastAttackTimer <= 2.0f){attacking = false;} //Cant damage the player anymore meant to simulate the weapon being drawn back
            //Debug.Log(attacking);
        }
    }

    //Checks if the enemy is hiting the collidor and then deal damage
    public void OnTriggerStay(Collider collision){
        if ((collision.tag == "Enemy") && attacking){
            if (canMeleeAttack){
                collision.GetComponent<Sc_Health>().TakeDamage(meleeDamage);
            }
        }
    }

    //If the attack button is pressed (Left mouse button currently) then flip some values to allow for damage to take place
    private void Attacking_performed(InputAction.CallbackContext context){
        if(!context.performed || lastAttackTimer > 0.0f) return;
        lastAttackTimer = 4.0f;
        attacking = true;
    }
}
