using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    RaycastHit hit;
    LayerMask layerMask;

    public static Action<GameObject> pickedUpGunEvent;

    private float maxDistanceToCheck;

    private bool beingLookedAt;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layerMask = LayerMask.GetMask("Interactible");
        StartCoroutine(CheckInteractible());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CheckInteractible()
    {
        yield return new WaitForSeconds(0.2f);
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistanceToCheck, layerMask))
        {
            beingLookedAt = true;
        }

        if(hit.collider.tag == "Gun")
        {
            Gun foundGun = hit.collider.GetComponent<Gun>();

            if (foundGun != null)
            {
                foundGun.ShowPickUpInfo();
            }
        }

        StartCoroutine(CheckInteractible());
    }

    public void AquireItem()
    {
        if (hit.collider == null)
        {
            return;
        }
        
        pickedUpGunEvent?.Invoke(hit.collider.gameObject);
    }
}
