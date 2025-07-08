using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("HUD References")]
    [SerializeField] private TextMeshProUGUI worldSizeValueText;
    [SerializeField] private TextMeshProUGUI foodCountValueText;
    [SerializeField] private TextMeshProUGUI organismCountValueText;

    [Header("Wold Parameters")]
    [SerializeField] private WorldManager2D worldManager;
    [SerializeField] private GameObject microscopeCamera;
    //private Vector2 _worldSize;

    [Header("Food Panels")]
    [SerializeField] private GameObject foodPanel;
    [SerializeField] private TextMeshProUGUI foodSizeValueText;
    [SerializeField] private TextMeshProUGUI foodMinSizeValueText;
    [SerializeField] private TextMeshProUGUI foodMaxSizeValueText;
    [SerializeField] private TextMeshProUGUI growthSpeedValueText;
    [SerializeField] private TextMeshProUGUI sproutFrequencyValueText;
    [SerializeField] private TextMeshProUGUI sproutDistanceValueText;

    [Header("Organism Panels")]
    [SerializeField] private GameObject organismPanel;
    [SerializeField] private TextMeshProUGUI generationValueText;
    [SerializeField] private TextMeshProUGUI energyValueText;
    [SerializeField] private TextMeshProUGUI baseSpeedValueText;
    [SerializeField] private TextMeshProUGUI maxEnergyValueText;
    [SerializeField] private TextMeshProUGUI metabolismRateValueText;
    [SerializeField] private TextMeshProUGUI sightRangeValueText;
    [SerializeField] private TextMeshProUGUI numberOfRaysValueText;
    [SerializeField] private TextMeshProUGUI angleBetweenRaysValueText;



    [Header("Data References")]
    [SerializeField] private Spawner spawner;
    private FoodController currentFood;
    private OrganismController currentOrganism;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        if (microscopeCamera == null)
        {
            microscopeCamera = GameObject.FindGameObjectWithTag("MicroscopeHUD");
            microscopeCamera.SetActive(false); // Ensure the microscope camera is initially inactive
        }
        HideAllPanels();
    }

    private void Start()
    {
        Invoke("DelayedInitialization", 0.1f); // Delay initialization to ensure other components are ready
    }

    private void Update()
    {
        if (foodPanel.activeSelf && currentFood!=null)
        {
            foodSizeValueText.text = currentFood.GetCurrentSize().ToString("F2");
        }
        if ( organismPanel.activeSelf && currentOrganism!=null)
        {
            energyValueText.text = currentOrganism.GetEnergy().ToString("F2");
        }
    }

    private void DelayedInitialization()
    {
        if (worldManager == null)
        {
            worldManager = WorldManager2D.Instance;
            if (worldManager == null)
            {
                Debug.LogError("HUDManager: WorldManager2D instance not found. Please assign it in the inspector.");
            }
        }
        if (worldManager != null)
        {
            //Debug.Log($"World Size HUD: {worldManager.WorldSize.x} x {worldManager.WorldSize.y} y");
            worldSizeValueText.text = $"{worldManager.WorldSize.x} x {worldManager.WorldSize.y}";
            worldManager.OnMicroscopeCameraEnabled += ActivateMicroscope;
            worldManager.OnMicroscopeCameraDisabled += DeactivateMicroscope;
        }

        if (spawner == null)
        {
            spawner = Spawner.Instance;
            if (spawner == null)
            {
                Debug.LogError("HUDManager: Spawner instance not found. Please assign it in the inspector.");
            }
        }
        if (spawner != null)
        {
            spawner.OnFoodListChanged += UpdateFoodCount;
            spawner.OnOrganismListChanged += UpdateOrganismCount;

            UpdateFoodCount();
            UpdateOrganismCount();
        }
    }

    public void ShowFoodDetails(FoodController foodController)
    {
        if (foodPanel != null && foodController != null)
        {
            currentFood = foodController;
            foodPanel.SetActive(true);
            foodSizeValueText.text = foodController.GetCurrentSize().ToString("F1");
            foodMinSizeValueText.text = foodController.GetMinSize().ToString("F1");
            foodMaxSizeValueText.text = foodController.GetMaxSize().ToString("F1");
            growthSpeedValueText.text = foodController.GetGrowthSpeed().ToString("F2");
            sproutFrequencyValueText.text = foodController.GetSproutFrequency().ToString("F0");
            sproutDistanceValueText.text = foodController.GetSproutDistance().ToString("F0");
        }
    }

    public void ShowOrganismDetails(OrganismController organismController)
    {
        if (organismPanel != null && organismController != null)
        {
            currentOrganism = organismController;
            organismPanel.SetActive(true);
            generationValueText.text = organismController.GetGeneration().ToString();
            baseSpeedValueText.text = organismController.GetBaseSpeed().ToString("F1");
            maxEnergyValueText.text = organismController.GetMaxEnergy().ToString("F1");
            metabolismRateValueText.text = organismController.GetMetabolismRate().ToString("F2");
            sightRangeValueText.text = organismController.GetSightRange().ToString("F2");
            numberOfRaysValueText.text = organismController.GetNumberOfRays().ToString();
            angleBetweenRaysValueText.text = organismController.GetAngleBetweenRays().ToString("F0");
        }
    }

    private void UpdateFoodCount()
    {
        foodCountValueText.text = spawner.GetFoodCount().ToString();
    }

    private void UpdateOrganismCount()
    {
        organismCountValueText.text = spawner.GetOrganismCount().ToString();
    }

    private void ActivateMicroscope()
    {
        if (microscopeCamera != null)
        {
            microscopeCamera.SetActive(true);
        }
    }

    private void DeactivateMicroscope()
    {
        // Logic to deactivate the microscope camera
        if (microscopeCamera != null)
        {
            microscopeCamera.SetActive(false);
        }
    }

    public void HideAllPanels()
    {
        if (foodPanel != null) foodPanel.SetActive(false);
        if (organismPanel != null) organismPanel.SetActive(false);
        currentOrganism = null;
        currentFood = null;
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnFoodListChanged -= UpdateFoodCount;
            spawner.OnOrganismListChanged -= UpdateOrganismCount;
        }
    }
}
