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
}
