using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AITraits : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] AudioClip;

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
                enemyAIScripts[i].SetUpTraits(Aggressive);
            }
            else if (7.5f > randomValue && randomValue >= 0.5f)
            {
                //Debug.Log(Bold.ReturnAgressionValue());
                enemyAIScripts[i].SetUpTraits(Bold);
            }
            else if(5.0f > randomValue && randomValue >= 0.25f)
            {
                //Debug.Log(Cautious.ReturnAgressionValue());
                enemyAIScripts[i].SetUpTraits(Cautious);
            }
            else
            {
                //Debug.Log(Scared.ReturnAgressionValue());
                enemyAIScripts[i].SetUpTraits(Scared);
            }
        }
        yield return null;
    }
}

public class Trait
{
    private string traitName;
    private float healthChange, agressionValue, approchPlayerChange;
    private AudioClip[] audioclips;

    public Trait()
    {

    }

    public Trait(string traitName, float healthChange, float agressionValueChange, float approchPlayerChange)
    {
        this.traitName = traitName;
        this.healthChange = healthChange;
        this.agressionValue = agressionValueChange;
        this.approchPlayerChange = approchPlayerChange;
    }

    public void SetUpName(string traitName)
    {
        this.traitName = traitName;
    }

    public void SetUpHealthChange(float healthChange)
    {
        this.healthChange = healthChange;
    }

    public void SetUpAgressionValue(float agressionValue)
    {
        this.agressionValue = agressionValue;
    }

    public void SetUpApprochingPlayer(float approchPlayerChange)
    {
        this.approchPlayerChange = approchPlayerChange;
    }

    public string ReturnName()
    {
        return traitName;
    }

    public float ReturnHealthChange()
    {
        return healthChange;
    }

    public float ReturnAgressionValue()
    {
        return agressionValue;
    }

    public float ReturnApprochingPlayer()
    {
        return approchPlayerChange;
    }
}
