using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Player_Movement : MonoBehaviour{
    private PlayerInputActions playerInputActions;

    [SerializeField]
    [Tooltip("Max speed character can go at after it has fully accelerated.")]
    [Range(0f, 100f)]
    private float maxSpeed;
    [SerializeField]
    [Tooltip("How fast can the character speed up and reach its max speed.")]
    [Range(0f, 100f)]
    private float acceleration;
    private Vector3 desiredVelocity, velocity;
    public Rigidbody rb;

    private Vector2 inputVector;

    //Ground check shenanigens
    [SerializeField]
    [Tooltip("Height power that will control how high the character can jump.")]
    [Range(0f, 100f)]
    private float jumpingPower;
    public Transform groundCheck;
    public LayerMask groundMask;
    private float groundDistance;
    private bool isGrounded, jumping;

    //Gravity and jumping
    private Vector3 upAxis;

    public void Awake(){
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_Performed;
    }

    // Start is called before the first frame update
    void Start(){
        groundDistance = 0.1f;
        jumping = false;
    }

    public void FixedUpdate(){
        upAxis = -Physics.gravity.normalized;
    }

    // Update is called once per frame
    void Update(){Movement();}

    public void Movement(){
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector = Vector2.ClampMagnitude(inputVector, 1f);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log(isGrounded);
        //Debug.Log(jumping);
        //Debug.Log(transform.up);

        if (jumping){
            jumping = false;
            Jump();
        }

        desiredVelocity = new Vector3(inputVector.x, 0f, inputVector.y) * maxSpeed;
        float maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        Vector3 displacement = velocity * Time.deltaTime;
        //transform.position += displacement;

        Vector3 movement = transform.right * displacement.x + transform.up * displacement.y + transform.forward * displacement.z;
        transform.position += movement;
    }

    public void Jump(){
        if (isGrounded){
            //Debug.Log("Jumping high");
            rb.AddForce(Vector3.up * jumpingPower,ForceMode.Impulse);
        }
    }

    public void Jump_Performed(InputAction.CallbackContext context){
        if (!context.performed) return;
        Debug.Log("Jumping");
        jumping = true;
    }
}
