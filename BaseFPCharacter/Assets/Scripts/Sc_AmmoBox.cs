using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AmmoBox : MonoBehaviour
{
    [SerializeField]
    private int ammoCount;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Sc_Attacking>().AddingAmmo(ammoCount);
            Destroy(gameObject);
        }
    }
}
