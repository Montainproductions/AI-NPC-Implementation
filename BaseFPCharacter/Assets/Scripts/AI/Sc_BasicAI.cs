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
    [SerializeField]
    [Tooltip("Current Weapon Object")]
    private Sc_Attacking attackScript;

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
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

        //Debug.Log(Vector3.Distance(player.transform.position, gameObject.transform.position));

        if (Vector3.Distance(player.transform.position, gameObject.transform.position) > 2)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, player.transform.position, Time.deltaTime * speed);
        }
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        int layerMask = 1 << 6;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, fwd, 10, layerMask))
        {


            StartCoroutine(attackScript.Attacking());
            Debug.Log("Can Attack");
            yield return null;
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Cant Attack");
            yield return null;
        }
        yield return null;
    }

    public void RecentlyAttacked()
    {
        recentlyAttacked = true;
        waitTimeAffAttack = 3;
    }
}
