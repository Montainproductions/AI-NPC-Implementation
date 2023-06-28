using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_UIMenuAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mainMenuButtons, pauseMenuButtons, levelSelectorMenu;

    [SerializeField]
    private Transform[] mainMenuOgTransform, pauseMenuOgTransform, levelMenuOgTranform;

    private int force, angle;

    private Rigidbody2D rb2D;

    public void Start()
    {
        angle = -1;
    }

    public IEnumerator MainMenuAnimation()
    {
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            rb2D = mainMenuButtons[i].AddComponent<Rigidbody2D>();
            AddingRandomForce(rb2D);
            StartCoroutine(ReturnButtons(mainMenuButtons[i].transform, mainMenuButtons[i].GetComponent<Rigidbody2D>(), mainMenuOgTransform[i]));
        }
        yield return null;
    }

    public IEnumerator PauseMenuAnimation()
    {
        for (int i = 0; i < pauseMenuButtons.Length; i++)
        {
            rb2D = pauseMenuButtons[i].AddComponent<Rigidbody2D>();
            AddingRandomForce(rb2D);
            StartCoroutine(ReturnButtons(pauseMenuButtons[i].transform, pauseMenuButtons[i].GetComponent<Rigidbody2D>(), pauseMenuOgTransform[i]));
        }
        yield return null;
    }

    public IEnumerator LevelMenuAnimation()
    {
        for (int i = 0; i < levelSelectorMenu.Length; i++)
        {
            rb2D = levelSelectorMenu[i].AddComponent<Rigidbody2D>();
            AddingRandomForce(rb2D);
            StartCoroutine(ReturnButtons(levelSelectorMenu[i].transform, levelSelectorMenu[i].GetComponent<Rigidbody2D>(), levelMenuOgTranform[i]));
        }
        yield return null;
    }

    public void AddingRandomForce(Rigidbody2D rb2D)
    {
        while (angle < 0 || ((-10 <= angle) && (angle <= 10)))
        {
            angle = Random.Range(-90, 90);
        }

        force = Random.Range(10, 15);


        float xcomponent = angle * force;
        float ycomponent = angle * force;

        Vector2 targetPosition = new Vector2(angle * 2, angle * 2);

        rb2D.AddForce(targetPosition);
        angle = -1;
    }

    IEnumerator ReturnButtons(Transform obj, Rigidbody2D rb2D, Transform newPosition)
    {
        yield return new WaitForSeconds(2.1f);
        Destroy(rb2D);
        obj.position = newPosition.position;
        obj.rotation = newPosition.rotation;
        yield return null;
    }
}
