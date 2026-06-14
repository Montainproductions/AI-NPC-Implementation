using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_HealthBarAI : MonoBehaviour
{
    [SerializeField]
    private Sc_Health enemyHealth;
    [SerializeField]
    private GameObject healthBarUI;
    [SerializeField]
    private Slider sliderUI;

    // Start is called before the first frame update
    void Start()
    {
        sliderUI.value = HealthPercentage();
    }

    // Update is called once per frame
    void Update()
    {
        sliderUI.value = HealthPercentage();
    }

    public float HealthPercentage()
    {
        return enemyHealth.CurrentHealthValue() / enemyHealth.MaxHealthValue();
    }
}
