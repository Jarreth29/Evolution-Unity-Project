using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldManager2D : MonoBehaviour
{
    public static WorldManager2D Instance { get; private set; } // Singleton instance
    [SerializeField] private HUDManager hudManager; // Reference to the HUDManager instance

    [Header("World Settings")]
    public Vector2 _worldSize = new Vector2(); // Size of the world
    // Ratio of the screen width to the world size (70% for the world, 30% for the UI)
    [Range(0.3f, 0.9f)] public float worldScreenRatio = 0.7f;

    [Header("References")]
    public SpriteRenderer worldBackground; // Reference to the background sprite renderer
    public BoxCollider2D worldBoundary; // Reference to the boundary collider

    [Header("Main Camera Settings")]
    [SerializeField] public Camera gameCamera; // Reference to the main camera

    [Header("Microscope Camera Settings")]
    private bool cameraFollowing = false; // Reference to the camera that follows the player

    [Header("Audio Settings")]
    [SerializeField] private GameObject backgroundMusic; // Reference to the game soundtrack
    [SerializeField] private AudioClip musicTrack;
    [SerializeField] private AudioSource pauseAudioSource; // Reference to the audio source for pause sound
    [SerializeField] private AudioClip pauseInSound; // Reference to the audio clip for pause sound
    [SerializeField] private AudioClip pauseOutSound; // Reference to the audio clip for resume sound

    private GameObject target; // Reference to the target object (e.g., player or organism)
    [SerializeField] private LayerMask organismLayer; // Layer mask for detecting organisms
    [SerializeField] private LayerMask foodLayer; // Layer mask for detecting food
    private bool showAllRays = false; // Flag to show all rays

    [Header("Pause Settings")]
    [SerializeField] private GameObject HUDPause; // Reference to the pause menu GameObject

    [Header("QuitSimulation Settings")]
    [SerializeField] private GameObject HUDQuitSimulation; // Reference to the quit simulation panel GameObject
    [SerializeField] private Button HUDQuitYesButton; // Reference to the quit simulation button GameObject
    [SerializeField] private Button HUDQuitNoButton; // Reference to the confirm quit simulation button GameObject

    [Header("Food Panel")]
    [SerializeField] private GameObject HUDFoodPanel; // Reference to the food panel GameObject

    [Header("Organism Panel")]
    [SerializeField] private GameObject HUDOrganismPanel; // Reference to the organism panel GameObject

    public delegate void ShowAllRays();
    public event ShowAllRays OnShowAllRays;
    public event ShowAllRays OnHideAllRays;
    public delegate void MicroscopeCameraDelegate();
    public event MicroscopeCameraDelegate OnMicroscopeCameraEnabled;
    public event MicroscopeCameraDelegate OnMicroscopeCameraDisabled;

    public Vector2 WorldSize
    {
        get { return _worldSize; }
        private set { _worldSize = value; }
    }

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
        pauseAudioSource.ignoreListenerPause = true; // Ignore listener pause for the pause audio source
    }

    void Start()
    {
        backgroundMusic = GameObject.Find("BackgroundMusic");
        if (backgroundMusic != null)
        {
            BackgroundMusic bgMusicScript = backgroundMusic.GetComponent<BackgroundMusic>();
            if (bgMusicScript != null)
            {
                musicTrack = bgMusicScript.gameSoundtrack; // Get the menu soundtrack from the BackgroundMusic script
                bgMusicScript.PlayTrack(musicTrack, true); // Play the background music track
            }
            else
            {
                Debug.LogWarning("BackgroundMusic script not found on BackgroundMusic GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("Menu Background Music GameObject not found.");

        }
        InitializeReferences();
        LoadWorldSize();

        if (HUDQuitSimulation != null)
        {
            HUDQuitSimulation.SetActive(false); // Ensure the quit simulation panel is initially inactive
            if (HUDQuitYesButton != null)
            {
                HUDQuitYesButton.onClick.AddListener(QuitYes); // Add listener for the quit yes button
            }
            if (HUDQuitNoButton != null)
            {
                HUDQuitNoButton.onClick.AddListener(QuitNo); // Add listener for the quit no button
            }
        }
        if (HUDPause != null)
        {
            HUDPause.SetActive(false); // Ensure the pause menu is initially inactive
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("G key pressed, Activate/Deactivate global rays.");
            showAllRays = !showAllRays; // Toggle the flag for showing rays
            if (showAllRays)
            {
                OnShowAllRays?.Invoke(); // Invoke the event to hide all rays
            }
            else
            {
                OnHideAllRays?.Invoke(); // Invoke the event to show all rays
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (HUDQuitSimulation.activeSelf)
            {
                Debug.Log("Left Mouse clicked, Quit Simulation panel is active.");
                return; // Do not proceed if the quit simulation panel is active
            }
            SelectDetailPanel();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (HUDQuitSimulation.activeSelf)
            {
                Debug.Log("Left Mouse clicked, Quit Simulation panel is active.");
                return; // Do not proceed if the quit simulation panel is active
            }
            if (target != null && target.CompareTag("Organism"))
            {
                target.GetComponent<OrganismController>().HideRaysEvent(); // Hide rays from previous target
            }
            Debug.Log("Right Mouse clicked");
            SelectDetailPanel();
            FocusOnOrganism();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !HUDQuitSimulation.activeSelf)
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ApplicationQuit();
        }
    }

    private void FixedUpdate()
    {
        if (target == null && cameraFollowing) // If target is null, stop following
        {
            cameraFollowing = false;
            OnMicroscopeCameraDisabled?.Invoke(); // Invoke the event to disable microscope camera
            AdjustCamera(); // Adjust camera to the world size
        }
        else if (cameraFollowing && gameCamera != null && target != null)
        {
            // Update the camera position to follow the player
            Vector3 targetPosition = target.transform.position; // Assuming the player is at this GameObject's position
            float cameraViewWorldWidth = gameCamera.orthographicSize * 2f * gameCamera.aspect; // Calculate the camera view width in world units
            float uiSpaceWorldWidth = cameraViewWorldWidth * (1f - worldScreenRatio); // Calculate the UI space in world units
            float leftOffset = -uiSpaceWorldWidth * 0.5f; // Calculate the left offset based on the UI space
            float offsetCameraX = targetPosition.x - leftOffset; // Adjust the camera position based on the target position and left offset
            gameCamera.transform.position = new Vector3(offsetCameraX, targetPosition.y, gameCamera.transform.position.z);
        }
    }

    private void SelectDetailPanel()
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.HideAllPanels(); // Hide all panels in the HUDManager
        }
        Vector2 mousePosition = gameCamera.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world coordinates
        LayerMask combinedLayers = foodLayer | organismLayer; // Combine the food and organism layers
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, combinedLayers);
        if (hit.collider != null)
        {
            Debug.Log("Hit object: " + hit.collider.tag);
            if (hit.collider.CompareTag("Food"))
            {
                FoodController foodController = hit.collider.GetComponent<FoodController>();
                Debug.Log("Food selected");
                HUDManager.Instance.ShowFoodDetails(foodController); // Show the food details in the HUDManager
            }
            else if (hit.collider.CompareTag("Organism"))
            {
                Debug.Log("Organism selected");
                OrganismController organismController = hit.collider.GetComponent<OrganismController>();
                HUDManager.Instance.ShowOrganismDetails(organismController); // Show the organism details in the HUDManager
            }
            else
            {
                return;
            }
        }
    }

    private void TogglePause()
    {
        if (HUDPause == null)
        {
            HUDPause = GameObject.FindGameObjectWithTag("HUDPause");
        }
        
        if (Time.timeScale == 1f)
        {
            if (HUDPause != null)
            {
                HUDPause.SetActive(true); // Show the pause menu
            }
            else
            {
                Debug.LogWarning("HUDPause GameObject not found. Please assign it in the inspector.");
            }
            Time.timeScale = 0f; // Pause the game
            AudioListener.pause = true; // Pause the audio listener
            if (pauseAudioSource != null && pauseInSound != null)
            {
                pauseAudioSource.PlayOneShot(pauseInSound); // Play the pause sound
            }

            Debug.Log("Game Paused");
        }
        else
        {
            if (HUDPause != null)
            {
                HUDPause.SetActive(false); // Hide the pause menu
            }
            else
            {
                Debug.LogWarning("HUDPause GameObject not found. Please assign it in the inspector.");
            }
            AudioListener.pause = false; // Resume the audio listener
            Time.timeScale = 1f; // Resume the game
            Debug.Log("Game Resumed");
        }
    }

    private void ApplicationQuit()
    {
        Debug.Log("Application Quit");
        if (HUDPause != null)
        {
            HUDPause.SetActive(false); // Hide the pause menu
        }
        else
        {
            Debug.LogWarning("HUDPause GameObject not found. Please assign it in the inspector.");
        }
        if (pauseAudioSource != null && pauseOutSound != null)
        {
            pauseAudioSource.PlayOneShot(pauseInSound); // Play the resume sound
            Time.timeScale = 0f; // Pause the game
        }
        if (HUDQuitSimulation != null)
        {
            HUDQuitSimulation.SetActive(true); // Show the quit simulation panel
        }
        else
        {
            Debug.LogWarning("HUDQuitSimulation GameObject not found. Please assign it in the inspector.");
        }        
    }

    private void QuitYes()
    {
        BackgroundMusic bgMusicScript = backgroundMusic.GetComponent<BackgroundMusic>();
        if (bgMusicScript != null)
        {
            musicTrack = bgMusicScript.menuSoundtrack; // Get the menu soundtrack from the BackgroundMusic script
            bgMusicScript.PlayTrack(musicTrack, true); // Play the background music track
        }

        SceneManager.LoadScene("MainMenuScene"); // Load the main menu scene
    }

    private void QuitNo()
    {
        if (HUDQuitSimulation != null)
        {
            Time.timeScale = 1f; // Pause the game
            HUDQuitSimulation.SetActive(false); // Hide the quit simulation panel
        }
        else
        {
            Debug.LogWarning("HUDQuitSimulation GameObject not found. Please assign it in the inspector.");
        }
    }

    private void FocusOnOrganism()
    {
        Vector2 mousePosition = gameCamera.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world coordinates
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, organismLayer); // Cast a ray to detect organisms
        if (hit.collider != null)
        {
            Debug.Log("Hit object: " + hit.collider.tag);
            if (hit.collider.CompareTag("Organism"))
            {
                //HUDManager.Instance.HideAllPanels(); // Hide all panels in the HUDManager
                Debug.Log("Organism selected");
                if (target != null)
                {
                    target.GetComponent<OrganismController>().isShowingRays = false; // Hide rays from previous target
                }
                target = hit.collider.gameObject; // Set the target to the clicked organism
                cameraFollowing = true;
                OnMicroscopeCameraEnabled?.Invoke(); // Invoke the event to enable microscope camera
                target.GetComponent<OrganismController>().ShowRaysEvent(); // Show rays when camera is following the target
                gameCamera.orthographicSize = 10f; // Set the camera size to a large value for better visibility

                return; // Exit the method if an organism is selected
            }
        }
        if (cameraFollowing)
        {
            if (target != null)
            {
                target.GetComponent<OrganismController>().HideRaysEvent(); // Hide rays if not following                    
            }
            target = null; // Reset the target if not an organism
            cameraFollowing = false; // Reset camera following state if not an organism
            OnMicroscopeCameraDisabled?.Invoke(); // Invoke the event to disable microscope camera
            AdjustCamera(); // Adjust camera to the world size
        }
    }

    private void InitializeReferences()
    {
        // Ensure the GameObject has a SpriteRenderer and BoxCollider2D component
        if (worldBackground == null)
            worldBackground = GetComponentInChildren<SpriteRenderer>();
        if (worldBoundary == null)
            worldBoundary = GetComponentInChildren<BoxCollider2D>();
        // Ensure the GameObject has a Camera component
        if (gameCamera == null)
            gameCamera = Camera.main;
    }

    private void LoadWorldSize()
    {
        // Load the world size from PlayerPrefs or use the default value
        float x = PlayerPrefs.GetFloat("WorldWidth", 100f);
        float y = PlayerPrefs.GetFloat("WorldHeight", 100f);
        SetWorldSize(x, y);
    }

    // Public method to set the world size
    public void SetWorldSize(Vector2 newSize)
    {
        WorldSize = newSize;
        //Debug.Log($"World Size Set: {WorldSize.x} x {WorldSize.y}");
        UpdateWorld();
    }

    // Public method to set the world size using separate width and height
    public void SetWorldSize(float width, float height)
    {
        SetWorldSize(new Vector2(width, height));
    }

    private void UpdateWorld()
    {
        UpdateBackground();
        UpdateBoundary();
        //PositionWorld();
        AdjustCamera();
    }

    private void UpdateBackground()
    {
        if (worldBackground != null)
        {
            worldBackground.size = Vector2.one; // Reset the size to 1
            worldBackground.transform.localScale = new Vector3(
                WorldSize.x + 5,
                WorldSize.y + 5,
                1f
            );
        }
    }

    private void UpdateBoundary()
    {
        if (worldBoundary != null)
        {
            worldBoundary.size = new Vector2(WorldSize.x - 1, WorldSize.y - 1);
            worldBoundary.transform.localScale = Vector3.one; // Reset the scale to 1
        }
    }

    // Adjust the camera size and position the world base on the worldScreenRatio (70% for the world, 30% for the UI)
    private void AdjustCamera()
    {
        // Calculate camera size based on world size
        gameCamera.transform.position = new Vector3(0f, 0f, -10f); // Set camera position to default    
        float cameraSize = WorldSize.y * 0.5f;
        gameCamera.orthographicSize = cameraSize;

        // Calculate the total screen width based on the camera size and aspect ratio
        float totalWidth = cameraSize * 2f * gameCamera.aspect;
        // Calculate the UI space based on the world screen ratio (70% for the world, 30% for the UI)
        float uiSpace = totalWidth * (1f - worldScreenRatio);
        // Calculate the left offset based on the UI space
        float leftOffset = uiSpace * 0.5f;
        transform.position = new Vector3(-leftOffset, 0f, 0f);

        //Debug.Log($"Corrected Values - World: {worldSize}, " +
        //      $"Camera Size: {cameraSize}, " +
        //      $"Total Width: {totalWidth}, " +
        //      $"UI Space: {uiSpace}, " +
        //      $"Offset: {leftOffset}");

    }

    public void DestroyWorldManager()
    {
        if (Instance == this)
        {
            Instance = null; // Reset the singleton instance
            Destroy(gameObject); // Destroy the WorldManager2D instance
            Debug.Log("WorldManager destroyed and cleaned up.");
        }
        else
        {
            Debug.LogWarning("WorldManager2D instance already destroyed or not found.");
        }
    }
}
