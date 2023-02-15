using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Foiliage : MonoBehaviour
{
    private GameObject Player;
    private Sc_Player_Movement playerMovementScript;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerMovementScript.IsHidden();
        }
    }
}
