using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip menuSoundtrack; // Soundtrack for the main menu
    public AudioClip gameSoundtrack; // Soundtrack for the game scene

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevents this object from being destroyed when loading a new scene
            audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to this GameObject
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this one
        }
    }

    public void PlayTrack(AudioClip track, bool loop = true)
    {
        if (audioSource != null)
        {
            audioSource.clip = track; // Set the audio clip to play
            audioSource.loop = loop; // Set whether the track should loop
            audioSource.Play(); // Start playing the track
        }
        else
        {
            Debug.LogWarning("AudioSource component is not assigned.");
        }
    }

    public void StopTrack()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop the currently playing track
        }
    }

    public void PauseTrack()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause(); // Pause the currently playing track
        }
    }
}
