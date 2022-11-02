using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Player_Movement : MonoBehaviour{
    private PlayerInputActions playerInputActions;

    [SerializeField]
    [Range(0f, 100f)]
    private float maxSpeed;
    [SerializeField]
    [Range(0f, 100f)]
    private float acceleration;
    private Vector3 desiredVelocity, velocity;
    private float jumpingPower;
    //public Rigidbody rb;

    public Vector2 inputVector;

    //Ground check shenanigens
    private float gravity;
    private Vector3 jumpingVelocity;
    public Transform groundCheck;
    private float groundDistance;
    public LayerMask groundMask;
    private bool isGrounded, jumping;

    //Gravity and jumping
    public Vector3 upAxis;

    public void Awake(){
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_Performed;
    }

    // Start is called before the first frame update
    void Start(){
        groundDistance = 0.1f;
        jumping = false;
        isGrounded= true;
    }

    // Update is called once per frame
    void Update(){Movement();}

    public void FixedUpdate(){
        upAxis = -Physics.gravity.normalized;
    }

    public void Movement(){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log(isGrounded);

        /*if (isGrounded && velocity.y < 0){velocity.y = -1;}

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        */

        if (jumping && isGrounded){
            Debug.Log("Is Jumping");
            jumpingVelocity = new Vector3(0, 20f, 0);
            //controller.Move(jumpingVelocity * Time.deltaTime);
            //gameObject.transform.position += jumpingVelocity;
            jumping = false;
        }

        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector = Vector2.ClampMagnitude(inputVector, 1f);

        desiredVelocity = new Vector3(inputVector.x, 0f, inputVector.y) * maxSpeed;
        float maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        Vector3 displacement = velocity * Time.deltaTime;
        transform.localPosition += displacement;
    }

    public void Jump_Performed(InputAction.CallbackContext context){
        if (!context.performed) return;
        Debug.Log("Jumping");
        jumping = true;
    }
}
