using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AITraits : MonoBehaviour
{
    public AudioClip[] AudioClip = new AudioClip[132];

    public string behaviour;
    public float agressionValueChange, approchPlayerChange;
    //public AudioClip[] audioclips = new AudioClip[33];

    public void Start()
    {


        //Setting up agression trait
        behaviour = "Agression";
        agressionValueChange = 3;
        approchPlayerChange = 2;

        //Setting up bold trait
        behaviour = "Bold";
        agressionValueChange = 1.5f;
        approchPlayerChange = 0.5f;

        //Setting up cautious trait
        behaviour = "Cautious";
        agressionValueChange = -1.5f;
        approchPlayerChange = -0.5f;

        //Setting up scared trait
        behaviour = "Scared";
        agressionValueChange = -3;
        approchPlayerChange = -2;
    }
}
