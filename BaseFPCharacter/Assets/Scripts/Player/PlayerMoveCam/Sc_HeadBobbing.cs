using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_HeadBobbing : MonoBehaviour{
    //https://sharpcoderblog.com/blog/head-bobbing-effect-in-unity-3d

    [SerializeField]
    [Tooltip("Will the camerea bob when walking?")]
    private bool canBob;
    [SerializeField]
    [Tooltip("Height power that will control how high the character can jump.")]
    [Range(0f, 100f)]
    private float walkingBobbingSpeed = 14f;
    [SerializeField]
    [Tooltip("Amount the camera will bob when walking.")]
    [Range(0f, 1f)]
    public float bobbingAmount = 0.05f;
    public Sc_Player_Movement controller;
    float defaultPosY = 0; //Current position of camera
    float timer = 0; //Timer for camera restart

    // Start is called before the first frame update
    void Start(){
        defaultPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update(){
        if(!canBob) return; //If the player isnt allowed to have a bobbing head then stop
        //Some fancy sine movment for the camera so it is moving up and down at a constant rate
        if(Mathf.Abs(controller.desiredVelocity.x) > 0.1f || Mathf.Abs(controller.desiredVelocity.z) > 0.1f){
            //Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }else{
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), transform.localPosition.z);
        }
    }
}
