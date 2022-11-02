using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Player_Movement : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    public float speed;
    public CharacterController controller;
    public Rigidbody rb;

    //Ground check shenanigens
    private float gravity;
    private Vector3 velocity;
    public Transform groundCheck;
    private float groundDistance;
    public LayerMask groundMask;
    private bool isGrounded;

    public void Awake(){
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_Performed;
    }

    // Start is called before the first frame update
    void Start(){
        gravity = -9.81f;
        groundDistance = 0.4f;
    }

    // Update is called once per frame
    void Update(){Movement();}

    public void Movement(){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log(isGrounded);

        if (isGrounded && velocity.y < 0){velocity.y = -1;}

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;

        controller.Move(movement * Time.deltaTime * speed);
    }

    public void Jump_Performed(InputAction.CallbackContext context){
        if (!context.performed) return;

        rb.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
    }
}
