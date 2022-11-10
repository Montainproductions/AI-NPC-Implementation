using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Attacking : MonoBehaviour{
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
    private float lastAttackTimer;
    private bool attacking, hitTarget;

    //Shooting
    [SerializeField]
    [Tooltip("Is shooting allowed?")]
    private bool canFireGunAttack;
    private int totalAmmo, maxAmmoInClip, ammoInClip;

    public void Awake(){
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Attacking.performed += Attacking_performed;
    }

    // Start is called before the first frame update
    void Start(){
        hitTarget = false;
        ammoInClip = maxAmmoInClip;
    }

    // Update is called once per frame
    void Update(){
        var lookPos = (Vector3.up * Sc_Player_Camera.Instance.mouseX) - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

        if (canMeleeAttack){
            if(attacking){Sc_Basic_UI.Instance.CantAttackUI();}
            else{Sc_Basic_UI.Instance.CanAttackUI();}
            if(lastAttackTimer >= 0.0f){lastAttackTimer -= Time.deltaTime;}
            if(lastAttackTimer <= 2.0f){attacking = false;}
            //Debug.Log(attacking);
        }

        if (canFireGunAttack){

        }
    }

    public void OnTriggerStay(Collider collision){
        if ((collision.tag == "Enemy") && attacking){
            if (canMeleeAttack){
                collision.GetComponent<Sc_Health>().TakeDamage(meleeDamage);
            }
        }
    }

    private void Attacking_performed(InputAction.CallbackContext context){
        if(!context.performed || lastAttackTimer > 0.0f) return;
        lastAttackTimer = 4.0f;
        attacking = true;
    }
}
