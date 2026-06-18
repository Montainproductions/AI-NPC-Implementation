using NUnit;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LightActivation : MonoBehaviour
{
    [SerializeField]
    private int activationPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BindEvent();
    }

    public void BindEvent()
    {
        GunRangeDirector.setLightsActive += LightWarningActivation;
    }

    public void UnbindEvent() 
    {
        GunRangeDirector.setLightsActive -= LightWarningActivation;
    }

    public void LightWarningActivation(int positionActivation, float timeBeforeActivation)
    {
        if (activationPosition != positionActivation) { return; }

        StartCoroutine(LightWarningShow(timeBeforeActivation));
    }

    public IEnumerator LightWarningShow(float timeBeforeActivation)
    {
        Light lightSource = gameObject.GetComponent<Light>();

        float elapsed = 0f;

        float start = 0.0f;
        float end = 0.0f;

        for (int i = 0; i < 4; i++)
        {
            if (i%2 == 0)
            {
                start = lightSource.intensity;
                end = lightSource.intensity / 3;
            }
            else
            {
                start = lightSource.intensity / 3;
                end = lightSource.intensity;
            }
            
            elapsed = 0;

            while (elapsed < timeBeforeActivation)
            {
                elapsed += Time.deltaTime;

                float progress = elapsed / timeBeforeActivation;

                lightSource.intensity = Mathf.Lerp(start, end, progress);
                yield return null;
            }
        }

        lightSource.intensity = end;

        yield return null;
    }

}
