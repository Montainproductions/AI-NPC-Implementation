using NUnit;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LightActivation : MonoBehaviour
{
    [SerializeField]
    private int activationPosition;

    private float elapsed = 0f;

    private float start = 0.0f;
    private float end = 0.0f;

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

        elapsed = 0f;

        start = 0.0f;
        end = 0.0f;

        for (int i = 0; i < 4; i++)
        {
            if (i%2 == 0)
            {
                start = lightSource.intensity;
                end = lightSource.intensity / 4;
            }
            else
            {
                start = lightSource.intensity;
                end = lightSource.intensity * 4;
            }
            
            elapsed = 0;

            while (elapsed < timeBeforeActivation)
            {
                elapsed += Time.deltaTime;

                float progress = elapsed / (timeBeforeActivation*2);

                lightSource.intensity = Mathf.Lerp(start, end, progress);
                yield return null;
            }
        }

        lightSource.intensity = end;

        yield return null;
    }

}
