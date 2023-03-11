using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Foiliage : MonoBehaviour
{
    [SerializeField]
    private Sc_Player_Movement playerMovementScript;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && playerMovementScript.ReturnIsCrouching())
        {
            playerMovementScript.IsHidden();
        }
    }
}
