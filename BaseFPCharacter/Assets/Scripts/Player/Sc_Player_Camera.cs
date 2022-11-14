using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Player_Camera : MonoBehaviour{
    //Singleton
    public static Sc_Player_Camera Instance{get; private set;}

    //The new player input system
    private PlayerInputActions playerInputActions;

    private bool inUI;

    //Camera variab;es
    public Camera playerCamera;
    public float mouseSensativity, mouseX, mouseY;
    public Transform playerBody;
    public float xRotation = 0f;

    public void Awake(){
        //Setting up singleton
        Instance = this;

        //Sets up the player input system
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Start is called before the first frame update
    void Start(){inUI = false;}

    // Update is called once per frame
    void Update(){
        //Will lock or unlock the mouse if the player is in a pause menu or not
        if(inUI){Cursor.lockState = CursorLockMode.None;}
        else{Cursor.lockState = CursorLockMode.Locked;}

        //The camera so that the player can look around with the mouse
        Camera();
    }

    public void UIMouse(){inUI = !inUI;}

    //Method in charge of knowing how the mouse moves and also moves the camera respectivly
    public void Camera(){
        //Aquires the vector 2 values for the mouse current position
        float horizontal = playerInputActions.Player.Mouse.ReadValue<Vector2>().x;
        float vertical = playerInputActions.Player.Mouse.ReadValue<Vector2>().y;

        //The relative distance the camera will need to rotate
        mouseX = horizontal * mouseSensativity * Time.deltaTime;
        mouseY = vertical * mouseSensativity * Time.deltaTime;

        //Moving the camera in the y direction and clamping it so that it dosent go to far up or down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Moves the camera in the x direction in the correct direction
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
