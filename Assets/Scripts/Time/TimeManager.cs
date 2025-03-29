using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<float> OnTimeUpdated; 
    public event Action<int> OnDayChanged;    

    [SerializeField] private float gameTimeMultiplier = 120f;
    [SerializeField] private Toggle testingModeToggle;
    [SerializeField] private TMP_Text timeDisplayText; 


    private DateTime simulatedTime;
    private bool isTestingMode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SyncWithSystemTime();
        if (testingModeToggle != null) testingModeToggle.onValueChanged.AddListener(ToggleTestingMode);
        UpdateTimeDisplay(); 
    }

    private void Update()
    {
        if (isTestingMode)
        {
            simulatedTime = simulatedTime.AddSeconds(Time.deltaTime * gameTimeMultiplier);
        }
        else
        {
            simulatedTime = DateTime.Now;
        }

        float currentHour = GetCurrentHour();
        OnTimeUpdated?.Invoke(currentHour);

        if (simulatedTime.Day != CurrentDay) OnDayChanged?.Invoke(simulatedTime.Day);
        UpdateGameTime();
        UpdateTimeDisplay(); 
    }

    private void UpdateGameTime()
    {
        CurrentDay = simulatedTime.Day;
        CurrentMonth = simulatedTime.Month;
        CurrentYear = simulatedTime.Year;
    }

    private void UpdateTimeDisplay()
    {
        if (timeDisplayText != null)
        {
            string timeString = simulatedTime.ToString("HH:mm:ss");
            timeDisplayText.text = timeString;

        }
    }

    public float GetCurrentHour() => (float)simulatedTime.TimeOfDay.TotalHours;
    public int CurrentDay { get; private set; }
    public int CurrentMonth { get; private set; }
    public int CurrentYear { get; private set; }

    private void ToggleTestingMode(bool isOn)
    {
        isTestingMode = isOn;
        if (!isOn) SyncWithSystemTime();
    }

    private void SyncWithSystemTime() => simulatedTime = DateTime.Now;
}