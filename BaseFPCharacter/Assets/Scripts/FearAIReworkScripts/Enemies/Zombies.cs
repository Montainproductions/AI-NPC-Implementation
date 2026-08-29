using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Zombies : CharacterGeneral
{
    private double levelHealth;
 
    public HealthSystem healthSystem;

    [SerializeField]
    private NavMeshAgent agent;

    private Transform[] allPlayerArray;
    private Transform closestPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CheckInteractible());
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = closestPlayer.transform.position;
    }

    IEnumerator CheckInteractible()
    {
        closestPlayer = allPlayerArray[0];
        yield return new WaitForSeconds(0.1f);
    }

    public void SetHealth(double newHealth)
    {
        healthSystem.SetHealth(newHealth);
    }

    public void SetPlayerTransforms(Transform[] allPlayerTransforms)
    {
        allPlayerArray = allPlayerTransforms;
    }
    
}
