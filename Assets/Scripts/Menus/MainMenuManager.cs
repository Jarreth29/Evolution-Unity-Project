using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private GameObject backgroundMusic; // Reference to the game soundtrack
    [SerializeField] private AudioClip musicTrack;

    [Header("UI Elements")]
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (WorldManager2D.Instance != null) WorldManager2D.Instance.DestroyWorldManager(); // Reset the world if it exists
        if (Spawner.Instance != null) Spawner.Instance.DestroySpawner(); // Reset the spawner if it exists
        startButton.onClick.AddListener(StartSimulation);
        settingsButton.onClick.AddListener(SettingsMenu);
        quitButton.onClick.AddListener(QuitGame);
        Time.timeScale = 1; // Ensure the game is running at normal speed
        PlayBackgroundMusic();
    }

    private void StartSimulation()
    {
        // Load the game scene
        SceneManager.LoadScene("GameScene");
    }

    private void SettingsMenu()
    {
        // Load the options menu scene
        SceneManager.LoadScene("SettingsScene");
    }

    private void QuitGame()
    {
        // Quit the application
        Application.Quit();
        #if UNITY_EDITOR
        Debug.Log("Application has been quit in the editor. Use Play mode to test quitting.");
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
        #else
        Debug.Log("Application has been quit.");
        #endif
    }

    private void PlayBackgroundMusic()
    {
        backgroundMusic = GameObject.Find("BackgroundMusic"); // Find the background music GameObject by tag
        if (backgroundMusic != null)
        {
            BackgroundMusic bgMusicScript = backgroundMusic.GetComponent<BackgroundMusic>();
            if (bgMusicScript != null)
            {
                musicTrack = bgMusicScript.menuSoundtrack; // Assign the music track for the main menu
                if (musicTrack != bgMusicScript.audioSource.clip) // Check if the current track is different
                {
                    bgMusicScript.PlayTrack(musicTrack, true); // Play the main menu soundtrack
                }
            }
            else
            {
                Debug.LogWarning("AudioSource or music track is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("Background music GameObject is not assigned.");
        }
    }
}
