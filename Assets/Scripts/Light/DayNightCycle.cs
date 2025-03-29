using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [SerializeField] private Light2D globalLight;
    [SerializeField] private float transitionSpeed = 1f;

    [Header("Time Phases (24H)")]
    [SerializeField] private float morningTime = 5f;
    [SerializeField] private float noonTime = 11f;
    [SerializeField] private float eveningTime = 15f;
    [SerializeField] private float nightTime = 18f;

    [Header("Lighting Settings")]
    [SerializeField] private float minBrightness = 0.2f;
    [SerializeField] private float maxBrightness = 1f;
    [SerializeField] private Color morningColor = new Color(0.8f, 0.7f, 1f);
    [SerializeField] private Color noonColor = Color.white;
    [SerializeField] private Color eveningColor = new Color(0.8f, 0.5f, 0.5f);
    [SerializeField] private Color nightColor = new Color(0.2f, 0.2f, 0.4f);

    private float targetIntensity;
    private Color targetColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        TimeManager.Instance.OnTimeUpdated += UpdateLighting;
        UpdateLighting(TimeManager.Instance.GetCurrentHour());
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null) TimeManager.Instance.OnTimeUpdated -= UpdateLighting;
    }

    private void Update()
    {
        globalLight.color = Color.Lerp(globalLight.color, targetColor, Time.deltaTime * transitionSpeed);
        globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Time.deltaTime * transitionSpeed);
    }

    private void UpdateLighting(float hour)
    {
        float t;
        if (hour >= morningTime && hour < noonTime)
        {
            t = Mathf.InverseLerp(morningTime, noonTime, hour);
            targetColor = Color.Lerp(morningColor, noonColor, t);
            targetIntensity = Mathf.Lerp(minBrightness, maxBrightness, t);
        }
        else if (hour >= noonTime && hour < eveningTime)
        {
            targetColor = noonColor;
            targetIntensity = maxBrightness;
        }
        else if (hour >= eveningTime && hour < nightTime)
        {
            t = Mathf.InverseLerp(eveningTime, nightTime, hour);
            targetColor = Color.Lerp(eveningColor, nightColor, t);
            targetIntensity = Mathf.Lerp(maxBrightness, minBrightness, t);
        }
        else
        {
            targetColor = nightColor;
            targetIntensity = minBrightness;
        }
    }
}