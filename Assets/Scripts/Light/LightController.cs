using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightController : MonoBehaviour
{
    [SerializeField] private Light2D lampLight;
    [SerializeField] private float transitionSpeed = 1f;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float turnOnTime = 15f;
    [SerializeField] private float turnOffTime = 6f;

    private Coroutine fadeRoutine;

    private void Start()
    {
        TimeManager.Instance.OnTimeUpdated += UpdateLightState;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null) TimeManager.Instance.OnTimeUpdated -= UpdateLightState;
    }

    private void UpdateLightState(float hour)
    {
        bool shouldBeOn = (turnOnTime < turnOffTime) ?
            (hour >= turnOnTime && hour < turnOffTime) :
            (hour >= turnOnTime || hour < turnOffTime);

        float targetIntensity = shouldBeOn ? maxIntensity : minIntensity;

        if (Mathf.Abs(lampLight.intensity - targetIntensity) > 0.01f)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeLight(targetIntensity));
        }
    }

    private IEnumerator FadeLight(float target)
    {
        float start = lampLight.intensity;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            lampLight.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }

        lampLight.intensity = target;
    }
}