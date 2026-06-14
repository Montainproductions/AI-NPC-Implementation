using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sc_MainMenu : MonoBehaviour
{
    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToTutorial()
    {
        SceneManager.LoadScene(1);
    }

    public void ToEnemyMap()
    {
        SceneManager.LoadScene(2);
    }

    public void ToEnemyBaseLayout()
    {
        SceneManager.LoadScene(3);
    }
}
