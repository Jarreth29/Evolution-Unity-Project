using System.CodeDom.Compiler;
using System.Collections;
using UnityEngine;

public class OrganismController : MonoBehaviour
{
    [Header("Organism Phenotype")]

    [Header("Organism Movement")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;

    [Header("Organism Energy")]
    private float maxEnergy;
    [SerializeField] private float energy;
    [SerializeField] private float metabolicRate; // Energy consumption rate per second

    [Header("Organism Detection System")]
    [SerializeField] private float sightRange;
    [SerializeField] private int numberOfRays;
    [SerializeField] private float angleBetweenRays; // Angle between rays in degrees
    RaycastHit2D hit;
    LineRenderer lineRenderer; // Optional: for visualizing rays
    public bool isShowingRays = false; // Flag to control ray visibility
    private Material lineMaterial; // Material for the line renderer

    [Header("Organism Offspring")]
    [SerializeField] private int generation = 0; // Generation number

    [Header("Organism States")]
    [SerializeField] private bool isEating = false;
    [SerializeField] private bool isHungry = false;
    [SerializeField] private bool pregnancy = false; // Placeholder for pregnancy state

    [Header("World Parameters")]
    private Transform _worldBoundary;
    private float _worldWidth;
    private float _worldHeight;

    [SerializeField] private float maxOrganismsInSimulation = 500; // Maximum number of organisms in the simulation

    private Vector3 _currentDirection;
    private Vector3 movement;
    private float X = 0;
    private float Y = 0;
    [SerializeField] private LayerMask foodLayer;
    private GameObject _targetFood = null;

    private Spawner spawner;

    public void SetSpawner(Spawner s)
    {
        spawner = s;
    }

    public float Energy
    {
        get { return energy; }
        set
        {
            energy = Mathf.Clamp(value, 0, maxEnergy);
        }
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (WorldManager2D.Instance != null)
        {
            _worldBoundary = WorldManager2D.Instance.transform;
            _worldWidth = WorldManager2D.Instance.WorldSize.x;
            _worldHeight = WorldManager2D.Instance.WorldSize.y;
            WorldManager2D.Instance.OnShowAllRays += ShowRaysEvent; // Subscribe to the event to toggle ray visibility
            WorldManager2D.Instance.OnHideAllRays += HideRaysEvent; // Subscribe to the event to toggle ray visibility
        }
        ChangeRandomDirection();
        if (generation == 0)
        {
            SetupLineRenderer();
        }
        maxOrganismsInSimulation = PlayerPrefs.GetFloat("InitialMaxOrganisms", 500); // Set the maximum number of organisms in the simulation
    }

    private void Update()
    {
        if (isShowingRays && lineRenderer != null && lineRenderer.enabled)
        {
            ShowRays();
        }
    }

    public void ShowRaysEvent()
    {
        isShowingRays = true;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true; // Enable the line renderer to show rays
        }
    }
    public void HideRaysEvent()
    {
        isShowingRays = false;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Disable the line renderer to hide rays
        }
    }

    private void ShowRays()
    {
        int index = 0;
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = ((2 * i + 1 - numberOfRays) * angleBetweenRays / 2); // Calculate the angle for each ray
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 rayDirection = rotation * _currentDirection;
            Vector3 rayStart = transform.position + _currentDirection * 1f; // Start the ray slightly above the organism
            Vector3 rayEnd = rayStart + rayDirection * sightRange; // End point of the ray
            lineRenderer.SetPosition(index++, rayStart);
            lineRenderer.SetPosition(index++, rayEnd);
        }
    }

    private void FixedUpdate()
    {
        if (!isEating)
        {
            ManageEnergy();
            if (isHungry)
            {
                SearchForFood();
            }
            Movement(X, Y);
        }

    }

    public void ManageEnergy()
    {
        if (Energy < maxEnergy * 0.80f)
        {
            isHungry = true;
            if (Energy < maxEnergy * 0.75f)
            {
                pregnancy = false; // Reset pregnancy state when hungry
                if (IsInvoking("GestationPeriod"))
                {
                    CancelInvoke("GestationPeriod"); // Stop the gestation period if hungry
                    //Debug.Log("Gestation period cancelled due to hunger.");
                }
            }
            if (Energy < maxEnergy * 0.25f)
            {
                speed = baseSpeed / 2; // Slow down when energy is low
            }
        }
        else
        {
            if (!pregnancy)
            {
                pregnancy = true; // Placeholder for pregnancy state
                //Debug.Log("Pregnancy started.");
                if (!IsInvoking("GestationPeriod"))
                {
                    // Start the gestation period
                    //Debug.Log("Gestation period started.");
                    InvokeRepeating("GestationPeriod", 10f, 10f);
                }

            }
            isHungry = false;
            speed = baseSpeed; // Restore speed when energy is sufficient
        }
        if (Energy <= 0)
        {
            // Handle organism death or energy depletion
            Debug.Log("Organism has no energy left!");
            WorldManager2D.Instance.OnShowAllRays -= ShowRaysEvent; // Unsubscribe to the event to toggle ray visibility
            WorldManager2D.Instance.OnHideAllRays -= HideRaysEvent; // Unsubscribe to the event to toggle ray visibility
            CancelInvoke("GestationPeriod"); // Stop the gestation period if organism is dead
            spawner.RemoveOrganism(gameObject);
        }
        if (!isEating)
        {
            energy -= metabolicRate; // Energy consumption per second
        }
    }

    private void GestationPeriod()
    {
        if (!pregnancy)
        {
            CancelInvoke("GestationPeriod"); // Stop the gestation period if not pregnant
            Debug.Log("Gestation period ended.");
            return; // Exit if not pregnant
        }
        //Debug.Log("Gestation period ended.");
        if (spawner.GetOrganismCount() >= maxOrganismsInSimulation)
        {
            Debug.Log($"Max food limit reached ({maxOrganismsInSimulation} , cannot sprout new seedlings.g.");
            return; // Exit if the world is full
        }
        CreateNewOffsping();
    }

    private void CreateNewOffsping()
    {
        if (spawner != null)
        {
            Energy = maxEnergy * 0.75f; // Lower energy for the parent organism
            //Debug.Log("Creating new offspring.");
            Vector3 spawnPosition = transform.position - _currentDirection; // Spawn slightly away from the parent organism
            GameObject newOrganism = spawner.SpawnOrganism(spawnPosition);
            if (newOrganism == null)
            {
                Debug.Log("Failed to spawn new organism.");
                return; // Exit if spawning failed
            }
            if (newOrganism != null)
            {
                OrganismController newOrganismController = newOrganism.GetComponent<OrganismController>();
                if (newOrganismController != null)
                {
                    Mutate(newOrganismController); // Apply mutations to the new organism
                    newOrganismController.SetSpawner(spawner); // Set the spawner reference in the organism controller
                    newOrganismController.generation = generation + 1; // Increment generation number
                    newOrganismController.Energy = maxEnergy * 0.75f;
                    newOrganismController.speed = newOrganismController.baseSpeed; // Reset speed for the new organism
                    newOrganismController.SetupLineRenderer();
                }
            }
        }
    }

    public void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfRays * 2; // Set the number of positions for the line renderer
        lineRenderer.widthMultiplier = 0.2f; // Set the width of the lines
        lineRenderer.useWorldSpace = true; // Use world space for the line renderer
        lineRenderer.sortingLayerName = "LineRenderer"; // Set the sorting layer for the line renderer
        lineRenderer.sortingOrder = 15; // Set the sorting order for the line renderer
        lineRenderer.enabled = isShowingRays; // Enable or disable the line renderer based on the flag
    }

    private void Mutate(OrganismController organism)
    {
        // This method applies mutations to the organism's phenotype
        organism.baseSpeed = GetMutation(baseSpeed, 0.5f, 0.1f, 5);
        //organism.rotationSpeed = GetMutation(rotationSpeed, 0.1f, 0.1f, 10f);
        organism.maxEnergy = GetMutation(maxEnergy, 5f, 10, 1000);
        organism.metabolicRate = GetMutation(metabolicRate, 0.05f, 0.01f, 1f);
        organism.sightRange = GetMutation(sightRange, 1f, 0.1f, 10f);
        float mutatedRays = GetMutation(numberOfRays, 1, 1, 10); // Randomize the number of rays
        organism.numberOfRays = Mathf.Clamp(Mathf.CeilToInt(mutatedRays), 1, 10); // Round to int the number of rays
        float mutatedAngle = GetMutation(angleBetweenRays, 5f, 1f, 36f); // Randomize the angle between rays
        organism.angleBetweenRays = Mathf.Clamp(Mathf.CeilToInt(mutatedAngle), 1, 36); // Round to int the angle between rays
    }

    private void Movement(float X, float Y)
    {
        X = Mathf.Clamp(X, -1f, 1f);
        Y = Mathf.Clamp(Y, -1f, 1f);

        movement = new Vector3(X, Y, 0) * speed * Time.fixedDeltaTime;
        transform.position = transform.position + movement;
        float angle = Mathf.Atan2(Y, X) * Mathf.Rad2Deg - 90;
        Quaternion toRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    private void SearchForFood()
    {
        if (_targetFood != null)
        {
            return;
        }
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = ((2 * i + 1 - numberOfRays) * angleBetweenRays / 2); // Calculate the angle for each ray
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 rayDirection = rotation * _currentDirection;
            Vector3 rayStart = transform.position + _currentDirection * 1f; // Start the ray slightly above the organism
            Debug.DrawRay(rayStart, rayDirection * sightRange, Color.red);
            // Perform the raycast
            hit = Physics2D.Raycast(rayStart, rayDirection, sightRange, foodLayer);
            if (hit.collider != null)
            {
                //Debug.Log("Food detected: " + hit.collider.gameObject.name);
                // Check if the food is not already being eaten
                if (!hit.collider.gameObject.GetComponent<FoodController>().IsBeingEaten)
                {
                    _targetFood = hit.collider.gameObject;
                    Vector3 directionToFood = (hit.collider.transform.position - transform.position).normalized;
                    _currentDirection = directionToFood;
                    X = _currentDirection.x;
                    Y = _currentDirection.y;
                }
            }
        }
    }

    private IEnumerator EatFood(GameObject food)
    {
        isEating = true;
        FoodController foodController = food.GetComponent<FoodController>();
        if (foodController == null)
        {
            Debug.LogError("FoodController component not found on the food object.");
            isEating = false; // Exit if FoodController is not found
            yield break; // Exit if FoodController is not found
        }
        if (foodController.IsBeingEaten)
        {
            //Debug.Log("Food is already being eaten.");
            isEating = false; // Exit if food is already being eaten
            yield break; // Exit if food is already being eaten
        }
        if (foodController != null)
        {
            foodController.IsBeingEaten = true; // Mark the food as being eaten
            Energy += foodController.EnergyValue; // Example energy gain

            yield return new WaitForSeconds(5f); // Simulate eating time

            foodController.ConsumeFood(); // Example method to consume food
        }
        _targetFood = null; // Reset target food
        isEating = false;
        //ChangeRandomDirection();
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

    public void SetInitialPhenotype()
    {
        baseSpeed = PlayerPrefs.GetFloat("OrganismBaseSpeed", 1f);
        maxEnergy = PlayerPrefs.GetFloat("OrganismMaxEnergy", 100f);
        metabolicRate = PlayerPrefs.GetFloat("OrganismEnergyConsumptionRate", 0.05f);
        sightRange = PlayerPrefs.GetFloat("OrganismSightRange", 3f);
        numberOfRays = PlayerPrefs.GetInt("OrganismNumberOfRays", 3);
        angleBetweenRays = PlayerPrefs.GetFloat("OrganismAngleBetweenRays", 10f);
        speed = baseSpeed;
        rotationSpeed = 5f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WorldBoundary"))
        {
            BoundaryCollision();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food") && isHungry)
        {
            //Debug.Log("Organism collided with food: " + other.gameObject.name);
            StartCoroutine(EatFood(other.gameObject));
        }
    }

    private void ChangeRandomDirection()
    {
        _currentDirection = Random.insideUnitCircle.normalized;
        X = _currentDirection.x;
        Y = _currentDirection.y;
    }

    private void BoundaryCollision()
    {
        //Debug.Log("Hit World Boundary");
        Vector3 toCenter = (_worldBoundary.position - transform.position).normalized;
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        _currentDirection = toCenter + randomDirection * 0.5f;
        _currentDirection.Normalize();
        X = _currentDirection.x;
        Y = _currentDirection.y;
        transform.position = transform.position + (Vector3)_currentDirection * 0.5f;
    }

    // Getters for Organims
    public float GetBaseSpeed() { return baseSpeed; }
    public float GetRotationSpeed() { return rotationSpeed; }
    public float GetMaxEnergy() { return maxEnergy; }
    public float GetEnergy() { return energy; }
    public float GetMetabolismRate() { return metabolicRate; }
    public float GetSightRange() { return sightRange; }
    public int GetNumberOfRays() { return numberOfRays; }
    public float GetAngleBetweenRays() { return angleBetweenRays; }
    public int GetGeneration() { return generation; }
    public bool IsEating() { return isEating; }
    public bool IsHungry() { return isHungry; }
    public bool IsPregnant() { return pregnancy; }
}