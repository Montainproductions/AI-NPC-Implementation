using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BoxShot : MonoBehaviour
{
    private bool hasBeenShot;

    public void Start()
    {
        hasBeenShot = false;
    }

    public bool ReturnShot()
    {
        return hasBeenShot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            hasBeenShot = true;
        }
    }
}
