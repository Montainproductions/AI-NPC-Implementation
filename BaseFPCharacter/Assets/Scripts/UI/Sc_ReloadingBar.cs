using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Sc_ReloadingBar : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    private bool alreadyReloading;

    [SerializeField]
    private Sc_BaseGun currentGun;
    private float reloadTimer;

    [SerializeField]
    private GameObject progressBar;

    [SerializeField]
    private Image LoadingBar;
    private float currentValue;

    public void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Reload.started += Action_performed;
        playerInputActions.Player.Reload.canceled += Action_performed;
    }

    // Use this for initialization
    void Start()
    {
        alreadyReloading = false;
        progressBar.SetActive(false);
        reloadTimer = currentGun.ReturnReloadTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (alreadyReloading)
        {
            progressBar.SetActive(true);
            if (currentValue < reloadTimer)
            {
                currentValue += Time.deltaTime;
            }
            else
            {
                alreadyReloading = false;
                progressBar.SetActive(false);
            }

            LoadingBar.fillAmount = currentValue / reloadTimer;

        }
    }

    public void Action_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Reloading");
        if (context.started)
        {
            Debug.Log("Start Reloading");
            if (!alreadyReloading || currentGun.ReturnCurrentAmmo() < currentGun.ReturnMaxClipAmmo())
            {
                alreadyReloading = true;
                //ProgressBar();
            }
        }
    }
}
