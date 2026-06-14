using UnityEngine;

public class ItemCanvasFollowPlayerCamera : MonoBehaviour
{
    private GameObject playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
