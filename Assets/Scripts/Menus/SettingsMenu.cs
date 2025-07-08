using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Maximum Entities")]
    [SerializeField] private Slider maxNumberEntitiesSlider;
    [SerializeField] private TextMeshProUGUI maxNumberEntitiesValueText;
    [SerializeField] private TextMeshProUGUI maxNumberEntitiesMaxValueText;
    private const string MaxEntitiesKey = "MaxEntities";
    private const float DefaultMaxEntities = 1000;
    [SerializeField] private Slider maxFoodOrganismSlider;
    [SerializeField] private TextMeshProUGUI maxFoodValueText;
    [SerializeField] private TextMeshProUGUI maxOrganismValueText;

    [Header("Initial Food")]
    [SerializeField] private Slider initialFoodSlider;
    [SerializeField] private TextMeshProUGUI initialFoodValueText;
    [SerializeField] private TextMeshProUGUI initialFoodMaxValueText;
    private const string InitialFoodKey = "InitialFood";
    private const string InitialMaxFoodKey = "InitialMaxFood";
    private const float DefaultInitialFood = 500f;

    [Header("Initial Organisms")]
    [SerializeField] private Slider initialOrganismsSlider;
    [SerializeField] private TextMeshProUGUI initialOrganismsValueText;
    [SerializeField] private TextMeshProUGUI initialOrganismsMaxValueText;
    private const string InitialOrganismsKey = "InitialOrganisms";
    private const string InitialMaxOrganismsKey = "InitialMaxOrganisms";
    private const float DefaultInitialOrganisms = 100f;

    [Header("Respawn Food Settings")]
    [SerializeField] private Toggle respawnFoodToggle;
    [SerializeField] private TextMeshProUGUI respawnFoodThresholdText;
    [SerializeField] private Slider respawnFoodThresholdScrollBar;
    [SerializeField] private TextMeshProUGUI respawnFoodThresholdValueText;
    [SerializeField] private TextMeshProUGUI respawnFoodIntervalText;
    [SerializeField] private Slider respawnFoodIntervalScrollBar;
    [SerializeField] private TextMeshProUGUI respawnFoodIntervalValueText;
    private const string RespawnFoodToggleKey = "RespawnFoodToggle";
    private const string RespawnFoodThresholdKey = "RespawnFoodThreshold";
    private const string RespawnFoodIntervalKey = "RespawnFoodInterval";
    private const float DefaultRespawnFoodThreshold = 0.25f; // 25% of the total food
    private const float DefaultRespawnFoodInterval = 60f; // 60 seconds
    private const bool DefaultRespawnFoodToggle = true;

    [Header("Size Selection Buttons")]
    public Button smallButton;
    public Button mediumButton;
    public Button largeButton;

    [Header("Action Buttons")]
    public Button applyButton;
    public Button backButton;

    [Header("Changes Applied")]
    public TextMeshProUGUI changesAppliedText;
    public float changesAppliedDuration = 2f; // Duration to show the changes applied message

    [Header("World Sizes")]
    //private WorldSizePreset currentSelectedPreset;
    private Vector2 smallWorldSize = new Vector2(50f, 50f);
    private Vector2 mediumWorldSize = new Vector2(100f, 100f);
    private Vector2 largeWorldSize = new Vector2(200f, 200f);
    private Vector2 selectedWorldSize = new Vector2 (100f,100f);

    [Header("Settings Controllers")]
    [SerializeField] private FoodSettingsController foodSettingsController;
    [SerializeField] private OrganismSettingsController organismSettingsController;

    void Start()
    {
        LoadPlayerPrefs();
        float saveWidth = PlayerPrefs.GetFloat("WorldWidth", mediumWorldSize.x);
        float saveHeight = PlayerPrefs.GetFloat("WorldHeight", mediumWorldSize.y);
        SelectSize(new Vector2 (saveWidth, saveHeight));
        smallButton.onClick.AddListener(() => SelectSize(smallWorldSize));
        mediumButton.onClick.AddListener(() => SelectSize(mediumWorldSize));
        largeButton.onClick.AddListener(() => SelectSize(largeWorldSize));
        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackButton);
        maxNumberEntitiesSlider.onValueChanged.AddListener(OnTotalEntitiesChanged);
        initialFoodSlider.onValueChanged.AddListener(OnInitialFoodChanged);
        initialOrganismsSlider.onValueChanged.AddListener(OnInitialOrganismsChanged);
        maxFoodOrganismSlider.onValueChanged.AddListener(OnMaxFoodOrganismChanged);

        respawnFoodToggle.onValueChanged.AddListener(OnRespawnToggleChanged);
        OnRespawnToggleChanged(respawnFoodToggle.isOn);

        respawnFoodThresholdScrollBar.onValueChanged.AddListener(value =>
        {
            respawnFoodThresholdValueText.text = (value * 100).ToString("F0") + "%";
        });

        respawnFoodIntervalScrollBar.onValueChanged.AddListener(value =>
        {
            respawnFoodIntervalValueText.text = value.ToString("F0");
        });

        UpdateButtonStates();

        if (changesAppliedText != null)
        {
            changesAppliedText.gameObject.SetActive(false);
        }
        AdjustSliders();
    }

    private void LoadPlayerPrefs()
    {
        maxNumberEntitiesSlider.value = PlayerPrefs.GetInt(MaxEntitiesKey, (int)DefaultMaxEntities);
        initialFoodSlider.value = PlayerPrefs.GetInt(InitialFoodKey, (int)DefaultInitialFood);
        initialOrganismsSlider.value = PlayerPrefs.GetInt(InitialOrganismsKey, (int)DefaultInitialOrganisms);
        respawnFoodToggle.isOn = PlayerPrefs.GetInt(RespawnFoodToggleKey, DefaultRespawnFoodToggle ? 1 : 0) == 1;
        respawnFoodThresholdScrollBar.value = PlayerPrefs.GetFloat(RespawnFoodThresholdKey, DefaultRespawnFoodThreshold);
        respawnFoodIntervalScrollBar.value = PlayerPrefs.GetFloat(RespawnFoodIntervalKey, DefaultRespawnFoodInterval);
        initialFoodSlider.maxValue = PlayerPrefs.GetFloat(InitialMaxFoodKey, DefaultInitialFood);
        initialOrganismsSlider.maxValue = PlayerPrefs.GetFloat(InitialMaxOrganismsKey, DefaultInitialOrganisms);
    }

    private void SelectSize(Vector2 size)
    {
        selectedWorldSize = size;
        UpdateLimitsBaseOnWorldSize(size.x);
        foodSettingsController.UpdateMinsAndMaxs(size.x);
        UpdateButtonStates();
    }

    private void UpdateLimitsBaseOnWorldSize(float worldSize)
    {
        if (worldSize == 50f)
        {
            maxNumberEntitiesSlider.maxValue = 1000f;
        }
        else if (worldSize == 100f)
        {
            maxNumberEntitiesSlider.maxValue = 1500f;
        }
        else if (worldSize == 200f)
        {
            maxNumberEntitiesSlider.maxValue = 2500f;
        }
        maxNumberEntitiesSlider.minValue = 0f;
        initialFoodSlider.minValue = 0f;
        initialOrganismsSlider.minValue = 0f;
        respawnFoodThresholdScrollBar.minValue = 0.01f;
        respawnFoodThresholdScrollBar.maxValue = 1f; // 0% to 100%
        respawnFoodIntervalScrollBar.minValue = 1f;
        respawnFoodIntervalScrollBar.maxValue = 600f; // 0 to 600 seconds
        
        AdjustSliders();
    }

    private void OnTotalEntitiesChanged(float totalValue)
    {
        AdjustSliders();
    }
    private void OnInitialFoodChanged(float foodValue)
    {
        AdjustSliders();
    }
    private void OnInitialOrganismsChanged(float organismsValue)
    {
        AdjustSliders();
    }
    private void OnMaxFoodOrganismChanged(float foodValue)
    {
        AdjustSliders();
    }

    private void OnRespawnToggleChanged(bool isOn)
    {
        respawnFoodThresholdText.gameObject.SetActive(isOn);
        respawnFoodIntervalText.gameObject.SetActive(isOn);
        UpdateUITexts();
    }

    private void AdjustSliders()
    {
        float maxEntities = maxNumberEntitiesSlider.value;
        maxFoodOrganismSlider.maxValue = maxEntities;

        initialFoodSlider.maxValue = maxFoodOrganismSlider.value;
        initialOrganismsSlider.maxValue = maxEntities - maxFoodOrganismSlider.value;
        if (initialFoodSlider.value > initialFoodSlider.maxValue)
        {
            initialFoodSlider.value = initialFoodSlider.maxValue;
        }
        if (initialFoodSlider.value < initialFoodSlider.minValue)
        {
            initialFoodSlider.value = initialFoodSlider.minValue;
        }
        if (initialOrganismsSlider.value > initialOrganismsSlider.maxValue)
        {
            initialOrganismsSlider.value = initialOrganismsSlider.maxValue;
        }
        if (initialOrganismsSlider.value < initialOrganismsSlider.minValue)
        {
            initialOrganismsSlider.value = initialOrganismsSlider.minValue;
        }
        SaveSettings();
        UpdateUITexts();
    }

    private void UpdateUITexts()
    {
        maxNumberEntitiesValueText.text = maxNumberEntitiesSlider.value.ToString("F0");
        maxNumberEntitiesMaxValueText.text = maxNumberEntitiesSlider.maxValue.ToString("F0");

        maxFoodValueText.text = maxFoodOrganismSlider.value.ToString("F0");
        maxOrganismValueText.text = (maxNumberEntitiesSlider.value - maxFoodOrganismSlider.value).ToString("F0");

        initialFoodValueText.text = initialFoodSlider.value.ToString("F0");
        initialFoodMaxValueText.text = initialFoodSlider.maxValue.ToString("F0");

        initialOrganismsValueText.text = initialOrganismsSlider.value.ToString("F0");
        initialOrganismsMaxValueText.text = initialOrganismsSlider.maxValue.ToString("F0");

        respawnFoodThresholdValueText.text = (respawnFoodThresholdScrollBar.value * 100).ToString("F0") + "%";
        respawnFoodIntervalValueText.text = (respawnFoodIntervalScrollBar.value).ToString("F0");
    }

    private void UpdateButtonStates()
    {
        smallButton.interactable = (selectedWorldSize != smallWorldSize);
        mediumButton.interactable = (selectedWorldSize != mediumWorldSize);
        largeButton.interactable = (selectedWorldSize != largeWorldSize);
    }

    private void ApplySettings()
    {
        SaveSettings();
        foodSettingsController.SaveSettings();
        organismSettingsController.SaveSettings();
        PlayerPrefs.Save();
        ShowStatusMessage("Changes Applied", Color.green);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("WorldWidth", selectedWorldSize.x);
        PlayerPrefs.SetFloat("WorldHeight", selectedWorldSize.y);
        PlayerPrefs.SetInt(MaxEntitiesKey, (int)maxNumberEntitiesSlider.value);
        PlayerPrefs.SetInt(InitialFoodKey, (int)initialFoodSlider.value);
        PlayerPrefs.SetInt(InitialOrganismsKey, (int)initialOrganismsSlider.value);
        PlayerPrefs.SetInt(RespawnFoodToggleKey, respawnFoodToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat(RespawnFoodThresholdKey, respawnFoodThresholdScrollBar.value);
        PlayerPrefs.SetFloat(RespawnFoodIntervalKey, respawnFoodIntervalScrollBar.value);
        PlayerPrefs.SetFloat(InitialMaxFoodKey, initialFoodSlider.maxValue);
        PlayerPrefs.SetFloat(InitialMaxOrganismsKey, initialOrganismsSlider.maxValue);
    }

    private void BackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void ShowStatusMessage(string message, Color textColor)
    {
        if (changesAppliedText != null)
        {
            changesAppliedText.text = message;
            changesAppliedText.color = textColor;
            changesAppliedText.gameObject.SetActive(true);
            StartCoroutine(HiddenMessageAfterDelay(changesAppliedDuration));
        }
        else
        {
            Debug.LogWarning("Changes Applied Text is not assigned in the inspector.");
        }
    }

    private IEnumerator HiddenMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (changesAppliedText != null)
        {
            changesAppliedText.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        maxNumberEntitiesSlider.onValueChanged.RemoveListener(OnTotalEntitiesChanged);
        initialFoodSlider.onValueChanged.RemoveListener(OnInitialFoodChanged);
        initialOrganismsSlider.onValueChanged.RemoveListener(OnInitialOrganismsChanged);
        smallButton.onClick.RemoveAllListeners();
        mediumButton.onClick.RemoveAllListeners();
        largeButton.onClick.RemoveAllListeners();
        applyButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        respawnFoodToggle.onValueChanged.RemoveListener(OnRespawnToggleChanged);
        respawnFoodThresholdScrollBar.onValueChanged.RemoveAllListeners();
        maxFoodOrganismSlider.onValueChanged.RemoveListener(OnMaxFoodOrganismChanged);
        respawnFoodIntervalScrollBar.onValueChanged.RemoveAllListeners();

    }
}