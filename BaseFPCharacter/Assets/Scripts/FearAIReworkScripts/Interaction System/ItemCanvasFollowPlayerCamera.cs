using System;
using UnityEngine;

public class ItemCanvasFollowPlayerCamera : MonoBehaviour
{
    private GameObject playerCamera;

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerCamera != null)
        {
            // Forces the canvas rotation to perfectly mirror the camera's rotation
            transform.rotation = playerCamera.transform.rotation;
        }
    }

    public void SetCamera(GameObject newCamera)
    {
        playerCamera = newCamera;
    }
}
