using UnityEngine;

public class TestAudioTrigger : MonoBehaviour
{
    // Reference to the AudioController component.
    // Drag and drop the GameObject with the AudioController in the Inspector.
    public AudioController audioController;

    void Update()
    {
        // When the player presses the P key, play the sound.
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (audioController != null)
            {
                audioController.PlaySound();
            }
            else
            {
                Debug.LogWarning("AudioController reference is missing!");
            }
        }

        // When the player presses the S key, stop the sound.
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (audioController != null)
            {
                audioController.StopSound();
            }
            else
            {
                Debug.LogWarning("AudioController reference is missing!");
            }
        }
    }
}
