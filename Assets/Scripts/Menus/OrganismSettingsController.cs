using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrganismSettingsController : MonoBehaviour
{
    [Header("Base Speed")]
    [SerializeField] private Slider baseSpeedSlider;
    [SerializeField] private TextMeshProUGUI baseSpeedValueText;
    [SerializeField] private TextMeshProUGUI baseSpeedMinValueText;
    [SerializeField] private TextMeshProUGUI baseSpeedMaxValueText;
    private const string BaseSpeedKey = "OrganismBaseSpeed";
    private const float DefaultBaseSpeed = 1f;

    [Header("Max Energy")]
    [SerializeField] private Slider maxEnergySlider;
    [SerializeField] private TextMeshProUGUI maxEnergyValueText;
    [SerializeField] private TextMeshProUGUI maxEnergyMinValueText;
    [SerializeField] private TextMeshProUGUI maxEnergyMaxValueText;
    private const string MaxEnergyKey = "OrganismMaxEnergy";
    private const float DefaultMaxEnergy = 100f;

    [Header("Metabolism Rate")]
    [SerializeField] private Slider metabolismRateSlider;
    [SerializeField] private TextMeshProUGUI metabolismRateValueText;
    [SerializeField] private TextMeshProUGUI metabolismRateMinValueText;
    [SerializeField] private TextMeshProUGUI metabolismRateMaxValueText;
    private const string MetabolismRateKey = "OrganismMetabolismRate";
    private const float DefaultMetabolismRate = 0.1f;

    [Header("Sight Range")]
    [SerializeField] private Slider sightRangeSlider;
    [SerializeField] private TextMeshProUGUI sightRangeValueText;
    [SerializeField] private TextMeshProUGUI sightRangeMinValueText;
    [SerializeField] private TextMeshProUGUI sightRangeMaxValueText;
    private const string SightRangeKey = "OrganismSightRange";
    private const float DefaultSightRange = 3f;

    [Header("Number of Rays")]
    [SerializeField] private Slider numberOfRaysSlider;
    [SerializeField] private TextMeshProUGUI numberOfRaysValueText;
    [SerializeField] private TextMeshProUGUI numberOfRaysMinValueText;
    [SerializeField] private TextMeshProUGUI numberOfRaysMaxValueText;
    private const string NumberOfRaysKey = "OrganismNumberOfRays";
    private const float DefaultNumberOfRays = 3;

    [Header("Angle Between Rays")]
    [SerializeField] private Slider angleBetweenRaysSlider;
    [SerializeField] private TextMeshProUGUI angleBetweenRaysValueText;
    [SerializeField] private TextMeshProUGUI angleBetweenRaysMinValueText;
    [SerializeField] private TextMeshProUGUI angleBetweenRaysMaxValueText;
    private const string AngleBetweenRaysKey = "OrganismAngleBetweenRays";
    private const float DefaultAngleBetweenRays = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateMinsAndMaxs();
        if (baseSpeedSlider != null)
        {
            float savedBaseSpeed = PlayerPrefs.GetFloat(BaseSpeedKey, DefaultBaseSpeed);
            baseSpeedSlider.value = Mathf.Clamp(savedBaseSpeed, baseSpeedSlider.minValue, baseSpeedSlider.maxValue);
            UpdateValueText(baseSpeedValueText, baseSpeedSlider.value);
            baseSpeedSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(baseSpeedValueText, newValue);
            });
        }

        if (maxEnergySlider != null)
        {
            float savedMaxEnergy = PlayerPrefs.GetFloat(MaxEnergyKey, DefaultMaxEnergy);
            maxEnergySlider.value = Mathf.Clamp(savedMaxEnergy, maxEnergySlider.minValue, maxEnergySlider.maxValue);
            UpdateValueText(maxEnergyValueText, maxEnergySlider.value, "F0");
            maxEnergySlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(maxEnergyValueText, newValue, "F0");
            });
        }
        if (metabolismRateSlider != null)
        {
            float savedMetabolismRate = PlayerPrefs.GetFloat(MetabolismRateKey, DefaultMetabolismRate);
            metabolismRateSlider.value = Mathf.Clamp(savedMetabolismRate, metabolismRateSlider.minValue, metabolismRateSlider.maxValue);
            UpdateValueText(metabolismRateValueText, metabolismRateSlider.value);
            metabolismRateSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(metabolismRateValueText, newValue, "F2");
            });
        }
        if (sightRangeSlider != null)
        {
            float savedSightRange = PlayerPrefs.GetFloat(SightRangeKey, DefaultSightRange);
            sightRangeSlider.value = Mathf.Clamp(savedSightRange, sightRangeSlider.minValue, sightRangeSlider.maxValue);
            UpdateValueText(sightRangeValueText, sightRangeSlider.value, "F1");
            sightRangeSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(sightRangeValueText, newValue, "F1");
            });
        }
        if (numberOfRaysSlider != null)
        {
            float savedNumberOfRays = PlayerPrefs.GetFloat(NumberOfRaysKey, DefaultNumberOfRays);
            numberOfRaysSlider.value = Mathf.Clamp(savedNumberOfRays, numberOfRaysSlider.minValue, numberOfRaysSlider.maxValue);
            UpdateValueText(numberOfRaysValueText, numberOfRaysSlider.value, "F0");
            numberOfRaysSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(numberOfRaysValueText, newValue, "F0");
            });
        }
        if (angleBetweenRaysSlider != null)
        {
            float savedAngleBetweenRays = PlayerPrefs.GetFloat(AngleBetweenRaysKey, DefaultAngleBetweenRays);
            angleBetweenRaysSlider.value = Mathf.Clamp(savedAngleBetweenRays, angleBetweenRaysSlider.minValue, angleBetweenRaysSlider.maxValue);
            UpdateValueText(angleBetweenRaysValueText, angleBetweenRaysSlider.value, "F0");
            angleBetweenRaysSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(angleBetweenRaysValueText, newValue, "F0");
            });
        }
    }

    public void UpdateMinsAndMaxs()
    {
        baseSpeedSlider.minValue = 0.1f;
        baseSpeedSlider.maxValue = 5f;
        maxEnergySlider.minValue = 10f;
        maxEnergySlider.maxValue = 1000f;
        metabolismRateSlider.minValue = 0.01f;
        metabolismRateSlider.maxValue = 1f;
        sightRangeSlider.minValue = 0.1f;
        sightRangeSlider.maxValue = 10f;
        numberOfRaysSlider.minValue = 1;
        numberOfRaysSlider.maxValue = 10f;
        angleBetweenRaysSlider.minValue = 1f;
        angleBetweenRaysSlider.maxValue = 36f;
        ClampValuesToMinAndMax();
    }

    private void ClampValuesToMinAndMax()
    {
        baseSpeedSlider.value = Mathf.Clamp(baseSpeedSlider.value, baseSpeedSlider.minValue, baseSpeedSlider.maxValue);
        maxEnergySlider.value = Mathf.Clamp(maxEnergySlider.value, maxEnergySlider.minValue, maxEnergySlider.maxValue);
        metabolismRateSlider.value = Mathf.Clamp(metabolismRateSlider.value, metabolismRateSlider.minValue, metabolismRateSlider.maxValue);
        sightRangeSlider.value = Mathf.Clamp(sightRangeSlider.value, sightRangeSlider.minValue, sightRangeSlider.maxValue);
        numberOfRaysSlider.value = Mathf.Clamp(numberOfRaysSlider.value, numberOfRaysSlider.minValue, numberOfRaysSlider.maxValue);
        angleBetweenRaysSlider.value = Mathf.Clamp(angleBetweenRaysSlider.value, angleBetweenRaysSlider.minValue, angleBetweenRaysSlider.maxValue);
        UpdateOrganimsUIText();
    }

    public void UpdateValueText(TextMeshProUGUI text, float value, string format = "F1")
    {
        if (text != null)
        {
            text.text = value.ToString(format);
        }
    }

    public void UpdateOrganimsUIText()
    {
        baseSpeedValueText.text = baseSpeedSlider.value.ToString("F1");
        baseSpeedMinValueText.text = baseSpeedSlider.minValue.ToString("F1");
        baseSpeedMaxValueText.text = baseSpeedSlider.maxValue.ToString("F1");
        maxEnergyValueText.text = maxEnergySlider.value.ToString("F0");
        maxEnergyMinValueText.text = maxEnergySlider.minValue.ToString("F0");
        maxEnergyMaxValueText.text = maxEnergySlider.maxValue.ToString("F0");
        metabolismRateValueText.text = metabolismRateSlider.value.ToString("F2");
        metabolismRateMinValueText.text = metabolismRateSlider.minValue.ToString("F2");
        metabolismRateMaxValueText.text = metabolismRateSlider.maxValue.ToString("F2");
        sightRangeValueText.text = sightRangeSlider.value.ToString("F1");
        sightRangeMinValueText.text = sightRangeSlider.minValue.ToString("F1");
        sightRangeMaxValueText.text = sightRangeSlider.maxValue.ToString("F1");
        numberOfRaysValueText.text = numberOfRaysSlider.value.ToString("F0");
        numberOfRaysMinValueText.text = numberOfRaysSlider.minValue.ToString("F0");
        numberOfRaysMaxValueText.text = numberOfRaysSlider.maxValue.ToString("F0");
        angleBetweenRaysValueText.text = angleBetweenRaysSlider.value.ToString("F0");
        angleBetweenRaysMinValueText.text = angleBetweenRaysSlider.minValue.ToString("F0");
        angleBetweenRaysMaxValueText.text = angleBetweenRaysSlider.maxValue.ToString("F0");
    }

    public void SaveSettings()
    {
        // Save the values to PlayerPrefs
        PlayerPrefs.SetFloat(BaseSpeedKey, baseSpeedSlider.value);
        PlayerPrefs.SetFloat(MaxEnergyKey, maxEnergySlider.value);
        PlayerPrefs.SetFloat(MetabolismRateKey, metabolismRateSlider.value);
        PlayerPrefs.SetFloat(SightRangeKey, sightRangeSlider.value);
        PlayerPrefs.SetInt(NumberOfRaysKey, (int)numberOfRaysSlider.value);
        PlayerPrefs.SetFloat(AngleBetweenRaysKey, angleBetweenRaysSlider.value);
    }

    public void OnDestroy()
    {
        baseSpeedSlider.onValueChanged.RemoveAllListeners();
        maxEnergySlider.onValueChanged.RemoveAllListeners();
        metabolismRateSlider.onValueChanged.RemoveAllListeners();
        sightRangeSlider.onValueChanged.RemoveAllListeners();
        numberOfRaysSlider.onValueChanged.RemoveAllListeners();
        angleBetweenRaysSlider.onValueChanged.RemoveAllListeners();
    }
}
