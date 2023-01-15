using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BasicAI : MonoBehaviour
{
    private GameObject player;
    private float speed;

    //Attacking
    private bool recentlyAttacked;
    private float waitTimeAffAttack, damping;

    private float currentHealth, maxHealth;

    private GameObject star;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;

        recentlyAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) > 3)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, player.transform.position, Time.deltaTime * speed);
        }
        Attack();

        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void Attack()
    {
        int layerMask = 1 << 6;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, fwd, 10, layerMask))
        {
            
            Debug.Log("Can Attack");
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Cant Attack");
        }
    }

    public void RecentlyAttacked()
    {
        recentlyAttacked = true;
        waitTimeAffAttack = 3;
    }
}
