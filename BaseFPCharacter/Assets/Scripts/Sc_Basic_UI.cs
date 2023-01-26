using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Basic_UI : MonoBehaviour{
    //Singleton
    public static Sc_Basic_UI Instance { get; private set; }

    [SerializeField]
    [Tooltip("Wether the player can do a melee attack or not.")]
    private GameObject canAttack, cantAttack;

    public void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start(){
        CanAttackUI();
    }

    // Update is called once per frame
    void Update(){
        
    }

    //Activates the green square to signify that the player can melee
    public void CanAttackUI(){
        //Debug.Log("Can attack");
        canAttack.SetActive(true);
        cantAttack.SetActive(false);
    }

    //Activates red square to signify that the player can not melee
    public void CantAttackUI(){
        //Debug.Log("Cant attack");
        canAttack.SetActive(false);
        cantAttack.SetActive(true);
    }
}
