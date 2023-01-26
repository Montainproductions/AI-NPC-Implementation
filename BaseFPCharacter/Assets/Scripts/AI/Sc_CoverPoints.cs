using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Sc_CoverPoints : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    // Bit shift the index of the layer (8) to get a bit mask
    private int layerMask = 1 << 7;

    private RaycastHit hit;

    [HideInInspector]
    public bool behindCover, beingUsed;

    public void Start()
    {
        beingUsed = false;
    }

    public void FixedUpdate()
    {
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.TransformDirection(player.transform.position), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.TransformDirection(player.transform.position) * hit.distance, Color.red);
            Debug.Log("Object Hit, Can be used for cover");

            behindCover = true;
        }
        else
        {
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.TransformDirection(player.transform.position) * 1000, Color.white);
            Debug.Log("Did not Hit");

            behindCover = false;
        }
    }
}
