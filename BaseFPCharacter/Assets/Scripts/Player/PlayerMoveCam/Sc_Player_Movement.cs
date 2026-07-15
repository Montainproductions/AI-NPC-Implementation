using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Player_Movement : MonoBehaviour{
    //Sets up the singleton so that the player can be called by some other scripts without needing to create a new variable for it.
    public static Sc_Player_Movement Instance { get; private set; }

    //The player input system
    private PlayerInputActions playerInputActions;

    //Main movement variables
    [SerializeField]
    [Tooltip("Max speed character can go at after it has fully accelerated.")]
    [Range(0f, 100f)]
    private float maxSpeed;
    [SerializeField]
    [Tooltip("How fast can the character speed up and reach its max speed.")]
    [Range(0f, 100f)]
    private float acceleration;
    private Vector3 movement, velocity; //Vec3 Variables for determining where and at what rate will the player be moving at.
    private Vector3 desiredVelocity; //Vec3 variable for the speed of character (Is public so the head bobbing script can grab the variable
    private Rigidbody rb; //RigidBody

    private Vector2 inputVector; //The input vectors 

    //Ground check shenanigens
    [SerializeField]
    [Tooltip("Is jumping allowed?")]
    private bool canJump;
    [SerializeField]
    [Tooltip("Height power that will control how high the character can jump.")]
    [Range(0f, 100f)]
    private float jumpingPower; //The power of the jump affecting how high they can go
    public Transform groundCheck; //The position of the ground check
    public LayerMask groundMask; //The layer of the ground that it will check
    private float groundDistance; //How close to the ground can the player get before stoping to fall
    private bool isGrounded; //Bools for if the player is touching the ground or if its jumping

    [SerializeField]
    [Tooltip("Is crouching allowed?")]
    private bool canCrouch; //Is the player allowed to crouch
    private bool isCrouching; //Is the character currently crouching

    private bool isHidden;

    private Vector3 displacement;

    //HeadBobbing
    //https://sharpcoderblog.com/blog/head-bobbing-effect-in-unity-3d

    //Sets up the singleton, player input and the rb component
    public void Awake(){
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Sprint.performed += Sprint_performed;
        playerInputActions.Player.Sprint.canceled += Sprint_performed;
        playerInputActions.Player.Jump.performed += Jump_Performed;
        playerInputActions.Player.Crouch.performed += Crouch_performed;
        playerInputActions.Player.Crouch.canceled += Crouch_performed;

        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start(){
        //Jumping
        groundDistance = 0.1f;

        isCrouching = false;
        //Hiding
        isHidden = false;
    }

    // Update is called once per frame
    void Update(){
        Movement();
    }

    private void FixedUpdate()
    {
        PhysicsMovment();
    }

    //The main movement script in charge of allowing the player to move around, jumping and crouching
    public void Movement()
    {
            inputVector = Vector2.ClampMagnitude(playerInputActions.Player.Movement.ReadValue<Vector2>(), 1f);
    }

    public void PhysicsMovment()
    {
        //Checks if the player is touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 worldInput = transform.right * inputVector.x + transform.forward * inputVector.y;
        Vector3 desiredVelocity = worldInput * maxSpeed;

        Vector3 velocity = rb.linearVelocity;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        rb.linearVelocity = velocity;
    }

    public void Jump(){
        //If the player is allowed to jump, is currently jumping and is grounded then it will jump
        if(!canJump) return;

        //If the player is touching the ground then add force to the player
        if (isGrounded){
            rb.AddForce(Vector3.up * jumpingPower,ForceMode.Impulse);
        }
    }

    public void Crouching(){
        //If the player is allowed to crouch then
        if (!canCrouch) return;
        
        if (isCrouching && transform.localScale.y > 0.5f){
            //Debug.Log("Crouching");
            transform.localScale -= new Vector3(0, 0.5f, 0); //Decrease the size of character to half size
        }else if (!isCrouching && transform.localScale.y < 1f){
            //Debug.Log("Standing up");
            transform.localScale += new Vector3(0, 0.5f, 0); //Increase the size of character to normal size
        }
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public bool ReturnIsHidden()
    {
        return isHidden;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Foilage" && isCrouching)
        {
            isHidden = true;
        }
        else
        {
            isHidden = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Foilage")
        {
            isHidden = false;
        }
    }

    private void Sprint_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            maxSpeed = maxSpeed * 1.7f;
            acceleration = acceleration * 1.45f;
        }
        else if (context.canceled)
        {
            maxSpeed = maxSpeed / 1.7f;
            acceleration = acceleration / 1.45f;
        }
    }

    //Input action for pressing space
    public void Jump_Performed(InputAction.CallbackContext context){
        if (!context.performed) return;
        Jump();
    }

    //Input action for pressing left shift
    private void Crouch_performed(InputAction.CallbackContext context){
        if(context.performed){ //Start crouching
            isCrouching = true;
            Crouching();
        }
        else if(context.canceled){ //Stop crouching
            isCrouching = false;
            Crouching();
        }
    }
}
