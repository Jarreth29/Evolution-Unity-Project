using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodSettingsController : MonoBehaviour
{
    [Header("Food Min Size")]
    [SerializeField] private Slider foodMinSizeSlider;
    [SerializeField] private TextMeshProUGUI foodMinSizeValueText;
    [SerializeField] private TextMeshProUGUI foodMinSliderMinValueText;
    [SerializeField] private TextMeshProUGUI foodMinSliderMaxValueText;
    private const string FoodMinSizeKey = "FoodMinSize";
    private const float DefaultFoodMinSize = 0.1f;

    [Header("Food Max Size")]
    [SerializeField] private Slider foodMaxSizeSlider;
    [SerializeField] private TextMeshProUGUI foodMaxSizeValueText;
    [SerializeField] private TextMeshProUGUI foodMaxSliderMinValueText;
    [SerializeField] private TextMeshProUGUI foodMaxSliderMaxValueText;
    private const string FoodMaxSizeKey = "FoodMaxSize";
    private const float DefaultFoodMaxSize = 2f;

    [Header("Growth Speed")]
    [SerializeField] private Slider growthSpeedSlider;
    [SerializeField] private TextMeshProUGUI growthSpeedValueText;
    [SerializeField] private TextMeshProUGUI growthSpeedMinValueText;
    [SerializeField] private TextMeshProUGUI growthSpeedMaxValueText;
    private const string GrowthSpeedKey = "FoodGrowthSpeed";
    private const float DefaultGrowthSpeed = 0.05f;

    [Header("Sprout Frequency")]
    [SerializeField] private Slider sproutFrequencySlider;
    [SerializeField] private TextMeshProUGUI sproutFrequencyValueText;
    [SerializeField] private TextMeshProUGUI sproutFrequencyMinValueText;
    [SerializeField] private TextMeshProUGUI sproutFrequencyMaxValueText;
    private const string SproutFrequencyKey = "FoodSproutFrequency";
    private const float DefaultSproutFrequency = 10f;

    [Header("Sprout Distance")]
    [SerializeField] private Slider sproutDistanceSlider;
    [SerializeField] private TextMeshProUGUI sproutDistanceValueText;
    [SerializeField] private TextMeshProUGUI sproutDistanceMinValueText;
    [SerializeField] private TextMeshProUGUI sproutDistanceMaxValueText;
    private const string SproutDistanceKey = "FoodSproutDistance";
    private const float DefaultSproutDistance = 3f;

    

    private void Start()
    {
        UpdateMinsAndMaxs(PlayerPrefs.GetFloat("WorldWidth", 100f)); // Default to medium size if not set
        if (foodMinSizeSlider != null)
        {            
            float savedFoodMinSize = PlayerPrefs.GetFloat(FoodMinSizeKey, DefaultFoodMinSize);
            foodMinSizeSlider.value = Mathf.Clamp(savedFoodMinSize, foodMinSizeSlider.minValue, foodMinSizeSlider.maxValue);
            UpdateValueText(foodMinSizeValueText, foodMinSizeSlider.value);

            foodMinSizeSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(foodMinSizeValueText, newValue);
            });
        }
        if (foodMaxSizeSlider != null)
        {
            float savedFoodMaxSize = PlayerPrefs.GetFloat(FoodMaxSizeKey, DefaultFoodMaxSize);
            foodMaxSizeSlider.value = Mathf.Clamp(savedFoodMaxSize, foodMaxSizeSlider.minValue, foodMaxSizeSlider.maxValue);
            UpdateValueText(foodMaxSizeValueText, foodMaxSizeSlider.value);
            foodMaxSizeSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(foodMaxSizeValueText, newValue);
            });
        }
        if (growthSpeedSlider != null)
        {
            float savedGrowthSpeed = PlayerPrefs.GetFloat(GrowthSpeedKey, DefaultGrowthSpeed);
            growthSpeedSlider.value = Mathf.Clamp(savedGrowthSpeed, growthSpeedSlider.minValue, growthSpeedSlider.maxValue);
            UpdateValueText(growthSpeedValueText, growthSpeedSlider.value, "F2");
            growthSpeedSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(growthSpeedValueText, newValue, "F2");
            });
        }
        if (sproutFrequencySlider != null)
        {
            float savedSproutFrequency = PlayerPrefs.GetFloat(SproutFrequencyKey, DefaultSproutFrequency);
            sproutFrequencySlider.value = Mathf.Clamp(savedSproutFrequency, sproutFrequencySlider.minValue, sproutFrequencySlider.maxValue);
            UpdateValueText(sproutFrequencyValueText, sproutFrequencySlider.value,"F0");
            sproutFrequencySlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(sproutFrequencyValueText, newValue, "F0");
            });
        }
        if (sproutDistanceSlider != null)
        {
            float savedSproutDistance = PlayerPrefs.GetFloat(SproutDistanceKey, DefaultSproutDistance);
            sproutDistanceSlider.value = Mathf.Clamp(savedSproutDistance, sproutDistanceSlider.minValue, sproutDistanceSlider.maxValue);
            UpdateValueText(sproutDistanceValueText, sproutDistanceSlider.value, "F0");
            sproutDistanceSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateValueText(sproutDistanceValueText, newValue, "F0");
            });
        }

    }

    public void UpdateMinsAndMaxs(float worldSize)
    {
        if (worldSize == 50f)
        {
            foodMinSizeSlider.minValue = 0.1f;
            foodMinSizeSlider.maxValue = 0.3f;
            foodMaxSizeSlider.minValue = foodMinSizeSlider.value;
            foodMaxSizeSlider.maxValue = 1f;
            
        }
        else if (worldSize == 100f)
        {
            foodMinSizeSlider.minValue = 0.1f;
            foodMinSizeSlider.maxValue = 1f;
            foodMaxSizeSlider.minValue = foodMinSizeSlider.value;
            foodMaxSizeSlider.maxValue = 3f;
        }
        else if (worldSize == 200f)
        {
            foodMinSizeSlider.minValue = 0.1f;
            foodMinSizeSlider.maxValue = 1f;
            foodMaxSizeSlider.minValue = foodMinSizeSlider.value;
            foodMaxSizeSlider.maxValue = 5f;
        }
        growthSpeedSlider.minValue = 0.01f;
        growthSpeedSlider.maxValue = 1f;
        sproutFrequencySlider.minValue = 1f;
        sproutFrequencySlider.maxValue = 30f;
        sproutDistanceSlider.minValue = 1f;
        sproutDistanceSlider.maxValue = 10f;
        ClampValuesToMinAndMax();
    }

    public void ClampValuesToMinAndMax()
    {
        if (foodMinSizeSlider != null)
        {
            foodMinSizeSlider.value = Mathf.Clamp(foodMinSizeSlider.value, foodMinSizeSlider.minValue, foodMinSizeSlider.maxValue);
        }
        if (foodMaxSizeSlider != null)
        {
            foodMaxSizeSlider.value = Mathf.Clamp(foodMaxSizeSlider.value, foodMaxSizeSlider.minValue, foodMaxSizeSlider.maxValue);
        }
        if (growthSpeedSlider != null)
        {
            growthSpeedSlider.value = Mathf.Clamp(growthSpeedSlider.value, growthSpeedSlider.minValue, growthSpeedSlider.maxValue);
        }
        if (sproutFrequencySlider != null)
        {
            sproutFrequencySlider.value = Mathf.Clamp(sproutFrequencySlider.value, sproutFrequencySlider.minValue, sproutFrequencySlider.maxValue);
        }
        if (sproutDistanceSlider != null)
        {
            sproutDistanceSlider.value = Mathf.Clamp(sproutDistanceSlider.value, sproutDistanceSlider.minValue, sproutDistanceSlider.maxValue);
        }
        UpdateFoodUIText();
    }

    public void UpdateValueText(TextMeshProUGUI textElement, float value, string format = "F1")
    {
        if (textElement != null)
        {
            textElement.text = value.ToString(format);
        }
        if (textElement == foodMinSizeValueText)
        {
            foodMaxSizeSlider.minValue = value;
        }
        ClampValuesToMinAndMax();
    }

    public void UpdateFoodUIText()
    {
        if (foodMinSliderMinValueText != null)
        {
            foodMinSliderMinValueText.text = foodMinSizeSlider.minValue.ToString("F1");
        }
        if (foodMinSliderMaxValueText != null)
        {
            foodMinSliderMaxValueText.text = foodMinSizeSlider.maxValue.ToString("F1");
        }
        if (foodMaxSliderMinValueText != null)
        {
            foodMaxSliderMinValueText.text = foodMaxSizeSlider.minValue.ToString("F1");
        }
        if (foodMaxSliderMaxValueText != null)
        {
            foodMaxSliderMaxValueText.text = foodMaxSizeSlider.maxValue.ToString("F1");
        }
        if (growthSpeedMinValueText != null)
        {
            growthSpeedMinValueText.text = growthSpeedSlider.minValue.ToString("F2");
        }
        if (growthSpeedMaxValueText != null)
        {
            growthSpeedMaxValueText.text = growthSpeedSlider.maxValue.ToString("F2");
        }
        if (sproutFrequencyMinValueText != null)
        {
            sproutFrequencyMinValueText.text = sproutFrequencySlider.minValue.ToString("F0");
        }
        if (sproutFrequencyMaxValueText != null)
        {
            sproutFrequencyMaxValueText.text = sproutFrequencySlider.maxValue.ToString("F0");
        }
        if (sproutDistanceMinValueText != null)
        {
            sproutDistanceMinValueText.text = sproutDistanceSlider.minValue.ToString("F0");
        }
        if (sproutDistanceMaxValueText != null)
        {
            sproutDistanceMaxValueText.text = sproutDistanceSlider.maxValue.ToString("F0");
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(FoodMinSizeKey, foodMinSizeSlider.value);
        PlayerPrefs.SetFloat(FoodMaxSizeKey, foodMaxSizeSlider.value);
        PlayerPrefs.SetFloat(GrowthSpeedKey, growthSpeedSlider.value);
        PlayerPrefs.SetFloat(SproutFrequencyKey, sproutFrequencySlider.value);
        PlayerPrefs.SetFloat(SproutDistanceKey, sproutDistanceSlider.value);
        PlayerPrefs.Save();
        Debug.Log("Food settings saved.");
    }

    private void OnDestroy()
    {
        foodMinSizeSlider.onValueChanged.RemoveAllListeners();
        foodMaxSizeSlider.onValueChanged.RemoveAllListeners();
        growthSpeedSlider.onValueChanged.RemoveAllListeners();
        sproutFrequencySlider.onValueChanged.RemoveAllListeners();
        sproutDistanceSlider.onValueChanged.RemoveAllListeners();
    }
}
