using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Player_Camera : MonoBehaviour{
    public static Sc_Player_Camera Instance{get; private set;}

    private PlayerInputActions playerInputActions;

    private bool inUI;

    //Camera
    public Camera playerCamera;
    public float mouseSensativity, mouseX, mouseY;
    public Transform playerBody;
    public float xRotation = 0f;

    public void Awake(){
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Start is called before the first frame update
    void Start(){inUI = false;}

    // Update is called once per frame
    void Update(){
        if(inUI){Cursor.lockState = CursorLockMode.None;}
        else{Cursor.lockState = CursorLockMode.Locked;}
        Camera();
    }

    public void UIMouse(){inUI = !inUI;}

    public void Camera(){
        float horizontal = playerInputActions.Player.Mouse.ReadValue<Vector2>().x;
        float vertical = playerInputActions.Player.Mouse.ReadValue<Vector2>().y;

        float mouseX = horizontal * mouseSensativity * Time.deltaTime;
        float mouseY = vertical * mouseSensativity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
