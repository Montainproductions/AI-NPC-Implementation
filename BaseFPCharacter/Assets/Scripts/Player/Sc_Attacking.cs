using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject enemyAttacked;

    //Shooting
    [SerializeField]
    [Tooltip("Is shooting allowed?")]
    private bool canFireGunAttack;

    public void Awake(){
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Attacking.performed += Attacking_performed;
    }

    // Start is called before the first frame update
    void Start(){
        hitTarget = false;
    }

    // Update is called once per frame
    void Update(){
        if(!canMeleeAttack) return;

        if(attacking){lastAttackTimer -= Time.deltaTime;}
        if(lastAttackTimer <= 0){attacking = false;}
        
        var lookPos = (Vector3.up * Sc_Player_Camera.Instance.mouseX) - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

        //if (hitTarget){enemyAttacked.GetComponent<Sc_Health>().TakeDamage(meleeDamage);}

        if (!canFireGunAttack) return;
    }

    public void OnCollisionEnter(Collider collision){
        if(collision.tag == "Enemy"){
            if (attacking){
                Debug.Log("Hit Enemy");
                collision.gameObject.GetComponent<Sc_Health>().TakeDamage(meleeDamage);
            }
        }
    }

    private void Attacking_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        attacking = true;
    }
}
