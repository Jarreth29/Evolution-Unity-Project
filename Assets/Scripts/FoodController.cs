using UnityEngine;

public class FoodController : MonoBehaviour
{
    [Header("Food Phenotype")]
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    [SerializeField] private float growthSpeed;
    [SerializeField] private float limitSpawnRadius;
    [SerializeField] private float sproutFrequency;
    [SerializeField] private float sproutDistance;

    [Header("Food Behaviour")]
    [SerializeField] private bool isGrowing = true;
    [SerializeField] private bool beingEaten = false;
    [SerializeField] private int seedlingsProduced = 0;
    [SerializeField] private float energyValue;

    [Header("Food Gizmos")]
    private Vector3 debugSpawnPosition;
    private float debugCheckRadius;
    private bool showDebugOverlap = false;

    [Header("Food References")]
    private Spawner spawner;
    [SerializeField] private float maxFoodInSimulation;

    private void Start()
    {
        maxFoodInSimulation = PlayerPrefs.GetFloat("InitialMaxFood", 2500);
    }


    public void SetSpawner(Spawner s)
    {
        spawner = s;
    }

    private void Update()
    {
        if (isGrowing)
        {
            Grow();
        }
    }

    private void Grow()
    {
        if (beingEaten)
        {
            return;
        }
        if (transform.localScale.x < maxSize)
        {
            float newScale = transform.localScale.x + growthSpeed * Time.deltaTime;
            setSize = newScale;
            energyValue = transform.localScale.x * 10;
        }
        else
        {
            isGrowing = false;
            transform.localScale = new Vector3(maxSize, maxSize, 0);
            energyValue = transform.localScale.x * 10;
            if (!IsInvoking("SproutSeedling"))
            {
                InvokeRepeating("SproutSeedling", sproutFrequency, sproutFrequency);
            }
        }
    }

    private void SproutSeedling()
    {
        if (beingEaten)
        {
            CancelInvoke("SproutSeedling");
            return;
        }

        if (spawner.GetFoodCount() >= maxFoodInSimulation)
        {
            Debug.Log($"Max food limit reached ({maxFoodInSimulation}), cannot sprout new seedlings.");
            return;
        }

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnDirection = new Vector3(randomDirection.x, randomDirection.y, 0);
        Vector3 spawnPosition = transform.position + spawnDirection * sproutDistance;
        //Vector3 spawnSize = new Vector3(0.1f, 0.1f, 0);
        GameObject newFood;
        if (!IsCrowded(spawnPosition, limitSpawnRadius))
        {
            newFood = spawner.SpawnFood(spawnPosition);
            if (newFood != null)
            {
                FoodController foodController = newFood.GetComponent<FoodController>();
                if (foodController != null)
                {
                    MutateFood(foodController); // Apply mutations to the new food object
                    foodController.SetSpawner(spawner); // Set the spawner reference in the food controller
                    foodController.setSize = 0.1f;
                    foodController.isGrowing = true;
                    //Debug.Log($"Food spawned at {spawnPosition} with size {foodController.setSize}\n" +
                    //    $"Max Size: {foodController.maxSize},\nGrowth Speed: {foodController.growthSpeed}, " +
                    //    $"\nSprout Frequency: {foodController.sproutFrequency}, \nSprout Distance: {foodController.sproutDistance}");
                    //ConectParentChild(foodController);
                }
            }
        }
        seedlingsProduced++;
    }

    private void ConectParentChild(FoodController childFood)
    {
        Debug.DrawLine(transform.position, childFood.transform.position, Color.cyan, 3600f);
    }

    private void MutateFood(FoodController fController)
    {
        // This method applies mutations to the food object
        fController.maxSize = GetMutation(maxSize, 0.2f, 0.1f, 5f);
        fController.growthSpeed = GetMutation(growthSpeed, 0.05f, 0.01f, 1f);
        fController.limitSpawnRadius = maxSize / 2;
        fController.sproutFrequency = GetMutation(sproutFrequency, 0.1f, 1f, 30f);
        fController.sproutDistance = GetMutation(sproutDistance, 0.1f, fController.maxSize, 10f);
    }

    private float GetMutation(float baseValue, float mutationRange, float min, float max)
    {
        // This method applies a mutation to the given value based on the mutation rate
        if (Random.value < 0.01f) // 1% chance to generate big mutation
        {
            mutationRange = mutationRange * 2;
        }
        float mutation = Random.Range(-mutationRange, mutationRange);
        return Mathf.Clamp(baseValue + mutation, min, max); // Clamp the value between min and max
    }

    private bool IsCrowded(Vector3 spawnPosition, float radius)
    {
        // This method checks if the spawn position is crowded with other food objects
        debugSpawnPosition = spawnPosition;
        debugCheckRadius = radius;
        showDebugOverlap = false;

        int foodLayerMask = LayerMask.GetMask("Food");
        Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPosition, radius, foodLayerMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                return true; // Found another food object within the radius
            }
        }
        return false; // No other food objects found within the radius
    }

    public void ConsumeFood()
    {
        CancelInvoke("SproutSeedling");
        if (spawner != null)
        {
            beingEaten = false;
            spawner.RemoveFood(gameObject);
        }
    }

    public void SetInitialPhenotype()
    {
        minSize = PlayerPrefs.GetFloat("FoodMinSize", 0.1f);
        maxSize = PlayerPrefs.GetFloat("FoodMaxSize", 1f);
        growthSpeed = PlayerPrefs.GetFloat("FoodGrowthSpeed", 0.05f);
        sproutFrequency = PlayerPrefs.GetFloat("FoodSproutFrequency", 10f);
        sproutDistance = PlayerPrefs.GetFloat("FoodSproutDistance", 3f);
        limitSpawnRadius = maxSize / 2;
        isGrowing = true;
        beingEaten = false;
        seedlingsProduced = 0;
    }

    public float EnergyValue
    {
        get { return energyValue; }
        set { energyValue = value; }
    }

    public bool IsBeingEaten
    {
        get { return beingEaten; }
        set { beingEaten = value; }
    }

    public float setSize
    {
        get { return transform.localScale.x; }
        set { transform.localScale = new Vector3(value, value, 0); }
    }

    private void OnDrawGizmos()
    {
        if (showDebugOverlap)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(debugSpawnPosition, debugCheckRadius);
        }
    }

    // Getters for Food Phenotype
    public float GetMinSize() { return minSize; }
    public float GetMaxSize() { return maxSize; }
    public float GetGrowthSpeed() { return growthSpeed; }
    public float GetLimitSpawnRadius() { return limitSpawnRadius; }
    public float GetSproutFrequency() { return sproutFrequency; }
    public float GetSproutDistance() { return sproutDistance; }
    public int GetSeedlingsProduced() { return seedlingsProduced; }
    public float GetCurrentSize() { return transform.localScale.x; }
}
