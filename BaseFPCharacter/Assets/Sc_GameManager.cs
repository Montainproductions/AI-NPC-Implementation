using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Sc_GameManager : MonoBehaviour
{
    public static Sc_GameManager Instance { get; set; }

    [SerializeField]
    private GameObject deathCanves;

    private PlayerInputActions playerInputActions;

    [SerializeField]
    private GameObject pauseMenu;
    private bool pauseActivated;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Escape.performed += Escape_performed;
        playerInputActions.Player.Restart.performed += GameRestarted_performed;
    }

    public void Start()
    {
        pauseActivated = false;
        pauseMenu.SetActive(pauseActivated);
    }

    public void PlayerDied(Vector3 spawnLocation)
    {
        Instantiate(deathCanves, spawnLocation, Quaternion.identity);
    }

    public void ChangeLevel(int newLevel)
    {
        SceneManager.LoadScene(newLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Escape_performed(InputAction.CallbackContext context)
    {
        if (SceneManager.sceneCountInBuildSettings != 0)
        {
            pauseActivated = !pauseActivated;
            pauseMenu.SetActive(pauseActivated);

            /*GameState currentGameState = GameStateManager.Instance.CurrentGameState;
            GameState newGameState = currentGameState == GameState.Gameplay
                ? GameState.Paused
                : GameState.Gameplay;

            GameStateManager.Instance.SetState(newGameState);*/
        }
    }

    private void GameRestarted_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeLevel(0);
        }
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
