using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

/// <summary>
/// This script is attached to each individual empty Game Object that are around the cover object. Each of the points are meant to symbolize diffrent cover position that the AI can go to the point and use for cover.
/// </summary>
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

    //This method checks if the point is being used by another AI NPC and then with a raycast towards the player it will check if its behind the cover object. The method will then return wether the point is behind cover.
    public bool IsBehindCover()
    {
        if (beingUsed) {
            behindCover = false;
        }
        else
        {
            Vector3 direction = player.transform.position - gameObject.transform.position;
            if (Physics.Raycast(gameObject.transform.position, direction, out hit, 10, layerMask))
            {
                //Debug.DrawRay(gameObject.transform.position, direction * hit.distance, Color.red);
                //Debug.Log("Object Hit, Can be used for cover");*/

                behindCover = true;
            }
            else
            {
                //Debug.DrawRay(gameObject.transform.position, direction * 1000, Color.white);
                //Debug.Log("Did not Hit");*/

                behindCover = false;
            }
        }
        return behindCover;
    }
}
