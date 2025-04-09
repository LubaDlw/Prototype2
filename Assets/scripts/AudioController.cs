using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Drag and drop your audio clip from the Assets folder into this field in the Inspector.
    public AudioClip soundClip;

    // This AudioSource will be used to play the sound.
    private AudioSource audioSource;

    void Start()
    {
        // Try to get an AudioSource component from the GameObject this script is attached to.
        audioSource = GetComponent<AudioSource>();

        // If no AudioSource exists, add one.
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign the provided audio clip to the AudioSource.
        audioSource.clip = soundClip;
    }

    // Function to start playing the sound.
    public void PlaySound()
    {
        if (audioSource != null && soundClip != null)
        {
            // Only play if the audio isn't already playing.
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("AudioSource or SoundClip not assigned!");
        }
    }

    // Function to stop playing the sound.
    public void StopSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
