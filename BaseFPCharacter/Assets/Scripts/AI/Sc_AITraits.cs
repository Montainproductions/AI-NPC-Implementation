using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AITraits : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] agressiveAudioClips1, boldAudioClips1;

    [SerializeField]
    private Sc_AIStateManager[] enemyAIScripts;

    [HideInInspector]
    public Trait Aggressive = new Trait("Aggressive", 12, 3, 2);
    [HideInInspector]
    public Trait Bold = new Trait("Bold", 6, 1.5f, 0.5f);
    [HideInInspector]
    public Trait Cautious = new Trait("Cautious", -6, -1.5f, -0.5f);
    [HideInInspector]
    public Trait Scared = new Trait("Scared", -12, -3, -2);

    public void Start()
    {
        StartCoroutine(GiveEnemiesTraits());
    }

    IEnumerator GiveEnemiesTraits()
    {
        yield return new WaitForSeconds(0.1f);
        for(int i = 0; i < enemyAIScripts.Length; i++)
        {
            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue >= 0.75f)
            {
                //Debug.Log(Aggressive.ReturnAgressionValue());
                enemyAIScripts[i].SetUpTraits(Aggressive, agressiveAudioClips1);
            }
            else if (7.5f > randomValue && randomValue >= 0.5f)
            {
                //Debug.Log(Bold.ReturnAgressionValue());
                enemyAIScripts[i].SetUpTraits(Bold, boldAudioClips1);
            }
            else if(5.0f > randomValue && randomValue >= 0.25f)
            {
                //Debug.Log(Cautious.ReturnAgressionValue());
                enemyAIScripts[i].SetUpTraits(Cautious, agressiveAudioClips1);
            }
            else
            {
                //Debug.Log(Scared.ReturnAgressionValue());
                enemyAIScripts[i].SetUpTraits(Scared, boldAudioClips1);
            }
        }
        yield return null;
    }
}
