using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; } // Singleton instance
    [Header("Spawner Pools")]
    //[SerializeField] private ObjectPool<GameObject> foodPool;
    //[SerializeField] private ObjectPool<GameObject> organismPool;
    //[SerializeField] bool usePooling = false; // Toggle for using object pooling
    //private int activeFoodCount;
    //private int activeOrganismCount;

    [Header("FoodSpawner Settings")]
    [SerializeField] int initialNumberOfFood;
    [SerializeField] int maxNumberOfFood = 1000; // Maximum number of food items allowed in the world
    [SerializeField] float respawningtime = 100f; // Time in seconds to wait before respawning food
    [SerializeField] bool respawnFood = true; // Toggle for respawning food
    [SerializeField] float foodThreshold = 0.25f; // Threshold for respawning food (25% of initial food count)
    [SerializeField] GameObject foodPrefab;
    private List<GameObject> foodList;

    [Header("OrganismSpawner Settings")]
    [SerializeField] int initialNumberOfOrganisms;
    [SerializeField] GameObject organismPrefab;
    private List<GameObject> organismList;

    [Header("Wold Parameters")]
    [SerializeField] private WorldManager2D worldManager;
    private Vector2 _worldSize;
    private Vector2 _worldCenter;
    [SerializeField] private float maxEntities;

    public delegate void ListChangeHandler();
    public event ListChangeHandler OnFoodListChanged;
    public event ListChangeHandler OnOrganismListChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
    }

    void Start()
    {
        maxEntities = PlayerPrefs.GetFloat("MaxEntities", 1000); // Get the maximum number of entities from PlayerPrefs
        initialNumberOfFood = PlayerPrefs.GetInt("InitialFood", 500); // Get the initial number of food from PlayerPrefs
        initialNumberOfOrganisms = PlayerPrefs.GetInt("InitialOrganisms", 100); // Get the initial number of organisms from PlayerPrefs
        respawnFood = PlayerPrefs.GetInt("RespawnFoodToggle", 1) == 1; // Get the respawn food setting from PlayerPrefs
        respawningtime = PlayerPrefs.GetFloat("RespawnFoodInterval", 100f); // Get the respawning time from PlayerPrefs
        foodThreshold = PlayerPrefs.GetFloat("RespawnFoodThreshold", 0.25f); // Get the food threshold for respawning from PlayerPrefs
        maxNumberOfFood = PlayerPrefs.GetInt("InitialMaxFood", 1000); // Get the maximum number of food items from PlayerPrefs

        foodList = new List<GameObject>();
        organismList = new List<GameObject>();
        Debug.Log("Initialize spawner Coroutines...");
        StartCoroutine(InitializeWorldReferences());
        if (respawnFood)
        {
            StartCoroutine(RespawnFoodPeriodically());
        }

    }

    private IEnumerator RespawnFoodPeriodically()
    {
        Debug.Log("Starting food respawn coroutine... Time");
        while (true)
        {
            yield return new WaitForSeconds(respawningtime); // Wait for x seconds before respawning food
            Debug.Log("Checking food count for respawn...");
            int foodCount = GetFoodCount();
            Debug.Log($"Food count: {foodCount}, Threshold: {foodThreshold}. Respawning when food below: {maxNumberOfFood * foodThreshold}");
            if (GetFoodCount() < maxNumberOfFood * foodThreshold)
            {
                Debug.Log($"Food count is low, respawning food in {respawningtime} seconds");
                InitialSpawnFood();
                OnFoodListChanged?.Invoke(); // Notify subscribers about the food list change
            }
        }
    }

    private IEnumerator InitializeWorldReferences()
    {
        yield return new WaitForSeconds(0.1f); // Wait for a short time to ensure the WorldManager2D is initialized
        worldManager = WorldManager2D.Instance;

        if (worldManager != null)
        {
            _worldSize = worldManager.WorldSize;
            _worldCenter = worldManager.transform.position;

            InitialSpawn();
        }
        else
        {
            Debug.LogError("WorldManager2D not found in the scene.");
            yield break; // Exit the coroutine if WorldManager2D is not found
        }

    }

    private void InitialSpawn()
    {
        if (worldManager == null)
        {
            Debug.LogError("WorldManager2D not found. Cannot spawn food or organisms.");
            return;
        }
        InitialSpawnFood();
        InitialSpawnOrganism();

        OnFoodListChanged?.Invoke(); // Notify subscribers about the initial food list
        OnOrganismListChanged?.Invoke(); // Notify subscribers about the initial organism list
    }

    private void InitialSpawnFood()
    {
        for (int i = 0; i < initialNumberOfFood; i++)
        {
            SpawnInitialFood();
        }
    }

    private void InitialSpawnOrganism()
    {
        for (int i = 0; i < initialNumberOfOrganisms; i++)
        {
            SpawnNewOrganism();
        }
    }

    private void SpawnInitialFood()
    {
        Vector2 spawnPosition = GetRandomPosition();
        Vector3 spawnSize = GetRandomSize(0.1f, 0.5f); // Random size between 0.1 and 0.3
        GameObject initalFood = SpawnFood(spawnPosition);
        if (initalFood != null)
        {
            FoodController foodController = initalFood.GetComponent<FoodController>();
            if (foodController != null)
            {
                foodController.SetSpawner(this); // Set the spawner reference in the food controller
                foodController.SetInitialPhenotype(); // Set the initial phenotype of the food
                foodController.setSize = Random.Range(0.1f, 0.5f); ; // Set the size of the food object
            }
            else
            {
                Debug.LogError("FoodController component not found on the food prefab.");
            }
        }
    }

    public GameObject SpawnFood(Vector2 spawnPosition)
    {
        if (!CheckInBoundary(spawnPosition))
        {
            Debug.LogWarning($"Spawn position is outside the world boundaries.");
            return null;
        }
        GameObject food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        food.transform.localScale = Vector3.one;
        food.name = "Food"; // Set the name of the food object
        AddFood(food);
        FoodController foodController = food.GetComponent<FoodController>();
        if (foodController != null)
        {
            foodController.SetSpawner(this); // Set the spawner reference in the food controller
        }
        else
        {
            Debug.LogError("FoodController component not found on the food prefab.");
        }
        return food;
    }

    private void SpawnNewOrganism()
    {
        Vector2 spawnPosition = GetRandomPosition();
        GameObject newOrganism = SpawnOrganism(spawnPosition);
        if (newOrganism != null)
        {
            OrganismController organismController = newOrganism.GetComponent<OrganismController>();
            if (organismController != null)
            {
                organismController.SetSpawner(this); // Set the spawner reference in the organism controller
                organismController.SetInitialPhenotype(); // Set the initial phenotype of the organism
                float maxEnergy = organismController.GetMaxEnergy();
                organismController.Energy = Random.Range(maxEnergy/2, maxEnergy); // Set random energy between 50 and 100
                organismController.SetupLineRenderer(); // Setup the line renderer for the organism
            }

        }
    }

    public GameObject SpawnOrganism(Vector2 spawnPosition)
    {
        if (!CheckInBoundary(spawnPosition))
        {
            Debug.LogWarning($"Spawn position is outside the world boundaries.");
            return null;
        }
        GameObject organism = Instantiate(organismPrefab, spawnPosition, Quaternion.identity);
        organism.name = "Organism"; // Set the name of the organism object
        AddOrganism(organism);
        OrganismController organismController = organism.GetComponent<OrganismController>();
        if (organismController != null)
        {
            organismController.SetSpawner(this); // Set the spawner reference in the organism controller
        }
        else
        {
            Debug.LogError("OrganismController component not found on the organism prefab.");
        }
        return organism;
    }

    public void AddFood(GameObject food)
    {
        if (!foodList.Contains(food))
        {
            foodList.Add(food);
            OnFoodListChanged?.Invoke(); // Notify subscribers about the food list change
            //Debug.Log($"Food added to the list: {food.name}");
        }
        else
        {
            Debug.Log($"Food already exists in the list: {food.name}");
        }
    }

    public void RemoveFood(GameObject food)
    {
        if (foodList.Contains(food))
        {
            foodList.Remove(food);
            OnFoodListChanged?.Invoke(); // Notify subscribers about the food list change
            Destroy(food); // Destroy the food object
            //Debug.Log($"Food removed from the list: {food.name}");
        }
        else
        {
            Debug.Log($"Food not found in the list: {food.name}");
        }

    }

    public void AddOrganism(GameObject organism)
    {
        if (!organismList.Contains(organism))
        {
            organismList.Add(organism);
            OnOrganismListChanged?.Invoke(); // Notify subscribers about the organism list change
            //Debug.Log($"Organism added to the list: {organism.name}");
        }
        else
        {
            Debug.Log($"Organism already exists in the list: {organism.name}");
        }
    }

    public void RemoveOrganism(GameObject organism)
    {
        if (organismList.Contains(organism))
        {
            organismList.Remove(organism);
            OnOrganismListChanged?.Invoke(); // Notify subscribers about the organism list change
            Destroy(organism); // Destroy the organism object
        }
        else
        {
            Debug.Log($"Organism not found in the list: {organism.name}");
        }
    }

    public List<GameObject> GetFoodList()
    {
        return foodList;
    }

    public List<GameObject> GetOrganismList()
    {
        return organismList;
    }

    public int GetFoodCount()
    {
        return foodList.Count;
    }

    public int GetOrganismCount()
    {
        return organismList.Count;
    }

    public float GetMaxEntities()
    {
        return maxEntities;
    }

    private Vector2 GetRandomPosition()
    {
        // Generate a random position within the world boundaries
        Vector2 randomPosition = new Vector2(
            Random.Range((_worldCenter.x - _worldSize.x / 2) + 1, (_worldCenter.x + _worldSize.x / 2) - 1),
            Random.Range((_worldCenter.y - _worldSize.y / 2) + 1, (_worldCenter.y + _worldSize.y / 2) - 1)
        );
        return randomPosition;
    }

    private Vector3 GetRandomSize(float minRange, float maxRange)
    {
        // Generate a random size for the food object
        float randomSize = Random.Range(minRange, maxRange);
        return new Vector3(randomSize, randomSize, 0.1f); // Set the scale of the food object
    }

    private bool CheckInBoundary(Vector2 position)
    {
        // Check if the position is within the world boundaries
        return (position.x >= ((_worldCenter.x - _worldSize.x / 2) + 1) && position.x <= ((_worldCenter.x + _worldSize.x / 2) - 1) &&
                position.y >= ((_worldCenter.y - _worldSize.y / 2) + 1) && position.y <= ((_worldCenter.y + _worldSize.y / 2) - 1));
    }

    public void DestroySpawner()
    {
        // Clean up the spawner instance
        if (Instance == this)
        {
            Instance = null; // Clear the singleton instance
            StopAllCoroutines(); // Stop all coroutines
            foodList.Clear(); // Clear the food list
            organismList.Clear(); // Clear the organism list
            Destroy(gameObject); // Destroy the spawner GameObject
            Debug.Log("Spawner destroyed and cleaned up.");
        }
    }
}
