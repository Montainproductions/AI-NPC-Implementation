using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class ReloadingBar : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    [SerializeField]
    private Sc_BaseGun currentGun;

    [SerializeField]
    private GameObject progressBar;

    [SerializeField]
    private Image LoadingBar;

    public void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Use this for initialization
    void Start()
    {
        progressBar.SetActive(false);
        
        BindEvents();
    }

    public void BindEvents()
    {
        PlayerInventory.reloadEvent += ReloadItem;
    }

    public void UnbindEvent()
    {
        PlayerInventory.reloadEvent -= ReloadItem;
    }

    public void ReloadItem(GunInfo infoGun) { StartCoroutine(ReloadItemCorutine(infoGun)); }

    public IEnumerator ReloadItemCorutine(GunInfo infoGun)
    {
        progressBar.SetActive(true);

        float reloadTimer = 0;
        float timeDifference = 0.01f;
        while (reloadTimer < infoGun.reloadTime)
        {
            yield return new WaitForSeconds(timeDifference);
            reloadTimer += timeDifference;
            LoadingBar.fillAmount = reloadTimer / infoGun.reloadTime;
        }

        progressBar.SetActive(false);
        yield return null;
    }
}
