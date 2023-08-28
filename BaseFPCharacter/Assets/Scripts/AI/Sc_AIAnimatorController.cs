using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Sc_AIAnimatorController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Sc_AIStateManager aiManager;
    [SerializeField]
    private Sc_AIStatesManagerHierarchical aiHFSMManager;

    private bool isIdling, isAttacking;
    private string AITeam;

    // Start is called before the first frame update
    void Start()
    {
        isIdling = false;
        isAttacking = false;

        if (gameObject.tag == "Enemy")
        {
            AITeam = "Enemy";
        }
        else if(gameObject.tag == "AllyBase")
        {
            AITeam = "AllyBase";
        }
        else if (gameObject.tag == "Ally")
        {
            AITeam = "Ally";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aiManager != null)
        {
            animator.SetBool("IsIdling", aiManager.ReturnIsIdling());
            animator.SetBool("IsAttacking", aiManager.ReturnIsAttacking());
            animator.SetBool("IsWalking", aiManager.ReturnIsWalking());
        }
        else if(aiHFSMManager != null)
        {
            animator.SetBool("IsIdling", aiHFSMManager.ReturnIsIdling());
            animator.SetBool("IsAttacking", aiHFSMManager.ReturnIsAttacking());
            animator.SetBool("IsWalking", aiHFSMManager.ReturnIsWalking());
        }
    }
}
