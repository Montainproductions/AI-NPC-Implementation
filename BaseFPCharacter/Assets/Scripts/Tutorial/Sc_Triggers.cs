using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Triggers : MonoBehaviour
{
    private bool playerActivated;

    public void Start()
    {
        playerActivated = false;
    }

    public bool ReturnPlayerActivation()
    {
        return playerActivated;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player"){
            playerActivated = true;
        }
    }
}
