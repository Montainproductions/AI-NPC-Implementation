using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Sc_Player_Movement : MonoBehaviour{
    //The player input system
    private PlayerInputActions playerInputActions;
    //Sets up the singleton so that the player can be called by some other scripts without needing to create a new variable for it.
    public static Sc_Player_Movement Instance { get; private set; }

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
    [HideInInspector]
    public Vector3 desiredVelocity; //Vec3 variable for the speed of character (Is public so the head bobbing script can grab the variable
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
    private bool isGrounded, jumping; //Bools for if the player is touching the ground or if its jumping

    [SerializeField]
    [Tooltip("Is crouching allowed?")]
    private bool canCrouch; //Is the player allowed to crouch
    private bool isCrouching; //Is the character currently crouching

    private bool isHidden;

    //HeadBobbing
    //https://sharpcoderblog.com/blog/head-bobbing-effect-in-unity-3d

    //Sets up the singleton, player input and the rb component
    public void Awake(){
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_Performed;
        playerInputActions.Player.Crouch.performed += Crouch_performed;
        playerInputActions.Player.Crouch.canceled += Crouch_performed;
        playerInputActions.Player.Restart.performed += gameRestarted_performed;

        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start(){
        //Jumping
        groundDistance = 0.1f;
        jumping = false;

        //Hiding
        isHidden = false;
    }

    // Update is called once per frame
    void Update(){Movement();}

    //The main movement script in charge of allowing the player to move around, jumping and crouching
    public void Movement(){
        //Recives the values from the player input system to determine movement of character
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector = Vector2.ClampMagnitude(inputVector, 1f);

        //Checks if the player is touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log(isGrounded);
        //Debug.Log(jumping);
        //Debug.Log(transform.up);

        //Jumping method
        Jump();

        //Crouching method
        Crouching();

        desiredVelocity = new Vector3(inputVector.x, 0f, inputVector.y) * maxSpeed; //Max speed of character
        float maxSpeedChange = acceleration * Time.deltaTime; //How much the current speed changes over time
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange); //Slowly increase current velocity in the x direction untill max is reached
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange); //Slowly increase current velocity in the z direction untill max is reached
        Vector3 displacement = velocity * Time.deltaTime; //Distance the player will be moved by
        //transform.position += displacement;

        movement = transform.right * displacement.x + transform.up * displacement.y + transform.forward * displacement.z; //Distance moved depending on world vectors
        transform.position += movement; //Move character
    }

    public void Jump(){
        //If the player is allowed to jump, is currently jumping and is grounded then it will jump
        if(!canJump) return;
        if(!jumping) return;

        //If the player is touching the ground then add force to the player
        if (isGrounded){
            //Debug.Log("Jumping high");
            rb.AddForce(Vector3.up * jumpingPower,ForceMode.Impulse);
        }

        jumping = false;
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

    public void IsHidden()
    {
        isHidden = true;
    }

    public void NotHidden()
    {
        isHidden = false;
    }

    public bool ReturnIsHidden()
    {
        return isHidden;
    }

    public bool ReturnIsCrouching()
    {
        return isCrouching;
    }

    //Input action for pressing space
    public void Jump_Performed(InputAction.CallbackContext context){
        if (!context.performed) return;
        //Debug.Log("Jumping");
        jumping = true;
    }

    //Input action for pressing left shift
    private void Crouch_performed(InputAction.CallbackContext context){
        if(context.performed){ //Start crouching
            //Debug.Log("Crouching");
            isCrouching = true;
        }else if(context.canceled){ //Stop crouching
            //Debug.Log("Standing up");
            isCrouching = false;
        }
    }

    private void gameRestarted_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(0);
        }
    }
}
