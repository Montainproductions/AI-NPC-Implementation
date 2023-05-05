using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class Sc_PlayerAction : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    [SerializeField]
    private GameObject pressFText;

    private bool canActivate, actionButtonPressed;

    private float turnOnTimer;

    [SerializeField]
    private float maxTimer;

    [SerializeField]
    private GameObject circleProgress;
    //[SerializeField]
    //private TextMeshProUGUI ProgressIndicator;
    [SerializeField]
    private Image LoadingBar;

    public void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Action.started += Action_performed;
        playerInputActions.Player.Action.canceled += Action_performed;
    }

    // Start is called before the first frame update
    void Start()
    {
        canActivate = false;
        circleProgress.SetActive(canActivate);
        pressFText.SetActive(canActivate);
    }

    // Update is called once per frame
    void Update()
    {
        if (actionButtonPressed)
        {
            ProgressBar();
            circleProgress.SetActive(true);
            if (turnOnTimer < maxTimer)
            {
                turnOnTimer += Time.deltaTime;
            }
            else
            {
                circleProgress.SetActive(false);
                turnOnTimer = 0;
                if (gameObject.tag == "LevelChange")
                {
                    Sc_GameManager.Instance.ChangeLevel(2);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void ProgressBar()
    {
        if (turnOnTimer < maxTimer)
        {
            turnOnTimer += Time.deltaTime;
        }

        LoadingBar.fillAmount = turnOnTimer / maxTimer;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canActivate = true;
            pressFText.SetActive(canActivate);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canActivate = false;
            pressFText.SetActive(canActivate);
        }
    }

    public void Action_performed(InputAction.CallbackContext context)
    {
        if (context.started && canActivate)
        {
            actionButtonPressed = true;
        }
        if (context.canceled)
        {
            actionButtonPressed = false;
        }
    }
}
