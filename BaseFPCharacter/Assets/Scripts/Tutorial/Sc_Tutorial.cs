using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sc_Tutorial : MonoBehaviour
{
    public Sc_Tutorial Instance { get; set; }

    [SerializeField]
    private AudioClip[] tutorialInstructions, idleTalking;
    [SerializeField]
    private AudioSource[] tutorialSource;

    [SerializeField]
    private Sc_Triggers boxTrigger;
    [SerializeField]
    private GameObject[] boxShot;
    private int boxesShotCount;


    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        boxesShotCount = 0;
        StartCoroutine(StartOfTutorial());
    }

    // Update is called once per frame
    void Update()
    {
        if (boxTrigger.ReturnPlayerActivation())
        {
            ShootingRangeStart();
        }
    }

    public void BoxShot()
    {

    }

    public void ShootingRangeStart()
    {
        tutorialSource[1].PlayOneShot(tutorialInstructions[1]);
    }

    public void ShootingRangeEnd()
    {
        tutorialSource[2].PlayOneShot(tutorialInstructions[2]);
    }

    IEnumerator StartOfTutorial()
    {

        yield return new WaitForSeconds(1.2f);
        tutorialSource[0].PlayOneShot(tutorialInstructions[0]);
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(ShootingRange());
        yield return null;
    }

    IEnumerator ShootingRange()
    {
        for (int i = 0; i < boxShot.Length; i++)
        {
            if (boxShot[i].GetComponent<Sc_BoxShot>().ReturnShot())
            {
                boxesShotCount++;
            }
        }

        if (boxesShotCount == boxShot.Length)
        {
            ShootingRangeEnd();
        }
        else
        {
            boxesShotCount = 0;
            StartCoroutine(ShootingRange());
        }
        yield return null;
    }
}
